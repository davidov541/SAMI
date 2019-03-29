using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps.Football;
using SAMI.IOInterfaces.Interfaces.Football;
using SAMI.Test.Utilities;

namespace FootballAppTests
{
    [TestClass]
    public class FootballAppTests : BaseAppTests
    {
        [TestMethod]
        public void FootballAppInvalidWithoutIOInterfacesTest()
        {
            TestInvalidApp(new FootballApp(), "We cannot connect to the Football score server currently.");
        }

        [TestMethod]
        public void FootballAppInvalidWithoutTeamListTest()
        {
            Mock<IFootballSensor> sensor = new Mock<IFootballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(new List<FootballTeam>());
            AddComponentToConfigurationManager(sensor.Object);
            TestInvalidApp(new FootballApp(), "We cannot connect to the Football score server currently.");
        }
    }
}
