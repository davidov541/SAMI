using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps.Podcast;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Audio;
using SAMI.Test.Utilities;

namespace PodcastTests
{
    [DeploymentItem("testRssFeed.xml")]
    [TestClass]
    public class PodcastConversationTests : BaseConversationTests
    {
        [TestMethod]
        public void TestPlayPodcastNormalConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "podcast"},
                {"Subcommand", "playnext"},
                {"Podcast", "Test Podcast"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IMediaFilePlayer> mediaPlayer = new Mock<IMediaFilePlayer>(MockBehavior.Strict);
            mediaPlayer.Setup(s => s.IsValid).Returns(true);
            mediaPlayer.Setup(s => s.CurrentState).Returns(PlayerState.Stopped);
            mediaPlayer.Setup(s => s.PlayAudioFile("http://www.podtrac.com/pts/redirect.mp3/twit.cachefly.net/audio/tnt/tnt1129/tnt1129.mp3"));
            AddComponentToConfigurationManager(mediaPlayer.Object);
            
            PodcastInfo info =  new PodcastInfo("Test Podcast", new Uri(Directory.GetCurrentDirectory() + "/testRssFeed.xml", UriKind.Absolute), false);
            info.Initialize(configMan);

            CurrentConversation = new PodcastConversation(configMan, new List<PodcastInfo> { info });

            Assert.AreEqual("", RunSingleConversation<PodcastConversation>(input));

            mediaPlayer.Verify(s => s.CurrentState, Times.Exactly(1));
            mediaPlayer.Verify(s => s.PlayAudioFile("http://www.podtrac.com/pts/redirect.mp3/twit.cachefly.net/audio/tnt/tnt1129/tnt1129.mp3"), Times.Exactly(1));
        }

        [TestMethod]
        public void TestPlayPodcastAlreadyPlaying()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "podcast"},
                {"Subcommand", "playnext"},
                {"Podcast", "Test Podcast"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IMediaFilePlayer> mediaPlayer = new Mock<IMediaFilePlayer>(MockBehavior.Strict);
            mediaPlayer.Setup(s => s.IsValid).Returns(true);
            mediaPlayer.Setup(s => s.CurrentState).Returns(PlayerState.Playing);
            mediaPlayer.Setup(s => s.StopAudioFile());
            mediaPlayer.Setup(s => s.PlayAudioFile("http://www.podtrac.com/pts/redirect.mp3/twit.cachefly.net/audio/tnt/tnt1129/tnt1129.mp3"));
            AddComponentToConfigurationManager(mediaPlayer.Object);

            PodcastInfo info = new PodcastInfo("Test Podcast", new Uri(Directory.GetCurrentDirectory() + "/testRssFeed.xml", UriKind.Absolute), false);
            info.Initialize(configMan);

            CurrentConversation = new PodcastConversation(configMan, new List<PodcastInfo> { info });

            Assert.AreEqual("", RunSingleConversation<PodcastConversation>(input));

            mediaPlayer.Verify(s => s.CurrentState, Times.Exactly(1));
            mediaPlayer.Verify(s => s.StopAudioFile(), Times.Exactly(1));
            mediaPlayer.Verify(s => s.PlayAudioFile("http://www.podtrac.com/pts/redirect.mp3/twit.cachefly.net/audio/tnt/tnt1129/tnt1129.mp3"), Times.Exactly(1));
        }

        [TestMethod]
        public void TestPlayPodcastAlreadyPaused()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "podcast"},
                {"Subcommand", "playnext"},
                {"Podcast", "Test Podcast"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IMediaFilePlayer> mediaPlayer = new Mock<IMediaFilePlayer>(MockBehavior.Strict);
            mediaPlayer.Setup(s => s.IsValid).Returns(true);
            mediaPlayer.Setup(s => s.CurrentState).Returns(PlayerState.Paused);
            mediaPlayer.Setup(s => s.StopAudioFile());
            mediaPlayer.Setup(s => s.PlayAudioFile("http://www.podtrac.com/pts/redirect.mp3/twit.cachefly.net/audio/tnt/tnt1129/tnt1129.mp3"));
            AddComponentToConfigurationManager(mediaPlayer.Object);

            PodcastInfo info = new PodcastInfo("Test Podcast", new Uri(Directory.GetCurrentDirectory() + "/testRssFeed.xml", UriKind.Absolute), false);
            info.Initialize(configMan);

            CurrentConversation = new PodcastConversation(configMan, new List<PodcastInfo> { info });

            Assert.AreEqual("", RunSingleConversation<PodcastConversation>(input));

            mediaPlayer.Verify(s => s.CurrentState, Times.Exactly(1));
            mediaPlayer.Verify(s => s.StopAudioFile(), Times.Exactly(1));
            mediaPlayer.Verify(s => s.PlayAudioFile("http://www.podtrac.com/pts/redirect.mp3/twit.cachefly.net/audio/tnt/tnt1129/tnt1129.mp3"), Times.Exactly(1));
        }

        [TestMethod]
        public void TestPausePodcastNormalConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "podcast"},
                {"Subcommand", "pause"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IMediaFilePlayer> mediaPlayer = new Mock<IMediaFilePlayer>(MockBehavior.Strict);
            mediaPlayer.Setup(s => s.IsValid).Returns(true);
            mediaPlayer.Setup(s => s.CurrentState).Returns(PlayerState.Playing);
            mediaPlayer.Setup(s => s.PauseAudioFile());
            AddComponentToConfigurationManager(mediaPlayer.Object);

            PodcastInfo info = new PodcastInfo("Test Podcast", new Uri(Directory.GetCurrentDirectory() + "/testRssFeed.xml", UriKind.Absolute), false);
            info.Initialize(configMan);

            CurrentConversation = new PodcastConversation(configMan, new List<PodcastInfo> { info });

            Assert.AreEqual("", RunSingleConversation<PodcastConversation>(input));

            mediaPlayer.Verify(s => s.CurrentState, Times.Exactly(1));
            mediaPlayer.Verify(s => s.PauseAudioFile(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestPausePodcastAlreadyStopped()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "podcast"},
                {"Subcommand", "pause"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IMediaFilePlayer> mediaPlayer = new Mock<IMediaFilePlayer>(MockBehavior.Strict);
            mediaPlayer.Setup(s => s.IsValid).Returns(true);
            mediaPlayer.Setup(s => s.CurrentState).Returns(PlayerState.Stopped);
            AddComponentToConfigurationManager(mediaPlayer.Object);

            PodcastInfo info = new PodcastInfo("Test Podcast", new Uri(Directory.GetCurrentDirectory() + "/testRssFeed.xml", UriKind.Absolute), false);
            info.Initialize(configMan);

            CurrentConversation = new PodcastConversation(configMan, new List<PodcastInfo> { info });

            Assert.AreEqual("", RunSingleConversation<PodcastConversation>(input));

            mediaPlayer.Verify(s => s.CurrentState, Times.Exactly(1));
        }

        [TestMethod]
        public void TestPausePodcastAlreadyPaused()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "podcast"},
                {"Subcommand", "pause"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IMediaFilePlayer> mediaPlayer = new Mock<IMediaFilePlayer>(MockBehavior.Strict);
            mediaPlayer.Setup(s => s.IsValid).Returns(true);
            mediaPlayer.Setup(s => s.CurrentState).Returns(PlayerState.Paused);
            AddComponentToConfigurationManager(mediaPlayer.Object);

            PodcastInfo info = new PodcastInfo("Test Podcast", new Uri(Directory.GetCurrentDirectory() + "/testRssFeed.xml", UriKind.Absolute), false);
            info.Initialize(configMan);

            CurrentConversation = new PodcastConversation(configMan, new List<PodcastInfo> { info });

            Assert.AreEqual("", RunSingleConversation<PodcastConversation>(input));

            mediaPlayer.Verify(s => s.CurrentState, Times.Exactly(1));
        }

        [TestMethod]
        public void TestResumePodcastNormalConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "podcast"},
                {"Subcommand", "resume"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IMediaFilePlayer> mediaPlayer = new Mock<IMediaFilePlayer>(MockBehavior.Strict);
            mediaPlayer.Setup(s => s.IsValid).Returns(true);
            mediaPlayer.Setup(s => s.CurrentState).Returns(PlayerState.Paused);
            mediaPlayer.Setup(s => s.ResumeAudioFile());
            AddComponentToConfigurationManager(mediaPlayer.Object);

            PodcastInfo info = new PodcastInfo("Test Podcast", new Uri(Directory.GetCurrentDirectory() + "/testRssFeed.xml", UriKind.Absolute), false);
            info.Initialize(configMan);

            CurrentConversation = new PodcastConversation(configMan, new List<PodcastInfo> { info });

            Assert.AreEqual("", RunSingleConversation<PodcastConversation>(input));

            mediaPlayer.Verify(s => s.CurrentState, Times.Exactly(1));
            mediaPlayer.Verify(s => s.ResumeAudioFile(), Times.Exactly(1));
        }

        [TestMethod]
        public void TestResumePodcastAlreadyPlaying()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "podcast"},
                {"Subcommand", "resume"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IMediaFilePlayer> mediaPlayer = new Mock<IMediaFilePlayer>(MockBehavior.Strict);
            mediaPlayer.Setup(s => s.IsValid).Returns(true);
            mediaPlayer.Setup(s => s.CurrentState).Returns(PlayerState.Playing);
            AddComponentToConfigurationManager(mediaPlayer.Object);

            PodcastInfo info = new PodcastInfo("Test Podcast", new Uri(Directory.GetCurrentDirectory() + "/testRssFeed.xml", UriKind.Absolute), false);
            info.Initialize(configMan);

            CurrentConversation = new PodcastConversation(configMan, new List<PodcastInfo> { info });

            Assert.AreEqual("", RunSingleConversation<PodcastConversation>(input));

            mediaPlayer.Verify(s => s.CurrentState, Times.Exactly(1));
        }

        [TestMethod]
        public void TestResumePodcastAlreadyStopped()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "podcast"},
                {"Subcommand", "resume"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IMediaFilePlayer> mediaPlayer = new Mock<IMediaFilePlayer>(MockBehavior.Strict);
            mediaPlayer.Setup(s => s.IsValid).Returns(true);
            mediaPlayer.Setup(s => s.CurrentState).Returns(PlayerState.Stopped);
            AddComponentToConfigurationManager(mediaPlayer.Object);

            PodcastInfo info = new PodcastInfo("Test Podcast", new Uri(Directory.GetCurrentDirectory() + "/testRssFeed.xml", UriKind.Absolute), false);
            info.Initialize(configMan);

            CurrentConversation = new PodcastConversation(configMan, new List<PodcastInfo> { info });

            Assert.AreEqual("", RunSingleConversation<PodcastConversation>(input));

            mediaPlayer.Verify(s => s.CurrentState, Times.Exactly(1));
        }
    }
}
