using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps.Echo;
using SAMI.Test.Utilities;

namespace EchoAppTests
{
    [DeploymentItem("EchoGrammar.grxml")]
    [TestClass]
    public class EchoGrammarTests : BaseGrammarTests
    {
        [TestMethod]
        public void TestCanYouSayHelloGrammar()
        {
            Dictionary<String, String> expectedOutput = new Dictionary<string, string>
            {
                { "Command", "echo" },
                { "Param", "hello" }
            };
            TestGrammar<EchoApp>("can you say hello", expectedOutput);
        }

        [TestMethod]
        public void TestGoodMorningGrammar()
        {
            Dictionary<String, String> expectedOutput = new Dictionary<string, string>
            {
                { "Command", "echo" },
                { "Param", "Good Morning" }
            };
            TestGrammar<EchoApp>("good morning", expectedOutput);
        }

        [TestMethod]
        public void TestGoodNightGrammar()
        {
            Dictionary<String, String> expectedOutput = new Dictionary<string, string>
            {
                { "Command", "echo" },
                { "Param", "Good Night" }
            };
            TestGrammar<EchoApp>("good night", expectedOutput);
        }

        [TestMethod]
        public void TestIntroductionGrammar()
        {
            Dictionary<String, String> expectedOutput = new Dictionary<string, string>
            {
                { "Command", "echo" },
                { "Param", "My name is Sammie. I am an extensible personal assistant who can give you the next showtime at the theater, while also controling various devices around the house. Feel free to ask me anything, and I will have an answer for you." }
            };
            TestGrammar<EchoApp>("Can you introduce yourself", expectedOutput);
        }
    }
}
