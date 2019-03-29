using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps.Jokes;
using SAMI.Test.Utilities;

namespace SAMI.Tests.Jokes
{
    [TestClass]
    public class JokesConversationTests : BaseConversationTests
    {
        [TestMethod]
        public void TestCashewJokeConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "jokes"},
            };
            Assert.AreEqual("Knock knock", RunSingleConversation<CashewJokesConversation>(input, false));
            Assert.AreEqual("Cash", RunSingleConversation<CashewJokesConversation>(input, false));
            Assert.AreEqual("No thanks, but I’d like some peanuts!", RunSingleConversation<CashewJokesConversation>(input));
        }

        [TestMethod]
        public void TestCashewJokeGrammarRules()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "jokes"},
            };
            RunSingleConversation<CashewJokesConversation>(input, false);
            Assert.AreEqual("whosThereRule", CurrentConversation.GrammarRuleName);
            RunSingleConversation<CashewJokesConversation>(input, false);
            Assert.AreEqual("cashWhoRule", CurrentConversation.GrammarRuleName);
            RunSingleConversation<CashewJokesConversation>(input);
        }

        [TestMethod]
        public void TestOneLinerJokeConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "jokes"},
            };
            CurrentConversation = new OneLinerConversation(GetConfigurationManager(), "A really funny joke here");
            Assert.AreEqual("A really funny joke here", RunSingleConversation<CashewJokesConversation>(input));
        }
    }
}
