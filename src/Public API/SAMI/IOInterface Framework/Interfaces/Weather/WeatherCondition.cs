using System;

namespace SAMI.IOInterfaces.Interfaces.Weather
{
    /// <summary>
    /// Represents the weather condition (current or future) for a specific time and location.
    /// </summary>
    public class WeatherCondition
    {
        /// <summary>
        /// The temperature (in fahrenheit).
        /// </summary>
        public double Temperature
        {
            get;
            set;
        }

        /// <summary>
        /// A user-readable string which describes the current conditions.
        /// </summary>
        public String ConditionDescription
        {
            get;
            set;
        }

        /// <summary>
        /// The time at which this condition is valid for.
        /// </summary>
        public DateTime Time
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor for WeatherCondition.
        /// </summary>
        /// <param name="time">Value for <see cref="Time"/></param>
        /// <param name="temp">Value for <see cref="Temperature"/></param>
        /// <param name="condition">Value for <see cref="ConditionDescription"/></param>
        public WeatherCondition(DateTime time, double temp, String condition)
        {
            Time = time;
            Temperature = temp;
            ConditionDescription = condition;
        }
    }
}
