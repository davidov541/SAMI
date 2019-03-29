using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps.Football;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Football;
using SAMI.Test.Utilities;

namespace FootballAppTests
{
    [DeploymentItem("FootballGrammar.grxml")]
    [TestClass]
    public class FootballGrammarTests : BaseGrammarTests
    {
        [TestMethod]
        public void GetScoreGrammarTest()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Football"},
                {"Team", "TestTeam"},
                {"Parameter", "Score"},
            };

            Mock<IFootballSensor> mock = new Mock<IFootballSensor>(MockBehavior.Strict);
            mock.Setup(s => s.IsValid).Returns(true);
            mock.Setup(s => s.Teams).Returns(new List<FootballTeam>
                {
                    new FootballTeam("Test", "Team", "TestTeam"),
                    new FootballTeam("Test 2", "Teams 2", "TestTeam2")
                });
            AddComponentToConfigurationManager(mock.Object);

            TestGrammar<FootballApp>("How are the Test team doing", expectedParams);
            TestGrammar<FootballApp>("What's the score of the Test Team game", expectedParams);

            mock.Verify(s => s.Teams, Times.Exactly(2));
        }
    }
}
