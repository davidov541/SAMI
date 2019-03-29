using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Test.Utilities;

namespace SAMI.Test.Infrastructure_Tests
{
    [DeploymentItem(unformattedFilePath)]
    [DeploymentItem(formattedFilePath)]
    [TestClass]
    public class XmlGrammarTests : BaseSAMITests
    {
        private const String unformattedFileName = "UnformattedTestGrammarFile.xml";
        private const String unformattedFilePath = @"Voice Infrastructure Tests\" + unformattedFileName;
        private const String formattedFileName = "FormattedTestGrammarFile.xml";
        private const String formattedFilePath = @"Voice Infrastructure Tests\" + formattedFileName;

        [TestMethod]
        public void TestXmlGrammarFormatForDocumentConstructor()
        {
            XmlDocument unformattedDoc = new XmlDocument();
            unformattedDoc.Load(unformattedFileName);
            XmlGrammar grammar = new XmlGrammar(unformattedDoc);

            XmlDocument formattedDoc = new XmlDocument();
            formattedDoc.Load(formattedFileName);
            Assert.AreEqual(formattedDoc.InnerXml, grammar.RootDocument.InnerXml);
        }

        [TestMethod]
        public void TestXmlGrammarFormatForDocumentNameConstructor()
        {
            XmlDocument unformattedDoc = new XmlDocument();
            unformattedDoc.Load(unformattedFileName);
            XmlGrammar grammar = new XmlGrammar("TestRule", unformattedDoc);

            XmlDocument formattedDoc = new XmlDocument();
            formattedDoc.Load(formattedFileName);
            Assert.AreEqual(formattedDoc.InnerXml, grammar.RootDocument.InnerXml);
        }

        [TestMethod]
        public void TestXmlGrammarFormatForConfigManagerConstructor()
        {
            IConfigurationManager configManager = GetConfigurationManager();
            XmlGrammar grammar = new XmlGrammar(configManager, unformattedFileName, GetType());

            XmlDocument formattedDoc = new XmlDocument();
            formattedDoc.Load(formattedFileName);
            Assert.AreEqual(formattedDoc.InnerXml, grammar.RootDocument.InnerXml);
        }

        [TestMethod]
        public void TestXmlGrammarFormatForConfigManagerNameConstructor()
        {
            IConfigurationManager configManager = GetConfigurationManager();
            XmlGrammar grammar = new XmlGrammar(configManager, "TestRuleName", unformattedFileName, GetType());

            XmlDocument formattedDoc = new XmlDocument();
            formattedDoc.Load(formattedFileName);
            Assert.AreEqual(formattedDoc.InnerXml, grammar.RootDocument.InnerXml);
        }

        [TestMethod]
        public void TestXmlGrammarRuleNameForConfigManagerConstructor()
        {
            IConfigurationManager configManager = GetConfigurationManager();
            XmlGrammar grammar = new XmlGrammar(configManager, unformattedFileName, GetType());

            Assert.AreEqual("overallCommand", grammar.RuleName);
        }

        [TestMethod]
        public void TestXmlGrammarRuleNameForDocumentConstructor()
        {
            IConfigurationManager configManager = GetConfigurationManager();
            XmlDocument unformattedDoc = new XmlDocument();
            unformattedDoc.Load(unformattedFileName);
            XmlGrammar grammar = new XmlGrammar(unformattedDoc);

            Assert.AreEqual("overallCommand", grammar.RuleName);
        }

        [TestMethod]
        public void TestXmlGrammarRulesList()
        {
            IConfigurationManager configManager = GetConfigurationManager();
            XmlDocument unformattedDoc = new XmlDocument();
            unformattedDoc.Load(unformattedFileName);
            XmlGrammar grammar = new XmlGrammar(unformattedDoc);

            List<String> expectedRules = new List<string>
            {
                "overallCommand",
                "movieCommand",
                "MovieName",
                "TheaterName",
            };
            Assert.AreEqual(expectedRules.Count, grammar.Rules.Count());
            for (int i = 0; i < grammar.Rules.Count(); i++)
            {
                Assert.AreEqual("rule", grammar.Rules.ElementAt(i).LocalName);
                Assert.AreEqual(expectedRules[i], grammar.Rules.ElementAt(i).Attributes["id"].Value);
            }
        }
    }
}
