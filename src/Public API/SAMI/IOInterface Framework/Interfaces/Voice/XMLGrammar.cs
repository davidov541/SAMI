using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using SAMI.Configuration;

namespace SAMI.IOInterfaces.Interfaces.Voice
{
    /// <summary>
    /// Represents a single grammar that can be 
    /// recognized fully by SAMI.
    /// </summary>
    public class XmlGrammar
    {
        /// <summary>
        /// Name of the rule.
        /// This is how conversations refer to this grammar
        /// when defining which grammar should be used.
        /// </summary>
        public String RuleName
        {
            get;
            private set;
        }

        /// <summary>
        /// The XML Document which represents the grammar.
        /// </summary>
        public XmlDocument RootDocument
        {
            get;
            private set;
        }

        /// <summary>
        /// The list of XmlElements which make up all of the rules
        /// that this grammar depends on.
        /// </summary>
        internal IEnumerable<XmlElement> Rules
        {
            get
            {
                foreach (XmlElement element in RootDocument.LastChild.ChildNodes)
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Constructor for XMLGrammar.
        /// </summary>
        /// <param name="ruleName">Name of this rule.</param>
        /// <param name="rootDocument">XMLDocument representing this grammar.</param>
        public XmlGrammar(String ruleName, XmlDocument rootDocument)
        {
            RuleName = ruleName;
            RootDocument = rootDocument;
            FormatXmlGrammar();
        }

        /// <summary>
        /// Constructor for XMLGrammar.
        /// </summary>
        /// <param name="ruleName">Name of this rule.</param>
        /// <param name="documentPath">Path to the file which represents this grammar.</param>
        public XmlGrammar(IConfigurationManager configManager, String ruleName, String documentPath, Type owningType)
        {
            RuleName = ruleName;
            RootDocument = new XmlDocument();
            RootDocument.Load(configManager.GetPathForFile(documentPath, owningType));
            FormatXmlGrammar();
        }

        /// <summary>
        /// Constructor for XMLGrammar.
        /// The rule name is set to be the value of root in the grammar.
        /// </summary>
        /// <param name="documentPath">Path to the file which represents this grammar.</param>
        public XmlGrammar(IConfigurationManager configManager, String documentPath, Type owningType)
        {
            RootDocument = new XmlDocument();
            RootDocument.Load(configManager.GetPathForFile(documentPath, owningType));
            RuleName = RootDocument.LastChild.Attributes["root"].Value;
            FormatXmlGrammar();
        }

        /// <summary>
        /// Constructor for XMLGrammar.
        /// The rule name is set to be the value of root in the grammar.
        /// </summary>
        /// <param name="rootDocument">XMLDocument representing this grammar.</param>
        public XmlGrammar(XmlDocument rootDocument)
        {
            RootDocument = rootDocument;
            RuleName = RootDocument.LastChild.Attributes["root"].Value;
            FormatXmlGrammar();
        }

        private void FormatXmlGrammar()
        {
            foreach (XmlElement itemElement in RootDocument.GetElementsByTagName("item"))
            {
                String innerText = String.Empty;
                foreach (XmlNode node in itemElement.ChildNodes)
                {
                    if (node.NodeType == XmlNodeType.Text)
                    {
                        String oldItem = node.InnerText.Trim();
                        String[] words = oldItem.Split(' ');
                        String newItem = String.Empty;
                        for (int i = 0; i < words.Length; i++)
                        {
                            int num;
                            if (Int32.TryParse(words[i], out num))
                            {
                                words[i] = ConvertIntegerToWords(num);
                            }
                            newItem += words[i] + " ";
                        }
                        newItem.Trim();
                        node.InnerText = newItem;
                    }
                }
            }
        }

        private String ConvertIntegerToWords(int number)
        {
            if (number == 0)
            {
                return "zero";
            }

            if (number < 0)
            {
                return "minus " + ConvertIntegerToWords(Math.Abs(number));
            }

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += ConvertIntegerToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += ConvertIntegerToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += ConvertIntegerToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (!String.IsNullOrEmpty(words))
                {
                    words += "and ";
                }

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                {
                    words += unitsMap[number];
                }
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                    {
                        words += "-" + unitsMap[number % 10];
                    }
                }
            }

            return words;
        }
    }
}
