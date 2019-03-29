using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps.Alarm;
using SAMI.Test.Utilities;

namespace AlarmAppTests
{
    [DeploymentItem("AlarmGrammar.grxml")]
    [TestClass]
    public class AlarmGrammarTests : BaseGrammarTests
    {
        [TestMethod]
        public void TestAlarmDurationSettingGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Alarm"},
                {"SubCommand", "Duration"},
                {"Hours", "0"},
                {"Minutes", "5"},
                {"Seconds", "0"},
            };
            TestGrammar<AlarmApp>("Set an alarm for five minutes", expectedParams);
        }

        [TestMethod]
        public void TestTimerDurationSettingGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Alarm"},
                {"SubCommand", "Duration"},
                {"Hours", "0"},
                {"Minutes", "5"},
                {"Seconds", "0"},
            };
            TestGrammar<AlarmApp>("Set a timer for five minutes", expectedParams);
        }

        [TestMethod]
        public void TestAlarmAbsoluteSettingGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Alarm"},
                {"SubCommand", "Time"},
                {"Time", "Hours=12;Minutes=0;TimeOfDay=pm;"},
            };
            TestGrammar<AlarmApp>("Set an alarm for noon", expectedParams);
        }

        [TestMethod]
        public void TestTimerAbsoluteSettingGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Alarm"},
                {"SubCommand", "Time"},
                {"Time", "Hours=12;Minutes=0;TimeOfDay=pm;"},
            };
            TestGrammar<AlarmApp>("Set a timer for noon", expectedParams);
        }

        [TestMethod]
        public void TestListAlarmsGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Alarm"},
                {"SubCommand", "List"},
            };
            TestGrammar<AlarmApp>("List the alarms", expectedParams);
        }

        [TestMethod]
        public void TestListTimersGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Alarm"},
                {"SubCommand", "List"},
            };
            TestGrammar<AlarmApp>("List the timers", expectedParams);
        }

        [TestMethod]
        public void TestListAlarmsRunningGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Alarm"},
                {"SubCommand", "List"},
            };
            TestGrammar<AlarmApp>("What alarms are running", expectedParams);
        }

        [TestMethod]
        public void TestListTimersRunningGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Alarm"},
                {"SubCommand", "List"},
            };
            TestGrammar<AlarmApp>("What timers are running", expectedParams);
        }

        [TestMethod]
        public void TestRemoveAllAlarmsGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Alarm"},
                {"SubCommand", "DeleteAll"},
            };
            TestGrammar<AlarmApp>("Remove all alarms", expectedParams);
        }

        [TestMethod]
        public void TestRemoveAllTimersGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Alarm"},
                {"SubCommand", "DeleteAll"},
            };
            TestGrammar<AlarmApp>("Remove all timers", expectedParams);
        }

        [TestMethod]
        public void TestDeleteAllAlarmsGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Alarm"},
                {"SubCommand", "DeleteAll"},
            };
            TestGrammar<AlarmApp>("Delete all alarms", expectedParams);
        }

        [TestMethod]
        public void TestDeleteAllTimersGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Alarm"},
                {"SubCommand", "DeleteAll"},
            };
            TestGrammar<AlarmApp>("Delete all timers", expectedParams);
        }

        [TestMethod]
        public void TestRemoveLastAlarmGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Alarm"},
                {"SubCommand", "DeleteLast"},
            };
            TestGrammar<AlarmApp>("Remove the last alarm", expectedParams);
        }

        [TestMethod]
        public void TestRemoveLastTimerGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Alarm"},
                {"SubCommand", "DeleteLast"},
            };
            TestGrammar<AlarmApp>("Remove the last timer", expectedParams);
        }

        [TestMethod]
        public void TestDeleteLastAlarmGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Alarm"},
                {"SubCommand", "DeleteLast"},
            };
            TestGrammar<AlarmApp>("Delete the last alarm", expectedParams);
        }

        [TestMethod]
        public void TestDeleteLastTimerGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Alarm"},
                {"SubCommand", "DeleteLast"},
            };
            TestGrammar<AlarmApp>("Delete the last timer", expectedParams);
        }
    }
}
