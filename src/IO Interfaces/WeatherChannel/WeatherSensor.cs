using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml;
using SAMI.Apps;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Weather;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Weather
{
    [ParseableElement("WeatherChannel", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class WeatherSensor : IWeatherSensor
    {
        private String _apiKey;

        public String Name
        {
            get
            {
                return "The Weather Channel";
            }
        }

        /// <inheritdoc />
        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        public IEnumerable<PersistentProperty> Properties
        {
            get
            {
                yield return new PersistentProperty("APIKey", () => _apiKey, key => _apiKey = key);
            }
        }

        public IEnumerable<IParseable> Children
        {
            get
            {
                yield break;
            }
        }

        public WeatherSensor()
        {
        }

        public void Initialize(IConfigurationManager manager)
        {
        }

        public void Dispose()
        {
        }

        public void AddChild(IParseable component)
        {
        }

        public WeatherCondition LoadConditions(Location location)
        {
            XmlDocument xmldoc = GetConditionDocument(location);
            try
            {
                XmlNode conditions = xmldoc.SelectSingleNode("response/current_observation");
                return new WeatherCondition(DateTime.Now, Double.Parse(conditions.SelectSingleNode("temp_f").InnerText), conditions.SelectSingleNode("weather").InnerText);
            }
            catch (XmlException)
            {
                return null;
            }
        }

        protected virtual XmlDocument GetConditionDocument(Location location)
        {
            XmlDocument xmldoc = new XmlDocument();
            if (!String.IsNullOrEmpty(location.City) && !String.IsNullOrEmpty(location.State))
            {
                xmldoc.Load(String.Format("http://api.wunderground.com/api/{0}/conditions/q/{1}/{2}.xml", _apiKey, location.State, location.City.Replace(' ', '_')));
            }
            else
            {
                xmldoc.Load(String.Format("http://api.wunderground.com/api/{0}/conditions/q/{1}.xml", _apiKey, location.ZipCode));
            }
            return xmldoc;
        }

        public IEnumerable<DailyForecast> LoadDailyForecasts(Location location)
        {
            XmlDocument xmldoc = GetForecastDocument(location);
            foreach (XmlElement forecast in xmldoc.SelectNodes("response/forecast/simpleforecast/forecastdays/forecastday").OfType<XmlElement>())
            {
                DailyForecast daily = null;
                try
                {
                    DateTime day = new DateTime(Int32.Parse(forecast.SelectSingleNode("date/year").InnerText),
                        Int32.Parse(forecast.SelectSingleNode("date/month").InnerText),
                        Int32.Parse(forecast.SelectSingleNode("date/day").InnerText));
                    WeatherCondition low = new WeatherCondition(day,
                        Int32.Parse(forecast.SelectSingleNode("low/fahrenheit").InnerText),
                        forecast.SelectSingleNode("conditions").InnerText);
                    WeatherCondition high = new WeatherCondition(day,
                        Int32.Parse(forecast.SelectSingleNode("high/fahrenheit").InnerText),
                        forecast.SelectSingleNode("conditions").InnerText);
                    daily = new DailyForecast(high, low);
                }
                catch (XmlException)
                {
                }

                if (daily != null)
                {
                    yield return daily;
                }
            }
        }

        protected virtual XmlDocument GetForecastDocument(Location location)
        {
            XmlDocument xmldoc = new XmlDocument();
            if (!String.IsNullOrEmpty(location.City) && !String.IsNullOrEmpty(location.State))
            {
                xmldoc.Load(String.Format("http://api.wunderground.com/api/{0}/forecast10day/q/{1}/{2}.xml", _apiKey, location.State, location.City.Replace(' ', '_')));
            }
            else
            {
                xmldoc.Load(String.Format("http://api.wunderground.com/api/{0}/forecast10day/q/{1}.xml", _apiKey, location.ZipCode));
            }
            return xmldoc;
        }

        public IEnumerable<WeatherCondition> LoadHourlyForecasts(Location location)
        {
            XmlDocument xmldoc = GetHourlyDocument(location);
            foreach (XmlElement forecast in xmldoc.SelectNodes("response/hourly_forecast/forecast").OfType<XmlElement>())
            {
                WeatherCondition condition = null;
                try
                {
                    int monthNum = Int32.Parse(forecast.SelectSingleNode("FCTTIME/mon").InnerText);
                    int yearNum = Int32.Parse(forecast.SelectSingleNode("FCTTIME/year").InnerText);
                    int dayNum = Int32.Parse(forecast.SelectSingleNode("FCTTIME/mday").InnerText);
                    int hourNum = Int32.Parse(forecast.SelectSingleNode("FCTTIME/hour").InnerText);
                    DateTime time = new DateTime(yearNum, monthNum, dayNum, hourNum, 0, 0);

                    double temp = Double.Parse(forecast.SelectSingleNode("temp/english").InnerText);
                    String description = forecast.SelectSingleNode("condition").InnerText;
                    condition = new WeatherCondition(time, temp, description);
                }
                catch (XmlException)
                {
                }

                if (condition != null)
                {
                    yield return condition;
                }
            }
        }

        protected virtual XmlDocument GetHourlyDocument(Location location)
        {
            XmlDocument xmldoc = new XmlDocument();
            if (!String.IsNullOrEmpty(location.City) && !String.IsNullOrEmpty(location.State))
            {
                xmldoc.Load(String.Format("http://api.wunderground.com/api/{0}/hourly10day/q/{1}/{2}.xml", _apiKey, location.State, location.City.Replace(' ', '_')));
            }
            else
            {
                xmldoc.Load(String.Format("http://api.wunderground.com/api/{0}/hourly10day/q/{1}.xml", _apiKey, location.ZipCode));
            }
            return xmldoc;
        }
    }
}
