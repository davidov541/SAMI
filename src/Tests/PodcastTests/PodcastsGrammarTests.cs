using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAMI.Apps.Podcast;
using SAMI.Configuration;
using SAMI.Test.Utilities;

namespace PodcastTests
{
    [TestClass]
    [DeploymentItem("PodcastGrammar.grxml")]
    public class PodcastsGrammarTests : BaseGrammarTests
    {
        [TestMethod]
        [DeploymentItem("testRssFeed.xml")]
        public void TestPlayPodcastGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "podcast"},
                {"Subcommand", "playnext"},
                {"Podcast", "Test Podcast"},
            };
            IConfigurationManager configManager = GetConfigurationManager();

            PodcastApp app = new PodcastApp();
            PodcastInfo info =  new PodcastInfo("Test Podcast", new Uri(Directory.GetCurrentDirectory() + "/testRssFeed.xml", UriKind.Absolute), false);
            app.AddChild(info);

            TestGrammar(app, configManager, "Play test podcast", expectedParams);
            TestGrammar(app, configManager, "Play the test podcast", expectedParams);
            TestGrammar(app, configManager, "Play test podcast podcast", expectedParams);
            TestGrammar(app, configManager, "Play the test podcast podcast", expectedParams);
        }

        [TestMethod]
        public void TestPausePodcastGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "podcast"},
                {"Subcommand", "pause"},
            };
            IConfigurationManager configManager = GetConfigurationManager();

            PodcastApp app = new PodcastApp();
            PodcastInfo info = new PodcastInfo("Test Podcast", new Uri(Directory.GetCurrentDirectory() + "/testRssFeed.xml", UriKind.Absolute), false);
            app.AddChild(info);

            TestGrammar(app, configManager, "Pause the current podcast", expectedParams);
        }

        [TestMethod]
        public void TestResumePodcastGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "podcast"},
                {"Subcommand", "resume"},
            };
            IConfigurationManager configManager = GetConfigurationManager();

            PodcastApp app = new PodcastApp();
            PodcastInfo info = new PodcastInfo("Test Podcast", new Uri(Directory.GetCurrentDirectory() + "/testRssFeed.xml", UriKind.Absolute), false);
            app.AddChild(info);

            TestGrammar(app, configManager, "Resume the current podcast", expectedParams);
        }
    }
}
