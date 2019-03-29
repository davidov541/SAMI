using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps.Current;
using SAMI.Test.Utilities;

namespace SAMI.Test.Current
{
    [DeploymentItem("CurrentGrammar.grxml")]
    [TestClass]
    public class CurrentGrammarTests : BaseGrammarTests
    {
        [TestMethod]
        public void TestWhatTimeIsItGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "current"},
                {"Param", "time"},
            };
            TestGrammar<CurrentApp>("what time is it", expectedParams);
        }

        [TestMethod]
        public void TestWhatIsTodaysDateGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "current"},
                {"Param", "date"},
            };
            TestGrammar<CurrentApp>("what's today's date", expectedParams);
        }

        [TestMethod]
        public void TestWhatDayOfTheWeekIsItGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "current"},
                {"Param", "day"},
            };
            TestGrammar<CurrentApp>("what day of the week is it", expectedParams);
        }
    }
}
