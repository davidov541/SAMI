using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using SAMI.Configuration;
using SAMI.IOInterfaces;
using SAMI.Persistence;

namespace SAMI.Apps
{
    /// <summary>
    /// Indicates a reference to an IO Resource that an app may use.
    /// These should be used for apps that may have access to multiple IO resources
    /// that can do its work, but it should not use all of them at once.
    /// </summary>
    [ParseableElement("Reference", ParseableElementType.Support)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class IOInterfaceReference : IParseable
    {
        private IConfigurationManager _configManager;

        /// <summary>
        /// Name of the IO Interface to which this reference refers.
        /// </summary>
        public String Name
        {
            get;
            private set;
        }

        /// <summary>
        /// String which categorizes this reference.
        /// This can be used by apps that have different types of references, in order to
        /// classify which type of reference this one is.
        /// This may be left empty if not used by the app.
        /// </summary>
        public String Tag
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates whether this IParseable instance is valid.
        /// </summary>
        public bool IsValid
        {
            get 
            {
                return _configManager != null && _configManager.FindAllComponentsOfType<IIOInterface>().Any(s => s.Name.Equals(Name));
            }
        }

        /// <summary>
        /// Returns a list of properties that are serializeable in the XML representation of this class.
        /// </summary>
        public IEnumerable<PersistentProperty> Properties
        {
            get 
            {
                yield return new PersistentProperty("Tag", () => Tag, t => Tag = t);
                yield return new PersistentProperty("References", () => Name, n => Name = n);
            }
        }

        /// <summary>
        /// Returns a list of children that should be serialized inside of the tag for this class.
        /// </summary>
        public IEnumerable<IParseable> Children
        {
            get 
            {
                yield break;
            }
        }

        /// <summary>
        /// Default constructor for Reference.
        /// </summary>
        /// <param name="manager">Configuration manager that owns this component.</param>
        public IOInterfaceReference()
        {
            Tag = String.Empty;
            Name = String.Empty;
        }

        /// <summary>
        /// Default constructor for Reference.
        /// </summary>
        /// <param name="manager">Configuration manager that owns this component.</param>
        public IOInterfaceReference(String tag, String name, IConfigurationManager manager)
            : this()
        {
            Tag = tag;
            Name = name;
        }

        /// <summary>
        /// Initializes this component, once all children have been created.
        /// </summary>
        public void Initialize(IConfigurationManager manager)
        {
            _configManager = manager;
        }

        /// <summary>
        /// Adds a child to this component.
        /// </summary>
        /// <param name="child">Child to add.</param>
        public void AddChild(IParseable child)
        {
        }

        /// <summary>
        /// Disposes of this object. Should only be called once, when the component is being let go.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
