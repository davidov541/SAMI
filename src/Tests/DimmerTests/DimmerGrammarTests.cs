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
    [DeploymentItem("DimmerGrammar.grxml")]
    [TestClass]
    public class DimmerGrammarTests : BaseGrammarTests
    {
        [TestMethod]
        public void TestSetLevelGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Dimmer"},
                {"Switch", "Mock Light Switch"},
                {"Level", "50"},
            };
            IConfigurationManager configManager = GetConfigurationManager();
            Mock<IDimmableLightSwitchController> switchOne = new Mock<IDimmableLightSwitchController>(MockBehavior.Strict);
            switchOne.Setup(s => s.Name).Returns("Mock Light Switch");
            switchOne.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(switchOne.Object);
            Mock<IDimmableLightSwitchController> switchTwo = new Mock<IDimmableLightSwitchController>(MockBehavior.Strict);
            switchTwo.Setup(s => s.Name).Returns("Mock Light Switch Two");
            switchTwo.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(switchTwo.Object);

            TestGrammar<DimmerApp>("Set the Mock Light Switch to fifty percent", expectedParams);
        }
    }
}
