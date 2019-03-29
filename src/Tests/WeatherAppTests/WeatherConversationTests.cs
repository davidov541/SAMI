using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps;
using SAMI.Apps.Weather;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Weather;
using SAMI.Test.Utilities;

namespace WeatherAppTests
{
    [TestClass]
    public class WeatherConversationTests : BaseConversationTests
    {
        [TestMethod]
        public void TestCurrentWeatherSuccess()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", "TimeOfDay=now;"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            weatherSensor.Setup(s => s.LoadConditions(new Location("Austin", "Texas", 78759))).Returns(new WeatherCondition(DateTime.Now, 70, "Rainy"));
            AddComponentToConfigurationManager(weatherSensor.Object);

            Assert.AreEqual("According to Mock Weather Sensor. Right now, it is 70 degrees and Rainy..", RunSingleConversation<WeatherConversation>(input));

            weatherSensor.Verify(s => s.LoadConditions(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestCurrentWeatherMultipleSuccess()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", "TimeOfDay=now;"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            weatherSensor.Setup(s => s.LoadConditions(new Location("Austin", "Texas", 78759))).Returns(new WeatherCondition(DateTime.Now, 70, "Rainy"));
            AddComponentToConfigurationManager(weatherSensor.Object);

            Mock<IWeatherSensor> weatherSensor2 = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor2.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor2.Setup(s => s.IsValid).Returns(true);
            weatherSensor2.Setup(s => s.LoadConditions(new Location("Austin", "Texas", 78759))).Returns(new WeatherCondition(DateTime.Now, 80, "Sunny"));
            AddComponentToConfigurationManager(weatherSensor2.Object);

            Assert.AreEqual("According to Mock Weather Sensor. Right now, it is 70 degrees and Rainy..According to Mock Weather Sensor. Right now, it is 80 degrees and Sunny..", RunSingleConversation<WeatherConversation>(input));

            weatherSensor.Verify(s => s.LoadConditions(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
            weatherSensor2.Verify(s => s.LoadConditions(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestCurrentWeatherFailure()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", "TimeOfDay=now;"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            weatherSensor.Setup(s => s.LoadConditions(new Location("Austin", "Texas", 78759))).Returns((WeatherCondition)null);
            AddComponentToConfigurationManager(weatherSensor.Object);

            Assert.AreEqual("Weather conditions are not available right now.", RunSingleConversation<WeatherConversation>(input));

            weatherSensor.Verify(s => s.LoadConditions(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestTomorrowWeatherSuccess()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", "DayOfWeek=tomorrow;"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            DateTime tomorrow = DateTime.Now.AddDays(1);
            WeatherCondition high = new WeatherCondition(tomorrow, 90, "Sunny");
            WeatherCondition low = new WeatherCondition(tomorrow, 70, "Rainy");
            DailyForecast forecast = new DailyForecast(high, low);
            weatherSensor.Setup(s => s.LoadDailyForecasts(new Location("Austin", "Texas", 78759))).Returns(new List<DailyForecast> { forecast });
            AddComponentToConfigurationManager(weatherSensor.Object);

            String expectedResult = String.Format("According to Mock Weather Sensor. On {0}, it will be between 70 and 90 degrees and Rainy..", tomorrow.ToString("m", CultureInfo.InvariantCulture));
            Assert.AreEqual(expectedResult, RunSingleConversation<WeatherConversation>(input));

            weatherSensor.Verify(s => s.LoadDailyForecasts(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestTomorrowWeatherNullFailure()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", "DayOfWeek=tomorrow;"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            weatherSensor.Setup(s => s.LoadDailyForecasts(new Location("Austin", "Texas", 78759))).Returns((List<DailyForecast>)null);
            AddComponentToConfigurationManager(weatherSensor.Object);

            Assert.AreEqual("That forecast is not currently available.", RunSingleConversation<WeatherConversation>(input));

            weatherSensor.Verify(s => s.LoadDailyForecasts(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestTomorrowWeatherNoMatchFailure()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", "DayOfWeek=tomorrow;"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            DateTime today = DateTime.Now;
            WeatherCondition high = new WeatherCondition(today, 90, "Sunny");
            WeatherCondition low = new WeatherCondition(today, 70, "Rainy");
            DailyForecast forecast = new DailyForecast(high, low);
            weatherSensor.Setup(s => s.LoadDailyForecasts(new Location("Austin", "Texas", 78759))).Returns(new List<DailyForecast> { forecast });
            AddComponentToConfigurationManager(weatherSensor.Object);

            Assert.AreEqual("That forecast is not currently available.", RunSingleConversation<WeatherConversation>(input));

            weatherSensor.Verify(s => s.LoadDailyForecasts(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestSpecificTimeWeatherSuccess()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", "DayOfWeek=tomorrow;Hours=8;Minutes=0;TimeOfDay=am;"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            DateTime tomorrow = DateTime.Now.AddDays(1);
            DateTime specificTime = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 8, 0, 0);
            WeatherCondition forecast = new WeatherCondition(specificTime, 70, "Rainy");
            weatherSensor.Setup(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759))).Returns(new List<WeatherCondition> { forecast });
            AddComponentToConfigurationManager(weatherSensor.Object);

            String expectedResult = String.Format("According to Mock Weather Sensor. On {0} at 08:00 AM, it will be 70 degrees and Rainy..", tomorrow.ToString("m", CultureInfo.InvariantCulture));
            Assert.AreEqual(expectedResult, RunSingleConversation<WeatherConversation>(input));

            weatherSensor.Verify(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestSpecificTimeTodayWeatherSuccess()
        {
            DateTime today = DateTime.Now.AddMinutes(2);
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", String.Format("DayOfWeek=today;Hours={0};Minutes={1};TimeOfDay={2};", today.Hour, today.Minute, today.Hour == 12 ? "pm" : "am")},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            DateTime queryTime = new DateTime(today.Year, today.Month, today.Day, today.Hour, 0, 0);
            WeatherCondition forecast = new WeatherCondition(queryTime, 70, "Rainy");
            weatherSensor.Setup(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759))).Returns(new List<WeatherCondition> { forecast });
            AddComponentToConfigurationManager(weatherSensor.Object);

            String expectedResult = String.Format("According to Mock Weather Sensor. Today at {0}, it will be 70 degrees and Rainy..", today.ToString("hh:mm tt", CultureInfo.InvariantCulture));
            Assert.AreEqual(expectedResult, RunSingleConversation<WeatherConversation>(input));

            weatherSensor.Verify(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestSpecificTimeWeatherNoForecast()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", "DayOfWeek=tomorrow;Hours=8;Minutes=0;TimeOfDay=am;"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            DateTime tomorrow = DateTime.Now.AddDays(1);
            WeatherCondition forecast = new WeatherCondition(tomorrow, 70, "Rainy");
            weatherSensor.Setup(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759))).Returns(new List<WeatherCondition> { forecast });
            AddComponentToConfigurationManager(weatherSensor.Object);

            Assert.AreEqual("That forecast is not available currently.", RunSingleConversation<WeatherConversation>(input));

            weatherSensor.Verify(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestSpecificTimeWeatherEarlier()
        {
            DateTime earlier = DateTime.Now.Subtract(new TimeSpan(0, 1, 0, 0));
            if (earlier.Day + 1 == DateTime.Now.Day)
            {
                Assert.Inconclusive("This test can't be run between midnight and 1 AM, since an hour earlier is parsed as a year from an hour earlier, and we must have an hour difference for the weather to work.");
            }
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", String.Format("Month={0};Day={1};Hours={2};Minutes=0;TimeOfDay=am;", earlier.Month, earlier.Day, earlier.Hour)},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(weatherSensor.Object);

            Assert.AreEqual("I'm sorry, I can not get a forcast for the past.", RunSingleConversation<WeatherConversation>(input));
        }

        [TestMethod]
        public void TestGeneralTimeWeatherWarmUpSuccess()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", "DayOfWeek=tomorrow;TimeOfDay=evening;"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            DateTime tomorrow = DateTime.Now.AddDays(1);
            SamiDateTime samiTime = new SamiDateTime(tomorrow, DateTimeRange.Evening);
            WeatherCondition minForecast = new WeatherCondition(samiTime.GetMinTime(), 70, "Rainy");
            WeatherCondition maxForecast = new WeatherCondition(samiTime.GetMaxTime(), 90, "Sunny");
            weatherSensor.Setup(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759))).Returns(new List<WeatherCondition> { minForecast, maxForecast });
            AddComponentToConfigurationManager(weatherSensor.Object);

            String expectedResult = String.Format("According to Mock Weather Sensor. At {0} tomorrow, it will be 70 degrees and Rainy. It will warm up to 90 degrees by {1} and Sunny..", samiTime.GetMinTime().ToString("hh:mm tt", CultureInfo.InvariantCulture), samiTime.GetMaxTime().ToString("hh:mm tt", CultureInfo.InvariantCulture));
            Assert.AreEqual(expectedResult, RunSingleConversation<WeatherConversation>(input));

            weatherSensor.Verify(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGeneralTimeWeatherCoolDownSuccess()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", "DayOfWeek=tomorrow;TimeOfDay=evening;"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            DateTime tomorrow = DateTime.Now.AddDays(1);
            SamiDateTime samiTime = new SamiDateTime(tomorrow, DateTimeRange.Evening);
            WeatherCondition minForecast = new WeatherCondition(samiTime.GetMinTime(), 70, "Rainy");
            WeatherCondition maxForecast = new WeatherCondition(samiTime.GetMaxTime(), 50, "Sunny");
            weatherSensor.Setup(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759))).Returns(new List<WeatherCondition> { minForecast, maxForecast });
            AddComponentToConfigurationManager(weatherSensor.Object);

            String expectedResult = String.Format("According to Mock Weather Sensor. At {0} tomorrow, it will be 70 degrees and Rainy. It will cool down to 50 degrees by {1} and Sunny..", samiTime.GetMinTime().ToString("hh:mm tt", CultureInfo.InvariantCulture), samiTime.GetMaxTime().ToString("hh:mm tt", CultureInfo.InvariantCulture));
            Assert.AreEqual(expectedResult, RunSingleConversation<WeatherConversation>(input));

            weatherSensor.Verify(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGeneralTimeWeatherStayAtSuccess()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", "DayOfWeek=tomorrow;TimeOfDay=evening;"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            DateTime tomorrow = DateTime.Now.AddDays(1);
            SamiDateTime samiTime = new SamiDateTime(tomorrow, DateTimeRange.Evening);
            WeatherCondition minForecast = new WeatherCondition(samiTime.GetMinTime(), 70, "Rainy");
            WeatherCondition maxForecast = new WeatherCondition(samiTime.GetMaxTime(), 70, "Sunny");
            weatherSensor.Setup(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759))).Returns(new List<WeatherCondition> { minForecast, maxForecast });
            AddComponentToConfigurationManager(weatherSensor.Object);

            String expectedResult = String.Format("According to Mock Weather Sensor. At {0} tomorrow, it will be 70 degrees and Rainy. It will stay at 70 degrees by {1} and Sunny..", samiTime.GetMinTime().ToString("hh:mm tt", CultureInfo.InvariantCulture), samiTime.GetMaxTime().ToString("hh:mm tt", CultureInfo.InvariantCulture));
            Assert.AreEqual(expectedResult, RunSingleConversation<WeatherConversation>(input));

            weatherSensor.Verify(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGeneralTimeCurrentWeatherWarmUpSuccess()
        {
            DateTimeRange range = SamiDateTime.GetDateTimeRange(DateTime.Now);
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", String.Format("DayOfWeek=today;TimeOfDay={0};", range.ToString().ToLower())},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            DateTime today = DateTime.Now;
            SamiDateTime samiTime = new SamiDateTime(today, range);
            WeatherCondition minForecast = new WeatherCondition(DateTime.Now, 70, "Rainy");
            WeatherCondition maxForecast = new WeatherCondition(samiTime.GetMaxTime(), 90, "Sunny");
            weatherSensor.Setup(s => s.LoadConditions(new Location("Austin", "Texas", 78759))).Returns(minForecast);
            weatherSensor.Setup(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759))).Returns(new List<WeatherCondition> { maxForecast });
            AddComponentToConfigurationManager(weatherSensor.Object);

            String expectedResult = String.Format("According to Mock Weather Sensor. Currently, it is 70 degrees and Rainy. It will warm up to 90 degrees by {0} and Sunny..", samiTime.GetMaxTime().ToString("hh:mm tt", CultureInfo.InvariantCulture));
            Assert.AreEqual(expectedResult, RunSingleConversation<WeatherConversation>(input));

            weatherSensor.Verify(s => s.LoadConditions(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
            weatherSensor.Verify(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGeneralTimeCurrentWeatherCoolDownSuccess()
        {
            DateTimeRange range = SamiDateTime.GetDateTimeRange(DateTime.Now);
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", String.Format("DayOfWeek=today;TimeOfDay={0};", range.ToString().ToLower())},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            DateTime today = DateTime.Now;
            SamiDateTime samiTime = new SamiDateTime(today, range);
            WeatherCondition minForecast = new WeatherCondition(DateTime.Now, 70, "Rainy");
            WeatherCondition maxForecast = new WeatherCondition(samiTime.GetMaxTime(), 50, "Sunny");
            weatherSensor.Setup(s => s.LoadConditions(new Location("Austin", "Texas", 78759))).Returns(minForecast);
            weatherSensor.Setup(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759))).Returns(new List<WeatherCondition> { maxForecast });
            AddComponentToConfigurationManager(weatherSensor.Object);

            String expectedResult = String.Format("According to Mock Weather Sensor. Currently, it is 70 degrees and Rainy. It will cool down to 50 degrees by {0} and Sunny..", samiTime.GetMaxTime().ToString("hh:mm tt", CultureInfo.InvariantCulture));
            Assert.AreEqual(expectedResult, RunSingleConversation<WeatherConversation>(input));

            weatherSensor.Verify(s => s.LoadConditions(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
            weatherSensor.Verify(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGeneralTimeCurrentWeatherStayAtSuccess()
        {
            DateTimeRange range = SamiDateTime.GetDateTimeRange(DateTime.Now);
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", String.Format("DayOfWeek=today;TimeOfDay={0};", range.ToString().ToLower())},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            DateTime today = DateTime.Now;
            SamiDateTime samiTime = new SamiDateTime(today, range);
            WeatherCondition minForecast = new WeatherCondition(DateTime.Now, 70, "Rainy");
            WeatherCondition maxForecast = new WeatherCondition(samiTime.GetMaxTime(), 70, "Sunny");
            weatherSensor.Setup(s => s.LoadConditions(new Location("Austin", "Texas", 78759))).Returns(minForecast);
            weatherSensor.Setup(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759))).Returns(new List<WeatherCondition> { maxForecast });
            AddComponentToConfigurationManager(weatherSensor.Object);

            String expectedResult = String.Format("According to Mock Weather Sensor. Currently, it is 70 degrees and Rainy. It will stay at 70 degrees by {0} and Sunny..", samiTime.GetMaxTime().ToString("hh:mm tt", CultureInfo.InvariantCulture));
            Assert.AreEqual(expectedResult, RunSingleConversation<WeatherConversation>(input));

            weatherSensor.Verify(s => s.LoadConditions(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
            weatherSensor.Verify(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGeneralTimeCurrentWeatherFailInPast()
        {
            DateTimeRange range = SamiDateTime.GetDateTimeRange(DateTime.Now);
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", String.Format("DayOfWeek=yesterday;TimeOfDay={0};", range.ToString().ToLower())},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(weatherSensor.Object);

            Assert.AreEqual("I'm sorry, I can not get a forcast for the past.", RunSingleConversation<WeatherConversation>(input));
        }

        [TestMethod]
        public void TestGeneralTimeCurrentWeatherNullCondition()
        {
            DateTimeRange range = SamiDateTime.GetDateTimeRange(DateTime.Now);
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", String.Format("DayOfWeek=today;TimeOfDay={0};", range.ToString().ToLower())},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            DateTime today = DateTime.Now;
            SamiDateTime samiTime = new SamiDateTime(today, range);
            WeatherCondition maxForecast = new WeatherCondition(samiTime.GetMaxTime(), 90, "Sunny");
            weatherSensor.Setup(s => s.LoadConditions(new Location("Austin", "Texas", 78759))).Returns((WeatherCondition)null);
            weatherSensor.Setup(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759))).Returns(new List<WeatherCondition> { maxForecast });
            AddComponentToConfigurationManager(weatherSensor.Object);

            Assert.AreEqual("That forecast is not available currently.", RunSingleConversation<WeatherConversation>(input));

            weatherSensor.Verify(s => s.LoadConditions(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
            weatherSensor.Verify(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGeneralTimeCurrentWeatherNullForecast()
        {
            DateTimeRange range = SamiDateTime.GetDateTimeRange(DateTime.Now);
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", String.Format("DayOfWeek=today;TimeOfDay={0};", range.ToString().ToLower())},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            DateTime today = DateTime.Now;
            SamiDateTime samiTime = new SamiDateTime(today, range);
            WeatherCondition minForecast = new WeatherCondition(DateTime.Now, 70, "Rainy");
            weatherSensor.Setup(s => s.LoadConditions(new Location("Austin", "Texas", 78759))).Returns(minForecast);
            weatherSensor.Setup(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759))).Returns((List<WeatherCondition>)null);
            AddComponentToConfigurationManager(weatherSensor.Object);

            Assert.AreEqual("That forecast is not available currently.", RunSingleConversation<WeatherConversation>(input));

            weatherSensor.Verify(s => s.LoadConditions(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
            weatherSensor.Verify(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGeneralTimeCurrentWeatherEmptyForecast()
        {
            DateTimeRange range = SamiDateTime.GetDateTimeRange(DateTime.Now);
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", String.Format("DayOfWeek=today;TimeOfDay={0};", range.ToString().ToLower())},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            DateTime today = DateTime.Now;
            SamiDateTime samiTime = new SamiDateTime(today, range);
            WeatherCondition minForecast = new WeatherCondition(DateTime.Now, 70, "Rainy");
            weatherSensor.Setup(s => s.LoadConditions(new Location("Austin", "Texas", 78759))).Returns(minForecast);
            weatherSensor.Setup(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759))).Returns(new List<WeatherCondition>());
            AddComponentToConfigurationManager(weatherSensor.Object);

            Assert.AreEqual("That forecast is not available currently.", RunSingleConversation<WeatherConversation>(input));

            weatherSensor.Verify(s => s.LoadConditions(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
            weatherSensor.Verify(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGeneralTimeWeatherNullForecast()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", "DayOfWeek=tomorrow;TimeOfDay=evening;"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            weatherSensor.Setup(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759))).Returns((IEnumerable<WeatherCondition>)null);
            AddComponentToConfigurationManager(weatherSensor.Object);

            Assert.AreEqual("That forecast is not currently available.", RunSingleConversation<WeatherConversation>(input));

            weatherSensor.Verify(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGeneralTimeWeatherNoFirstForecast()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", "DayOfWeek=tomorrow;TimeOfDay=evening;"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            DateTime tomorrow = DateTime.Now.AddDays(1);
            SamiDateTime samiTime = new SamiDateTime(tomorrow, DateTimeRange.Evening);
            WeatherCondition minForecast = new WeatherCondition(samiTime.GetMinTime(), 70, "Rainy");
            weatherSensor.Setup(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759))).Returns(new List<WeatherCondition>());
            AddComponentToConfigurationManager(weatherSensor.Object);

            Assert.AreEqual("That forecast is not currently available.", RunSingleConversation<WeatherConversation>(input));

            weatherSensor.Verify(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGeneralTimeWeatherNoSecondForecast()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "weather"},
                {"Location", "here"},
                {"Time", "DayOfWeek=tomorrow;TimeOfDay=evening;"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IWeatherSensor> weatherSensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            weatherSensor.Setup(s => s.Name).Returns("Mock Weather Sensor");
            weatherSensor.Setup(s => s.IsValid).Returns(true);
            DateTime tomorrow = DateTime.Now.AddDays(1);
            SamiDateTime samiTime = new SamiDateTime(tomorrow, DateTimeRange.Evening);
            WeatherCondition maxForecast = new WeatherCondition(samiTime.GetMaxTime(), 90, "Sunny");
            weatherSensor.Setup(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759))).Returns(new List<WeatherCondition> { maxForecast });
            AddComponentToConfigurationManager(weatherSensor.Object);

            Assert.AreEqual("That forecast is not currently available.", RunSingleConversation<WeatherConversation>(input));

            weatherSensor.Verify(s => s.LoadHourlyForecasts(new Location("Austin", "Texas", 78759)), Times.Exactly(1));
        }
    }
}
