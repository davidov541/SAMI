using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps;
using SAMI.Apps.Jokes;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Test.Utilities;

namespace SAMI.Tests.Jokes
{
    [TestClass]
    public class JokesConversationSelectionTests : BaseSAMITests
    {
        [TestMethod]
        public void TestJokeConversationSelectionBasic()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "jokes"},
            };
            Conversation convo;
            JokesApp app = new JokesApp();
            app.Initialize(GetConfigurationManager());
            app.TryCreateConversationFromPhrase(new Dialog(input, "Test Phrase"), out convo);
            Assert.IsTrue(convo is CashewJokesConversation || convo is OneLinerConversation);
        }

        [TestMethod]
        public void TestJokeConversationSelectionNeverRepeats()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "jokes"},
            };
            JokesApp app = new JokesApp();
            app.Initialize(GetConfigurationManager());
            List<Conversation> convos = new List<Conversation>();

            for (int i = 0; i < app.NumberOfJokes; i++)
            {
                Conversation convo;
                app.TryCreateConversationFromPhrase(new Dialog(input, "Test Phrase"), out convo);
                convos.Add(convo);
            }
            Assert.AreEqual(app.NumberOfJokes, convos.Distinct().Count());
            foreach (JokeConversation convo in convos)
            {
                Assert.AreEqual(1, convos.Count(c => c.Equals(convo)));
            }
        }

        [TestMethod]
        public void TestJokeConversationSelectionLoops()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "jokes"},
            };
            JokesApp app = new JokesApp();
            app.Initialize(GetConfigurationManager());
            List<Conversation> convos = new List<Conversation>();

            for (int i = 0; i < app.NumberOfJokes * 2; i++)
            {
                Conversation convo;
                app.TryCreateConversationFromPhrase(new Dialog(input, "Test Phrase"), out convo);
                convos.Add(convo);
            }
            foreach (JokeConversation convo in convos)
            {
                Assert.AreEqual(2, convos.Count(c => c.Equals(convo)));
            }
        }
    }
}
