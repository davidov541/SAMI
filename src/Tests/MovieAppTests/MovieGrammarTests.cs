using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SAMI.Apps;
using SAMI.Apps.Movie;
using SAMI.IOInterfaces.Interfaces.Movies;
using SAMI.Test.Utilities;

namespace MovieAppTests
{
    [TestClass]
    public class MovieGrammarTests : BaseGrammarTests
    {
        private static IEnumerable<Showtime> _fakeShowtimes = new List<Showtime>
            {
                new Showtime("First Test Show", "First Test Theater", "9:45 PM"),
                new Showtime("Second Test Show", "First Test Theater", "8:45 PM"),
                new Showtime("Third Test Show", "Second Test Theater", "10:45 PM"),
            };

        [TestMethod]
        public void TestWhatMoviesArePlayingGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Names"},
                {"Time", "TimeOfDay=today;"},
                {"Theater", ""},
            };
            Mock<IMovieSensor> db = new Mock<IMovieSensor>(MockBehavior.Strict);
            db.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(db.Object);

            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What movies are playing", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What movies will be playing", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What movies are showing", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What movies will be showing", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
        }

        [TestMethod]
        public void TestWhatMoviesArePlayingAtTheaterGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Names"},
                {"Time", "TimeOfDay=today;"},
                {"Theater", "First Test Theater"},
            };
            Mock<IMovieSensor> db = new Mock<IMovieSensor>(MockBehavior.Strict);
            db.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(db.Object);

            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What movies are playing at First Test Theater", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What movies will be playing at First Test Theater", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What movies are showing at First Test Theater", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What movies will be showing at First Test Theater", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
        }

        [TestMethod]
        public void TestWhatMoviesArePlayingAtTheaterTomorrowGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Names"},
                {"Time", "DayOfWeek=tomorrow;"},
                {"Theater", "First Test Theater"},
            };
            Mock<IMovieSensor> db = new Mock<IMovieSensor>(MockBehavior.Strict);
            db.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(db.Object);

            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What movies are playing at First Test Theater tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What movies will be playing at First Test Theater tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What movies are showing at First Test Theater tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What movies will be showing at First Test Theater tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
        }

        [TestMethod]
        public void TestWhatMoviesArePlayingTomorrowGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Names"},
                {"Time", "DayOfWeek=tomorrow;"},
                {"Theater", ""},
            };
            Mock<IMovieSensor> db = new Mock<IMovieSensor>(MockBehavior.Strict);
            db.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(db.Object);

            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What movies are playing tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What movies will be playing tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What movies are showing tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What movies will be showing tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
        }

        [TestMethod]
        public void TestWhenIsMoviePlayingGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Showtimes"},
                {"Time", "TimeOfDay=today;"},
                {"Theater", ""},
                {"Title", "First Test Show"},
            };
            Mock<IMovieSensor> db = new Mock<IMovieSensor>(MockBehavior.Strict);
            db.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(db.Object);

            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What time is First Test Show playing", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When is First Test Show playing", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When will First Test Show playing", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();

            
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What time is First Test Show be playing", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When is First Test Show be playing", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When will First Test Show be playing", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();

            
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What time is First Test Show showing", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When is First Test Show showing", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When will First Test Show showing", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();

            
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What time is First Test Show be showing", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When is First Test Show be showing", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When will First Test Show be showing", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
        }

        [TestMethod]
        public void TestWhenIsMoviePlayingAtTheaterGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Showtimes"},
                {"Time", "TimeOfDay=today;"},
                {"Theater", "First Test Theater"},
                {"Title", "First Test Show"},
            };
            Mock<IMovieSensor> db = new Mock<IMovieSensor>(MockBehavior.Strict);
            db.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(db.Object);

            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What time is First Test Show playing at First Test Theater", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When is First Test Show playing at First Test Theater", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When will First Test Show playing at First Test Theater", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();

            
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What time is First Test Show be playing at First Test Theater", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When is First Test Show be playing at First Test Theater", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When will First Test Show be playing at First Test Theater", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();

            
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What time is First Test Show showing at First Test Theater", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When is First Test Show showing at First Test Theater", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When will First Test Show showing at First Test Theater", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();

            
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What time is First Test Show be showing at First Test Theater", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When is First Test Show be showing at First Test Theater", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When will First Test Show be showing at First Test Theater", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
        }

        [TestMethod]
        public void TestWhenIsMoviePlayingTomorrowGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Showtimes"},
                {"Time", "DayOfWeek=tomorrow;"},
                {"Theater", ""},
                {"Title", "First Test Show"},
            };
            Mock<IMovieSensor> db = new Mock<IMovieSensor>(MockBehavior.Strict);
            db.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(db.Object);

            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What time is First Test Show playing tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When is First Test Show playing tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When will First Test Show playing tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();

            
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What time is First Test Show be playing tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When is First Test Show be playing tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When will First Test Show be playing tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();

            
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What time is First Test Show showing tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When is First Test Show showing tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When will First Test Show showing tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();

            
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What time is First Test Show be showing tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When is First Test Show be showing tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When will First Test Show be showing tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
        }

        [TestMethod]
        public void TestWhenIsMoviePlayingAtTheaterTomorrowGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Showtimes"},
                {"Time", "DayOfWeek=tomorrow;"},
                {"Theater", "First Test Theater"},
                {"Title", "First Test Show"},
            };
            Mock<IMovieSensor> db = new Mock<IMovieSensor>(MockBehavior.Strict);
            db.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(db.Object);

            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What time is First Test Show playing at First Test Theater tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When is First Test Show playing at First Test Theater tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When will First Test Show playing at First Test Theater tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();

            
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What time is First Test Show be playing at First Test Theater tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When is First Test Show be playing at First Test Theater tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When will First Test Show be playing at First Test Theater tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();

            
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What time is First Test Show showing at First Test Theater tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When is First Test Show showing at First Test Theater tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When will First Test Show showing at First Test Theater tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();

            
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What time is First Test Show be showing at First Test Theater tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When is First Test Show be showing at First Test Theater tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
            db.ResetCalls();
            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("When will First Test Show be showing at First Test Theater tomorrow", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
        }

        [TestMethod]
        public void TestWhatIsMovieAboutGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Description"},
                {"Time", "TimeOfDay=today;"},
                {"Title", "First Test Show"},
                {"Theater", ""},
            };
            Mock<IMovieSensor> db = new Mock<IMovieSensor>(MockBehavior.Strict);
            db.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(db.Object);

            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What is First Test Show about", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
        }

        [TestMethod]
        public void TestWhatIsMovieRatedGrammar()
        {
            Dictionary<String, String> expectedParams = new Dictionary<string, string>
            {
                {"Command", "Movie"},
                {"SubCommand", "Rating"},
                {"Time", "TimeOfDay=today;"},
                {"Title", "First Test Show"},
                {"Theater", ""},
            };
            Mock<IMovieSensor> db = new Mock<IMovieSensor>(MockBehavior.Strict);
            db.Setup(s => s.IsValid).Returns(true);
            AddComponentToConfigurationManager(db.Object);

            db.Setup(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null)).Returns(_fakeShowtimes);
            TestGrammar<MovieApp>("What is First Test Show Rated", expectedParams);
            db.Verify(s => s.GetMovieShowtimes(new SamiDateTime(DateTimeRange.AnyTime), null, null), Times.Exactly(2));
        }
    }
}
