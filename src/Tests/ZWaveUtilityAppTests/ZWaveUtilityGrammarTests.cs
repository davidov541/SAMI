using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps.ZWaveUtility;
using SAMI.IOInterfaces.Interfaces.ZWave;
using SAMI.Test.Utilities;

namespace ZWaveUtilityAppTests
{
    [DeploymentItem("ZWaveUtilityGrammar.grxml")]
    [TestClass]
    public class ZWaveUtilityGrammarTests : BaseGrammarTests
    {
        [TestMethod]
        public void PairNodeSuccess()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "ZWaveUtility"},
                {"NodeName", "Mock Node"},
            };
            Mock<IZWaveNode> switchOne = new Mock<IZWaveNode>(MockBehavior.Strict);
            switchOne.Setup(s => s.Name).Returns("Mock Node");
            switchOne.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(switchOne.Object);
            Mock<IZWaveNode> switchTwo = new Mock<IZWaveNode>(MockBehavior.Strict);
            switchTwo.Setup(s => s.Name).Returns("Mock Node Two");
            switchTwo.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(switchTwo.Object);

            TestGrammar<ZWaveUtilityApp>("Pair the Mock Node", expectedParams);
        }
    }
}
