using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAMI.IOInterfaces.Interfaces.Weather
{
    /// <summary>
    /// Represents the weather forecast for a single day.
    /// </summary>
    public class DailyForecast
    {
        /// <summary>
        /// The weather conditions during the heat of the day.
        /// </summary>
        public WeatherCondition High
        {
            get;
            set;
        }

        /// <summary>
        /// The weather conditions during the coldest period of the day.
        /// </summary>
        public WeatherCondition Low
        {
            get;
            set;
        }

        /// <summary>
        /// Constructor for DailyForecast.
        /// </summary>
        /// <param name="high">Value for <see cref="High"/></param>
        /// <param name="low">Value for <see cref="Low"/></param>
        public DailyForecast(WeatherCondition high, WeatherCondition low)
        {
            High = high;
            Low = low;
        }
    }
}
