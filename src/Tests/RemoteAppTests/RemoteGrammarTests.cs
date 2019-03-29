using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps;
using SAMI.Apps.Remote;
using SAMI.IOInterfaces.Interfaces.Remote;
using SAMI.Test.Utilities;

namespace RemoteAppTests
{
    [DeploymentItem("RemoteGrammar.grxml")]
    [TestClass]
    public class RemoteGrammarTests : BaseGrammarTests
    {
        #region DVR Tests
        [TestMethod]
        public void PauseThisShowGrammar()
        {
            TestGrammar("None", "pause", "pause this show");
        }

        [TestMethod]
        public void PlayGrammar()
        {
            TestGrammar("None", "play", "play");
        }

        [TestMethod]
        public void RecordThisShowGrammar()
        {
            TestGrammar("None", "record", "record this show");
        }

        [TestMethod]
        public void WhatIsOnTvGrammar()
        {
            TestGrammar("None", "info", "what is on tv");
        }

        [TestMethod]
        public void TurnToChannelNameGrammar()
        {
            TestGrammar("Test Channel", "None", "turn to Test Channel");
        }

        [TestMethod]
        public void TurnToChannelNumberGrammar()
        {
            TestGrammar("100", "None", "turn to one hundred");
        }
        #endregion

        #region TV Tests
        [TestMethod]
        public void TurnOnTheTvToChannelGrammar()
        {
            TestGrammar("Test Channel", "powerup", "turn on the tv to Test Channel");
        }

        [TestMethod]
        public void TurnTheTvOnToChannelGrammar()
        {
            TestGrammar("Test Channel", "powerup", "turn the tv on to Test Channel");
        }
        #endregion

        private void TestGrammar(String channel, String remoteButton, String dialog)
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "remote"},
                {"remoteButton", remoteButton},
                {"channel", channel},
            };
            Mock<ITVRemote> remote = new Mock<ITVRemote>(MockBehavior.Strict);
            remote.Setup(s => s.IsValid).Returns(true);
            remote.Setup(s => s.GetChannels()).Returns(new List<String> { "Test Channel" });
            AddComponentToConfigurationManager(remote.Object);
            RemoteApp app = new RemoteApp();
            app.AddChild(new IOInterfaceReference());

            TestGrammar(app, GetConfigurationManager(), dialog, expectedParams);

            remote.Verify(s => s.GetChannels(), Times.Exactly(1));
        }
    }
}
