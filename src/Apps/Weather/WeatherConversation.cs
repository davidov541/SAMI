using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.IOInterfaces.Interfaces.Weather;

namespace SAMI.Apps.Weather
{
    internal class WeatherConversation : Conversation
    {
        protected override String CommandName
        {
            get
            {
                return "weather";
            }
        }

        public WeatherConversation(IConfigurationManager configManager)
            : base(configManager)
        {
        }

        public override string Speak()
        {
            Dialog phrase = CurrentDialog;
            SamiDateTime time = ParseTime(phrase.GetPropertyValue("Time"));
            Location location = ParseLocation(phrase.GetPropertyValue("Location"));

            base.Speak();
            ConversationIsOver = true;

            String result = String.Empty;
            foreach (IWeatherSensor sensor in ConfigManager.FindAllComponentsOfType<IWeatherSensor>())
            {
                String sensorResult = String.Empty;
                if (TryStateForecastForSensor(time, location, sensor, out sensorResult))
                {
                    result += String.Format("According to {0}. {1}.", sensor.Name, sensorResult);
                }
                else if (String.IsNullOrEmpty(result))
                {
                    result = sensorResult;
                }
            }
            return result;
        }

        private static bool TryStateForecastForSensor(SamiDateTime time, Location location, IWeatherSensor sensor, out String message)
        {
            if (time.Range == DateTimeRange.Now)
            {
                WeatherCondition condition = sensor.LoadConditions(location);
                if (condition == null)
                {
                    message = "Weather conditions are not available right now.";
                    return false;
                }
                message = String.Format("Right now, it is {0} degrees and {1}.",
                    condition.Temperature,
                    condition.ConditionDescription);
                return true;
            }
            else if (time.Range == DateTimeRange.Day)
            {
                // We are asking about a specific day, not a specific time.
                IEnumerable<DailyForecast> forecasts = sensor.LoadDailyForecasts(location);
                if (forecasts == null)
                {
                    message = "That forecast is not currently available.";
                    return false;
                }
                DailyForecast forecast = forecasts.SingleOrDefault(f => f.High.Time.Date.Equals(time.Time.Date));
                if (forecast == null)
                {
                    message = "That forecast is not currently available.";
                    return false;
                }
                message = String.Format("On {0}, it will be between {2} and {1} degrees and {3}.",
                    time.Time.ToString("m", CultureInfo.InvariantCulture),
                    forecast.High.Temperature,
                    forecast.Low.Temperature,
                    forecast.Low.ConditionDescription);
                return true;
            }
            else if (time.Range == DateTimeRange.SpecificTime)
            {
                // Make sure we're not trying to get the time in the past
                if (time.Time < DateTime.Now)
                {
                    message = "I'm sorry, I can not get a forcast for the past.";
                    return false;
                }

                // We want a specific time.
                WeatherCondition condition = sensor.LoadHourlyForecasts(location).SingleOrDefault(c => c.Time.Equals(new DateTime(time.Time.Year, time.Time.Month, time.Time.Day, time.Time.Hour, 0, 0)));
                if (condition == null)
                {
                    message = "That forecast is not available currently.";
                    return false;
                }
                if (time.Time.Day == DateTime.Today.Day)
                {
                    message = String.Format("Today at {0}, it will be {1} degrees and {2}.",
                        time.Time.ToString("hh:mm tt", CultureInfo.InvariantCulture),
                        condition.Temperature,
                        condition.ConditionDescription);
                    return true;
                }
                else
                {
                    message = String.Format("On {0} at {1}, it will be {2} degrees and {3}.",
                        time.Time.ToString("m", CultureInfo.InvariantCulture),
                        time.Time.ToString("hh:mm tt", CultureInfo.InvariantCulture),
                        condition.Temperature,
                        condition.ConditionDescription);
                    return true;
                }

            }
            else if (time.Range == DateTimeRange.EarlyMorning || 
                time.Range == DateTimeRange.Morning || 
                time.Range == DateTimeRange.Afternoon || 
                time.Range == DateTimeRange.Evening || 
                time.Range == DateTimeRange.Night)
            {
                DateTime firstTime = time.GetMinTime();
                DateTime secondTime = time.GetMaxTime();

                // Make sure we're not trying to get the time in the past
                if (secondTime < DateTime.Now)
                {
                    message = "I'm sorry, I can not get a forcast for the past.";
                    return false;
                }

                double firstTemp, secondTemp;

                if (firstTime < DateTime.Now)
                {
                    // We are already in the time span requested.
                    WeatherCondition condition = sensor.LoadConditions(location);
                    IEnumerable<WeatherCondition> forecasts = sensor.LoadHourlyForecasts(location);
                    WeatherCondition forecast = forecasts == null ? null : forecasts.SingleOrDefault(c => c.Time.Equals(new DateTime(secondTime.Year, secondTime.Month, secondTime.Day, secondTime.Hour, 0, 0)));

                    if (condition == null || forecast == null)
                    {
                        message = "That forecast is not available currently.";
                        return false;
                    }

                    firstTemp = condition.Temperature;
                    secondTemp = forecast.Temperature;

                    String temperatureCompare;

                    if (firstTemp < secondTemp)
                    {
                        temperatureCompare = "warm up to";
                    }
                    else if (firstTemp > secondTemp)
                    {
                        temperatureCompare = "cool down to";
                    }
                    else
                    {
                        temperatureCompare = "stay at";
                    }

                    message = String.Format("Currently, it is {0} degrees and {1}. It will {2} {3} degrees by {4} and {5}.",
                        firstTemp,
                        condition.ConditionDescription,
                        temperatureCompare,
                        secondTemp,
                        secondTime.ToString("hh:mm tt", CultureInfo.InvariantCulture),
                        forecast.ConditionDescription);
                    return true;
                }
                else
                {
                    // We want a specific time.
                    IEnumerable<WeatherCondition> forecasts = sensor.LoadHourlyForecasts(location);
                    WeatherCondition firstForecast = forecasts == null ? null : forecasts.SingleOrDefault(c => c.Time.Equals(new DateTime(firstTime.Year, firstTime.Month, firstTime.Day, firstTime.Hour, 0, 0)));
                    WeatherCondition secondForecast = forecasts == null ? null : forecasts.SingleOrDefault(c => c.Time.Equals(new DateTime(secondTime.Year, secondTime.Month, secondTime.Day, secondTime.Hour, 0, 0)));

                    if (firstForecast == null || secondForecast == null)
                    {
                        message = "That forecast is not currently available.";
                        return false;
                    }

                    // "At (time) (nothing|tomorrow|on day), it will be (firsttemp) degrees and (firstcondition). It will (warm up to|cool down to|stay at) (secondtemp) degrees and be (secondcondition)."
                    // Deturmine how to say the day
                    String day;
                    if (time.Time.Date == DateTime.Today.Date)
                    {
                        // Today
                        day = "today";
                    }
                    else if (time.Time.Date == DateTime.Today.AddDays(1).Date)
                    {
                        // Tomorrow
                        day = "tomorrow";
                    }
                    else
                    {
                        // Some other day
                        day = "on " + time.Time.ToString("m", CultureInfo.InvariantCulture);
                    }
                    firstTemp = firstForecast.Temperature;
                    secondTemp = secondForecast.Temperature;

                    String temperatureCompare;

                    if (firstTemp < secondTemp)
                    {
                        temperatureCompare = "warm up to";
                    }
                    else if (firstTemp > secondTemp)
                    {
                        temperatureCompare = "cool down to";
                    }
                    else
                    {
                        temperatureCompare = "stay at";
                    }


                    message = String.Format("At {0} {1}, it will be {2} degrees and {3}. It will {4} {5} degrees by {6} and {7}.",
                        firstTime.ToString("hh:mm tt", CultureInfo.InvariantCulture),
                        day,
                        firstTemp,
                        firstForecast.ConditionDescription,
                        temperatureCompare,
                        secondTemp,
                        secondTime.ToString("hh:mm tt", CultureInfo.InvariantCulture),
                        secondForecast.ConditionDescription);
                    return true;

                }
            }
            message = "Not Supported";
            return false;
        }
    }
}
