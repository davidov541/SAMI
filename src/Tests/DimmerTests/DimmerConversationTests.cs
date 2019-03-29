using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps.Dimmer;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.LightSwitch;
using SAMI.Test.Utilities;

namespace DimmerAppTests
{
    [TestClass]
    public class DimmerConversationTests : BaseConversationTests
    {
        [TestMethod]
        public void TestSetLightLevelConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Dimmer"},
                {"Switch", "Mock Light Switch"},
                {"Level", "50"},
            };

            Mock<IDimmableLightSwitchController> switchOne = new Mock<IDimmableLightSwitchController>(MockBehavior.Strict);
            switchOne.Setup(s => s.Name).Returns("Mock Light Switch");
            switchOne.Setup(s => s.IsValid).Returns(true);
            switchOne.Setup(s => s.SetLightLevel(0.5));
            AddComponentToConfigurationManager(switchOne.Object);
            Mock<IDimmableLightSwitchController> switchTwo = new Mock<IDimmableLightSwitchController>(MockBehavior.Strict);
            switchTwo.Setup(s => s.Name).Returns("Mock Light Switch Two");
            switchTwo.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(switchTwo.Object);

            Assert.AreEqual("OK", RunSingleConversation<DimmerConversation>(input));

            switchOne.Verify(s => s.SetLightLevel(0.5), Times.Exactly(1));
        }
    }
}
