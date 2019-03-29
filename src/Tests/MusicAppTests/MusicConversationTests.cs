using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps.Music;
using SAMI.IOInterfaces.Interfaces.Audio;
using SAMI.Test.Utilities;

namespace MusicAppTests
{
    [TestClass]
    public class MusicConversationTests : BaseConversationTests
    {
        private List<String> _fakePlaylists = new List<String>
            {
                "test playlist one",
                "test playlist two",
            };

        [TestMethod]
        public void TestPlayRandomConversation()
        {
            TestPlayValidConversation(true);
        }

        [TestMethod]
        public void TestPlayConversation()
        {
            TestPlayValidConversation(false);
        }

        private void TestPlayValidConversation(bool random)
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", random ? "playrandom" : "play"},
                {"Playlist", "test playlist one"},
            };

            SetupMusicController();
            _mc.Setup(s => s.GetPlaylists()).Returns(_fakePlaylists);
            _mc.Setup(s => s.TryStartPlaylist("test playlist one", random)).Returns(true);

            Assert.AreEqual("O K", RunSingleConversation<MusicConversation>(input));

            _mc.Verify(s => s.GetPlaylists(), Times.Exactly(1));
            _mc.Verify(s => s.TryStartPlaylist("test playlist one", random), Times.Exactly(1));
        }

        [TestMethod]
        public void TestPlayRandomNoValidControllerConversation()
        {
            TestNoValidControllerConversation(true);
        }

        [TestMethod]
        public void TestPlayNoValidControllerConversation()
        {
            TestNoValidControllerConversation(false);
        }

        private void TestNoValidControllerConversation(bool random)
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", random ? "playrandom" : "play"},
                {"Playlist", "test playlist one"},
            };


            Assert.AreEqual("Could not find that playlist. Please ask for a different playlist, or ask me for one of the available playlists.", RunSingleConversation<MusicConversation>(input));
        }

        [TestMethod]
        public void TestPlayRandomInvalidPlaylistConversation()
        {
            TestInvalidPlaylistConversation(true);
        }

        [TestMethod]
        public void TestPlayInvalidPlaylistConversation()
        {
            TestInvalidPlaylistConversation(false);
        }

        private void TestInvalidPlaylistConversation(bool random)
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", random ? "playrandom" : "play"},
                {"Playlist", "invalid playlist"},
            };
            SetupMusicController();
            _mc.Setup(s => s.GetPlaylists()).Returns(_fakePlaylists);

            Assert.AreEqual("Could not find that playlist. Please ask for a different playlist, or ask me for one of the available playlists.", RunSingleConversation<MusicConversation>(input));

            _mc.Verify(s => s.GetPlaylists(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestPlayRandomCantPlayConversation()
        {
            TestCantPlayConversation(true);
        }

        [TestMethod]
        public void TestPlayCantPlayConversation()
        {
            TestCantPlayConversation(false);
        }

        private void TestCantPlayConversation(bool random)
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", random ? "playrandom" : "play"},
                {"Playlist", "test playlist one"},
            };
            SetupMusicController();
            _mc.Setup(s => s.GetPlaylists()).Returns(_fakePlaylists);
            _mc.Setup(s => s.TryStartPlaylist("test playlist one", random)).Returns(false);

            Assert.AreEqual("Could not find that playlist. Please ask for a different playlist, or ask me for one of the available playlists.", RunSingleConversation<MusicConversation>(input));

            _mc.Verify(s => s.GetPlaylists(), Times.Exactly(1));
            _mc.Verify(s => s.TryStartPlaylist("test playlist one", random), Times.Exactly(1));
        }

        [TestMethod]
        public void TestStopMusicConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "stop"},
            };
            SetupMusicController();
            _mc.Setup(s => s.State).Returns(PlayerState.Playing);
            _mc.Setup(s => s.StopMusic());

            Assert.AreEqual("O K", RunSingleConversation<MusicConversation>(input));

            _mc.Verify(s => s.State, Times.Exactly(1));
            _mc.Verify(s => s.StopMusic(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestStopMusicNoPlayingControllerConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "stop"},
            };
            SetupMusicController();
            _mc.Setup(s => s.State).Returns(PlayerState.Stopped);

            Assert.AreEqual("O K", RunSingleConversation<MusicConversation>(input));

            _mc.Verify(s => s.State, Times.Exactly(1));
        }

        [TestMethod]
        public void TestListPlaylistsConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "playlists"},
            };
            SetupMusicController();
            Mock<IMusicController> mc2 = new Mock<IMusicController>(MockBehavior.Strict);
            AddComponentToConfigurationManager(mc2.Object);
            _mc.Setup(s => s.GetPlaylists()).Returns(_fakePlaylists);
            mc2.Setup(s => s.GetPlaylists()).Returns(new List<String>
                {
                    "test playlist three",
                    "test playlist four",
                });

            Assert.AreEqual("Currently, the available playlists are test playlist one. test playlist two. test playlist three. and test playlist four.", RunSingleConversation<MusicConversation>(input));

            _mc.Verify(s => s.GetPlaylists(), Times.Exactly(1));
            mc2.Verify(s => s.GetPlaylists(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestListPlaylistsNoPlaylistsConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "playlists"},
            };
            SetupMusicController();
            _mc.Setup(s => s.GetPlaylists()).Returns(new List<String>());

            Assert.AreEqual("No playlists were found.", RunSingleConversation<MusicConversation>(input));

            _mc.Verify(s => s.GetPlaylists(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestDeleteSongConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "deletesong"},
            };
            SetupMusicController();
            _mc.Setup(s => s.State).Returns(PlayerState.Playing);
            _mc.Setup(s => s.DeleteCurrentTrack());
            _mc.Setup(s => s.SkipToNextTrack());

            Assert.AreEqual("O K", RunSingleConversation<MusicConversation>(input));

            _mc.Verify(s => s.DeleteCurrentTrack(), Times.Exactly(1));
            _mc.Verify(s => s.SkipToNextTrack(), Times.Exactly(1));
            _mc.Verify(s => s.State, Times.Exactly(1));
        }

        [TestMethod]
        public void TestDeleteSongNothingPlayingConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "deletesong"},
            };
            SetupMusicController();
            _mc.Setup(s => s.State).Returns(PlayerState.Stopped);

            Assert.AreEqual(String.Empty, RunSingleConversation<MusicConversation>(input));

            _mc.Verify(s => s.State, Times.Exactly(1));
        }

        [TestMethod]
        public void TestSkipSongConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "skip"},
            };
            SetupMusicController();
            _mc.Setup(s => s.State).Returns(PlayerState.Playing);
            _mc.Setup(s => s.SkipToNextTrack());

            Assert.AreEqual("O K", RunSingleConversation<MusicConversation>(input));

            _mc.Verify(s => s.SkipToNextTrack(), Times.Exactly(1));
            _mc.Verify(s => s.State, Times.Exactly(1));
        }

        [TestMethod]
        public void TestSkipSongNothingPlayingConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "skip"},
            };
            SetupMusicController();
            _mc.Setup(s => s.State).Returns(PlayerState.Stopped);

            Assert.AreEqual(String.Empty, RunSingleConversation<MusicConversation>(input));

            _mc.Verify(s => s.State, Times.Exactly(1));
        }

        [TestMethod]
        public void TestGetArtistConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "artist"},
            };
            SetupMusicController();
            _mc.Setup(s => s.State).Returns(PlayerState.Playing);
            _mc.Setup(s => s.GetCurrentArtistName()).Returns("Test Artist");

            Assert.AreEqual("This is by Test Artist.", RunSingleConversation<MusicConversation>(input));

            _mc.Verify(s => s.State, Times.Exactly(1));
            _mc.Verify(s => s.GetCurrentArtistName(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGetArtistNullReturnConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "artist"},
            };
            SetupMusicController();
            _mc.Setup(s => s.State).Returns(PlayerState.Playing);
            _mc.Setup(s => s.GetCurrentArtistName()).Returns((String)null);

            Assert.AreEqual(String.Empty, RunSingleConversation<MusicConversation>(input));

            _mc.Verify(s => s.State, Times.Exactly(1));
            _mc.Verify(s => s.GetCurrentArtistName(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGetArtistNothingPlayingConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "artist"},
            };
            SetupMusicController();
            _mc.Setup(s => s.State).Returns(PlayerState.Stopped);

            Assert.AreEqual(String.Empty, RunSingleConversation<MusicConversation>(input));

            _mc.Verify(s => s.State, Times.Exactly(1));
        }

        [TestMethod]
        public void TestGetSongConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "song"},
            };
            SetupMusicController();
            _mc.Setup(s => s.State).Returns(PlayerState.Playing);
            _mc.Setup(s => s.GetCurrentSongName()).Returns("Test Song");
            _mc.Setup(s => s.GetCurrentArtistName()).Returns("Test Artist");

            Assert.AreEqual("This is Test Song by Test Artist.", RunSingleConversation<MusicConversation>(input));

            _mc.Verify(s => s.State, Times.Exactly(1));
            _mc.Verify(s => s.GetCurrentSongName(), Times.Exactly(1));
            _mc.Verify(s => s.GetCurrentArtistName(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGetSongNullSongConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "song"},
            };
            SetupMusicController();
            _mc.Setup(s => s.State).Returns(PlayerState.Playing);
            _mc.Setup(s => s.GetCurrentSongName()).Returns((String)null);
            _mc.Setup(s => s.GetCurrentArtistName()).Returns("Test Artist");

            Assert.AreEqual(String.Empty, RunSingleConversation<MusicConversation>(input));

            _mc.Verify(s => s.State, Times.Exactly(1));
            _mc.Verify(s => s.GetCurrentSongName(), Times.Exactly(1));
            _mc.Verify(s => s.GetCurrentArtistName(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGetSongNullSongAndArtistConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "song"},
            };
            SetupMusicController();
            _mc.Setup(s => s.State).Returns(PlayerState.Playing);
            _mc.Setup(s => s.GetCurrentSongName()).Returns((String)null);
            _mc.Setup(s => s.GetCurrentArtistName()).Returns((String)null);

            Assert.AreEqual(String.Empty, RunSingleConversation<MusicConversation>(input));

            _mc.Verify(s => s.State, Times.Exactly(1));
            _mc.Verify(s => s.GetCurrentSongName(), Times.Exactly(1));
            _mc.Verify(s => s.GetCurrentArtistName(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGetSongNullArtistConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "song"},
            };
            SetupMusicController();
            _mc.Setup(s => s.State).Returns(PlayerState.Playing);
            _mc.Setup(s => s.GetCurrentSongName()).Returns("Test Song");
            _mc.Setup(s => s.GetCurrentArtistName()).Returns((String)null);

            Assert.AreEqual(String.Empty, RunSingleConversation<MusicConversation>(input));

            _mc.Verify(s => s.State, Times.Exactly(1));
            _mc.Verify(s => s.GetCurrentSongName(), Times.Exactly(1));
            _mc.Verify(s => s.GetCurrentArtistName(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGetSongNothingPlayingConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "music"},
                {"Subcommand", "song"},
            };
            SetupMusicController();
            _mc.Setup(s => s.State).Returns(PlayerState.Stopped);

            Assert.AreEqual(String.Empty, RunSingleConversation<MusicConversation>(input));

            _mc.Verify(s => s.State, Times.Exactly(1));
        }

        private Mock<IMusicController> _mc;
        private void SetupMusicController()
        {
            _mc = new Mock<IMusicController>(MockBehavior.Strict);
            AddComponentToConfigurationManager(_mc.Object);
        }
    }
}
