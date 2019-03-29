using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps.Music;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Audio;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Test.Utilities;

namespace MusicAppTests
{
    [TestClass]
    public class MusicAppTests : BaseAppTests
    {
        [TestMethod]
        public void TestAppInvalidWhenNoMusicController()
        {
            MusicApp app = new MusicApp();
            TestInvalidApp(app, "Spotify has encountered some issues with connecting to the service. Please try again later.");
            Assert.AreEqual(null, app.GetMainGrammar());
        }

        [TestMethod]
        public void TestAppInvalidWhenNoPlaylistsAvailable()
        {
            IConfigurationManager configManager = GetConfigurationManager();
            Mock<IMusicController> mc = new Mock<IMusicController>(MockBehavior.Strict);
            AddComponentToConfigurationManager(mc.Object);

            mc.Setup(s => s.GetPlaylists()).Returns(new List<String>());
            MusicApp app = new MusicApp();

            TestInvalidApp(app, "Spotify has encountered some issues with connecting to the service. Please try again later.");

            mc.Verify(s => s.GetPlaylists(), Times.Exactly(2));
        }
    }
}
