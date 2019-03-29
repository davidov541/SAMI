using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps.Podcast;
using SAMI.Configuration;
using SAMI.Test.Utilities;

namespace PodcastTests
{
    [TestClass]
    [DeploymentItem("testRssFeed.xml")]
    public class PodcastAppTests : BaseAppTests
    {
        [TestMethod]
        public void TestAppInvalidWhenNoPodcastInfo()
        {
            IConfigurationManager configManager = GetConfigurationManager();
            PodcastApp app = new PodcastApp();
            TestInvalidApp(app, "No podcasts have been registered with this app. Please register podcasts to play.");
        }

        [TestMethod]
        public void TestAppValidWhenPodcastInfoExists()
        {
            IConfigurationManager configManager = GetConfigurationManager();

            PodcastApp app = new PodcastApp();
            PodcastInfo info = new PodcastInfo("Test Podcast", new Uri(Directory.GetCurrentDirectory() + "/testRssFeed.xml", UriKind.Absolute), false);
            app.AddChild(info);
            
            Assert.AreEqual(true, app.IsValid);
        }
    }
}
