using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.IOInterfaces.Interfaces.Audio;
using SAMI.IOInterfaces.Spotify;
using SAMI.Test.Utilities;

namespace SpotifyTests
{
    [TestClass]
    public class SpotifyPlayerTests : BaseSAMITests
    {
        #region Constructor and Initialize Tests
        [TestMethod]
        public void ConstructorSetsCorrectValues()
        {
            SpotifyPlayer player = new SpotifyPlayer();

            Assert.AreEqual("Spotify", player.Name);
            Assert.AreEqual(2, player.Properties.Count());
            Assert.AreEqual(1, player.Properties.Count(p => p.Name.Equals("Username")));
            Assert.AreEqual(1, player.Properties.Count(p => p.Name.Equals("Password")));
            Assert.AreEqual(PlayerState.Stopped, player.State);
        }

        [TestMethod]
        public void InitializeSuccess()
        {
            SpotifyPlayer player = CreateInitializedPlayer();

            Assert.AreEqual(PlayerState.Stopped, player.State);
        }
        #endregion

        #region LogIn Tests
        [TestMethod]
        public void LogInSuccess()
        {
            SpotifyPlayer player = CreateInitializedPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);
            wrapper.Setup(s => s.LogIn());

            player.LogIn();

            wrapper.Verify(s => s.LogIn(), Times.Exactly(1));
            Assert.AreEqual(PlayerState.Stopped, player.State);
        }

        [TestMethod]
        public void LogInFailAlreadyLoggedIn()
        {
            SpotifyPlayer player = CreateInitializedPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);
            wrapper.Setup(s => s.LogIn());
            player.LogIn();
            wrapper.Verify(s => s.LogIn(), Times.Exactly(1));
            wrapper.ResetCalls();

            player.LogIn();

            Assert.AreEqual(PlayerState.Stopped, player.State);
        }
        #endregion

        #region LogOutTests
        [TestMethod]
        public void LogOutSuccess()
        {
            SpotifyPlayer player = CreateInitializedPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);
            wrapper.Setup(s => s.LogIn());
            player.LogIn();
            wrapper.Setup(p => p.LogOut());

            player.LogOut();

            wrapper.Verify(p => p.LogOut(), Times.Exactly(1));
            Assert.AreEqual(PlayerState.Stopped, player.State);
        }

        [TestMethod]
        public void LogOutSucceedAlreadyPlaying()
        {
            SpotifyPlayer player = CreatePlayingSpotifyPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);
            wrapper.Setup(p => p.LogOut());
            wrapper.Setup(p => p.StopMusic());

            player.LogOut();

            wrapper.Verify(p => p.StopMusic(), Times.Exactly(1));
            wrapper.Verify(p => p.LogOut(), Times.Exactly(1));
            Assert.AreEqual(PlayerState.Stopped, player.State);
        }

        [TestMethod]
        public void LogOutFailNotLoggedIn()
        {
            SpotifyPlayer player = CreateInitializedPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);

            player.LogOut();

            Assert.AreEqual(PlayerState.Stopped, player.State);
        }
        #endregion

        #region TryStartPlaylist Tests
        [TestMethod]
        public void TryStartPlaylistSuccess()
        {
            SpotifyPlayer player = CreateInitializedPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);
            wrapper.Setup(s => s.LogIn());
            player.LogIn();
            wrapper.ResetCalls();
            wrapper.Setup(s => s.TryStartPlaylist("test playlist", true)).Returns(true);

            player.TryStartPlaylist("test playlist", true);

            wrapper.Verify(s => s.TryStartPlaylist("test playlist", true), Times.Exactly(1));
            Assert.AreEqual(PlayerState.Playing, player.State);
        }

        [TestMethod]
        public void TryStartPlaylistSuccessNotLoggedIn()
        {
            SpotifyPlayer player = CreateInitializedPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);
            wrapper.ResetCalls();
            wrapper.Setup(s => s.LogIn());
            wrapper.Setup(s => s.TryStartPlaylist("test playlist", true)).Returns(true);

            player.TryStartPlaylist("test playlist", true);

            wrapper.Verify(s => s.LogIn(), Times.Exactly(1));
            wrapper.Verify(s => s.TryStartPlaylist("test playlist", true), Times.Exactly(1));
            Assert.AreEqual(PlayerState.Playing, player.State);
        }

        [TestMethod]
        public void TryStartPlaylistFailed()
        {
            SpotifyPlayer player = CreateInitializedPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);
            wrapper.Setup(s => s.LogIn());
            player.LogIn();
            wrapper.ResetCalls();
            wrapper.Setup(s => s.TryStartPlaylist("test playlist", true)).Returns(false);

            player.TryStartPlaylist("test playlist", true);

            wrapper.Verify(s => s.TryStartPlaylist("test playlist", true), Times.Exactly(1));
            Assert.AreEqual(PlayerState.Stopped, player.State);
        }
        #endregion

        #region StopMusic Tests
        [TestMethod]
        public void StopMusicSucceed()
        {
            SpotifyPlayer player = CreatePlayingSpotifyPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);
            wrapper.Setup(p => p.LogOut());
            wrapper.Setup(p => p.StopMusic());

            player.StopMusic();

            wrapper.Verify(p => p.StopMusic(), Times.Exactly(1));
            wrapper.Verify(p => p.LogOut(), Times.Exactly(1));
            Assert.AreEqual(PlayerState.Stopped, player.State);
        }

        [TestMethod]
        public void StopMusicFailNotPlaying()
        {
            SpotifyPlayer player = CreateInitializedPlayer();

            player.StopMusic();

            Assert.AreEqual(PlayerState.Stopped, player.State);
        }
        #endregion

        #region GetPlaylists Tests
        [TestMethod]
        public void GetPlaylistsSucceed()
        {
            SpotifyPlayer player = CreateLoggedInPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);
            List<String> expectedPlaylists = new List<string>
            {
                "Test Playlist",
                "Test Playlist 2",
                "Test Playlist 3",
            };
            wrapper.Setup(p => p.GetPlaylists()).Returns(expectedPlaylists);

            IEnumerable<String> playlists = player.GetPlaylists();

            wrapper.Verify(p => p.GetPlaylists(), Times.Exactly(1));
            Assert.AreEqual(expectedPlaylists.Count, playlists.Count());
            Assert.IsTrue(expectedPlaylists.Contains(playlists.ElementAt(0)));
            Assert.IsTrue(expectedPlaylists.Contains(playlists.ElementAt(1)));
            Assert.IsTrue(expectedPlaylists.Contains(playlists.ElementAt(2)));
            Assert.AreEqual(PlayerState.Stopped, player.State);
        }

        [TestMethod]
        public void GetPlaylistsSucceedNotLoggedIn()
        {
            SpotifyPlayer player = CreateInitializedPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);
            List<String> expectedPlaylists = new List<string>
            {
                "Test Playlist",
                "Test Playlist 2",
                "Test Playlist 3",
            };
            wrapper.Setup(p => p.LogIn());
            wrapper.Setup(p => p.GetPlaylists()).Returns(expectedPlaylists);
            wrapper.Setup(p => p.LogOut());

            IEnumerable<String> playlists = player.GetPlaylists();

            wrapper.Verify(p => p.GetPlaylists(), Times.Exactly(1));
            wrapper.Verify(p => p.LogIn(), Times.Exactly(1));
            wrapper.Verify(p => p.LogOut(), Times.Exactly(1));
            Assert.AreEqual(expectedPlaylists.Count, playlists.Count());
            Assert.IsTrue(expectedPlaylists.Contains(playlists.ElementAt(0)));
            Assert.IsTrue(expectedPlaylists.Contains(playlists.ElementAt(1)));
            Assert.IsTrue(expectedPlaylists.Contains(playlists.ElementAt(2)));
            Assert.AreEqual(PlayerState.Stopped, player.State);
        }
        #endregion

        #region DeleteCurrentTrack Tests
        [TestMethod]
        public void DeleteCurrentTrackSucceed()
        {
            SpotifyPlayer player = CreatePlayingSpotifyPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);
            wrapper.Setup(p => p.DeleteCurrentTrack());

            player.DeleteCurrentTrack();

            wrapper.Verify(p => p.DeleteCurrentTrack(), Times.Exactly(1));
            Assert.AreEqual(PlayerState.Playing, player.State);
            StopPlayer(player);
        }

        [TestMethod]
        public void DeleteCurrentTrackFailNotPlaying()
        {
            SpotifyPlayer player = CreateLoggedInPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);

            player.DeleteCurrentTrack();

            Assert.AreEqual(PlayerState.Stopped, player.State);
        }
        #endregion

        #region SkipToNextTrack Tests
        [TestMethod]
        public void SkipToNextTrackSucceed()
        {
            SpotifyPlayer player = CreatePlayingSpotifyPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);
            wrapper.Setup(p => p.SkipToNextTrack());

            player.SkipToNextTrack();

            wrapper.Verify(p => p.SkipToNextTrack(), Times.Exactly(1));
            Assert.AreEqual(PlayerState.Playing, player.State);
            StopPlayer(player);
        }

        [TestMethod]
        public void SkipToNextTrackFailNotPlaying()
        {
            SpotifyPlayer player = CreateLoggedInPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);

            player.SkipToNextTrack();

            Assert.AreEqual(PlayerState.Stopped, player.State);
        }
        #endregion 

        #region GetCurrentSongName Tests
        [TestMethod]
        public void GetCurrentSongNameSucceed()
        {
            SpotifyPlayer player = CreatePlayingSpotifyPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);
            String expectedSongName = "Test Song";
            wrapper.Setup(p => p.GetCurrentSongName()).Returns(expectedSongName);

            String songName = player.GetCurrentSongName();

            wrapper.Verify(p => p.GetCurrentSongName(), Times.Exactly(1));
            Assert.AreEqual(expectedSongName, songName);
            Assert.AreEqual(PlayerState.Playing, player.State);
            StopPlayer(player);
        }

        [TestMethod]
        public void GetCurrentSongNameFailNotPlaying()
        {
            SpotifyPlayer player = CreateLoggedInPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);

            String songName = player.GetCurrentSongName();

            Assert.IsNull(songName);
            Assert.AreEqual(PlayerState.Stopped, player.State);
        }
        #endregion

        #region GetCurrentArtistName Tests
        [TestMethod]
        public void GetCurrentArtistNameSucceed()
        {
            SpotifyPlayer player = CreatePlayingSpotifyPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);
            String expectedArtistName = "Test Artist";
            wrapper.Setup(p => p.GetCurrentArtistName()).Returns(expectedArtistName);

            String artistName = player.GetCurrentArtistName();

            wrapper.Verify(p => p.GetCurrentArtistName(), Times.Exactly(1));
            Assert.AreEqual(expectedArtistName, artistName);
            Assert.AreEqual(PlayerState.Playing, player.State);
            StopPlayer(player);
        }

        [TestMethod]
        public void GetCurrentArtistNameFailNotPlaying()
        {
            SpotifyPlayer player = CreateLoggedInPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);

            String artistName = player.GetCurrentArtistName();

            Assert.IsNull(artistName);
            Assert.AreEqual(PlayerState.Stopped, player.State);
        }
        #endregion

        #region Dispose Tests
        [TestMethod]
        public void DisposeInitialized()
        {
            SpotifyPlayer player = CreateInitializedPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);
            wrapper.Setup(w => w.Dispose());

            player.Dispose();

            wrapper.Verify(w => w.Dispose(), Times.Exactly(1));
            Assert.IsNull(player.Spotify);
            Assert.AreEqual(PlayerState.Stopped, player.State);
        }

        [TestMethod]
        public void DisposeLoggedIn()
        {
            SpotifyPlayer player = CreateLoggedInPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);
            wrapper.Setup(w => w.Dispose());
            wrapper.Setup(w => w.LogOut());

            player.Dispose();

            wrapper.Verify(w => w.LogOut(), Times.Exactly(1));
            wrapper.Verify(w => w.Dispose(), Times.Exactly(1));
            Assert.IsNull(player.Spotify);
            Assert.AreEqual(PlayerState.Stopped, player.State);
        }

        [TestMethod]
        public void DisposePlaying()
        {
            SpotifyPlayer player = CreatePlayingSpotifyPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);
            wrapper.Setup(w => w.Dispose());
            wrapper.Setup(w => w.LogOut());
            wrapper.Setup(w => w.StopMusic());

            player.Dispose();

            wrapper.Verify(w => w.StopMusic(), Times.Exactly(1));
            wrapper.Verify(w => w.LogOut(), Times.Exactly(1));
            wrapper.Verify(w => w.Dispose(), Times.Exactly(1));
            Assert.IsNull(player.Spotify);
            Assert.AreEqual(PlayerState.Stopped, player.State);
        }
        #endregion

        #region Utility Functions
        private SpotifyPlayer CreatePlayingSpotifyPlayer()
        {
            SpotifyPlayer player = CreateLoggedInPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);
            wrapper.Setup(s => s.TryStartPlaylist("test playlist", true)).Returns(true);
            player.TryStartPlaylist("test playlist", true);
            byte[] data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            IntPtr ptr = Marshal.AllocHGlobal(data.Length);
            Marshal.Copy(data, 0, ptr, data.Length);
            wrapper.Raise(p => p.MusicDataAvailable += null, new MusicDeliveryEventArgs(ptr, data.Length, 44100, 2, 8));
            wrapper.ResetCalls();
            return player;
        }

        private SpotifyPlayer CreateLoggedInPlayer()
        {
            SpotifyPlayer player = CreateInitializedPlayer();
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);
            wrapper.Setup(s => s.LogIn());
            player.LogIn();
            wrapper.ResetCalls();
            return player;
        }

        private SpotifyPlayer CreateInitializedPlayer()
        {
            SpotifyPlayer player = new SpotifyPlayer();
            Mock<ISpotifyWrapper> spotify = new Mock<ISpotifyWrapper>(MockBehavior.Strict);
            player.Spotify = spotify.Object;

            player.Initialize(GetConfigurationManager());
            return player;
        }

        private void StopPlayer(SpotifyPlayer player)
        {
            Mock<ISpotifyWrapper> wrapper = Mock.Get(player.Spotify);
            wrapper.Setup(p => p.LogOut());
            wrapper.Setup(p => p.StopMusic());

            player.StopMusic();
        }
        #endregion
    }
}
