using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps;
using SAMI.Apps.Movie;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Movies;
using SAMI.Test.Utilities;

namespace MovieAppTests
{
    [TestClass]
    public class MovieConversationTests : BaseConversationTests
    {
        [TestMethod]
        public void TestGetMovieListMultipleMoviesConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Names"},
                {"Time", "TimeOfDay=today;"},
                {"Theater", ""},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            IEnumerable<Showtime> fakeShowtimes = new List<Showtime>
            {
                new Showtime("First Test Show", "First Test Theater", "9:45 PM"),
                new Showtime("Second Test Show", "First Test Theater", "8:45 PM"),
                new Showtime("Third Test Show", "Second Test Theater", "10:45 PM"),
            };

            Mock<IMovieSensor> movieDb = new Mock<IMovieSensor>(MockBehavior.Strict);
            movieDb.Setup(s => s.IsValid).Returns(true);
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            movieDb.Setup(s => s.GetMovieShowtimes(new SamiDateTime(today, DateTimeRange.Day), null, String.Empty)).Returns(fakeShowtimes);
            AddComponentToConfigurationManager(movieDb.Object);

            String expectedResponse = String.Format("On {0}, these movies will be playing. First Test Show. Second Test Show. and Third Test Show", today.ToString("MMMM d"));
            Assert.AreEqual(expectedResponse, RunSingleConversation<MovieConversation>(input));

            movieDb.Verify(s => s.GetMovieShowtimes(new SamiDateTime(today, DateTimeRange.Day), null, String.Empty), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGetMovieListOneMovieConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Names"},
                {"Time", "TimeOfDay=today;"},
                {"Theater", ""},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            IEnumerable<Showtime> fakeShowtimes = new List<Showtime>
            {
                new Showtime("First Test Show", "First Test Theater", "9:45 PM"),
            };

            Mock<IMovieSensor> movieDb = new Mock<IMovieSensor>(MockBehavior.Strict);
            movieDb.Setup(s => s.IsValid).Returns(true);
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            movieDb.Setup(s => s.GetMovieShowtimes(new SamiDateTime(today, DateTimeRange.Day), null, String.Empty)).Returns(fakeShowtimes);
            AddComponentToConfigurationManager(movieDb.Object);

            String expectedResponse = String.Format("On {0}, these movies will be playing. First Test Show", today.ToString("MMMM d"));
            Assert.AreEqual(expectedResponse, RunSingleConversation<MovieConversation>(input));

            movieDb.Verify(s => s.GetMovieShowtimes(new SamiDateTime(today, DateTimeRange.Day), null, String.Empty), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGetMovieListZeroMoviesConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Names"},
                {"Time", "TimeOfDay=today;"},
                {"Theater", ""},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            IEnumerable<Showtime> fakeShowtimes = new List<Showtime>();

            Mock<IMovieSensor> movieDb = new Mock<IMovieSensor>(MockBehavior.Strict);
            movieDb.Setup(s => s.IsValid).Returns(true);
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            movieDb.Setup(s => s.GetMovieShowtimes(new SamiDateTime(today, DateTimeRange.Day), null, String.Empty)).Returns(fakeShowtimes);
            AddComponentToConfigurationManager(movieDb.Object);

            Assert.AreEqual("I'm sorry, no movies will be shown then.", RunSingleConversation<MovieConversation>(input));

            movieDb.Verify(s => s.GetMovieShowtimes(new SamiDateTime(today, DateTimeRange.Day), null, String.Empty), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGetMovieShowtimesMultipleMoviesConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Showtimes"},
                {"Time", "TimeOfDay=today;"},
                {"Theater", ""},
                {"Title", "First Test Show"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            IEnumerable<Showtime> fakeShowtimes = new List<Showtime>
            {
                new Showtime("First Test Show", "First Test Theater", "9:45 PM"),
                new Showtime("First Test Show", "First Test Theater", "8:45 PM"),
                new Showtime("First Test Show", "Second Test Theater", "10:45 PM"),
            };

            Mock<IMovieSensor> movieDb = new Mock<IMovieSensor>(MockBehavior.Strict);
            movieDb.Setup(s => s.IsValid).Returns(true);
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            movieDb.Setup(s => s.GetMovieShowtimes(new SamiDateTime(today, DateTimeRange.Day), "First Test Show", String.Empty)).Returns(fakeShowtimes);
            AddComponentToConfigurationManager(movieDb.Object);

            String expectedResponse = String.Format("On {0}, First Test Show will be shown at First Test Theater at 09:45 . 08:45 . First Test Show will be shown at Second Test Theater at 10:45 . ", today.ToString("MMMM d"));
            Assert.AreEqual(expectedResponse, RunSingleConversation<MovieConversation>(input));

            movieDb.Verify(s => s.GetMovieShowtimes(new SamiDateTime(today, DateTimeRange.Day), "First Test Show", String.Empty), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGetMovieShowtimesOneMovieConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Showtimes"},
                {"Time", "TimeOfDay=today;"},
                {"Theater", ""},
                {"Title", "First Test Show"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            IEnumerable<Showtime> fakeShowtimes = new List<Showtime>
            {
                new Showtime("First Test Show", "First Test Theater", "9:45 PM"),
            };

            Mock<IMovieSensor> movieDb = new Mock<IMovieSensor>(MockBehavior.Strict);
            movieDb.Setup(s => s.IsValid).Returns(true);
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            movieDb.Setup(s => s.GetMovieShowtimes(new SamiDateTime(today, DateTimeRange.Day), "First Test Show", String.Empty)).Returns(fakeShowtimes);
            AddComponentToConfigurationManager(movieDb.Object);

            String expectedResponse = String.Format("On {0}, First Test Show will be shown at First Test Theater at 09:45 . ", today.ToString("MMMM d"));
            Assert.AreEqual(expectedResponse, RunSingleConversation<MovieConversation>(input));

            movieDb.Verify(s => s.GetMovieShowtimes(new SamiDateTime(today, DateTimeRange.Day), "First Test Show", String.Empty), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGetMovieShowtimesZeroMoviesConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Showtimes"},
                {"Time", "TimeOfDay=today;"},
                {"Theater", ""},
                {"Title", "First Test Show"},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            IEnumerable<Showtime> fakeShowtimes = new List<Showtime>();

            Mock<IMovieSensor> movieDb = new Mock<IMovieSensor>(MockBehavior.Strict);
            movieDb.Setup(s => s.IsValid).Returns(true);
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            movieDb.Setup(s => s.GetMovieShowtimes(new SamiDateTime(today, DateTimeRange.Day), "First Test Show", String.Empty)).Returns(fakeShowtimes);
            AddComponentToConfigurationManager(movieDb.Object);

            Assert.AreEqual("I'm sorry, First Test Show will not be shown then.", RunSingleConversation<MovieConversation>(input));

            movieDb.Verify(s => s.GetMovieShowtimes(new SamiDateTime(today, DateTimeRange.Day), "First Test Show", String.Empty), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGetMovieDescriptionConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Description"},
                {"Time", "TimeOfDay=today;"},
                {"Title", "First Test Show"},
                {"Theater", ""},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IMovieSensor> movieDb = new Mock<IMovieSensor>(MockBehavior.Strict);
            movieDb.Setup(s => s.IsValid).Returns(true);
            movieDb.Setup(s => s.GetMovieDescription("First Test Show")).Returns("Description for First Test Show");
            AddComponentToConfigurationManager(movieDb.Object);

            Assert.AreEqual("Description for First Test Show", RunSingleConversation<MovieConversation>(input));

            movieDb.Verify(s => s.GetMovieDescription("First Test Show"), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGetMovieDescriptionNoDescriptionConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Description"},
                {"Time", "TimeOfDay=today;"},
                {"Title", "First Test Show"},
                {"Theater", ""},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IMovieSensor> movieDb = new Mock<IMovieSensor>(MockBehavior.Strict);
            movieDb.Setup(s => s.IsValid).Returns(true);
            movieDb.Setup(s => s.GetMovieDescription("First Test Show")).Returns("");
            AddComponentToConfigurationManager(movieDb.Object);

            Assert.AreEqual("I can't find the description for First Test Show.", RunSingleConversation<MovieConversation>(input));

            movieDb.Verify(s => s.GetMovieDescription("First Test Show"), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGetMovieDescriptionMultipleDescriptionsConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Description"},
                {"Time", "TimeOfDay=today;"},
                {"Title", "First Test Show"},
                {"Theater", ""},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IMovieSensor> movieDb1 = new Mock<IMovieSensor>(MockBehavior.Strict);
            movieDb1.Setup(s => s.IsValid).Returns(true);
            movieDb1.Setup(s => s.GetMovieDescription("First Test Show")).Returns("");
            AddComponentToConfigurationManager(movieDb1.Object);

            Mock<IMovieSensor> movieDb2 = new Mock<IMovieSensor>(MockBehavior.Strict);
            movieDb2.Setup(s => s.IsValid).Returns(true);
            movieDb2.Setup(s => s.GetMovieDescription("First Test Show")).Returns("Description for First Test Show");
            AddComponentToConfigurationManager(movieDb2.Object);

            Assert.AreEqual("Description for First Test Show", RunSingleConversation<MovieConversation>(input));

            movieDb1.Verify(s => s.GetMovieDescription("First Test Show"), Times.Exactly(1));
            movieDb2.Verify(s => s.GetMovieDescription("First Test Show"), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGetMovieRatingConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Rating"},
                {"Time", "TimeOfDay=today;"},
                {"Title", "First Test Show"},
                {"Theater", ""},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IMovieSensor> movieDb = new Mock<IMovieSensor>(MockBehavior.Strict);
            movieDb.Setup(s => s.IsValid).Returns(true);
            movieDb.Setup(s => s.GetMovieRating("First Test Show")).Returns("R");
            AddComponentToConfigurationManager(movieDb.Object);

            Assert.AreEqual("First Test Show is rated R.", RunSingleConversation<MovieConversation>(input));

            movieDb.Verify(s => s.GetMovieRating("First Test Show"), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGetMovieRatingNoRatingConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Rating"},
                {"Time", "TimeOfDay=today;"},
                {"Title", "First Test Show"},
                {"Theater", ""},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IMovieSensor> movieDb = new Mock<IMovieSensor>(MockBehavior.Strict);
            movieDb.Setup(s => s.IsValid).Returns(true);
            movieDb.Setup(s => s.GetMovieRating("First Test Show")).Returns("");
            AddComponentToConfigurationManager(movieDb.Object);

            Assert.AreEqual("I can't find the rating for First Test Show.", RunSingleConversation<MovieConversation>(input));

            movieDb.Verify(s => s.GetMovieRating("First Test Show"), Times.Exactly(1));
        }

        [TestMethod]
        public void TestGetMovieRatingsMultipleRatingsConversation()
        {
            Dictionary<String, String> input = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Rating"},
                {"Time", "TimeOfDay=today;"},
                {"Title", "First Test Show"},
                {"Theater", ""},
            };
            IConfigurationManager configMan = GetConfigurationManager();

            Mock<IMovieSensor> movieDb1 = new Mock<IMovieSensor>(MockBehavior.Strict);
            movieDb1.Setup(s => s.IsValid).Returns(true);
            movieDb1.Setup(s => s.GetMovieRating("First Test Show")).Returns("");
            AddComponentToConfigurationManager(movieDb1.Object);

            Mock<IMovieSensor> movieDb2 = new Mock<IMovieSensor>(MockBehavior.Strict);
            movieDb2.Setup(s => s.IsValid).Returns(true);
            movieDb2.Setup(s => s.GetMovieRating("First Test Show")).Returns("R");
            AddComponentToConfigurationManager(movieDb2.Object);

            Assert.AreEqual("First Test Show is rated R.", RunSingleConversation<MovieConversation>(input));

            movieDb1.Verify(s => s.GetMovieRating("First Test Show"), Times.Exactly(1));
            movieDb2.Verify(s => s.GetMovieRating("First Test Show"), Times.Exactly(1));
        }
    }
}
