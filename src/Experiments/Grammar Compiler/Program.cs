using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GrammarCompiler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Count() < 2)
            {
                Console.WriteLine("ERROR -- Expected directory to compile from, and file to create from compiled grammars.");
                return;
            }

            List<XmlDocument> fileDocs = new List<XmlDocument>();
            String ns = "http://www.w3.org/2001/06/grammar";
            Console.WriteLine("Looking in directory {0} for GRXML files.", args[0]);
            foreach (String file in Directory.GetFiles(args[0], "*.grxml", SearchOption.AllDirectories))
            {
                XmlDocument fileDoc = new XmlDocument();
                fileDoc.Load(file);
                fileDocs.Add(fileDoc);
                Console.WriteLine("Combining file {0}.", file);
            }
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateNode(XmlNodeType.XmlDeclaration, "", ""));

            XmlElement grammar = CreateElement(ns, doc, "grammar", new Dictionary<String, String>
            {
                { "version", "1.0" },
                { "xml:lang", "en-US" },
                { "mode", "voice" },
                { "root", "overallCommand" },
                { "tag-format", "semantics/1.0" },
            });
            XmlElement overallRule = CreateElement(ns, doc, "rule", new Dictionary<String, String>
            {
                { "id", "overallCommand" },
                { "scope", "public" },
            });
            XmlElement samiRule = CreateElement(ns, doc, "item", new Dictionary<String, String>());
            samiRule.InnerText = "Sammie";
            overallRule.AppendChild(samiRule);
            XmlElement oneofRule = CreateElement(ns, doc, "one-of", new Dictionary<String, String>());
            foreach (XmlDocument document in fileDocs)
            {
                if (document.LastChild.Attributes.GetNamedItem("root") != null)
                {
                    XmlElement item = CreateElement(ns, doc, "item", new Dictionary<String, String>());
                    XmlElement ruleref = CreateElement(ns, doc, "ruleref", new Dictionary<String, String>
                    {
                        { "uri", String.Format("#{0}", document.LastChild.Attributes["root"].Value) },
                    });
                    item.AppendChild(ruleref);
                    oneofRule.AppendChild(item);
                }
            }
            overallRule.AppendChild(oneofRule);
            grammar.AppendChild(overallRule);

            foreach (XmlDocument fileDoc in fileDocs)
            {
                foreach (XmlNode node in fileDoc.LastChild.ChildNodes)
	            {
                    XmlNode copiedNode = doc.ImportNode(node, true);
                    grammar.AppendChild(copiedNode);
	            }
            }
            doc.AppendChild(grammar);
            doc.Save(args[1]);
            Console.WriteLine("Output file to {0}.", args[1]);
        }

        private static XmlElement CreateElement(String ns, XmlDocument doc, String elementName, Dictionary<String, String> attributes)
        {
            XmlElement elem = doc.CreateElement(elementName, ns);
            foreach (KeyValuePair<String, String> attributeVal in attributes)
            {
                XmlAttribute attr = doc.CreateAttribute(attributeVal.Key);
                attr.Value = attributeVal.Value;
                elem.Attributes.Append(attr);
            }
            return elem;
        }
    }
}
