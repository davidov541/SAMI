using System;

namespace SAMI.Apps
{
    /// <summary>
    /// Represents a given date and/or time.
    /// Made separately from the System.DateTime class, since this
    /// can represent both relative and absolute times, as well as
    /// time ranges.
    /// </summary>
    public class SamiDateTime
    {
        private DateTimeRange _range;
        /// <summary>
        /// Indicates what kind of range this date time represents.
        /// </summary>
        public DateTimeRange Range
        {
            get
            {
                return _range;
            }
        }

        private DateTime _time;
        /// <summary>
        /// The date time instance for this class.
        /// Note that the fields in this instance which are valid
        /// are determined by <see cref="Range"/>, so not all values
        /// are guaranteed to be valid. In general, only the date will
        /// be valid, unless <see cref="Range"/> is <see cref="SpecificTime"/>.
        /// </summary>
        public DateTime Time
        {
            get
            {
                return _time;
            }
        }

        public static DateTimeRange GetDateTimeRange(DateTime time)
        {
            if (time.Hour < 8)
            {
                return DateTimeRange.EarlyMorning;
            }
            else if (time.Hour < 12)
            {
                return DateTimeRange.Morning;
            }
            else if (time.Hour < 17)
            {
                return DateTimeRange.Afternoon;
            }
            else if (time.Hour < 20)
            {
                return DateTimeRange.Evening;
            }
            else
            {
                return DateTimeRange.Night;
            }
        }

        /// <summary>
        /// Basic constructor for <see cref="SamiDateTime"/>
        /// </summary>
        /// <param name="dateTime">The value for <see cref="Time"/></param>
        /// <param name="dateTimeRange">The value for <see cref="Range"/></param>
        public SamiDateTime(DateTime dateTime, DateTimeRange dateTimeRange)
        {
            _time = dateTime;
            _range = dateTimeRange;
        }

        /// <summary>
        /// Constructor for <see cref="SamiDateTime"/> which uses a default
        /// value for <see cref="Time"/> as Today.
        /// </summary>
        /// <param name="dateTimeRange">The value for <see cref="Range"/></param>
        public SamiDateTime(DateTimeRange dateTimeRange)
        {
            // Default to using today
            _time = DateTime.Today;
            _range = dateTimeRange;
        }

        /// <summary>
        /// Determines if the given time is actually within the range that
        /// this date time represents.
        /// </summary>
        /// <param name="t">The date time to check for.</param>
        /// <returns>True if t is within the range represented by this instance.</returns>
        public Boolean TimeIsInRange(DateTime t)
        {
            return t >= GetMinTime() && t <= GetMaxTime();
        }

        /// <summary>
        /// Determines the minimum time this range represents.
        /// </summary>
        /// <returns>The minimum time this range represents.</returns>
        public DateTime GetMinTime()
        {
            switch (_range)
            {
                case DateTimeRange.AnyTime:
                    return DateTime.MinValue;

                case DateTimeRange.Now:
                    return DateTime.Now;

                case DateTimeRange.Day:
                    return _time.Date;

                case DateTimeRange.EarlyMorning:
                    return _time.Date;

                case DateTimeRange.Morning:
                    return _time.Date.AddHours(6);

                case DateTimeRange.Afternoon:
                    return _time.Date.AddHours(12);

                case DateTimeRange.Evening:
                    return _time.Date.AddHours(17);

                case DateTimeRange.Night:
                    return _time.Date.AddHours(20);

                case DateTimeRange.SpecificTime:
                default:
                    return _time;
            }
        }

        /// <summary>
        /// Determines the maximum time this range represents.
        /// </summary>
        /// <returns>The maximum time this range represents.</returns>
        public DateTime GetMaxTime()
        {
            switch (_range)
            {
                case DateTimeRange.AnyTime:
                    return DateTime.MaxValue;

                case DateTimeRange.Now:
                    return DateTime.Now;

                case DateTimeRange.Day:
                    return _time.Date.AddHours(24);

                case DateTimeRange.EarlyMorning:
                    return _time.Date.AddHours(8);

                case DateTimeRange.Morning:
                    return _time.Date.AddHours(12);

                case DateTimeRange.Afternoon:
                    return _time.Date.AddHours(17);

                case DateTimeRange.Evening:
                    return _time.Date.AddHours(20);

                case DateTimeRange.Night:
                    return _time.Date.AddHours(24);

                case DateTimeRange.SpecificTime:
                default:
                    return _time;
            }
        }

        public override string ToString()
        {
            if (Time != null)
            {
                return String.Format("SamiDateTime, Range={0}, Time={1}", Range.ToString(), Time.ToString());
            }
            else
            {
                return String.Format("SamiDateTime, Range={0}, Time=<null>", Range.ToString());
            }
        }

        public override bool Equals(object obj)
        {
            SamiDateTime other = obj as SamiDateTime;
            if (other == null)
            {
                return false;
            }

            if (Range != other.Range)
            {
                return false;
            }

            if (!Time.Equals(other.Time))
            {
                return false;
            }

            return true;
        }
    }
}
