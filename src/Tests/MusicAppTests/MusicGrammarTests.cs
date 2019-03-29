using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps.Music;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Audio;
using SAMI.Test.Utilities;

namespace MusicAppTests
{
    [DeploymentItem("MusicGrammar.xml")]
    [TestClass]
    public class MusicGrammarTests : BaseGrammarTests
    {
        private List<String> _fakePlaylists = new List<String>
            {
                "test playlist one",
                "test playlist two",
            };

        [TestMethod]
        public void TestPlayMyFavorites()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "playrandom"},
                {"Playlist", "favorites"},
            };
            SetupMusicController();

            IConfigurationManager configManager = GetConfigurationManager();
            MusicApp app = new MusicApp();
            TestGrammar<MusicApp>("Play my favorites", expectedParams);

            CleanupMusicControllers(1);
        }

        [TestMethod]
        public void TestStartPlaylist()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "play"},
                {"Playlist", "test playlist one"},
            };
            SetupMusicController();

            IConfigurationManager configManager = GetConfigurationManager();
            MusicApp app = new MusicApp();
            TestGrammar<MusicApp>("Start the playlist test playlist one", expectedParams);

            CleanupMusicControllers(1);
        }

        [TestMethod]
        public void TestPlaySomePlaylist()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "playrandom"},
                {"Playlist", "test playlist one"},
            };
            SetupMusicController();

            IConfigurationManager configManager = GetConfigurationManager();
            MusicApp app = new MusicApp();
            TestGrammar<MusicApp>("Play some test playlist one", expectedParams);

            CleanupMusicControllers(1);
        }

        [TestMethod]
        public void TestStopPlaylist()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "stop"},
            };
            SetupMusicController();

            IConfigurationManager configManager = GetConfigurationManager();
            MusicApp app = new MusicApp();
            TestGrammar<MusicApp>("Stop the music", expectedParams);

            CleanupMusicControllers(1);
        }

        [TestMethod]
        public void TestListPlaylists()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "playlists"},
            };
            SetupMusicController();

            IConfigurationManager configManager = GetConfigurationManager();
            MusicApp app = new MusicApp();
            TestGrammar<MusicApp>("What playlists are available", expectedParams);

            CleanupMusicControllers(1);
        }

        [TestMethod]
        public void TestDeleteSong()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "deletesong"},
            };
            SetupMusicController();

            IConfigurationManager configManager = GetConfigurationManager();
            MusicApp app = new MusicApp();
            TestGrammar<MusicApp>("Delete this song", expectedParams);

            CleanupMusicControllers(1);
        }

        [TestMethod]
        public void TestSkipSong()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "skip"},
            };
            SetupMusicController();

            IConfigurationManager configManager = GetConfigurationManager();
            MusicApp app = new MusicApp();

            TestGrammar<MusicApp>("Skip this song", expectedParams);

            CleanupMusicControllers(1);
        }

        [TestMethod]
        public void TestWhatIsPlaying()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "song"},
            };
            SetupMusicController();

            IConfigurationManager configManager = GetConfigurationManager();
            MusicApp app = new MusicApp();

            TestGrammar<MusicApp>("What song is this", expectedParams);
            TestGrammar<MusicApp>("What song is playing", expectedParams);

            CleanupMusicControllers(2);
        }

        [TestMethod]
        public void TestWhoIsPlaying()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "artist"},
            };
            SetupMusicController();

            IConfigurationManager configManager = GetConfigurationManager();
            MusicApp app = new MusicApp();

            TestGrammar<MusicApp>("Who is playing", expectedParams);
            TestGrammar<MusicApp>("Who is this", expectedParams);

            CleanupMusicControllers(2);
        }

        private Mock<IMusicController> _mc;
        private void SetupMusicController()
        {
            _mc = new Mock<IMusicController>(MockBehavior.Strict);
            AddComponentToConfigurationManager(_mc.Object);
            _mc.Setup(s => s.GetPlaylists()).Returns(_fakePlaylists);
        }

        private void CleanupMusicControllers(int numTests)
        {
            _mc.Verify(s => s.GetPlaylists(), Times.Exactly(numTests * 2));
        }
    }
}
