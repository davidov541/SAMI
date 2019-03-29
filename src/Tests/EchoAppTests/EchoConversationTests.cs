using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps.Echo;
using SAMI.Test.Utilities;

namespace SAMI.Tests.Echo
{
    [TestClass]
    public class EchoConversationTests : BaseConversationTests
    {
        [TestMethod]
        public void TestEchoHelloConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                { "Command", "echo" },
                { "Param", "hello" }
            };
            Assert.AreEqual("hello", RunSingleConversation<EchoConversation>(input));
        }

        [TestMethod]
        public void TestEchoHowAreYouConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                { "Command", "echo" },
                { "Param", "how are you?" }
            };
            Assert.AreEqual("how are you?", RunSingleConversation<EchoConversation>(input));
        }
    }
}
