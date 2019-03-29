using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps;
using SAMI.Apps.Baseball;
using SAMI.IOInterfaces.Interfaces.Baseball;
using SAMI.IOInterfaces.Interfaces.Remote;
using SAMI.IOInterfaces.Interfaces.Sports;
using SAMI.Test.Utilities;

namespace BaseballTests
{
    [TestClass]
    public class BaseballConversationTests : BaseConversationTests
    {
        private List<BaseballTeam> _mockTeams = new List<BaseballTeam>
            {
                new BaseballTeam("Mock City", "Mockers", "mockteam", "NLCentral"),
                new BaseballTeam("Fake City", "Fakers", "faketeam", "NLCentral"),
            };

        #region Scores Tests
        [TestMethod]
        public void ScoresNoGameClass()
        {
            SingleScoreTest(null, 2, 3, false, false, "The Mockers are not playing today.");
        }

        [TestMethod]
        public void ScoresTiedHomeTeam()
        {
            SingleScoreTest(GameState.Started, 2, 2, true, false, "The Mockers and the Fakers are tied at 2 a piece. with 3 on and 2 outs in the bottom of the fifth.");
        }

        [TestMethod]
        public void ScoresTiedAwayTeam()
        {
            SingleScoreTest(GameState.Started, 2, 2, false, false, "The Mockers and the Fakers are tied at 2 a piece. with 3 on and 2 outs in the bottom of the fifth.");
        }

        [TestMethod]
        public void ScoresWinning()
        {
            SingleScoreTest(GameState.Started, 4, 3, false, false, "The Mockers are beating the Fakers 4 to 3. with 3 on and 2 outs in the bottom of the fifth.");
        }

        [TestMethod]
        public void ScoresLosing()
        {
            SingleScoreTest(GameState.Started, 2, 3, false, false, "The Mockers are losing to the Fakers by a score of 3 to 2. with 3 on and 2 outs in the bottom of the fifth.");
        }

        [TestMethod]
        public void ScoresBottomOfInning()
        {
            SingleScoreTest(GameState.Started, 2, 3, false, true, "The Mockers are losing to the Fakers by a score of 3 to 2. with 3 on and 2 outs in the top of the fifth.");
        }

        [TestMethod]
        public void ScoresRainDelay()
        {
            SingleScoreTest(GameState.RainDelay, 2, 3, false, true, "The Mockers game is currently in a rain delay.");
        }

        [TestMethod]
        public void ScoresPostponed()
        {
            SingleScoreTest(GameState.Postponed, 2, 3, false, true, "The Mockers game has been postponed.");
        }

        [TestMethod]
        public void ScoresNoGameState()
        {
            SingleScoreTest(GameState.NoGame, 2, 3, false, true, "The Mockers did not have a game that day.");
        }

        [TestMethod]
        public void ScoresHasntStarted()
        {
            SingleScoreTest(GameState.GameHasntStarted, 2, 3, false, true, "The Mockers will be playing at 8:00 AM.");
        }

        [TestMethod]
        public void ScoresCompletedWon()
        {
            SingleScoreTest(GameState.Completed, 4, 3, false, true, "The Mockers beat the Fakers by a score of 4 to 3.");
        }

        [TestMethod]
        public void ScoresCompletedLost()
        {
            SingleScoreTest(GameState.Completed, 2, 3, false, true, "The Mockers lost to the Fakers by a score of 3 to 2.");
        }

        [TestMethod]
        public void ScoresDoubleHeader()
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
            sensor.Setup(s => s.League).Returns("MLB");
            BaseballGame game1 = CreateBaseballGame(GameState.Completed, 1, 3, true, new List<string>());
            BaseballGame game2 = CreateBaseballGame(GameState.Started, 4, 3, true, new List<string>());
            game2.InningNumber = 5;
            game2.TopOfInning = true;
            game2.NumberOnBase = 3;
            game2.NumberOfOuts = 2;
            sensor.Setup(s => s.LoadScoresForTeam(It.IsAny<DateTime>(), _mockTeams[0])).Returns(new List<BaseballGame> { game1, game2 });
            AddComponentToConfigurationManager(sensor.Object);
            CurrentConversation = new BaseballConversation(GetConfigurationManager(), new List<IOInterfaceReference>());

            Assert.AreEqual("In the first game, The Mockers lost to the Fakers by a score of 3 to 1.. In the second game, The Mockers are beating the Fakers 4 to 3. with 3 on and 2 outs in the top of the fifth..", RunSingleConversation<BaseballConversation>(expectedParams));

            sensor.Verify(s => s.LoadScoresForTeam(It.IsAny<DateTime>(), _mockTeams[0]), Times.Exactly(1));
        }

        [TestMethod]
        public void ScoresTripleHeader()
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
            sensor.Setup(s => s.League).Returns("MLB");
            BaseballGame game1 = CreateBaseballGame(GameState.Completed, 1, 3, true, new List<string>());
            BaseballGame game2 = CreateBaseballGame(GameState.Completed, 1, 3, true, new List<string>());
            BaseballGame game3 = CreateBaseballGame(GameState.Started, 4, 3, true, new List<string>());
            game3.InningNumber = 5;
            game3.TopOfInning = true;
            game3.NumberOnBase = 3;
            game3.NumberOfOuts = 2;
            sensor.Setup(s => s.LoadScoresForTeam(It.IsAny<DateTime>(), _mockTeams[0])).Returns(new List<BaseballGame> { game1, game2, game3 });
            AddComponentToConfigurationManager(sensor.Object);
            CurrentConversation = new BaseballConversation(GetConfigurationManager(), new List<IOInterfaceReference>());

            Assert.AreEqual("", RunSingleConversation<BaseballConversation>(expectedParams));

            sensor.Verify(s => s.LoadScoresForTeam(It.IsAny<DateTime>(), _mockTeams[0]), Times.Exactly(1));
        }

        private void SingleScoreTest(GameState? gameState, int mockScore, int fakeScore, bool isMockHome, bool isTopOfInning, String expectedResponse)
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
            sensor.Setup(s => s.League).Returns("MLB");
            if (gameState.HasValue)
            {
                BaseballGame game = CreateBaseballGame(gameState.Value, mockScore, fakeScore, isMockHome, new List<string>());
                game.InningNumber = 5;
                game.TopOfInning = isTopOfInning;
                game.NumberOnBase = 3;
                game.NumberOfOuts = 2;
                sensor.Setup(s => s.LoadScoresForTeam(It.IsAny<DateTime>(), _mockTeams[0])).Returns(new List<BaseballGame> { game });
            }
            else
            {
                sensor.Setup(s => s.LoadScoresForTeam(It.IsAny<DateTime>(), _mockTeams[0])).Returns(new List<BaseballGame>());
            }
            AddComponentToConfigurationManager(sensor.Object);
            CurrentConversation = new BaseballConversation(GetConfigurationManager(), new List<IOInterfaceReference>());

            Assert.AreEqual(expectedResponse, RunSingleConversation<BaseballConversation>(expectedParams));

            sensor.Verify(s => s.LoadScoresForTeam(It.IsAny<DateTime>(), _mockTeams[0]), Times.Exactly(1));
        }
        #endregion

        #region Standings Tests
        [TestMethod]
        public void StandingsNoWildcardFailure()
        {
            StandingsTest("faketeam", 2, null, "I couldn't find any information about the Fakers this year.");
        }

        [TestMethod]
        public void StandingsNoWildcardDivisionLeader()
        {
            StandingsTest("mockteam", 0, null, "The Mockers lead the NLCentral.");
        }

        [TestMethod]
        public void StandingsNoWildcardTrailing()
        {
            StandingsTest("mockteam", -2, null, "The Mockers are 2 games behind the leaders in the NLCentral.");
        }

        [TestMethod]
        public void StandingsWildcardFailure()
        {
            StandingsTest("faketeam", -2, -3, "I couldn't find any information about the Fakers this year.");
        }

        [TestMethod]
        public void StandingsWildcardDivisionLeader()
        {
            StandingsTest("mockteam", 0, 0, "The Mockers lead the NLCentral.");
        }

        [TestMethod]
        public void StandingsWildcardTrailingWildcard()
        {
            StandingsTest("mockteam", -1, -3, "The Mockers are 1 games behind the leaders in the NLCentral, and are 3 games behind in the wildcard standings.");
        }

        [TestMethod]
        public void StandingsWildcardLastWildcard()
        {
            StandingsTest("mockteam", -1, 0, "The Mockers are 1 games behind the leaders in the NLCentral, and just barely have a wildcard spot.");
        }

        [TestMethod]
        public void StandingsWildcardNotLastWildcard()
        {
            StandingsTest("mockteam", -1, 1, "The Mockers are 1 games behind the leaders in the NLCentral, and lead the wildcard standings by 1 games.");
        }

        private void StandingsTest(String teamName, int gamesBackDivisional, int? gamesBackWildcard, String expectedResponse)
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "baseball"},
                {"Parameter", "Standings"},
                {"Time", "DayOfWeek=today;"},
                {"Team", teamName},
            };
            Mock<IBaseballSensor> sensor = new Mock<IBaseballSensor>(MockBehavior.Strict);
            sensor.Setup(s => s.IsValid).Returns(true);
            sensor.Setup(s => s.Teams).Returns(_mockTeams);
            sensor.Setup(s => s.League).Returns("MLB");
            BaseballTeamStanding teamStandings = new BaseballTeamStanding();
            teamStandings.Team = _mockTeams[0];
            teamStandings.GamesBack = gamesBackDivisional;
            BaseballStandings standings;
            if (gamesBackWildcard.HasValue)
            {
                standings = new BaseballStandingsWithWildcard();
                BaseballTeamStanding teamStandingsWildcard = new BaseballTeamStanding();
                teamStandingsWildcard.Team = _mockTeams[0];
                teamStandingsWildcard.GamesBack = gamesBackWildcard.Value;
                (standings as BaseballStandingsWithWildcard).AddStandingsForTeam(teamStandings, teamStandingsWildcard);
            }
            else
            {
                standings = new BaseballStandings();
                standings.AddStandingsForTeam(teamStandings);
            }
            sensor.Setup(s => s.LoadStandings()).Returns(standings);
            AddComponentToConfigurationManager(sensor.Object);
            CurrentConversation = new BaseballConversation(GetConfigurationManager(), new List<IOInterfaceReference>());

            Assert.AreEqual(expectedResponse, RunSingleConversation<BaseballConversation>(expectedParams));

            sensor.Verify(s => s.LoadStandings(), Times.Exactly(1));
        }
        #endregion

        #region Turn to Channel Tests
        [TestMethod]
        public void TurnToChannelNoGameClass()
        {
            TurnToChannelTest(null, false, false, "The Mockers are not playing today.");
        }

        [TestMethod]
        public void TurnToChannelNoGameState()
        {
            TurnToChannelTest(GameState.NoGame, false, false, "The Mockers are not playing today.");
        }

        [TestMethod]
        public void TurnToChannelHasntStartedNotOnTv()
        {
            TurnToChannelTest(GameState.GameHasntStarted, false, false, "The Mockers game will not be available on national TV.");
        }

        [TestMethod]
        public void TurnToChannelHasntStartedOnTv()
        {
            TurnToChannelTest(GameState.GameHasntStarted, true, false, "The Mockers will be playing at 8:00 AM.");
        }

        [TestMethod]
        public void TurnToChannelRainDelayNotOnTv()
        {
            TurnToChannelTest(GameState.RainDelay, false, false, "The Mockers game is not available on national TV.");
        }

        [TestMethod]
        public void TurnToChannelRainDelayOnTv()
        {
            TurnToChannelTest(GameState.RainDelay, true, false, "The Mockers game is currently in a rain delay, but is available on ESPN.");
        }

        [TestMethod]
        public void TurnToChannelPostponed()
        {
            TurnToChannelTest(GameState.Postponed, false, false, "The Mockers game has been postponed.");
        }

        [TestMethod]
        public void TurnToChannelCompleted()
        {
            TurnToChannelTest(GameState.Completed, false, false, "The game has already completed. The Mockers beat the Fakers by a score of 5 to 2.");
        }

        [TestMethod]
        public void TurnToChannelInProgressNoRemote()
        {
            TurnToChannelTest(GameState.Started, true, false, "There is no T V remote connected to me for me to use. The Mockers game is on ESPN.");
        }

        [TestMethod]
        public void TurnToChannelInProgressNotOnTv()
        {
            TurnToChannelTest(GameState.Started, false, false, "The Mockers game is not on a national broadcast!");
        }

        [TestMethod]
        public void TurnToChannelInProgressSuccess()
        {
            TurnToChannelTest(GameState.Started, true, true, "OK");
        }

        [TestMethod]
        public void TurnToChannelOneGameFinishedOneInProgress()
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
            sensor.Setup(s => s.League).Returns("MLB");

            BaseballGame completedGame = CreateBaseballGame(GameState.Completed, 5, 2, true, new List<String> { "ESPN" });
            BaseballGame inProgressGame = CreateBaseballGame(GameState.Started, 5, 2, true, new List<String> { "ESPN" });
            sensor.Setup(s => s.LoadScoresForTeam(It.IsAny<DateTime>(), _mockTeams[0])).Returns(new List<BaseballGame> { completedGame, inProgressGame });
            AddComponentToConfigurationManager(sensor.Object);
            CurrentConversation = new BaseballConversation(GetConfigurationManager(), new List<IOInterfaceReference>());

            Assert.AreEqual("There is no T V remote connected to me for me to use. The Mockers game is on ESPN.", RunSingleConversation<BaseballConversation>(expectedParams));

            sensor.Verify(s => s.LoadScoresForTeam(It.IsAny<DateTime>(), _mockTeams[0]), Times.Exactly(1));
        }

        private void TurnToChannelTest(GameState? gameState, bool isOnTV, bool includesRemotes, String expectedResponse)
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
            sensor.Setup(s => s.League).Returns("MLB");
            if (gameState.HasValue)
            {
                List<String> channels = new List<string>();
                if (isOnTV)
                {
                    channels.Add("ESPN");
                }
                BaseballGame game = CreateBaseballGame(gameState.Value, 5, 2, true, channels);
                sensor.Setup(s => s.LoadScoresForTeam(It.IsAny<DateTime>(), _mockTeams[0])).Returns(new List<BaseballGame> { game });
            }
            else
            {
                sensor.Setup(s => s.LoadScoresForTeam(It.IsAny<DateTime>(), _mockTeams[0])).Returns(new List<BaseballGame>());
            }
            AddComponentToConfigurationManager(sensor.Object);
            List<IOInterfaceReference> references = new List<IOInterfaceReference>();
            Mock<ITVRemote> remote = new Mock<ITVRemote>(MockBehavior.Strict);
            Semaphore channelTurn = new Semaphore(0, 1);
            if (includesRemotes)
            {
                remote.Setup(s => s.Name).Returns("Mock Name");
                remote.Setup(s => s.GetChannels()).Returns(new List<String> { "ESPN" });
                remote.Setup(s => s.SendChannel("ESPN")).Callback(() => channelTurn.Release());
                AddComponentToConfigurationManager(remote.Object);
                IOInterfaceReference remoteRef = new IOInterfaceReference();
                remoteRef.Properties.Single(s => s.Name.Equals("References")).Setter("Mock Name");
                references.Add(remoteRef);
            }
            CurrentConversation = new BaseballConversation(GetConfigurationManager(), references);

            Assert.AreEqual(expectedResponse, RunSingleConversation<BaseballConversation>(expectedParams));

            sensor.Verify(s => s.LoadScoresForTeam(It.IsAny<DateTime>(), _mockTeams[0]), Times.Exactly(1));
            if (includesRemotes)
            {
                channelTurn.WaitOne();
                remote.Verify(s => s.SendChannel("ESPN"), Times.Exactly(1));
            }
        }
        #endregion

        #region What Channel Tests
        [TestMethod]
        public void WhichChannelNoGameClass()
        {
            WhatChannelTest(null, false, "The Mockers are not playing today.");
        }

        [TestMethod]
        public void WhichChannelNoGameState()
        {
            WhatChannelTest(GameState.NoGame, false, "The Mockers are not playing today.");
        }

        [TestMethod]
        public void WhichChannelHasntStartedIsntOnTv()
        {
            WhatChannelTest(GameState.GameHasntStarted, false, "The Mockers game will not be available on national TV.");
        }

        [TestMethod]
        public void WhichChannelHasntStartedIsOnTv()
        {
            WhatChannelTest(GameState.GameHasntStarted, true, "The Mockers game will be available on ESPN at 8:00 AM");
        }

        [TestMethod]
        public void WhichChannelRainDelayIsntOnTv()
        {
            WhatChannelTest(GameState.RainDelay, false, "The Mockers game is not available on national TV.");
        }

        [TestMethod]
        public void WhichChannelRainDelayIsOnTv()
        {
            WhatChannelTest(GameState.RainDelay, true, "The Mockers game is currently in a rain delay, but is available on ESPN");
        }

        [TestMethod]
        public void WhichChannelStartedIsntOnTv()
        {
            WhatChannelTest(GameState.Started, false, "The Mockers game is not available on national TV.");
        }

        [TestMethod]
        public void WhichChannelStartedIsOnTv()
        {
            WhatChannelTest(GameState.Started, true, "The Mockers game is available on ESPN.");
        }

        [TestMethod]
        public void WhichChannelCompleted()
        {
            WhatChannelTest(GameState.Completed, true, "The game has already completed. The Mockers beat the Fakers by a score of 5 to 2.");
        }

        [TestMethod]
        public void WhichChannelPostponed()
        {
            WhatChannelTest(GameState.Postponed, true, "The Mockers game has been postponed.");
        }

        [TestMethod]
        public void WhichChannelOneGameFinishedOneInProgress()
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
            sensor.Setup(s => s.League).Returns("MLB");

            BaseballGame completedGame = CreateBaseballGame(GameState.Completed, 5, 2, true, new List<String> { "ESPN" });
            BaseballGame inProgressGame = CreateBaseballGame(GameState.Started, 5, 2, true, new List<String> { "ESPN" });
            sensor.Setup(s => s.LoadScoresForTeam(It.IsAny<DateTime>(), _mockTeams[0])).Returns(new List<BaseballGame> { completedGame, inProgressGame });
            AddComponentToConfigurationManager(sensor.Object);
            CurrentConversation = new BaseballConversation(GetConfigurationManager(), new List<IOInterfaceReference>());

            Assert.AreEqual("The Mockers game is available on ESPN.", RunSingleConversation<BaseballConversation>(expectedParams));

            sensor.Verify(s => s.LoadScoresForTeam(It.IsAny<DateTime>(), _mockTeams[0]), Times.Exactly(1));
        }

        private void WhatChannelTest(GameState? gameState, bool isOnTV, String expectedResponse)
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
            sensor.Setup(s => s.League).Returns("MLB");
            if (gameState.HasValue)
            {
                List<String> channels = new List<string>();
                if (isOnTV)
                {
                    channels.Add("ESPN");
                }
                BaseballGame game = CreateBaseballGame(gameState.Value, 5, 2, true, channels);
                sensor.Setup(s => s.LoadScoresForTeam(It.IsAny<DateTime>(), _mockTeams[0])).Returns(new List<BaseballGame> { game });
            }
            else
            {
                sensor.Setup(s => s.LoadScoresForTeam(It.IsAny<DateTime>(), _mockTeams[0])).Returns(new List<BaseballGame>());
            }
            AddComponentToConfigurationManager(sensor.Object);
            CurrentConversation = new BaseballConversation(GetConfigurationManager(), new List<IOInterfaceReference>());

            Assert.AreEqual(expectedResponse, RunSingleConversation<BaseballConversation>(expectedParams));

            sensor.Verify(s => s.LoadScoresForTeam(It.IsAny<DateTime>(), _mockTeams[0]), Times.Exactly(1));
        }
        #endregion

        #region MLB.tv Tests
        [TestMethod]
        public void FreeMLBTVNoGame()
        {
            FreeMLBTVTest(null, "There are no free games on MLB.TV available today!");
        }

        [TestMethod]
        public void FreeMLBTVPostponed()
        {
            FreeMLBTVTest(GameState.Postponed, "There are no free games on MLB.TV available today!");
        }

        [TestMethod]
        public void FreeMLBTVNoGameError()
        {
            FreeMLBTVTest(GameState.NoGame, "Error, something went wrong!");
        }

        [TestMethod]
        public void FreeMLBTVCompletedGame()
        {
            FreeMLBTVTest(GameState.Completed, "The free game today has already completed.");
        }

        [TestMethod]
        public void FreeMLBTVGameHasntStarted()
        {
            FreeMLBTVTest(GameState.GameHasntStarted, "The Fakers at Mockers game at 8:00 AM today is free on MLB.TV.");
        }

        [TestMethod]
        public void FreeMLBTVGameRainDelay()
        {
            FreeMLBTVTest(GameState.RainDelay, "The Fakers at Mockers game, which is currently under a rain delay, is free on MLB.TV.");
        }

        [TestMethod]
        public void FreeMLBTVGameInProgress()
        {
            FreeMLBTVTest(GameState.Started, "The Fakers at Mockers game playing currently is free on MLB.TV.");
        }

        private void FreeMLBTVTest(GameState? gameState, String expectedResponse)
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
            sensor.Setup(s => s.League).Returns("MLB");
            if (gameState.HasValue)
            {
                BaseballGame game = CreateBaseballGame(gameState.Value, 5, 2, true, new List<String> { "MLB.tv" });
                sensor.Setup(s => s.LoadAllScores(It.IsAny<DateTime>())).Returns(new List<BaseballGame> { game });
            }
            else
            {
                sensor.Setup(s => s.LoadAllScores(It.IsAny<DateTime>())).Returns(new List<BaseballGame>());
            }
            AddComponentToConfigurationManager(sensor.Object);
            CurrentConversation = new BaseballConversation(GetConfigurationManager(), new List<IOInterfaceReference>());

            Assert.AreEqual(expectedResponse, RunSingleConversation<BaseballConversation>(expectedParams));

            sensor.Verify(s => s.LoadAllScores(It.IsAny<DateTime>()), Times.Exactly(1));
        }
        #endregion

        #region Utility
        private BaseballGame CreateBaseballGame(GameState gameState, int mockScore, int fakeScore, bool mockIsHome, List<String> channels)
        {
            BaseballGame game = new BaseballGame();
            game.Channels = channels;
            game.State = gameState;
            game.HomeTeamScore = new BaseballTeamScore();
            game.AwayTeamScore = new BaseballTeamScore();
            if (mockIsHome)
            {
                game.HomeTeamScore.Team = _mockTeams[0];
                game.HomeTeamScore.Score = mockScore;
                game.AwayTeamScore.Team = _mockTeams[1];
                game.AwayTeamScore.Score = fakeScore;
            }
            else
            {
                game.AwayTeamScore.Team = _mockTeams[0];
                game.AwayTeamScore.Score = mockScore;
                game.HomeTeamScore.Team = _mockTeams[1];
                game.HomeTeamScore.Score = fakeScore;
            }
            if (game.HomeTeamScore.Score > game.AwayTeamScore.Score)
            {
                game.WinningTeamScore = game.HomeTeamScore;
                game.LosingTeamScore = game.AwayTeamScore;
            }
            else
            {
                game.WinningTeamScore = game.AwayTeamScore;
                game.LosingTeamScore = game.HomeTeamScore;
            }
            game.StartingTime = new DateTime(2014, 12, 19, 8, 0, 0);
            return game;
        }
        #endregion
    }
}
