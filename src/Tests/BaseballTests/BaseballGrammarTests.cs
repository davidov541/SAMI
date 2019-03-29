using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps.Baseball;
using SAMI.IOInterfaces.Interfaces.Baseball;
using SAMI.Test.Utilities;

namespace BaseballTests
{
    [DeploymentItem("BaseballGrammar.grxml")]
    [TestClass]
    public class BaseballGrammarTests : BaseGrammarTests
    {
        private List<BaseballTeam> _mockTeams = new List<BaseballTeam>
            {
                new BaseballTeam("Mock City", "Mockers", "mockteam", "NLCentral"),
                new BaseballTeam("Fake City", "Fakers", "faketeam", "NLCentral"),
            };

        [TestMethod]
        public void HowIsTeamDoingFullName()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "baseball"},
                {"Parameter", "Score"},
                {"Time", "DayOfWeek=today;"},
                {"Team", "mockteam"},
            };
            Mock<IBaseballSensor> sensor = new Mock<IBaseballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(_mockTeams);
            AddComponentToConfigurationManager(sensor.Object);
            BaseballApp app = new BaseballApp();

            TestGrammar(app, GetConfigurationManager(), "How are the Mock City Mockers doing", expectedParams);
        }

        [TestMethod]
        public void HowIsTeamDoingMascotName()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "baseball"},
                {"Parameter", "Score"},
                {"Time", "DayOfWeek=today;"},
                {"Team", "mockteam"},
            };
            Mock<IBaseballSensor> sensor = new Mock<IBaseballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(_mockTeams);
            AddComponentToConfigurationManager(sensor.Object);
            BaseballApp app = new BaseballApp();

            TestGrammar(app, GetConfigurationManager(), "How are the Mockers doing", expectedParams);
        }

        [TestMethod]
        public void WhatsTheScoreFullName()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "baseball"},
                {"Parameter", "Score"},
                {"Time", "DayOfWeek=today;"},
                {"Team", "mockteam"},
            };
            Mock<IBaseballSensor> sensor = new Mock<IBaseballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(_mockTeams);
            AddComponentToConfigurationManager(sensor.Object);
            BaseballApp app = new BaseballApp();

            TestGrammar(app, GetConfigurationManager(), "What's the score of the Mock City Mockers game", expectedParams);
        }

        [TestMethod]
        public void WhatsTheScoreMascotName()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "baseball"},
                {"Parameter", "Score"},
                {"Time", "DayOfWeek=today;"},
                {"Team", "mockteam"},
            };
            Mock<IBaseballSensor> sensor = new Mock<IBaseballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(_mockTeams);
            AddComponentToConfigurationManager(sensor.Object);
            BaseballApp app = new BaseballApp();

            TestGrammar(app, GetConfigurationManager(), "What's the score of the Mockers game", expectedParams);
        }

        [TestMethod]
        public void HowDidTheTeamDoFullName()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "baseball"},
                {"Parameter", "Score"},
                {"Time", "DayOfWeek=yesterday;"},
                {"Team", "mockteam"},
            };
            Mock<IBaseballSensor> sensor = new Mock<IBaseballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(_mockTeams);
            AddComponentToConfigurationManager(sensor.Object);
            BaseballApp app = new BaseballApp();

            TestGrammar(app, GetConfigurationManager(), "How did the Mock City Mockers do yesterday", expectedParams);
        }

        [TestMethod]
        public void HowDidTheTeamDoMascotName()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "baseball"},
                {"Parameter", "Score"},
                {"Time", "DayOfWeek=yesterday;"},
                {"Team", "mockteam"},
            };
            Mock<IBaseballSensor> sensor = new Mock<IBaseballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(_mockTeams);
            AddComponentToConfigurationManager(sensor.Object);
            BaseballApp app = new BaseballApp();

            TestGrammar(app, GetConfigurationManager(), "How did the Mockers do yesterday", expectedParams);
        }

        [TestMethod]
        public void HowFarBackIsTheTeamFullName()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "baseball"},
                {"Parameter", "Standings"},
                {"Time", "DayOfWeek=today;"},
                {"Team", "mockteam"},
            };
            Mock<IBaseballSensor> sensor = new Mock<IBaseballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(_mockTeams);
            AddComponentToConfigurationManager(sensor.Object);
            BaseballApp app = new BaseballApp();

            TestGrammar(app, GetConfigurationManager(), "How far back are the Mock City Mockers", expectedParams);
        }

        [TestMethod]
        public void HowFarBackIsTheTeamMascotName()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "baseball"},
                {"Parameter", "Standings"},
                {"Time", "DayOfWeek=today;"},
                {"Team", "mockteam"},
            };
            Mock<IBaseballSensor> sensor = new Mock<IBaseballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(_mockTeams);
            AddComponentToConfigurationManager(sensor.Object);
            BaseballApp app = new BaseballApp();

            TestGrammar(app, GetConfigurationManager(), "How far back are the Mockers", expectedParams);
        }

        [TestMethod]
        public void WhereIsTheTeamInTheStandingsFullName()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "baseball"},
                {"Parameter", "Standings"},
                {"Time", "DayOfWeek=today;"},
                {"Team", "mockteam"},
            };
            Mock<IBaseballSensor> sensor = new Mock<IBaseballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(_mockTeams);
            AddComponentToConfigurationManager(sensor.Object);
            BaseballApp app = new BaseballApp();

            TestGrammar(app, GetConfigurationManager(), "Where are the Mock City Mockers in the standings", expectedParams);
        }

        [TestMethod]
        public void WhereIsTheTeamInTheStandingsMascotName()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "baseball"},
                {"Parameter", "Standings"},
                {"Time", "DayOfWeek=today;"},
                {"Team", "mockteam"},
            };
            Mock<IBaseballSensor> sensor = new Mock<IBaseballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(_mockTeams);
            AddComponentToConfigurationManager(sensor.Object);
            BaseballApp app = new BaseballApp();

            TestGrammar(app, GetConfigurationManager(), "Where are the Mockers in the standings", expectedParams);
        }

        [TestMethod]
        public void FreeMLBTVGame()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "baseball"},
                {"Parameter", "MLBTVFreeGame"},
                {"Time", "DayOfWeek=today;"},
                {"Team", ""},
            };
            Mock<IBaseballSensor> sensor = new Mock<IBaseballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(_mockTeams);
            AddComponentToConfigurationManager(sensor.Object);
            BaseballApp app = new BaseballApp();

            TestGrammar(app, GetConfigurationManager(), "What game is free tonight on MLB.TV", expectedParams);
        }

        [TestMethod]
        public void TurnToTheGameFullName()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "baseball"},
                {"Parameter", "TurnToGame"},
                {"Time", "DayOfWeek=today;"},
                {"Team", "mockteam"},
            };
            Mock<IBaseballSensor> sensor = new Mock<IBaseballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(_mockTeams);
            AddComponentToConfigurationManager(sensor.Object);
            BaseballApp app = new BaseballApp();

            TestGrammar(app, GetConfigurationManager(), "Turn to the Mock City Mockers game", expectedParams);
        }

        [TestMethod]
        public void TurnToTheGameMascotName()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "baseball"},
                {"Parameter", "TurnToGame"},
                {"Time", "DayOfWeek=today;"},
                {"Team", "mockteam"},
            };
            Mock<IBaseballSensor> sensor = new Mock<IBaseballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(_mockTeams);
            AddComponentToConfigurationManager(sensor.Object);
            BaseballApp app = new BaseballApp();

            TestGrammar(app, GetConfigurationManager(), "Turn to the Mockers game", expectedParams);
        }

        [TestMethod]
        public void WhatChannelIsTheGameOnFullName()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "baseball"},
                {"Parameter", "TVChannel"},
                {"Time", "DayOfWeek=today;"},
                {"Team", "mockteam"},
            };
            Mock<IBaseballSensor> sensor = new Mock<IBaseballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(_mockTeams);
            AddComponentToConfigurationManager(sensor.Object);
            BaseballApp app = new BaseballApp();

            TestGrammar(app, GetConfigurationManager(), "What channel is the Mock City Mockers game on", expectedParams);
        }

        [TestMethod]
        public void WhatChannelIsTheGameOnMascotName()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "baseball"},
                {"Parameter", "TVChannel"},
                {"Time", "DayOfWeek=today;"},
                {"Team", "mockteam"},
            };
            Mock<IBaseballSensor> sensor = new Mock<IBaseballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(_mockTeams);
            AddComponentToConfigurationManager(sensor.Object);
            BaseballApp app = new BaseballApp();

            TestGrammar(app, GetConfigurationManager(), "What channel is the Mockers game on", expectedParams);
        }
    }
}
