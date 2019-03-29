using System.Collections.Generic;
using SAMI.Apps;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Interfaces.Weather
{
    /// <summary>
    /// Interface for a component which supplies forecasts and current weather conditions.
    /// </summary>
    public interface IWeatherSensor : IIOInterface
    {
        /// <summary>
        /// Returns the current conditions at the location indicated.
        /// </summary>
        /// <param name="location">The location to query for.</param>
        /// <returns>Information about the current weather conditions at that location.</returns>
        WeatherCondition LoadConditions(Location location);

        /// <summary>
        /// Returns the next 10 days of daily forecasts at the location indicated.
        /// </summary>
        /// <param name="location">The location to query for.</param>
        /// <returns>A list of each of the daily forecasts for the next ten days.</returns>
        IEnumerable<DailyForecast> LoadDailyForecasts(Location location);

        /// <summary>
        /// Returns the next 10 days of hourly forecasts at the location indicated.
        /// </summary>
        /// <param name="location">The location to query for.</param>
        /// <returns>A list of each of the hourly forecasts for the next ten days.</returns>
        IEnumerable<WeatherCondition> LoadHourlyForecasts(Location location);
    }
}
