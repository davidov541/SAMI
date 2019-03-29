using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps;
using SAMI.Apps.Movie;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Movies;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Test.Utilities;

namespace MovieAppTests
{
    [DeploymentItem("MovieGrammar.grxml")]
    [TestClass]
    public class MovieAppTests : BaseAppTests
    {
        [TestMethod]
        public void TestAppInvalidWhenNoMovieDatabase()
        {
            MovieApp app = new MovieApp();
            TestInvalidApp(app, "No movie databases are available currently.");
        }

        [TestMethod]
        public void TestAppInvalidWhenNoShowtimesAvailable()
        {
            IConfigurationManager configManager = GetConfigurationManager();
            Mock<IMovieSensor> db = new Mock<IMovieSensor>(MockBehavior.Strict);
            db.Setup(s => s.IsValid).Returns(true);
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(new List<Showtime>());
            AddComponentToConfigurationManager(db.Object);

            MovieApp app = new MovieApp();
            TestInvalidApp(app, "No movie databases are available currently.");

            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(1));
        }

        [TestMethod]
        public void TestAppGrammarDoesNotIncludeDuplicates()
        {
            IEnumerable<Showtime> _fakeShowtimes = new List<Showtime>
            {
                new Showtime("First Test Show", "First Test Theater", "9:45 PM"),
                new Showtime("First Test Show", "First Test Theater", "8:45 PM"),
                new Showtime("Third Test Show", "Second Test Theater", "10:45 PM"),
                new Showtime("Third Test Show", "Second Test Theater", "11:45 PM"),
                new Showtime("Third Test Show", "Second Test Theater", "7:45 PM"),
                new Showtime("Third Test Show", "Second Test Theater", "6:45 PM"),
            };

            IConfigurationManager configManager = GetConfigurationManager();
            Mock<IMovieSensor> db = new Mock<IMovieSensor>(MockBehavior.Strict);
            db.Setup(s => s.IsValid).Returns(true);
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            AddComponentToConfigurationManager(db.Object);

            MovieApp app = new MovieApp();
            app.Initialize(GetConfigurationManager());

            XmlGrammar grammar = app.GetMainGrammar();
            XmlNode movieNameRule = grammar.RootDocument.LastChild.SelectSingleNode("rule[@id='MovieName']");

            foreach (XmlElement elem in grammar.RootDocument.LastChild.ChildNodes.OfType<XmlElement>())
            {
                if (elem.HasAttribute("id") && elem.Attributes["id"].Value.Equals("MovieName"))
                {
                    Assert.AreEqual(2, elem.FirstChild.ChildNodes.Count);
                }
            }

            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
        }
    }
}
