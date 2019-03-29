using System;
using System.Collections.Generic;
using System.Timers;
using SAMI.Configuration;

namespace SAMI.Apps.Alarm
{
    internal class AlarmManager : IAlarmManager
    {
        public event EventHandler<AlarmTriggeredEventArgs> AlarmTriggered;

        private List<ScheduledAlarm> _scheduledAlarms;
        private IConfigurationManager _configManager;

        public AlarmManager(IConfigurationManager configManager)
        {
            _configManager = configManager;
            _scheduledAlarms = new List<ScheduledAlarm>();
        }

        public void AddAlarm(String message, TimeSpan timeSpan)
        {
            Timer timer = new Timer(timeSpan.TotalMilliseconds);
            ScheduledAlarm newAlarm = new ScheduledAlarm(message, timer, timeSpan, DateTime.Now + timeSpan);
            timer.Elapsed += (sender, e) => AlarmCallback(newAlarm);
            timer.Enabled = true;
            timer.AutoReset = false;
            _scheduledAlarms.Add(newAlarm);
        }

        public void AddAlarm(String message, SamiDateTime time)
        {
            TimeSpan timeSpan = time.Time - DateTime.Now;
            Timer timer = new Timer(timeSpan.TotalMilliseconds);
            ScheduledAlarm newAlarm = new ScheduledAlarm(message, timer, time.Time);
            timer.Elapsed += (sender, e) => AlarmCallback(newAlarm);
            timer.Enabled = true;
            timer.AutoReset = false;
            _scheduledAlarms.Add(newAlarm);
        }

        private void AlarmCallback(object state)
        {
            ScheduledAlarm alarm = (ScheduledAlarm)state;
            alarm.Stop();
            _scheduledAlarms.Remove(alarm);
            OnAsyncAlertRaised(new AlarmTriggeredEventArgs(new InformationalConversation(_configManager, alarm.Message)));
        }

        private void OnAsyncAlertRaised(AlarmTriggeredEventArgs args)
        {
            if (AlarmTriggered != null)
            {
                AlarmTriggered(this, args);
            }
        }

        public List<ScheduledAlarm> GetScheduledAlarms()
        {
            return _scheduledAlarms;
        }

        public ScheduledAlarm DeleteLastAlarm()
        {
            int lastIndex = _scheduledAlarms.Count - 1;
            if (lastIndex >= 0)
            {
                ScheduledAlarm deletedAlarm = _scheduledAlarms[lastIndex];
                deletedAlarm.Stop();
                _scheduledAlarms.RemoveAt(lastIndex);
                return deletedAlarm;
            }
            else
            {
                return null;
            }
        }

        public bool DeleteAllAlarms()
        {
            if (_scheduledAlarms.Count == 0)
            {
                return false;
            }

            foreach (ScheduledAlarm alarm in _scheduledAlarms)
            {
                alarm.Stop();
            }
            _scheduledAlarms.Clear();
            return true;
        }
    }
}
