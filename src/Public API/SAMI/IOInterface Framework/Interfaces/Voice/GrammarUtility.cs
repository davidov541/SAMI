using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace SAMI.IOInterfaces.Interfaces.Voice
{
    /// <summary>
    /// A static class filled with methods and properties
    /// that are useful for creating grammars dynamically for use
    /// with speech recognizers.
    /// </summary>
    public static class GrammarUtility
    {
        private static String Namespace = "http://www.w3.org/2001/06/grammar";

        /// <summary>
        /// Name of the main grammar that is used outside of conversations.
        /// </summary>
        public const String MainGrammarName = "Default";
        /// <summary>
        /// XmlDocument describing a grammar the user saying either yes or no.
        /// The returned value in out.answer will be a serialized boolean, indicating
        /// if the user responded with yes or not.
        /// </summary>
        public static XmlGrammar GetYesNoGrammar(String commandName)
        {
            return GetGenericTrueFalseGrammar(String.Format("{0}_yesNoCommand", commandName), commandName, new List<String> { "yes" }, new List<String> { "no" });
        }

        /// <summary>
        /// XmlDocument describing a grammar the user saying either ok or cancel.
        /// The returned value in out.answer will be a serialized boolean, indicating
        /// if the user responded with yes or not.
        /// </summary>
        public static XmlGrammar GetOKCancelGrammar(String commandName)
        {
            return GetGenericTrueFalseGrammar(String.Format("{0}_okCancelCommand", commandName), commandName, new List<String> { "o k" }, new List<String> { "cancel" });
        }

        private const String _baseGrammar = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                                                <grammar version=""1.0"" xml:lang=""en-US"" mode=""voice"" root=""{0}"" xmlns=""http://www.w3.org/2001/06/grammar"" tag-format=""semantics/1.0"">
                                                    <rule id=""{0}"" scope=""public"">  
                                                        <tag>out.Command=""{1}"";</tag>
                                                        <one-of>  
                                                        </one-of>
                                                    </rule>
                                                </grammar>";

        /// <summary>
        /// Returns a grammar where any of the values in <see cref="trueOptions"/> return the value true, 
        /// while any of the values in <see cref="falseOptions"/> return the value false.
        /// This should only be used in special cases, otherwise <see cref="GetOKCancelGrammar"/> or <see cref="GetYesNoGrammar"/>
        /// are better options.
        /// The grammar returned by this method is fully functional.
        /// </summary>
        /// <param name="ruleName">Name of the rule that you can refer to from other grammars or use as the current grammar.</param>
        /// <param name="commandName">Name of the command value to set. This should match what your app is looking for.</param>
        /// <param name="trueOptions">A list of user-sayable phrases which correspond to true.</param>
        /// <param name="falseOptions">A list of user-sayable phrases which correspond to false.</param>
        /// <returns>An XmlGrammar instance which represents the grammar requested.</returns>
        public static XmlGrammar GetGenericTrueFalseGrammar(String ruleName, String commandName, IEnumerable<String> trueOptions, IEnumerable<String> falseOptions)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(String.Format(_baseGrammar, ruleName, commandName));

            XmlElement oneOfTagElement = doc.LastChild.LastChild.LastChild as XmlElement;
            foreach (String option in trueOptions)
            {
                XmlElement itemElement = CreateElement(doc, "item", new Dictionary<string,string>());
                XmlElement tagElement = CreateElement(doc, "tag", new Dictionary<string,string>());
                tagElement.InnerText = "out.answer=\"true\";";
                itemElement.InnerText = option;
                itemElement.AppendChild(tagElement);
                oneOfTagElement.AppendChild(itemElement);
            }

            foreach (String option in falseOptions)
            {
                XmlElement itemElement = CreateElement(doc, "item", new Dictionary<string, string>());
                XmlElement tagElement = CreateElement(doc, "tag", new Dictionary<string, string>());
                tagElement.InnerText = "out.answer=\"false\";";
                itemElement.InnerText = option;
                itemElement.AppendChild(tagElement);
                oneOfTagElement.AppendChild(itemElement);
            }
            return new XmlGrammar(doc);
        }

        /// <summary>
        /// Creates an element with the given name and the given attributes.
        /// This is a generally useful XML method, and has no SAMI-specific uses.
        /// </summary>
        /// <param name="doc">XMLDocument that the element will be added to.</param>
        /// <param name="elementName">Name of the element to be created.</param>
        /// <param name="attributes">A dictionary of names and values of attributes to be added to the element.</param>
        /// <returns>The XmlElement instance that is created.</returns>
        public static XmlElement CreateElement(XmlDocument doc, String elementName, Dictionary<String, String> attributes)
        {
            XmlElement elem = doc.CreateElement(elementName, Namespace);
            foreach (KeyValuePair<String, String> attributeVal in attributes)
            {
                XmlAttribute attr = doc.CreateAttribute(attributeVal.Key);
                attr.Value = attributeVal.Value;
                elem.Attributes.Append(attr);
            }
            return elem;
        }

        /// <summary>
        /// Creates a rule which matches one of the rule names given. 
        /// </summary>
        /// <param name="doc">Document to which the rule should be added.</param>
        /// <param name="ruleNames">The list of names of the rules that might be matched.</param>
        /// <returns>Top level one-of element of the rule.</returns>
        public static XmlElement CreateListOfRuleRefs(XmlDocument doc, IEnumerable<String> ruleNames)
        {
            XmlElement oneofRule = GrammarUtility.CreateElement(doc, "one-of", new Dictionary<String, String>());
            foreach (String rule in ruleNames)
            {
                XmlElement item = GrammarUtility.CreateElement(doc, "item", new Dictionary<String, String>());
                XmlElement ruleref = GrammarUtility.CreateElement(doc, "ruleref", new Dictionary<String, String>
                {
                    { "uri", String.Format("#{0}", rule) },
                });
                item.AppendChild(ruleref);
                oneofRule.AppendChild(item);
            }
            return oneofRule;
        }

        /// <summary>
        /// Creates a rule which matches one of the given strings.
        /// The out value is then set to the value itself.
        /// </summary>
        /// <param name="doc">XMLDocument to add this rule to.</param>
        /// <param name="possibleVals">List of values that can be matched.</param>
        /// <returns>Element which represents the rule that has been created.</returns>
        public static XmlElement CreateListOfPossibleStrings(XmlDocument doc, IEnumerable<String> possibleVals)
        {
            XmlElement oneofRule = GrammarUtility.CreateElement(doc, "one-of", new Dictionary<String, String>());
            foreach (String possibleVal in possibleVals)
            {
                XmlElement item = GrammarUtility.CreateElement(doc, "item", new Dictionary<String, String>());
                item.InnerText = possibleVal;
                XmlElement tag = GrammarUtility.CreateElement(doc, "tag", new Dictionary<String, String>());
                tag.InnerText = "out = \"" + possibleVal + "\";";
                item.AppendChild(tag);
                oneofRule.AppendChild(item);
            }
            return oneofRule;
        }

        /// <summary>
        /// Copies the given elements into the rule indicated.
        /// </summary>
        /// <param name="newRule">Element that the elements should be added into.</param>
        /// <param name="elementsToCopy">Elements to add in the given element.</param>
        public static void CopyElementsToNewRule(XmlElement newRule, IEnumerable<XmlElement> elementsToCopy)
        {
            foreach (XmlElement node in elementsToCopy)
            {
                XmlNode copiedNode = newRule.OwnerDocument.ImportNode(node, true);
                newRule.AppendChild(copiedNode);
            }
        }

        /// <summary>
        /// Creates an XML grammar which uses the base XML file passed in, and adds a rule for the list indicated.
        /// </summary>
        /// <param name="baseXmlFilePath">Absolute file path to the XML file tat we should use as the base.</param>
        /// <param name="ruleName">The name of the rule we are creating from the list.</param>
        /// <param name="list">List of values that the new rule may be. This list may be empty.</param>
        /// <returns>XmlGrammar instance which can be used to describe a grammar with the given base and the list of values in a rule.</returns>
        public static XmlGrammar CreateGrammarFromList(String baseXmlFilePath, String ruleName, List<String> list)
        {
            List<XmlElement> remoteElements = new List<XmlElement>();
            if (!list.Any())
            {
                // If the list is empty, the grammar file becomes invalid.
                // Therefore, I will add a nonsensical string that will never be detect
                // to prevent the grammar from erroring.
                // A better solution should be found for this, but it's not too important
                // right now.
                list.Add("XYZ123");
            }
            XmlDocument mainDoc = new XmlDocument();
            mainDoc.Load(baseXmlFilePath);
            XmlElement oneof = GrammarUtility.CreateListOfPossibleStrings(mainDoc, list);
            XmlElement rule = GrammarUtility.CreateElement(mainDoc, "rule", new Dictionary<string, string>
                {
                    {"id", ruleName},
                    {"scope", "public"},
                });
            rule.AppendChild(oneof);
            mainDoc.LastChild.AppendChild(rule);
            return new XmlGrammar(mainDoc);
        }
    }
}
