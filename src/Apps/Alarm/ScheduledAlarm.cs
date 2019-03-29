using System;
using System.Timers;

namespace SAMI.Apps.Alarm
{
    internal class ScheduledAlarm
    {
        private Timer _timer;

        public TimeSpan TimeLeft
        {
            get
            {
                /* If we were to just use DateTime.Now, twice, the if statement
                 * may pass the first time, but then the timespan go negative if
                 * the alarm was close to going off.  To prevent this, store the
                 * current time in a variable to reference in the timespan.
                 */
                DateTime now = DateTime.Now;
                if (now > AlarmTime)
                {
                    return new TimeSpan();
                }
                else
                {
                    return AlarmTime - now;
                }
            }
        }

        public TimeSpan TotalDuration
        {
            get;
            private set;
        }

        public DateTime AlarmTime
        {
            get;
            private set;
        }

        public bool IsDurationAlarm
        {
            get
            {
                return TotalDuration != TimeSpan.MinValue;
            }
        }

        public String Message
        {
            get;
            private set;
        }

        public ScheduledAlarm(String message, Timer timer, DateTime alarmTime)
        {
            Message = message;
            _timer = timer;
            TotalDuration = TimeSpan.MinValue;
            AlarmTime = alarmTime;
        }

        public ScheduledAlarm(String message, Timer timer, TimeSpan alarmDuration, DateTime alarmTime)
        {
            Message = message;
            _timer = timer;
            TotalDuration = alarmDuration;
            AlarmTime = alarmTime;
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
