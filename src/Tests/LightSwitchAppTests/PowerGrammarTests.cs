using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps.Power;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.LightSwitch;
using SAMI.Test.Utilities;

namespace LightsAppTests
{
    [DeploymentItem("PowerGrammar.grxml")]
    [TestClass]
    public class PowerGrammarTests : BaseGrammarTests
    {
        [TestMethod]
        public void TestTurnOnTheLightGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "power"},
                {"Direction", "on"},
                {"SwitchName", "Mock Light Switch"},
            };
            IConfigurationManager configManager = GetConfigurationManager();
            Mock<ILightSwitchController> switchOne = new Mock<ILightSwitchController>(MockBehavior.Strict);
            switchOne.Setup(s => s.Name).Returns("Mock Light Switch");
            switchOne.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(switchOne.Object);
            Mock<ILightSwitchController> switchTwo = new Mock<ILightSwitchController>(MockBehavior.Strict);
            switchTwo.Setup(s => s.Name).Returns("Mock Light Switch Two");
            switchTwo.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(switchTwo.Object);

            TestGrammar<PowerApp>("Turn on the mock light switch", expectedParams);
        }

        [TestMethod]
        public void TestTurnTheLightOnGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "power"},
                {"Direction", "on"},
                {"SwitchName", "Mock Light Switch"},
            };
            IConfigurationManager configManager = GetConfigurationManager();
            Mock<ILightSwitchController> switchOne = new Mock<ILightSwitchController>(MockBehavior.Strict);
            switchOne.Setup(s => s.Name).Returns("Mock Light Switch");
            switchOne.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(switchOne.Object);
            Mock<ILightSwitchController> switchTwo = new Mock<ILightSwitchController>(MockBehavior.Strict);
            switchTwo.Setup(s => s.Name).Returns("Mock Light Switch Two");
            switchTwo.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(switchTwo.Object);

            TestGrammar<PowerApp>("Turn the mock light switch on", expectedParams);
        }

        [TestMethod]
        public void TestTurnOffTheLightGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "power"},
                {"Direction", "off"},
                {"SwitchName", "Mock Light Switch Two"},
            };
            IConfigurationManager configManager = GetConfigurationManager();
            Mock<ILightSwitchController> switchOne = new Mock<ILightSwitchController>(MockBehavior.Strict);
            switchOne.Setup(s => s.Name).Returns("Mock Light Switch");
            switchOne.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(switchOne.Object);
            Mock<ILightSwitchController> switchTwo = new Mock<ILightSwitchController>(MockBehavior.Strict);
            switchTwo.Setup(s => s.Name).Returns("Mock Light Switch Two");
            switchTwo.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(switchTwo.Object);

            TestGrammar<PowerApp>("Turn off the mock light switch two", expectedParams);
        }

        [TestMethod]
        public void TestTurnTheLightOffGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "power"},
                {"Direction", "off"},
                {"SwitchName", "Mock Light Switch Two"},
            };
            IConfigurationManager configManager = GetConfigurationManager();
            Mock<ILightSwitchController> switchOne = new Mock<ILightSwitchController>(MockBehavior.Strict);
            switchOne.Setup(s => s.Name).Returns("Mock Light Switch");
            switchOne.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(switchOne.Object);
            Mock<ILightSwitchController> switchTwo = new Mock<ILightSwitchController>(MockBehavior.Strict);
            switchTwo.Setup(s => s.Name).Returns("Mock Light Switch Two");
            switchTwo.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(switchTwo.Object);

            TestGrammar<PowerApp>("Turn the mock light switch two off", expectedParams);
        }
    }
}
