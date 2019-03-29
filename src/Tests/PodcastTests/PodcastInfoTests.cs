using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps.Podcast;
using SAMI.Configuration;
using SAMI.Test.Utilities;

namespace PodcastTests
{
    [TestClass]
    public class PodcastInfoTests : BaseSAMITests
    {
        [TestMethod]
        [DeploymentItem("testRssFeed.xml")]
        public void TestItemHasPropertiesSet()
        {
            IConfigurationManager configManager = GetConfigurationManager();
            PodcastInfo info = new PodcastInfo("Test Podcast", new Uri(Directory.GetCurrentDirectory() + "/testRssFeed.xml", UriKind.Absolute), false);

            Assert.AreEqual(false, info.IsValid);
            Assert.AreEqual("Test Podcast", info.Name);
            Assert.AreEqual(null, info.NextAudioFileLocation);
        }

        [TestMethod]
        public void TestItemHasPropertiesSetAfterInitialize()
        {
            IConfigurationManager configManager = GetConfigurationManager();
            PodcastInfo info = new PodcastInfo("Test Podcast", new Uri(Directory.GetCurrentDirectory() + "/testRssFeed.xml", UriKind.Absolute), false);
            info.Initialize(configManager);

            Assert.AreEqual(true, info.IsValid);
            Assert.AreEqual("Test Podcast", info.Name);
            Assert.AreEqual(new Uri("http://www.podtrac.com/pts/redirect.mp3/twit.cachefly.net/audio/tnt/tnt1129/tnt1129.mp3"), info.NextAudioFileLocation);
        }

        [TestMethod]
        public void TestItemIgnoresInvalidLocalUri()
        {
            IConfigurationManager configManager = GetConfigurationManager();
            PodcastInfo info = new PodcastInfo("Test Podcast", new Uri(Directory.GetCurrentDirectory() + "/InvalidFeedFile.xml", UriKind.Absolute), false);
            info.Initialize(configManager);

            Assert.AreEqual(false, info.IsValid);
            Assert.AreEqual("Test Podcast", info.Name);
            Assert.AreEqual(null, info.NextAudioFileLocation);
        }

        [TestMethod]
        public void TestItemIgnoresInvalidWebUri()
        {
            IConfigurationManager configManager = GetConfigurationManager();
            PodcastInfo info = new PodcastInfo("Test Podcast", new Uri("http://www.samiautomation.com/invalidrss.xml", UriKind.Absolute), false);
            info.Initialize(configManager);

            Assert.AreEqual(false, info.IsValid);
            Assert.AreEqual("Test Podcast", info.Name);
            Assert.AreEqual(null, info.NextAudioFileLocation);
        }

        [TestMethod]
        public void TestItemMovesToNextFileNoSql()
        {
            IConfigurationManager configManager = GetConfigurationManager();
            PodcastInfo info = new PodcastInfo("Test Podcast", new Uri(Directory.GetCurrentDirectory() + "/testRssFeed.xml", UriKind.Absolute), false);
            info.Initialize(configManager);
            info.NextEpisodeFinished();

            Assert.AreEqual(true, info.IsValid);
            Assert.AreEqual("Test Podcast", info.Name);
            // Not correct behavior, but required until we are able to have access to database.
            Assert.AreEqual(new Uri("http://www.podtrac.com/pts/redirect.mp3/twit.cachefly.net/audio/tnt/tnt1129/tnt1129.mp3"), info.NextAudioFileLocation);
        }
    }
}
