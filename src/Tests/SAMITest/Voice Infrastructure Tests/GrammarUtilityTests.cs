using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Test.Utilities;

namespace SAMI.Test.Voice_Infrastructure_Tests
{
    [DeploymentItem(formattedFilePath)]
    [TestClass]
    public class GrammarUtilityTests : BaseSAMITests
    {
        private const String formattedFileName = "FormattedTestGrammarFile.xml";
        private const String formattedFilePath = @"Voice Infrastructure Tests\" + formattedFileName;

        [TestMethod]
        public void CreateGrammarFromListSuccess()
        {
            List<String> testListValues = new List<String>
            {
                "Test Item One",
                "Test Item Two",
                "Test Item Three",
            };
            XmlDocument doc = new XmlDocument();
            doc.Load(formattedFileName);
            XmlGrammar baseGrammar = new XmlGrammar(doc);

            XmlGrammar grammar = GrammarUtility.CreateGrammarFromList(formattedFileName, "testList",  testListValues);

            Assert.AreEqual(5, grammar.Rules.Count());
            for(int i = 0; i < baseGrammar.Rules.Count() - 1; i++)
            {
                Assert.AreEqual(baseGrammar.Rules.ElementAt(i).InnerXml, grammar.Rules.ElementAt(i).InnerXml);
            }
            XmlElement elem = grammar.Rules.Last();
            Assert.AreEqual("testList", elem.Attributes["id"].Value);
            XmlElement oneOfElem = elem.FirstChild as XmlElement;
            Assert.AreEqual(3, oneOfElem.ChildNodes.Count);
            Assert.AreEqual(testListValues[0], oneOfElem.ChildNodes[0].FirstChild.Value.Trim());
            Assert.AreEqual(testListValues[1], oneOfElem.ChildNodes[1].FirstChild.Value.Trim());
            Assert.AreEqual(testListValues[2], oneOfElem.ChildNodes[2].FirstChild.Value.Trim());
        }

        [TestMethod]
        public void CreateGrammarFromListNoItems()
        {
            List<String> testListValues = new List<String>();
            XmlDocument doc = new XmlDocument();
            doc.Load(formattedFileName);
            XmlGrammar baseGrammar = new XmlGrammar(doc);

            XmlGrammar grammar = GrammarUtility.CreateGrammarFromList(formattedFileName, "testList", testListValues);

            Assert.AreEqual(5, grammar.Rules.Count());
            for (int i = 0; i < baseGrammar.Rules.Count() - 1; i++)
            {
                Assert.AreEqual(baseGrammar.Rules.ElementAt(i).InnerXml, grammar.Rules.ElementAt(i).InnerXml);
            }
            XmlElement elem = grammar.Rules.Last();
            Assert.AreEqual("testList", elem.Attributes["id"].Value);
            XmlElement oneOfElem = elem.FirstChild as XmlElement;
            Assert.AreEqual(1, oneOfElem.ChildNodes.Count);
            Assert.AreEqual("XYZ123", oneOfElem.ChildNodes[0].FirstChild.Value.Trim());
        }
    }
}
