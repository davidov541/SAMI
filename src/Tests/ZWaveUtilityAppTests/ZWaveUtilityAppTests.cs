using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps;
using SAMI.Apps.ZWaveUtility;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.IOInterfaces.Interfaces.ZWave;
using SAMI.Test.Utilities;

namespace ZWaveUtilityAppTests
{
    [TestClass]
    public class ZWaveUtilityAppTests : BaseAppTests
    {
        [TestMethod]
        public void InvalidAppWhenNoNodesAvailable()
        {
            ZWaveUtilityApp app = new ZWaveUtilityApp();
            app.Initialize(GetConfigurationManager());
            TestInvalidApp(app, "No devices are available for pairing.");
        }

        [TestMethod]
        public void TryGetConversationPairingSuccess()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "ZWaveUtility"},
                {"NodeName", "Mock Node"},
            };
            ZWaveUtilityApp app = new ZWaveUtilityApp();
            app.Initialize(GetConfigurationManager());
            Mock<IZWaveNode> node = new Mock<IZWaveNode>(MockBehavior.Strict);
            node.Setup(s => s.Name).Returns("Mock Node");
            node.Setup(s => s.IsValid).Returns(true);
            node.Setup(s => s.TryStartPairing(app)).Returns(true);
            AddComponentToConfigurationManager(node.Object);

            Conversation returnedConvo = null;
            Assert.IsTrue(app.TryCreateConversationFromPhrase(new Dialog(expectedParams, "Pair the Mock Node"), out returnedConvo));

            Assert.AreEqual("Press the button on the Mock Node to pair it.", returnedConvo.Speak());
            node.Verify(s => s.TryStartPairing(app), Times.Exactly(1));
        }

        [TestMethod]
        public void TryGetConversationResetSuccess()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "ZWaveUtility"},
            };
            Mock<IZWaveNode> node = new Mock<IZWaveNode>(MockBehavior.Strict);
            node.Setup(s => s.Name).Returns("Mock Node");
            node.Setup(s => s.IsValid).Returns(true);
            node.Setup(s => s.ResetController());
            AddComponentToConfigurationManager(node.Object);
            ZWaveUtilityApp app = new ZWaveUtilityApp();
            app.Initialize(GetConfigurationManager());

            Conversation returnedConvo = null;
            Assert.IsTrue(app.TryCreateConversationFromPhrase(new Dialog(expectedParams, "Pair the Mock Node"), out returnedConvo));

            Assert.AreEqual("O K", returnedConvo.Speak());
            node.Verify(s => s.ResetController(), Times.Exactly(1));
        }

        [TestMethod]
        public void TryGetConversationSuccessPairingFailed()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "ZWaveUtility"},
                {"NodeName", "Mock Node"},
            };
            ZWaveUtilityApp app = new ZWaveUtilityApp();
            app.Initialize(GetConfigurationManager());
            Mock<IZWaveNode> node = new Mock<IZWaveNode>(MockBehavior.Strict);
            node.Setup(s => s.Name).Returns("Mock Node");
            node.Setup(s => s.IsValid).Returns(true);
            node.Setup(s => s.TryStartPairing(app)).Returns(false);
            AddComponentToConfigurationManager(node.Object);

            Conversation returnedConvo = null;
            Assert.IsTrue(app.TryCreateConversationFromPhrase(new Dialog(expectedParams, "Pair the Mock Node"), out returnedConvo));

            Assert.AreEqual("Press the button on the Mock Node to pair it.", returnedConvo.Speak());
            node.Verify(s => s.TryStartPairing(app), Times.Exactly(1));
        }

        private Conversation _raisedConvo;
        [TestMethod]
        public void TryGetConversationCreatedOnFinish()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "ZWaveUtility"},
                {"NodeName", "Mock Node"},
            };
            ZWaveUtilityApp app = new ZWaveUtilityApp();
            app.Initialize(GetConfigurationManager());
            Mock<IZWaveNode> node = new Mock<IZWaveNode>(MockBehavior.Strict);
            node.Setup(s => s.Name).Returns("Mock Node");
            node.Setup(s => s.IsValid).Returns(true);
            node.Setup(s => s.TryStartPairing(app)).Returns(true);
            AddComponentToConfigurationManager(node.Object);
            Conversation returnedConvo = null;
            app.TryCreateConversationFromPhrase(new Dialog(expectedParams, "Pair the Mock Node"), out returnedConvo);
            node.Verify(s => s.TryStartPairing(app), Times.Exactly(1));
            app.AsyncAlertRaised += (sender, e) => _raisedConvo = e.StartedConversation;

            app.PairingCompleted();

            Assert.IsNotNull(_raisedConvo);
            Assert.AreEqual("Mock Node has been successfully paired!", _raisedConvo.Speak());
        }

        [TestMethod]
        public void TryGetConversationFailInvalidName()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "ZWaveUtility"},
                {"NodeName", "Mock Node Two"},
            };
            Mock<IZWaveNode> node = new Mock<IZWaveNode>(MockBehavior.Strict);
            node.Setup(s => s.Name).Returns("Mock Node");
            node.Setup(s => s.IsValid).Returns(true);
            node.Setup(s => s.ResetController());
            AddComponentToConfigurationManager(node.Object);
            ZWaveUtilityApp app = new ZWaveUtilityApp();
            app.Initialize(GetConfigurationManager());

            Conversation returnedConvo = null;
            Assert.IsFalse(app.TryCreateConversationFromPhrase(new Dialog(expectedParams, "Pair the Mock Node"), out returnedConvo));
        }

        [TestMethod]
        public void TryGetConversationFailInvalidCommand()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "ZWaveUtility Fake"},
                {"NodeName", "Mock Node"},
            };
            Mock<IZWaveNode> node = new Mock<IZWaveNode>(MockBehavior.Strict);
            node.Setup(s => s.Name).Returns("Mock Node");
            node.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(node.Object);
            ZWaveUtilityApp app = new ZWaveUtilityApp();
            app.Initialize(GetConfigurationManager());

            Conversation returnedConvo = null;
            Assert.IsFalse(app.TryCreateConversationFromPhrase(new Dialog(expectedParams, "Pair the Mock Node"), out returnedConvo));
        }
    }
}
