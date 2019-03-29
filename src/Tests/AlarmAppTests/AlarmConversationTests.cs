using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Timers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps;
using SAMI.Apps.Alarm;
using SAMI.Test.Utilities;

namespace AlarmAppTests
{
    [TestClass]
    public class AlarmConversationTests : BaseConversationTests
    {
        [TestMethod]
        public void TestSetAlarmDurationConversation()
        {
            Mock<IAlarmManager> alarmMan = new Mock<IAlarmManager>(MockBehavior.Strict);
            CurrentConversation = new AlarmConversation(GetConfigurationManager(), alarmMan.Object);
            alarmMan.Setup(s => s.AddAlarm("Your 5 minute timer is complete", new TimeSpan(0, 5, 0)));

            Dictionary<String, String> input = new Dictionary<string, string>
                {
                    {"Command", "Alarm"},
                    {"SubCommand", "Duration"},
                    {"Hours", "0"},
                    {"Minutes", "5"},
                    {"Seconds", "0"},
                };
            Assert.AreEqual("The timer has been set to 5 minutes.", RunSingleConversation<AlarmConversation>(input));
            alarmMan.Verify(s => s.AddAlarm("Your 5 minute timer is complete", new TimeSpan(0, 5, 0)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestSetAlarmAbsoluteTimeConversation()
        {
            Mock<IAlarmManager> alarmMan = new Mock<IAlarmManager>(MockBehavior.Strict);
            CurrentConversation = new AlarmConversation(GetConfigurationManager(), alarmMan.Object);
            DateTime desiredTime = DateTime.Now.AddMinutes(10);
            desiredTime = new DateTime(desiredTime.Year, desiredTime.Month, desiredTime.Day, desiredTime.Hour, desiredTime.Minute, 0);
            String amPm = desiredTime.Hour > 11 ? "pm" : "am";
            int amPmHours = desiredTime.Hour > 12 ? desiredTime.Hour - 12 : desiredTime.Hour == 0 ? 12 : desiredTime.Hour;
            alarmMan.Setup(s => s.AddAlarm(String.Format("Your {0}:{1:00} {2} alarm is complete.", amPmHours, desiredTime.Minute, amPm.ToUpper()), new SamiDateTime(desiredTime, DateTimeRange.SpecificTime)));

            Dictionary<String, String> input = new Dictionary<string, string>
                {
                    {"Command", "Alarm"},
                    {"SubCommand", "Time"},
                    {"Time", String.Format("Hours={0};Minutes={1};TimeOfDay={2};", amPmHours, desiredTime.Minute, amPm)},
                };
            Assert.AreEqual(String.Format("The alarm has been set to {0}:{1:00} {2}", amPmHours, desiredTime.Minute, amPm.ToUpper()), RunSingleConversation<AlarmConversation>(input));
            alarmMan.Verify(s => s.AddAlarm(String.Format("Your {0}:{1:00} {2} alarm is complete.", amPmHours, desiredTime.Minute, amPm.ToUpper()), new SamiDateTime(desiredTime, DateTimeRange.SpecificTime)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestListAlarmsNoAlarmsConversation()
        {
            Mock<IAlarmManager> alarmMan = new Mock<IAlarmManager>(MockBehavior.Strict);
            CurrentConversation = new AlarmConversation(GetConfigurationManager(), alarmMan.Object);
            alarmMan.Setup(s => s.GetScheduledAlarms()).Returns(new List<ScheduledAlarm>());

            Dictionary<String, String> input = new Dictionary<string, string>
                {
                    {"Command", "Alarm"},
                    {"SubCommand", "List"},
                };
            Assert.AreEqual("You have 0 alarms scheduled. ", RunSingleConversation<AlarmConversation>(input));
            alarmMan.Verify(s => s.GetScheduledAlarms(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestListAlarmsOneAbsoluteAlarmConversation()
        {
            Mock<IAlarmManager> alarmMan = new Mock<IAlarmManager>(MockBehavior.Strict);
            CurrentConversation = new AlarmConversation(GetConfigurationManager(), alarmMan.Object);

            // Get first time to have the alarm.
            DateTime desiredTime = DateTime.Now.AddMinutes(10);
            String amPm = desiredTime.Hour > 11 ? "pm" : "am";
            int amPmHours = desiredTime.Hour > 12 ? desiredTime.Hour - 12 : desiredTime.Hour == 0 ? 12 : desiredTime.Hour;
            desiredTime = new DateTime(desiredTime.Year, desiredTime.Month, desiredTime.Day, desiredTime.Hour, desiredTime.Minute, 0);

            alarmMan.Setup(s => s.GetScheduledAlarms()).Returns(new List<ScheduledAlarm>
                {
                    new ScheduledAlarm("Testing alarm", new Timer(60 * 10), desiredTime),
                });

            // Set up conversation.
            Dictionary<String, String> input = new Dictionary<string, string>
                {
                    {"Command", "Alarm"},
                    {"SubCommand", "List"},
                };
            String response = RunSingleConversation<AlarmConversation>(input);

            // Check output.
            String regExPattern = String.Format("You have 1 alarm scheduled. You have [0-9] minutes (and ([1-9])?[0-9] second([s])?)? left on your {0}:{1:00} {2} alarm", amPmHours, desiredTime.Minute, amPm.ToUpper());
            Assert.IsTrue(Regex.IsMatch(response, regExPattern), response);
            alarmMan.Verify(s => s.GetScheduledAlarms(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestListAlarmsOneDurationAlarmConversation()
        {
            Mock<IAlarmManager> alarmMan = new Mock<IAlarmManager>(MockBehavior.Strict);
            CurrentConversation = new AlarmConversation(GetConfigurationManager(), alarmMan.Object);

            // Get first time to have the alarm.
            DateTime desiredTime = DateTime.Now.AddMinutes(10);
            String amPm = desiredTime.Hour > 11 ? "pm" : "am";
            int amPmHours = desiredTime.Hour > 11 ? desiredTime.Hour - 12 : desiredTime.Hour == 0 ? 12 : desiredTime.Hour;
            desiredTime = new DateTime(desiredTime.Year, desiredTime.Month, desiredTime.Day, desiredTime.Hour, desiredTime.Minute, 0);

            alarmMan.Setup(s => s.GetScheduledAlarms()).Returns(new List<ScheduledAlarm>
                {
                    new ScheduledAlarm("Testing alarm", new Timer(60 * 10), new TimeSpan(0, 10, 0), desiredTime),
                });

            // Set up conversation.
            Dictionary<String, String> input = new Dictionary<string, string>
                {
                    {"Command", "Alarm"},
                    {"SubCommand", "List"},
                };
            String response = RunSingleConversation<AlarmConversation>(input);

            // Check output.
            String regExPattern = String.Format("You have 1 alarm scheduled. You have (1)?[0-9] minute(s)? (and ([1-9])?[0-9] second([s])?)? left on your 10 minute alarm", amPmHours, desiredTime.Minute, amPm.ToUpper());
            Assert.IsTrue(Regex.IsMatch(response, regExPattern), response);
            alarmMan.Verify(s => s.GetScheduledAlarms(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestListAlarmsManyAbsoluteAlarmsConversation()
        {
            Mock<IAlarmManager> alarmMan = new Mock<IAlarmManager>(MockBehavior.Strict);
            CurrentConversation = new AlarmConversation(GetConfigurationManager(), alarmMan.Object);

            // Get first time to have the alarm.
            DateTime desiredTime1 = DateTime.Now.AddMinutes(10);
            String amPm1 = desiredTime1.Hour > 11 ? "pm" : "am";
            int amPmHours1 = desiredTime1.Hour > 12 ? desiredTime1.Hour - 12 : desiredTime1.Hour == 0 ? 12 : desiredTime1.Hour;
            desiredTime1 = new DateTime(desiredTime1.Year, desiredTime1.Month, desiredTime1.Day, desiredTime1.Hour, desiredTime1.Minute, 0);

            // Get second time to have the alarm.
            DateTime desiredTime2 = DateTime.Now.AddMinutes(20);
            String amPm2 = desiredTime2.Hour > 11 ? "pm" : "am";
            int amPmHours2 = desiredTime2.Hour > 12 ? desiredTime2.Hour - 12 : desiredTime2.Hour == 0 ? 12 : desiredTime2.Hour;
            desiredTime2 = new DateTime(desiredTime2.Year, desiredTime2.Month, desiredTime2.Day, desiredTime2.Hour, desiredTime2.Minute, 0);

            // Add alarms to be returned when requested.
            alarmMan.Setup(s => s.GetScheduledAlarms()).Returns(new List<ScheduledAlarm>
                {
                    new ScheduledAlarm("Testing alarm", new Timer(60 * 10), desiredTime1),
                    new ScheduledAlarm("Testing alarm two", new Timer(60 * 20), desiredTime2),
                });

            // Set up conversation.
            Dictionary<String, String> input = new Dictionary<string, string>
                {
                    {"Command", "Alarm"},
                    {"SubCommand", "List"},
                };
            String response = RunSingleConversation<AlarmConversation>(input);

            // Check output.
            String regExPattern = String.Format("You have 2 alarms scheduled. You have [0-9] minutes (and ([1-9])?[0-9] second([s])?)? left on your {0}:{1:00} {2} alarm. and 1[0-9] minutes (and ([1-9])?[0-9] second([s])?)? left on your {3}:{4:00} {5} alarm",
                amPmHours1, desiredTime1.Minute, amPm1.ToUpper(), amPmHours2, desiredTime2.Minute, amPm2.ToUpper());
            Assert.IsTrue(Regex.IsMatch(response, regExPattern), response);
            alarmMan.Verify(s => s.GetScheduledAlarms(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestListAlarmsManyDurationAlarmsConversation()
        {
            Mock<IAlarmManager> alarmMan = new Mock<IAlarmManager>(MockBehavior.Strict);
            CurrentConversation = new AlarmConversation(GetConfigurationManager(), alarmMan.Object);

            // Get first time to have the alarm.
            DateTime desiredTime1 = DateTime.Now.AddMinutes(10);
            desiredTime1 = new DateTime(desiredTime1.Year, desiredTime1.Month, desiredTime1.Day, desiredTime1.Hour, desiredTime1.Minute, 0);

            // Get second time to have the alarm.
            DateTime desiredTime2 = DateTime.Now.AddMinutes(20);
            desiredTime2 = new DateTime(desiredTime2.Year, desiredTime2.Month, desiredTime2.Day, desiredTime2.Hour, desiredTime2.Minute, 0);

            // Add alarms to be returned when requested.
            alarmMan.Setup(s => s.GetScheduledAlarms()).Returns(new List<ScheduledAlarm>
                {
                    new ScheduledAlarm("Testing alarm", new Timer(60 * 10), new TimeSpan(0, 10, 0), desiredTime1),
                    new ScheduledAlarm("Testing alarm two", new Timer(60 * 20), new TimeSpan(0, 20, 0), desiredTime2),
                });

            // Set up conversation.
            Dictionary<String, String> input = new Dictionary<string, string>
                {
                    {"Command", "Alarm"},
                    {"SubCommand", "List"},
                };
            String response = RunSingleConversation<AlarmConversation>(input);

            // Check output.
            String regExPattern = "You have 2 alarms scheduled. You have [0-9] minutes (and ([1-9])?[0-9] second([s])?)? left on your 10 minute alarm. and 1[0-9] minutes (and ([1-9])?[0-9] second([s])?)? left on your 20 minute alarm";
            Assert.IsTrue(Regex.IsMatch(response, regExPattern), response);
            alarmMan.Verify(s => s.GetScheduledAlarms(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestListAlarmsManyMixedAlarmsConversation()
        {
            Mock<IAlarmManager> alarmMan = new Mock<IAlarmManager>(MockBehavior.Strict);
            CurrentConversation = new AlarmConversation(GetConfigurationManager(), alarmMan.Object);

            // Get first time to have the alarm.
            DateTime desiredTime1 = DateTime.Now.AddMinutes(10);
            desiredTime1 = new DateTime(desiredTime1.Year, desiredTime1.Month, desiredTime1.Day, desiredTime1.Hour, desiredTime1.Minute, 0);

            // Get second time to have the alarm.
            DateTime desiredTime2 = DateTime.Now.AddMinutes(20);
            String amPm2 = desiredTime2.Hour > 11 ? "pm" : "am";
            int amPmHours2 = desiredTime2.Hour > 12 ? desiredTime2.Hour - 12 : desiredTime2.Hour == 0 ? 12 : desiredTime2.Hour;
            desiredTime2 = new DateTime(desiredTime2.Year, desiredTime2.Month, desiredTime2.Day, desiredTime2.Hour, desiredTime2.Minute, 0);

            // Add alarms to be returned when requested.
            alarmMan.Setup(s => s.GetScheduledAlarms()).Returns(new List<ScheduledAlarm>
                {
                    new ScheduledAlarm("Testing alarm", new Timer(60 * 10), new TimeSpan(0, 10, 0), desiredTime1),
                    new ScheduledAlarm("Testing alarm two", new Timer(60 * 20), desiredTime2),
                });

            // Set up conversation.
            Dictionary<String, String> input = new Dictionary<string, string>
                {
                    {"Command", "Alarm"},
                    {"SubCommand", "List"},
                };
            String response = RunSingleConversation<AlarmConversation>(input);

            // Check output.
            String regExPattern = String.Format("You have 2 alarms scheduled. You have [0-9] minutes (and ([1-9])?[0-9] second([s])?)? left on your 10 minute alarm. and 1[0-9] minutes and ([1-9])?[0-9] second([s])? left on your {0}:{1:00} {2} alarm",
                amPmHours2, desiredTime2.Minute, amPm2.ToUpper());
            Assert.IsTrue(Regex.IsMatch(response, regExPattern), response);
            alarmMan.Verify(s => s.GetScheduledAlarms(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestRemoveLastAlarmConversation()
        {
            Mock<IAlarmManager> alarmMan = new Mock<IAlarmManager>(MockBehavior.Strict);
            CurrentConversation = new AlarmConversation(GetConfigurationManager(), alarmMan.Object);

            // Get first time to have the alarm.
            DateTime desiredTime1 = DateTime.Now.AddMinutes(10);
            desiredTime1 = new DateTime(desiredTime1.Year, desiredTime1.Month, desiredTime1.Day, desiredTime1.Hour, desiredTime1.Minute, 0);

            // Add expected request to remove alarm.
            alarmMan.Setup(s => s.DeleteLastAlarm()).Returns(new ScheduledAlarm("Testing alarm", new Timer(60 * 10), new TimeSpan(0, 10, 0), desiredTime1));

            // Set up conversation.
            Dictionary<String, String> input = new Dictionary<string, string>
                {
                    {"Command", "Alarm"},
                    {"SubCommand", "DeleteLast"},
                };
            String response = RunSingleConversation<AlarmConversation>(input);

            // Check output.
            Assert.AreEqual("Deleted the 10 minute alarm", response);
            alarmMan.Verify(s => s.DeleteLastAlarm(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestRemoveAllAlarmsSuccessConversation()
        {
            Mock<IAlarmManager> alarmMan = new Mock<IAlarmManager>(MockBehavior.Strict);
            CurrentConversation = new AlarmConversation(GetConfigurationManager(), alarmMan.Object);

            // Add expected request to remove alarm.
            alarmMan.Setup(s => s.DeleteAllAlarms()).Returns(true);

            // Set up conversation.
            Dictionary<String, String> input = new Dictionary<string, string>
                {
                    {"Command", "Alarm"},
                    {"SubCommand", "DeleteAll"},
                };
            String response = RunSingleConversation<AlarmConversation>(input);

            // Check output.
            Assert.AreEqual("Deleted all alarms.", response);
            alarmMan.Verify(s => s.DeleteAllAlarms(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestRemoveAllAlarmsNoAlarmsConversation()
        {
            Mock<IAlarmManager> alarmMan = new Mock<IAlarmManager>(MockBehavior.Strict);
            CurrentConversation = new AlarmConversation(GetConfigurationManager(), alarmMan.Object);

            // Add expected request to remove alarm.
            alarmMan.Setup(s => s.DeleteAllAlarms()).Returns(false);

            // Set up conversation.
            Dictionary<String, String> input = new Dictionary<string, string>
                {
                    {"Command", "Alarm"},
                    {"SubCommand", "DeleteAll"},
                };
            String response = RunSingleConversation<AlarmConversation>(input);

            // Check output.
            Assert.AreEqual("There are no alarms to delete.", response);
            alarmMan.Verify(s => s.DeleteAllAlarms(), Times.Exactly(1));
        }
    }
}
