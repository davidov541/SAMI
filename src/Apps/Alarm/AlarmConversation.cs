using System;
using System.Collections.Generic;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Alarm
{
    internal class AlarmConversation : Conversation
    {
        private IAlarmManager _manager;

        protected override String CommandName
        {
            get
            {
                return "Alarm";
            }
        }

        internal AlarmConversation(IConfigurationManager configManager, IAlarmManager manager)
            : base(configManager)
        {
            _manager = manager;
        }

        public override string Speak()
        {
            base.Speak();
            ConversationIsOver = true;

            Dialog phrase = CurrentDialog;

            String alarmType = phrase.GetPropertyValue("SubCommand");
            if (alarmType.Equals("Time"))
            {
                // A specific time has been requested
                SamiDateTime alarmTime = ParseTime(phrase.GetPropertyValue("Time"));
                String alarmResponse = "Your " + alarmTime.Time.ToShortTimeString() + " alarm is complete.";
                _manager.AddAlarm(alarmResponse, alarmTime);
                return "The alarm has been set to " + alarmTime.Time.ToShortTimeString();
            }
            else if (alarmType.Equals("Duration"))
            {
                // A duration has been requested.
                int seconds = Convert.ToInt32(phrase.GetPropertyValue("Seconds"));
                int minutes = Convert.ToInt32(phrase.GetPropertyValue("Minutes"));
                int hours = Convert.ToInt32(phrase.GetPropertyValue("Hours"));
                TimeSpan ts = new TimeSpan(hours, minutes, seconds);
                if (ts.TotalSeconds == 0)
                {
                    return "";
                }
                else
                {
                    String alarmResponse = "Your " + SayTime(ts, false) + " timer is complete";
                    _manager.AddAlarm(alarmResponse, ts);
                    return "The timer has been set to " + SayTime(ts, true) + ".";
                }
            }
            else if (alarmType.Equals("List"))
            {
                List<ScheduledAlarm> alarms = _manager.GetScheduledAlarms();
                String response = "You have " + alarms.Count;
                if (alarms.Count == 1)
                {
                    response += " alarm scheduled. ";
                }
                else
                {
                    response += " alarms scheduled. ";
                }

                if (alarms.Count > 0)
                {
                    List<String> alarmStatus = new List<String>();
                    foreach (ScheduledAlarm alarm in alarms)
                    {
                        if (alarm.IsDurationAlarm)
                        {
                            alarmStatus.Add(SayTime(alarm.TimeLeft, true) + " left on your " + SayTime(alarm.TotalDuration, false) + " alarm");
                        }
                        else
                        {
                            alarmStatus.Add(SayTime(alarm.TimeLeft, true) + " left on your " + alarm.AlarmTime.ToShortTimeString() + " alarm");
                        }
                    }
                    response += "You have " + SayList(alarmStatus);
                }
                return response;
            }
            else if (alarmType.Equals("DeleteLast"))
            {
                ScheduledAlarm deletedAlarm = _manager.DeleteLastAlarm();
                if (deletedAlarm == null)
                {
                    return "There are no alarms to delete.";
                }
                else
                {
                    if (deletedAlarm.IsDurationAlarm)
                    {
                        return "Deleted the " + SayTime(deletedAlarm.TotalDuration, false) + " alarm";
                    }
                    else
                    {
                        return "Deleted the " + deletedAlarm.AlarmTime.ToShortTimeString() + " alarm";
                    }
                }
            }
            else if (alarmType.Equals("DeleteAll"))
            {
                if (_manager.DeleteAllAlarms())
                {
                    return "Deleted all alarms.";
                }
                else
                {
                    return "There are no alarms to delete.";
                }
            }
            else
            {
                return "The alarm type " + alarmType + " is not valid.";
            }
        }
    }
}
