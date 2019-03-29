using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps;
using SAMI.Apps.Alarm;
using SAMI.Test.Utilities;

namespace AlarmAppTests
{
    [TestClass]
    public class AlarmManagerTests : BaseSAMITests
    {
        [TestMethod]
        public void TestAlarmManagerAddDurationAlarm()
        {
            // Get alarm manager, and ensure initial state.
            IAlarmManager alarmMan = new AlarmManager(GetConfigurationManager());
            Assert.AreEqual(0, alarmMan.GetScheduledAlarms().Count);

            // Set a fake alarm to check.
            alarmMan.AddAlarm("Test Alarm", new TimeSpan(0, 5, 0));
            List<ScheduledAlarm> alarms = alarmMan.GetScheduledAlarms();

            // Run checks
            Assert.AreEqual(1, alarms.Count);
            Assert.AreEqual("Test Alarm", alarms.First().Message);
            Assert.AreEqual(new TimeSpan(0, 5, 0), alarms.First().TotalDuration);
            Assert.IsTrue(alarms.First().IsDurationAlarm);

            // Cleanup
            alarmMan.DeleteAllAlarms();
        }

        [TestMethod]
        public void TestAlarmManagerAddAbsoluteAlarm()
        {
            // Get alarm manager, and ensure initial state.
            IAlarmManager alarmMan = new AlarmManager(GetConfigurationManager());
            Assert.AreEqual(0, alarmMan.GetScheduledAlarms().Count);

            // Get fake time to wake up.
            DateTime desiredTime = DateTime.Now.AddMinutes(5);

            // Set a fake alarm to check.
            alarmMan.AddAlarm("Test Alarm", new SamiDateTime(desiredTime, DateTimeRange.SpecificTime));
            List<ScheduledAlarm> alarms = alarmMan.GetScheduledAlarms();

            // Run checks
            Assert.AreEqual(1, alarms.Count);
            Assert.AreEqual("Test Alarm", alarms.First().Message);
            Assert.AreEqual(desiredTime, alarms.First().AlarmTime);
            Assert.IsFalse(alarms.First().IsDurationAlarm);

            // Cleanup
            alarmMan.DeleteAllAlarms();
        }

        [TestMethod]
        public void TestAlarmManagerAddDuplicateAlarm()
        {
            // Get alarm manager, and ensure initial state.
            IAlarmManager alarmMan = new AlarmManager(GetConfigurationManager());
            Assert.AreEqual(0, alarmMan.GetScheduledAlarms().Count);

            // Get fake time to wake up.
            DateTime desiredTime = DateTime.Now.AddMinutes(5);

            // Set a fake alarm to check.
            alarmMan.AddAlarm("Test Alarm", new SamiDateTime(desiredTime, DateTimeRange.SpecificTime));

            // Set a duplicate alarm to check.
            alarmMan.AddAlarm("Test Alarm", new SamiDateTime(desiredTime, DateTimeRange.SpecificTime));
            List<ScheduledAlarm> alarms = alarmMan.GetScheduledAlarms();

            // Run checks
            Assert.AreEqual(2, alarms.Count);
            Assert.AreEqual("Test Alarm", alarms.First().Message);
            Assert.AreEqual(desiredTime, alarms.First().AlarmTime);
            Assert.IsFalse(alarms.First().IsDurationAlarm);

            // Cleanup
            alarmMan.DeleteAllAlarms();
        }

        [TestMethod]
        public void TestAlarmManagerRemoveAllAlarms()
        {
            // Get alarm manager, and ensure initial state.
            IAlarmManager alarmMan = new AlarmManager(GetConfigurationManager());
            Assert.AreEqual(0, alarmMan.GetScheduledAlarms().Count);

            // Get fake time to wake up.
            DateTime desiredTime = DateTime.Now.AddMinutes(5);

            // Set a fake alarm to check.
            alarmMan.AddAlarm("Test Alarm", new SamiDateTime(desiredTime, DateTimeRange.SpecificTime));

            // Set a second alarm to check.
            alarmMan.AddAlarm("Test Alarm 2", new SamiDateTime(desiredTime, DateTimeRange.SpecificTime));

            // Ensure all alarms were added.
            Assert.AreEqual(2, alarmMan.GetScheduledAlarms().Count);

            // Dleete all of the alarms.
            alarmMan.DeleteAllAlarms();

            // Make sure the alarms were removed properly.
            Assert.AreEqual(0, alarmMan.GetScheduledAlarms().Count);

            // Cleanup
            alarmMan.DeleteAllAlarms();
        }

        [TestMethod]
        public void TestAlarmManagerRemoveAllAlarmsNoAlarms()
        {
            // Get alarm manager, and ensure initial state.
            IAlarmManager alarmMan = new AlarmManager(GetConfigurationManager());
            Assert.AreEqual(0, alarmMan.GetScheduledAlarms().Count);

            // Dleete all of the alarms.
            alarmMan.DeleteAllAlarms();

            // Make sure the alarms were removed properly.
            Assert.AreEqual(0, alarmMan.GetScheduledAlarms().Count);

            // Cleanup
            alarmMan.DeleteAllAlarms();
        }

        [TestMethod]
        public void TestAlarmManagerRemoveLastAlarm()
        {
            // Get alarm manager, and ensure initial state.
            IAlarmManager alarmMan = new AlarmManager(GetConfigurationManager());
            Assert.AreEqual(0, alarmMan.GetScheduledAlarms().Count);

            // Get fake time to wake up.
            DateTime desiredTime = DateTime.Now.AddMinutes(5);

            // Set a fake alarm to check.
            alarmMan.AddAlarm("Test Alarm", new TimeSpan(0, 5, 0));

            // Set a second alarm to check.
            alarmMan.AddAlarm("Test Alarm 2", new SamiDateTime(desiredTime, DateTimeRange.SpecificTime));

            // Dleete all of the alarms.
            ScheduledAlarm alarm = alarmMan.DeleteLastAlarm();

            // Make sure the alarms were removed properly.
            Assert.AreEqual(1, alarmMan.GetScheduledAlarms().Count);
            Assert.AreEqual("Test Alarm 2", alarm.Message);
            Assert.AreEqual(desiredTime, alarm.AlarmTime);
            Assert.IsFalse(alarm.IsDurationAlarm);

            // Cleanup
            alarmMan.DeleteAllAlarms();
        }

        [TestMethod]
        public void TestAlarmManagerRemoveLastAlarmNoAlarms()
        {
            // Get alarm manager, and ensure initial state.
            IAlarmManager alarmMan = new AlarmManager(GetConfigurationManager());
            Assert.AreEqual(0, alarmMan.GetScheduledAlarms().Count);

            // Dleete all of the alarms.
            alarmMan.DeleteLastAlarm();

            // Make sure the alarms were removed properly.
            Assert.AreEqual(0, alarmMan.GetScheduledAlarms().Count);

            // Cleanup
            alarmMan.DeleteAllAlarms();
        }

        [TestMethod]
        public void TestAlarmManagerNotifyDurationAlarmUp()
        {
            // Get alarm manager, and ensure initial state.
            IAlarmManager alarmMan = new AlarmManager(GetConfigurationManager());

            // Setup listener.
            DateTime startTime = DateTime.Now;
            Semaphore sem = new Semaphore(0, 1);
            alarmMan.AlarmTriggered += (sender, e) =>
            {
                sem.Release();
            };

            // Set a fake alarm to check.
            alarmMan.AddAlarm("Test Alarm", new TimeSpan(0, 0, 1));

            // Wait for alarm triggered call back.
            sem.WaitOne(new TimeSpan(0, 1, 0));
            Assert.IsTrue(DateTime.Now.Subtract(startTime).TotalSeconds >= 1);

            // Cleanup
            alarmMan.DeleteAllAlarms();
        }

        [TestMethod]
        public void TestAlarmManagerNotifyAbsoluteAlarmUp()
        {
            // Get alarm manager, and ensure initial state.
            IAlarmManager alarmMan = new AlarmManager(GetConfigurationManager());

            // Setup listener.
            DateTime startTime = DateTime.Now;
            Semaphore sem = new Semaphore(0, 1);
            alarmMan.AlarmTriggered += (sender, e) =>
            {
                sem.Release();
            };

            // Set a fake alarm to check.
            DateTime desiredTime = DateTime.Now.AddSeconds(1);
            alarmMan.AddAlarm("Test Alarm", new SamiDateTime(desiredTime, DateTimeRange.SpecificTime));

            // Wait for alarm triggered call back.
            sem.WaitOne(new TimeSpan(0, 1, 0));
            Assert.IsTrue(DateTime.Now.Subtract(startTime).TotalSeconds >= 1);

            // Cleanup
            alarmMan.DeleteAllAlarms();
        }

        [TestMethod]
        public void TestAlarmManagerStopAlarm()
        {
            // Get alarm manager, and ensure initial state.
            IAlarmManager alarmMan = new AlarmManager(GetConfigurationManager());

            // Setup listener.
            DateTime startTime = DateTime.Now;
            Semaphore sem = new Semaphore(0, 1);
            alarmMan.AlarmTriggered += (sender, e) =>
            {
                sem.Release();
            };

            // Set a fake alarm to check.
            DateTime desiredTime = DateTime.Now.AddSeconds(1);
            alarmMan.AddAlarm("Test Alarm", new SamiDateTime(desiredTime, DateTimeRange.SpecificTime));

            // Wait for alarm triggered call back.
            alarmMan.GetScheduledAlarms().First().Stop();
            sem.WaitOne(new TimeSpan(0, 0, 5));
            Assert.IsTrue(DateTime.Now.Subtract(startTime).TotalSeconds >= 5);

            // Cleanup
            alarmMan.DeleteAllAlarms();
        }
    }
}
