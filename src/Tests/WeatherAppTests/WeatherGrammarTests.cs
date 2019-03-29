using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps.Weather;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Weather;
using SAMI.Test.Utilities;

namespace WeatherAppTests
{
    [DeploymentItem("WeatherGrammar.grxml")]
    [TestClass]
    public class WeatherGrammarTests : BaseGrammarTests
    {
        [TestMethod]
        public void TestHowIsTheWeather()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", "TimeOfDay=now;"},
            };

            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(weatherSensor.Object);

            TestGrammar<WeatherApp>("How is the weather", expectedParams);
            TestGrammar<WeatherApp>("How is the weather today", expectedParams);
            TestGrammar<WeatherApp>("What's the weather like", expectedParams);
        }

        [TestMethod]
        public void TestHowWillTheWeatherBeNoLocation()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", "DayOfWeek=tomorrow;"},
            };

            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(weatherSensor.Object);

            TestGrammar<WeatherApp>("How will the weather be tomorrow", expectedParams);
        }

        [TestMethod]
        public void TestHowWillTheWeatherBeWithLocation()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "45202"},
                {"Time", "DayOfWeek=tomorrow;"},
            };

            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(weatherSensor.Object);

            TestGrammar<WeatherApp>("How will the weather be in Cincinnati tomorrow", expectedParams);
        }

        [TestMethod]
        public void TestWhatsTheForecastNoLocation()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", "DayOfWeek=tomorrow;"},
            };

            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(weatherSensor.Object);

            TestGrammar<WeatherApp>("What's the forecast for tomorrow", expectedParams);
        }

        [TestMethod]
        public void TestWhatsTheForecastWithLocation()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "45202"},
                {"Time", "DayOfWeek=tomorrow;"},
            };

            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(weatherSensor.Object);

            TestGrammar<WeatherApp>("What's the forecast for Cincinnati tomorrow", expectedParams);
        }
    }
}
