using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps.Jokes;
using SAMI.Test.Utilities;

namespace SAMI.Tests.Jokes
{
    [DeploymentItem("whosThereRule.grxml")]
    [DeploymentItem("cashWhoRule.grxml")]
    [DeploymentItem("JokesGrammar.grxml")]
    [TestClass]
    public class JokesGrammarTests : BaseGrammarTests
    {
        [TestMethod]
        public void TestTellMeAJokeGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "jokes"},
            };
            TestGrammar<JokesApp>("Can you tell me a joke?", expectedParams);
        }

        [TestMethod]
        public void TestWhosThereGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "jokes"},
            };
            TestGrammar<JokesApp>("Who's there?", expectedParams, "whosThereRule");
        }

        [TestMethod]
        public void TestCashWhoGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "jokes"},
            };
            TestGrammar<JokesApp>("Cash who?", expectedParams, "cashWhoRule");
        }

        [TestMethod]
        public void TestCashewJokeRequiredGrammars()
        {
            JokeConversation convo = new CashewJokesConversation(GetConfigurationManager());
            Assert.AreEqual(2, convo.GrammarsNeeded.Count());
            Assert.IsTrue(convo.GrammarsNeeded.Any(g => g.RuleName.Equals("cashWhoRule")));
            Assert.IsTrue(convo.GrammarsNeeded.Any(g => g.RuleName.Equals("whosThereRule")));
        }

        [TestMethod]
        public void TestOneLinerRequiredGrammars()
        {
            JokeConversation convo = new OneLinerConversation(GetConfigurationManager(), "A really funny joke");
            Assert.AreEqual(0, convo.GrammarsNeeded.Count());
        }
    }
}
