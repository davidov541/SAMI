using System;
using System.Collections.Generic;

namespace SAMI.Apps.Alarm
{
    internal interface IAlarmManager
    {
        event EventHandler<AlarmTriggeredEventArgs> AlarmTriggered;

        void AddAlarm(String message, TimeSpan timeSpan);

        void AddAlarm(String message, SamiDateTime time);

        List<ScheduledAlarm> GetScheduledAlarms();

        ScheduledAlarm DeleteLastAlarm();

        bool DeleteAllAlarms();
    }
}
