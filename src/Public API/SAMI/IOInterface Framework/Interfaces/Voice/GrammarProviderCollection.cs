using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SAMI.IOInterfaces.Interfaces.Voice
{
    /// <summary>
    /// Represents a collection of GrammarProviders
    /// that are available for use by speech sensors.
    /// </summary>
    public class GrammarProviderCollection
    {
        private List<GrammarProvider> _grammarProviders = new List<GrammarProvider>();
        private List<String> _utilityFiles = new List<String>
        {
            "UtilityGrammar.grxml", 
            "DateTimeGrammar.grxml"
        };

        /// <summary>
        /// Event that is fired when one of the grammars
        /// represented by this collection changes.
        /// </summary>
        public event EventHandler GrammarChanged;

        /// <summary>
        /// Indicates if any of the grammars in this collection
        /// are available.
        /// </summary>
        public bool IsValidGrammar
        {
            get
            {
                return _grammarProviders.Any();
            }
        }

        /// <summary>
        /// A list of all of the grammars that are available for use, but are not
        /// part of the main grammar.
        /// </summary>
        public IEnumerable<XmlGrammar> ExtraGrammars
        {
            get
            {
                return _grammarProviders.SelectMany(provider => provider.ExtraGrammars);
            }
        }

        /// <summary>
        /// Adds a grammar provider to the collection.
        /// </summary>
        /// <param name="provider">Provider to add.</param>
        public void AddGrammarProvider(GrammarProvider provider)
        {
            provider.GrammarChanged += Provider_GrammarChanged;
            _grammarProviders.Add(provider);
            OnGrammarChanged(new EventArgs());
        }

        /// <summary>
        /// Removes a grammar provider from the collection.
        /// </summary>
        /// <param name="provider">Provider to remove.</param>
        public void RemoveGrammarProvider(GrammarProvider provider)
        {
            _grammarProviders.Remove(provider);
            OnGrammarChanged(new EventArgs());
        }

        /// <summary>
        /// Triggers the GrammarChanged event.
        /// </summary>
        /// <param name="e">EventArgs instance for this event. This parameter is currently not used.</param>
        protected void OnGrammarChanged(EventArgs e)
        {
            if (GrammarChanged != null)
            {
                GrammarChanged(this, e);
            }
        }

        private void Provider_GrammarChanged(object sender, EventArgs e)
        {
            OnGrammarChanged(e);
        }

        /// <summary>
        /// Creates a grammar file representing all of the main grammars of the grammar providers
        /// to the file requested. 
        /// </summary>
        /// <param name="fileToSaveTo">File to save the main grammar to.</param>
        public void UpdateMainGrammarFile(String fileToSaveTo)
        {
            List<String> rules = new List<string>();
            List<XmlElement> elements = new List<XmlElement>();

            // Add grammars from all of our grammar providers.
            foreach (GrammarProvider provider in _grammarProviders)
            {
                if (provider.MainGrammar != null)
                {
                    rules.Add(provider.MainGrammar.RuleName);
                    elements.AddRange(provider.MainGrammar.Rules);
                }
            }

            // Add utility rules.
            foreach (String fileName in _utilityFiles)
            {
                XmlDocument fileDoc = new XmlDocument();
                fileDoc.Load(fileName);
                foreach (XmlElement element in fileDoc.LastChild.ChildNodes)
                {
                    elements.Add(element);
                }
            }

            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateNode(XmlNodeType.XmlDeclaration, "", ""));

            XmlElement grammar = GrammarUtility.CreateElement(doc, "grammar", new Dictionary<String, String>
            {
                { "version", "1.0" },
                { "xml:lang", "en-US" },
                { "mode", "voice" },
                { "root", "overallCommand" },
                { "tag-format", "semantics/1.0" },
            });
            XmlElement overallRule = GrammarUtility.CreateElement(doc, "rule", new Dictionary<String, String>
            {
                { "id", "overallCommand" },
                { "scope", "public" },
            });
            XmlElement samiRule = GrammarUtility.CreateElement(doc, "item", new Dictionary<String, String>());
            samiRule.InnerText = "Sammie";
            overallRule.AppendChild(samiRule);

            overallRule.AppendChild(GrammarUtility.CreateListOfRuleRefs(doc, rules));
            grammar.AppendChild(overallRule);

            GrammarUtility.CopyElementsToNewRule(grammar, elements);
            doc.AppendChild(grammar);
            doc.Save(fileToSaveTo);
        }
    }
}
