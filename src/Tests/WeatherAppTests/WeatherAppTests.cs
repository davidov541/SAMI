using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps.Weather;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Weather;
using SAMI.Test.Utilities;

namespace WeatherAppTests
{
    [TestClass]
    public class WeatherAppTests : BaseAppTests
    {
        [TestMethod]
        public void TestAppIsInvalidWithNoSensors()
        {
            IConfigurationManager configManager = GetConfigurationManager();
            WeatherApp app = new WeatherApp();
            TestInvalidApp(app, "There are no sources of weather data currently installed. Please add an IO Resource which can supply weather data.");
        }

        [TestMethod]
        public void TestAppIsValidWithSensors()
        {
            IConfigurationManager configManager = GetConfigurationManager();
            Mock<IWeatherSensor> sensor = new Mock<IWeatherSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(sensor.Object);

            WeatherApp app = new WeatherApp();
            app.Initialize(configManager);

            Assert.AreEqual(true, app.IsValid);
        }
    }
}
