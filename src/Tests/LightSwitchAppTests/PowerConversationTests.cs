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
    [TestClass]
    public class PowerConversationTests : BaseConversationTests
    {
        [TestMethod]
        public void TestTurnOnLightSwitchConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "power"},
                {"Direction", "on"},
                {"SwitchName", "Mock Light Switch"},
            };

            Mock<ILightSwitchController> switchOne = new Mock<ILightSwitchController>(MockBehavior.Strict);
            switchOne.Setup(s => s.Name).Returns("Mock Light Switch");
            switchOne.Setup(s => s.IsValid).Returns(true);
            switchOne.Setup(s => s.TurnOn());
            AddComponentToConfigurationManager(switchOne.Object);
            Mock<ILightSwitchController> switchTwo = new Mock<ILightSwitchController>(MockBehavior.Strict);
            switchTwo.Setup(s => s.Name).Returns("Mock Light Switch Two");
            switchTwo.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(switchTwo.Object);

            Assert.AreEqual("OK", RunSingleConversation<PowerConversation>(input));

            (CurrentConversation as PowerConversation).CommandTask.Wait(5000);

            switchOne.Verify(s => s.TurnOn(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestTurnOffInvalidLightSwitchConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "power"},
                {"Direction", "off"},
                {"SwitchName", "Mock Light Switch Three"},
            };

            Mock<ILightSwitchController> switchOne = new Mock<ILightSwitchController>(MockBehavior.Strict);
            switchOne.Setup(s => s.Name).Returns("Mock Light Switch");
            switchOne.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(switchOne.Object);
            Mock<ILightSwitchController> switchTwo = new Mock<ILightSwitchController>(MockBehavior.Strict);
            switchTwo.Setup(s => s.Name).Returns("Mock Light Switch Two");
            switchTwo.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(switchTwo.Object);

            Assert.AreEqual(String.Empty, RunSingleConversation<PowerConversation>(input));
        }

        [TestMethod]
        public void TestTurnOffLightSwitchConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "power"},
                {"Direction", "off"},
                {"SwitchName", "Mock Light Switch Two"},
            };

            Mock<ILightSwitchController> switchOne = new Mock<ILightSwitchController>(MockBehavior.Strict);
            switchOne.Setup(s => s.Name).Returns("Mock Light Switch");
            switchOne.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(switchOne.Object);
            Mock<ILightSwitchController> switchTwo = new Mock<ILightSwitchController>(MockBehavior.Strict);
            switchTwo.Setup(s => s.Name).Returns("Mock Light Switch Two");
            switchTwo.Setup(s => s.IsValid).Returns(true);
            switchTwo.Setup(s => s.TurnOff());
            AddComponentToConfigurationManager(switchTwo.Object);

            Assert.AreEqual("OK", RunSingleConversation<PowerConversation>(input));

            (CurrentConversation as PowerConversation).CommandTask.Wait(5000);

            switchTwo.Verify(s => s.TurnOff(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestInvalidCommandLightSwitchConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "power"},
                {"Direction", "blah"},
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

            Assert.AreEqual("Sorry, that is not a valid light switch command.", RunSingleConversation<PowerConversation>(input));
        }
    }
}
