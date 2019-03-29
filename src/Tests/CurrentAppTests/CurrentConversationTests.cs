using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps.Current;
using SAMI.Test.Utilities;

namespace SAMI.Test.Current
{
    [TestClass]
    public class CurrentConversationTests : BaseConversationTests
    {
        [TestMethod]
        public void TestCurrentTimeConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "current"},
                {"Param", "time"},
            };
            String response = RunSingleConversation<CurrentConversation>(input);
            Assert.IsTrue(Regex.IsMatch(response, "It is [^.]*."), response + " did not match.");
        }

        [TestMethod]
        public void TestCurrentDateConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "current"},
                {"Param", "date"},
            };
            String response = RunSingleConversation<CurrentConversation>(input);
            Assert.IsTrue(Regex.IsMatch(response, "Today is [^.]*."), response + " did not match.");
        }

        [TestMethod]
        public void TestCurrentDayOfWeekConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "current"},
                {"Param", "day"},
            };
            String response = RunSingleConversation<CurrentConversation>(input);
            Assert.IsTrue(Regex.IsMatch(response, "Today is a [^.]*."), response + " did not match.");
        }
    }
}
