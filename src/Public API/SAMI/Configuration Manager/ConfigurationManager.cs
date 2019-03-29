using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using SAMI.Apps;
using SAMI.IOInterfaces;
using SAMI.Persistence;

namespace SAMI.Configuration
{
    /// <summary>
    /// Singleton class which contains information parsed from the configuration file for SAMI.
    /// </summary>
    internal class ConfigurationManager : IInternalConfigurationManager
    {
        private List<IParseable> _components;
        private XmlDocument _configDoc;
        private String _configFilePath;
        private CompositionContainer _compositionContainer;

        private static List<String> _pluginDirectories = new List<String>
        {
#if DEBUG
            ".",
#else
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SAMI"),
#endif
        };

        /// <summary>
        /// The location which SAMI resides in.
        /// </summary>
        public Location LocalLocation
        {
            get
            {
                return new Location(GetConfigurationToken("General/City"), GetConfigurationToken("General/State"), Int32.Parse(GetConfigurationToken("General/ZipCode")));
            }
        }

        /// <summary>
        /// The COM port that the xbee hub is on.
        /// </summary>
        public String XBeeCOM
        {
            get
            {
                return GetConfigurationToken("General/XBeeCOM");
            }
        }

        public String ZWaveCOM
        {
            get
            {
                return GetConfigurationToken("General/ZWaveCOM");
            }
        }

        /// <summary>
        /// The raw list of components available for this configuration manager.
        /// This should only be used for testing purposes.
        /// </summary>
        internal IEnumerable<IParseable> Components
        {
            get
            {
                return _components;
            }
        }

        public ConfigurationManager(String configurationFile)
        {
            _configFilePath = configurationFile;
            if (!File.Exists(_configFilePath))
            {
                _configFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SAMI", configurationFile);
            }
            _configDoc = new XmlDocument();
            _configDoc.Load(_configFilePath);
            UpdateControllerList();
        }

        public ConfigurationManager()
        {
            _components = new List<IParseable>();
        }

        internal void Dispose()
        {
            _components.ForEach(c => c.Dispose());
        }

        /// <summary>
        /// Returns the full absolute path of the file given. 
        /// This should be used whenever requesting a file by a relative path,
        /// since the actual location of that file may not be within the working directory.
        /// </summary>
        /// <param name="fileName">Relative path of the file.</param>
        /// <param name="owningType">A type inside of the library which owns the file. This will be where fileName should be relative to.</param>
        /// <returns>A string representing the absolute path to the file requested.</returns>
        public String GetPathForFile(string fileName, Type owningType)
        {
            String assemblyLocation = Assembly.GetAssembly(owningType).Location;
            String assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
            return Path.Combine(assemblyDirectory, fileName);
        }

        internal static void AddPluginDirectory(String pluginDirectory)
        {
            _pluginDirectories.Add(Path.GetFullPath(pluginDirectory));
        }

        /// <summary>
        /// Returns a list of all of the components of the given type that
        /// were specified in the configuration file, and available from plug-ins.
        /// </summary>
        /// <typeparam name="T">Type to search for.</typeparam>
        /// <returns>All of the components that match that given type.</returns>
        public IEnumerable<T> FindAllComponentsOfType<T>()
            where T : IParseable
        {
            return _components.OfType<T>().Where(c => c.IsValid);
        }

        public IEnumerable<T> FindAllComponentsOfTypeEvenInvalid<T>()
            where T : IParseable
        {
            return _components.OfType<T>();
        }

        private String GetConfigurationToken(String path)
        {
            return _configDoc.SelectSingleNode("Configuration/" + path).InnerText;
        }

        private void UpdateControllerList()
        {
            AggregateCatalog aggCat = new AggregateCatalog();
            foreach (String pluginDirectory in _pluginDirectories)
            {
                DirectoryCatalog cat = new DirectoryCatalog(pluginDirectory);
                aggCat.Catalogs.Add(cat);
            }

            _compositionContainer = new CompositionContainer(aggCat);
            try
            {
                _compositionContainer.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }

            _components = new List<IParseable>();
            // For older configuration files. At some point, we will need to take this out and just say screw it.
            if (_configDoc.SelectNodes("Configuration/Components").Count > 0)
            {
                foreach (IParseable component in ParseChildrenComponents(_configDoc.SelectSingleNode("Configuration/Components"), ParseableElementType.IOInterface))
                {
                    AddComponent(component);
                }
            }

            // For newer configuration files for the IO interfaces.
            if (_configDoc.SelectNodes("Configuration/IOInterfaces").Count > 0)
            {
                foreach (IParseable component in ParseChildrenComponents(_configDoc.SelectSingleNode("Configuration/IOInterfaces"), ParseableElementType.IOInterface))
                {
                    AddComponent(component);
                }
            }

            // Reads in the apps.
            foreach (IParseable component in ParseChildrenComponents(_configDoc.SelectSingleNode("Configuration/Apps"), ParseableElementType.App))
            {
                AddComponent(component);
            }

            _components.ForEach(component => InitializeComponents(component));
            OnInitializationComplete();
        }

        private void InitializeComponents(IParseable component)
        {
            component.Initialize(this);
            component.Children.ToList().ForEach(com => InitializeComponents(com));
        }

        private IEnumerable<IParseable> ParseChildrenComponents(XmlNode parent, ParseableElementType typeToLookFor)
        {
            List<XmlNode> extraNodes = new List<XmlNode>();
            foreach (XmlNode componentNode in parent.ChildNodes)
            {
                if (!ParseIncludeNode(extraNodes, componentNode))
                {
                    yield return ParseComponent(componentNode, typeToLookFor);
                }
            }

            while (extraNodes.Any())
            {
                List<XmlNode> newExtraNodes = new List<XmlNode>();
                foreach (XmlNode extraNode in extraNodes)
                {
                    if (!ParseIncludeNode(newExtraNodes, extraNode))
                    {
                        yield return ParseComponent(extraNode, typeToLookFor);
                    }
                }
                extraNodes = newExtraNodes;
            }
        }

        private static bool ParseIncludeNode(List<XmlNode> extraNodes, XmlNode componentNode)
        {
            if (componentNode.LocalName.Equals("SAMIInclude"))
            {
                XmlDocument extraNodesDoc = new XmlDocument();
                extraNodesDoc.Load(componentNode.Attributes["href"].Value);
                String path = "//";
                if ((componentNode as XmlElement).HasAttribute("path"))
                {
                    path = componentNode.Attributes["path"].Value;
                }
                foreach (XmlNode extraNode in extraNodesDoc.SelectNodes(path + "|" + path + "/../SAMIInclude"))
                {
                    extraNodes.Add(extraNode);
                }
                return true;
            }
            return false;
        }

        private IParseable ParseComponent(XmlNode componentNode, ParseableElementType typeToLookFor)
        {
            Lazy<IParseable, IParseableMetadata> parseable;
            try
            {
                parseable = _compositionContainer.GetExport<IParseable, IParseableMetadata>(componentNode.Name);
            }
            catch (ImportCardinalityMismatchException)
            {
                throw new InvalidOperationException(String.Format("Could not find any components with the name {0}.", componentNode.Name));
            }
            if (parseable.Metadata.Type != typeToLookFor)
            {
                switch (parseable.Metadata.Type)
                {
                    case ParseableElementType.App:
                        throw new InvalidOperationException(String.Format("There was an error while processing a declaration for the app {0}. Ensure that {0} is declared at the top level of the Apps tag in the configuration file.", componentNode.Name));
                    case ParseableElementType.IOInterface:
                        throw new InvalidOperationException(String.Format("There was an error while processing a declaration for the IO interface {0}. Ensure that {0} is declared at the top level of the IO Interfaces tag in the configuration file.", componentNode.Name));
                    case ParseableElementType.Support:
                        throw new InvalidOperationException(String.Format("There was an error while processing a declaration for the support component {0}. Ensure that {0} is declared underneath an app or an IO interface.", componentNode.Name));
                    default:
                        throw new NotImplementedException(String.Format("The component type {0} is not supported!", typeToLookFor.ToString()));
                }
            }
            IParseable component = parseable.Value;
            foreach (XmlAttribute attr in componentNode.Attributes)
            {
                PersistentProperty prop = component.Properties.Single(p => p.Name.Equals(attr.Name));
                prop.Setter(attr.Value);
            }
            foreach (IParseable childComponent in ParseChildrenComponents(componentNode, ParseableElementType.Support))
            {
                component.AddChild(childComponent);
            }
            return component;
        }

        /// <summary>
        /// Fires after Initialize has been called on all components.
        /// This should not change any settings on a component, but may
        /// start processes that depend on all components being initialized.
        /// </summary>
        public event EventHandler InitializationComplete;

        private void OnInitializationComplete()
        {
            if (InitializationComplete != null)
            {
                InitializationComplete(this, new EventArgs());
            }
        }

        internal void AddComponent(IParseable component)
        {
            _components.Add(component);
        }


        public void SaveConfiguration()
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", null));
            XmlElement configNode = doc.CreateElement("Configuration");

            configNode.AppendChild(CreateGeneralNode(doc));
            configNode.AppendChild(CreateAppsNode(doc));
            configNode.AppendChild(CreateIOInterfacesNode(doc));

            doc.AppendChild(configNode);
            doc.Save(_configFilePath);
        }

        private XmlElement CreateGeneralNode(XmlDocument doc)
        {
            XmlElement generalNode = doc.CreateElement("General");

            XmlElement cityNode = doc.CreateElement("City");
            cityNode.InnerText = LocalLocation.City;
            generalNode.AppendChild(cityNode);

            XmlElement stateNode = doc.CreateElement("State");
            stateNode.InnerText = LocalLocation.State;
            generalNode.AppendChild(stateNode);

            XmlElement zipNode = doc.CreateElement("ZipCode");
            zipNode.InnerText = LocalLocation.ZipCode.ToString();
            generalNode.AppendChild(zipNode);

            XmlElement versionNode = doc.CreateElement("CurrentVersion");
            versionNode.InnerText = "0.1.0.0";
            generalNode.AppendChild(versionNode);

            XmlElement xbeeNode = doc.CreateElement("XBeeCOM");
            xbeeNode.InnerText = XBeeCOM;
            generalNode.AppendChild(xbeeNode);

            XmlElement zwaveNode = doc.CreateElement("ZWaveCOM");
            zwaveNode.InnerText = ZWaveCOM;
            generalNode.AppendChild(zwaveNode);

            return generalNode;
        }

        private XmlElement CreateAppsNode(XmlDocument doc)
        {
            XmlElement appsNode = doc.CreateElement("Apps");

            foreach (IApp app in Components.OfType<IApp>())
            {
                appsNode.AppendChild(CreateIParseableNode(doc, app));
            }

            return appsNode;
        }

        private XmlElement CreateIOInterfacesNode(XmlDocument doc)
        {
            XmlElement ioInterfacesNode = doc.CreateElement("IOInterfaces");

            foreach (IIOInterface ioInterface in Components.OfType<IIOInterface>())
            {
                ioInterfacesNode.AppendChild(CreateIParseableNode(doc, ioInterface));
            }

            return ioInterfacesNode;
        }

        private XmlElement CreateIParseableNode(XmlDocument doc, IParseable parseable)
        {
            ParseableElementAttribute metadata = parseable.GetType().GetCustomAttribute<ParseableElementAttribute>();
            XmlElement parseableElem = doc.CreateElement(metadata.Name);
            foreach (PersistentProperty prop in parseable.Properties)
            {
                parseableElem.SetAttribute(prop.Name, prop.Getter());
            }
            foreach (IParseable child in parseable.Children)
            {
                parseableElem.AppendChild(CreateIParseableNode(doc, child));
            }
            return parseableElem;
        }
    }
}
