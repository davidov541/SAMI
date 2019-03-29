using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps.Football;
using SAMI.IOInterfaces.Interfaces.Football;
using SAMI.IOInterfaces.Interfaces.Sports;
using SAMI.Test.Utilities;

namespace FootballAppTests
{
    [TestClass]
    public class FootballConversationTest : BaseConversationTests
    {
        [TestMethod]
        public void GetScoreNoIOInterfacesConversationTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Football"},
                {"Team", "TestTeam"},
                {"Parameter", "Score"},
            };

            Assert.AreEqual(String.Empty, RunSingleConversation<FootballConversation>(input));
        }

        [TestMethod]
        public void GetScoreByeWeekTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Football"},
                {"Team", "TestTeam"},
                {"Parameter", "Score"},
            };

            Mock<IFootballSensor> sensor = new Mock<IFootballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(new List<FootballTeam>
                {
                    new FootballTeam("Test", "Team", "TestTeam"),
                    new FootballTeam("Test 2", "Teams 2", "TestTeam2")
                });
            sensor.Setup(s => s.LoadLatestScoreForTeam("TestTeam")).Returns((FootballGame)null);
            AddComponentToConfigurationManager(sensor.Object);
            Assert.AreEqual("The Team are not playing this week.", RunSingleConversation<FootballConversation>(input));
            sensor.Verify(s => s.Teams, Times.Exactly(2));
            sensor.Verify(s => s.LoadLatestScoreForTeam("TestTeam"), Times.Exactly(1));
        }

        [TestMethod]
        public void GetScoreHasntStartedRequestedTeamHomeTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Football"},
                {"Team", "TestTeam"},
                {"Parameter", "Score"},
            };

            // Setup teams.
            FootballTeam requestedTeam = new FootballTeam("Test", "Team", "TestTeam");
            FootballTeam oppositionTeam = new FootballTeam("Test 2", "Teams 2", "TestTeam2");

            // Setup game.
            FootballGame game = new FootballGame();
            game.State = GameState.GameHasntStarted;
            game.HomeTeamScore = new FootballTeamScore()
            {
                Score = 0,
                Team = requestedTeam
            };
            game.AwayTeamScore = new FootballTeamScore()
            {
                Score = 0,
                Team = oppositionTeam,
            };
            game.StartingTime = new DateTime(2014, 11, 30, 13, 0, 0);

            // Setup mock.
            Mock<IFootballSensor> sensor = new Mock<IFootballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(new List<FootballTeam>
                {
                    requestedTeam,
                    oppositionTeam,
                });
            sensor.Setup(s => s.LoadLatestScoreForTeam("TestTeam")).Returns(game);
            AddComponentToConfigurationManager(sensor.Object);

            // Run test
            Assert.AreEqual("The Team will be playing the Teams 2 at 1:00 PM on Sunday.", RunSingleConversation<FootballConversation>(input));
            sensor.Verify(s => s.Teams, Times.Exactly(2));
            sensor.Verify(s => s.LoadLatestScoreForTeam("TestTeam"), Times.Exactly(1));
        }

        [TestMethod]
        public void GetScoreHasntStartedRequestedTeamAwayTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Football"},
                {"Team", "TestTeam"},
                {"Parameter", "Score"},
            };

            // Setup teams.
            FootballTeam requestedTeam = new FootballTeam("Test", "Team", "TestTeam");
            FootballTeam oppositionTeam = new FootballTeam("Test 2", "Teams 2", "TestTeam2");

            // Setup game.
            FootballGame game = new FootballGame();
            game.State = GameState.GameHasntStarted;
            game.HomeTeamScore = new FootballTeamScore()
            {
                Score = 0,
                Team = oppositionTeam
            };
            game.AwayTeamScore = new FootballTeamScore()
            {
                Score = 0,
                Team = requestedTeam,
            };
            game.StartingTime = new DateTime(2014, 11, 30, 13, 0, 0);

            // Setup mock.
            Mock<IFootballSensor> sensor = new Mock<IFootballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(new List<FootballTeam>
                {
                    requestedTeam,
                    oppositionTeam,
                });
            sensor.Setup(s => s.LoadLatestScoreForTeam("TestTeam")).Returns(game);
            AddComponentToConfigurationManager(sensor.Object);

            // Run test
            Assert.AreEqual("The Team will be playing the Teams 2 at 1:00 PM on Sunday.", RunSingleConversation<FootballConversation>(input));
            sensor.Verify(s => s.Teams, Times.Exactly(2));
            sensor.Verify(s => s.LoadLatestScoreForTeam("TestTeam"), Times.Exactly(1));
        }

        [TestMethod]
        public void GetScoreCompletedRequestedTeamHomeTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Football"},
                {"Team", "TestTeam"},
                {"Parameter", "Score"},
            };

            // Setup teams.
            FootballTeam requestedTeam = new FootballTeam("Test", "Team", "TestTeam");
            FootballTeam oppositionTeam = new FootballTeam("Test 2", "Teams 2", "TestTeam2");

            // Setup game.
            FootballGame game = new FootballGame();
            game.State = GameState.Completed;
            game.HomeTeamScore = new FootballTeamScore()
            {
                Score = 21,
                Team = requestedTeam
            };
            game.AwayTeamScore = new FootballTeamScore()
            {
                Score = 7,
                Team = oppositionTeam,
            };
            game.StartingTime = new DateTime(2014, 11, 30, 13, 0, 0);

            // Setup mock.
            Mock<IFootballSensor> sensor = new Mock<IFootballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(new List<FootballTeam>
                {
                    requestedTeam,
                    oppositionTeam,
                });
            sensor.Setup(s => s.LoadLatestScoreForTeam("TestTeam")).Returns(game);
            AddComponentToConfigurationManager(sensor.Object);

            // Run test
            Assert.AreEqual("The Team beat the Teams 2 by a score of 21 to 7.", RunSingleConversation<FootballConversation>(input));
            sensor.Verify(s => s.Teams, Times.Exactly(2));
            sensor.Verify(s => s.LoadLatestScoreForTeam("TestTeam"), Times.Exactly(1));
        }

        [TestMethod]
        public void GetScoreCompletedRequestedTeamAwayTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Football"},
                {"Team", "TestTeam"},
                {"Parameter", "Score"},
            };

            // Setup teams.
            FootballTeam requestedTeam = new FootballTeam("Test", "Team", "TestTeam");
            FootballTeam oppositionTeam = new FootballTeam("Test 2", "Teams 2", "TestTeam2");

            // Setup game.
            FootballGame game = new FootballGame();
            game.State = GameState.Completed;
            game.HomeTeamScore = new FootballTeamScore()
            {
                Score = 7,
                Team = oppositionTeam
            };
            game.AwayTeamScore = new FootballTeamScore()
            {
                Score = 21,
                Team = requestedTeam,
            };
            game.StartingTime = new DateTime(2014, 11, 30, 13, 0, 0);

            // Setup mock.
            Mock<IFootballSensor> sensor = new Mock<IFootballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(new List<FootballTeam>
                {
                    requestedTeam,
                    oppositionTeam,
                });
            sensor.Setup(s => s.LoadLatestScoreForTeam("TestTeam")).Returns(game);
            AddComponentToConfigurationManager(sensor.Object);

            // Run test
            Assert.AreEqual("The Team beat the Teams 2 by a score of 21 to 7.", RunSingleConversation<FootballConversation>(input));
            sensor.Verify(s => s.Teams, Times.Exactly(2));
            sensor.Verify(s => s.LoadLatestScoreForTeam("TestTeam"), Times.Exactly(1));
        }

        [TestMethod]
        public void GetScoreInProgressTiedRequestedTeamHomeTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Football"},
                {"Team", "TestTeam"},
                {"Parameter", "Score"},
            };

            // Setup teams.
            FootballTeam requestedTeam = new FootballTeam("Test", "Team", "TestTeam");
            FootballTeam oppositionTeam = new FootballTeam("Test 2", "Teams 2", "TestTeam2");

            // Setup game.
            FootballGame game = new FootballGame();
            game.State = GameState.Started;
            game.HomeTeamScore = new FootballTeamScore()
            {
                Score = 14,
                Team = requestedTeam
            };
            game.AwayTeamScore = new FootballTeamScore()
            {
                Score = 14,
                Team = oppositionTeam,
            };
            game.StartingTime = new DateTime(2014, 11, 30, 13, 0, 0);

            // Setup mock.
            Mock<IFootballSensor> sensor = new Mock<IFootballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(new List<FootballTeam>
                {
                    requestedTeam,
                    oppositionTeam,
                });
            sensor.Setup(s => s.LoadLatestScoreForTeam("TestTeam")).Returns(game);
            AddComponentToConfigurationManager(sensor.Object);

            // Run test
            Assert.IsTrue(RunSingleConversation<FootballConversation>(input).StartsWith("The Team and the Teams 2 are tied at 14 a piece."));
            sensor.Verify(s => s.Teams, Times.Exactly(2));
            sensor.Verify(s => s.LoadLatestScoreForTeam("TestTeam"), Times.Exactly(1));
        }

        [TestMethod]
        public void GetScoreInProgressTiedRequestedTeamAwayTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Football"},
                {"Team", "TestTeam"},
                {"Parameter", "Score"},
            };

            // Setup teams.
            FootballTeam requestedTeam = new FootballTeam("Test", "Team", "TestTeam");
            FootballTeam oppositionTeam = new FootballTeam("Test 2", "Teams 2", "TestTeam2");

            // Setup game.
            FootballGame game = new FootballGame();
            game.State = GameState.Started;
            game.HomeTeamScore = new FootballTeamScore()
            {
                Score = 14,
                Team = oppositionTeam
            };
            game.AwayTeamScore = new FootballTeamScore()
            {
                Score = 14,
                Team = requestedTeam,
            };
            game.StartingTime = new DateTime(2014, 11, 30, 13, 0, 0);

            // Setup mock.
            Mock<IFootballSensor> sensor = new Mock<IFootballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(new List<FootballTeam>
                {
                    requestedTeam,
                    oppositionTeam,
                });
            sensor.Setup(s => s.LoadLatestScoreForTeam("TestTeam")).Returns(game);
            AddComponentToConfigurationManager(sensor.Object);

            // Run test
            Assert.IsTrue(RunSingleConversation<FootballConversation>(input).StartsWith("The Team and the Teams 2 are tied at 14 a piece."));
            sensor.Verify(s => s.Teams, Times.Exactly(2));
            sensor.Verify(s => s.LoadLatestScoreForTeam("TestTeam"), Times.Exactly(1));
        }

        [TestMethod]
        public void GetScoreInProgressRequestedTeamLosingTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Football"},
                {"Team", "TestTeam"},
                {"Parameter", "Score"},
            };

            // Setup teams.
            FootballTeam requestedTeam = new FootballTeam("Test", "Team", "TestTeam");
            FootballTeam oppositionTeam = new FootballTeam("Test 2", "Teams 2", "TestTeam2");

            // Setup game.
            FootballGame game = new FootballGame();
            game.State = GameState.Started;
            game.HomeTeamScore = new FootballTeamScore()
            {
                Score = 7,
                Team = requestedTeam
            };
            game.AwayTeamScore = new FootballTeamScore()
            {
                Score = 14,
                Team = oppositionTeam,
            };
            game.StartingTime = new DateTime(2014, 11, 30, 13, 0, 0);

            // Setup mock.
            Mock<IFootballSensor> sensor = new Mock<IFootballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(new List<FootballTeam>
                {
                    requestedTeam,
                    oppositionTeam,
                });
            sensor.Setup(s => s.LoadLatestScoreForTeam("TestTeam")).Returns(game);
            AddComponentToConfigurationManager(sensor.Object);

            // Run test
            String response = RunSingleConversation<FootballConversation>(input);
            Assert.IsTrue(response.StartsWith("The Team are losing to the Teams 2 by a score of 14 to 7. "), response);
            sensor.Verify(s => s.Teams, Times.Exactly(2));
            sensor.Verify(s => s.LoadLatestScoreForTeam("TestTeam"), Times.Exactly(1));
        }

        [TestMethod]
        public void GetScoreInProgressRequestedTeamWinningTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Football"},
                {"Team", "TestTeam"},
                {"Parameter", "Score"},
            };

            // Setup teams.
            FootballTeam requestedTeam = new FootballTeam("Test", "Team", "TestTeam");
            FootballTeam oppositionTeam = new FootballTeam("Test 2", "Teams 2", "TestTeam2");

            // Setup game.
            FootballGame game = new FootballGame();
            game.State = GameState.Started;
            game.HomeTeamScore = new FootballTeamScore()
            {
                Score = 14,
                Team = oppositionTeam
            };
            game.AwayTeamScore = new FootballTeamScore()
            {
                Score = 21,
                Team = requestedTeam,
            };
            game.StartingTime = new DateTime(2014, 11, 30, 13, 0, 0);

            // Setup mock.
            Mock<IFootballSensor> sensor = new Mock<IFootballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(new List<FootballTeam>
                {
                    requestedTeam,
                    oppositionTeam,
                });
            sensor.Setup(s => s.LoadLatestScoreForTeam("TestTeam")).Returns(game);
            AddComponentToConfigurationManager(sensor.Object);

            // Run test
            String response = RunSingleConversation<FootballConversation>(input);
            Assert.IsTrue(response.StartsWith("The Team are beating the Teams 2 by a score of 21 to 14. "), response);
            sensor.Verify(s => s.Teams, Times.Exactly(2));
            sensor.Verify(s => s.LoadLatestScoreForTeam("TestTeam"), Times.Exactly(1));
        }

        [TestMethod]
        public void GetScoreInProgressMidQuarterTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Football"},
                {"Team", "TestTeam"},
                {"Parameter", "Score"},
            };

            // Setup teams.
            FootballTeam requestedTeam = new FootballTeam("Test", "Team", "TestTeam");
            FootballTeam oppositionTeam = new FootballTeam("Test 2", "Teams 2", "TestTeam2");

            // Setup game.
            FootballGame game = new FootballGame();
            game.State = GameState.Started;
            game.HomeTeamScore = new FootballTeamScore()
            {
                Score = 14,
                Team = requestedTeam
            };
            game.AwayTeamScore = new FootballTeamScore()
            {
                Score = 14,
                Team = oppositionTeam,
            };
            game.StartingTime = new DateTime(2014, 11, 30, 13, 0, 0);
            game.Quarter = 2;
            game.TimeLeft = new TimeSpan(0, 10, 32);

            // Setup mock.
            Mock<IFootballSensor> sensor = new Mock<IFootballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(new List<FootballTeam>
                {
                    requestedTeam,
                    oppositionTeam,
                });
            sensor.Setup(s => s.LoadLatestScoreForTeam("TestTeam")).Returns(game);
            AddComponentToConfigurationManager(sensor.Object);

            // Run test
            Assert.IsTrue(RunSingleConversation<FootballConversation>(input).EndsWith("with 10 32 remaining in the second quarter."));
            sensor.Verify(s => s.Teams, Times.Exactly(2));
            sensor.Verify(s => s.LoadLatestScoreForTeam("TestTeam"), Times.Exactly(1));
        }

        [TestMethod]
        public void GetScoreInProgressEndOfQuarterTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Football"},
                {"Team", "TestTeam"},
                {"Parameter", "Score"},
            };

            // Setup teams.
            FootballTeam requestedTeam = new FootballTeam("Test", "Team", "TestTeam");
            FootballTeam oppositionTeam = new FootballTeam("Test 2", "Teams 2", "TestTeam2");

            // Setup game.
            FootballGame game = new FootballGame();
            game.State = GameState.Started;
            game.HomeTeamScore = new FootballTeamScore()
            {
                Score = 14,
                Team = requestedTeam
            };
            game.AwayTeamScore = new FootballTeamScore()
            {
                Score = 14,
                Team = oppositionTeam,
            };
            game.StartingTime = new DateTime(2014, 11, 30, 13, 0, 0);
            game.Quarter = 2;
            game.TimeLeft = new TimeSpan(0, 0, 0);

            // Setup mock.
            Mock<IFootballSensor> sensor = new Mock<IFootballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(new List<FootballTeam>
                {
                    requestedTeam,
                    oppositionTeam,
                });
            sensor.Setup(s => s.LoadLatestScoreForTeam("TestTeam")).Returns(game);
            AddComponentToConfigurationManager(sensor.Object);

            // Run test
            Assert.IsTrue(RunSingleConversation<FootballConversation>(input).EndsWith("at the end of the second quarter."));
            sensor.Verify(s => s.Teams, Times.Exactly(2));
            sensor.Verify(s => s.LoadLatestScoreForTeam("TestTeam"), Times.Exactly(1));
        }

        [TestMethod]
        public void GetScoreInProgressStartOfQuarterTest()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Football"},
                {"Team", "TestTeam"},
                {"Parameter", "Score"},
            };

            // Setup teams.
            FootballTeam requestedTeam = new FootballTeam("Test", "Team", "TestTeam");
            FootballTeam oppositionTeam = new FootballTeam("Test 2", "Teams 2", "TestTeam2");

            // Setup game.
            FootballGame game = new FootballGame();
            game.State = GameState.Started;
            game.HomeTeamScore = new FootballTeamScore()
            {
                Score = 14,
                Team = requestedTeam
            };
            game.AwayTeamScore = new FootballTeamScore()
            {
                Score = 14,
                Team = oppositionTeam,
            };
            game.StartingTime = new DateTime(2014, 11, 30, 13, 0, 0);
            game.Quarter = 2;
            game.TimeLeft = new TimeSpan(0, 15, 0);

            // Setup mock.
            Mock<IFootballSensor> sensor = new Mock<IFootballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(new List<FootballTeam>
                {
                    requestedTeam,
                    oppositionTeam,
                });
            sensor.Setup(s => s.LoadLatestScoreForTeam("TestTeam")).Returns(game);
            AddComponentToConfigurationManager(sensor.Object);

            // Run test
            Assert.IsTrue(RunSingleConversation<FootballConversation>(input).EndsWith("at the beginning of the second quarter."));
            sensor.Verify(s => s.Teams, Times.Exactly(2));
            sensor.Verify(s => s.LoadLatestScoreForTeam("TestTeam"), Times.Exactly(1));
        }
    }
}
