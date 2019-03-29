using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net;
using Codeplex.Data;
using SAMI.Apps;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Movies;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Movie
{
    [ParseableElement("TMSMovies", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class MovieSensor : IMovieSensor
    {
        private List<Movie> _movieList;
        private List<Showtime> _showtimeList;
        private String _apiKey;
        private String _searchRadius = "5";
        private IConfigurationManager _configManager;

        public String Name
        {
            get
            {
                return "TMS Database";
            }
        }

        /// <inheritdoc />
        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        /// <inheritdoc />
        public IEnumerable<PersistentProperty> Properties
        {
            get
            {
                yield return new PersistentProperty("APIKey", () => _apiKey, key => _apiKey = key);
                yield return new PersistentProperty("SearchRadius", () => _searchRadius, searchRadius => _searchRadius = searchRadius);
            }
        }

        /// <inheritdoc />
        public IEnumerable<IParseable> Children
        {
            get
            {
                yield break;
            }
        }

        public MovieSensor()
        {
            _movieList = new List<Movie>();
            _showtimeList = new List<Showtime>();
        }

        /// <inheritdoc />
        public void Initialize(IConfigurationManager configManager)
        {
            _configManager = configManager;
            UpdateMovieList(new SamiDateTime(DateTime.Today, DateTimeRange.SpecificTime));
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public void AddChild(IParseable component)
        {
        }

        internal void UpdateMovieList(SamiDateTime date)
        {
            try
            {
                String url = "http://data.tmsapi.com/v1/movies/showings?" +
                    "startDate=" + date.Time.ToString("yyyy-MM-dd") +
                    "&zip=" + _configManager.LocalLocation.ZipCode +
                    "&radius=" + _searchRadius +
                    "&api_key=" + _apiKey;
                String jsonResults = new WebClient().DownloadString(url);
                jsonResults = "{\"movies\":" + jsonResults + "}";

                var movieJson = DynamicJson.Parse(jsonResults);

                foreach (var movie in movieJson.movies)
                {
                    String movieTitle, rating, description;
                    movieTitle = movie.title;
                    // Define rating and description as empty strings in case they are not defined.
                    rating = "";
                    description = "";
                    if (movie.IsDefined("ratings"))
                    {
                        if (movie.ratings[0].IsDefined("code"))
                        {
                            rating = movie.ratings[0].code;
                        }
                    }
                    if (movie.IsDefined("shortDescription"))
                    {
                        description = movie.shortDescription;
                    }

                    Movie m = new Movie(movieTitle, description, rating);
                    if (!this._movieList.Contains(m))
                    {
                        this._movieList.Add(m);
                    }
                    foreach (dynamic showtime in movie.showtimes)
                    {
                        String theaterName = showtime.theatre.name;
                        String movieTime = showtime.dateTime;
                        Showtime movieShowtime = new Showtime(movieTitle, theaterName, movieTime);
                        if (!this._showtimeList.Contains(movieShowtime))
                        {
                            this._showtimeList.Add(movieShowtime);
                        }
                    }
                }

                // Clear out old entries
                List<Showtime> newList = new List<Showtime>();
                foreach (Showtime showtime in this._showtimeList)
                {
                    if (showtime.Time >= DateTime.Now)
                    {
                        newList.Add(showtime);
                    }
                }
                this._showtimeList = newList;
            }
            catch (Exception)
            {
            }
        }

        /// <inheritdoc />
        public IEnumerable<Showtime> GetMovieShowtimes(SamiDateTime timeFilter, String titleFilter = null, String theaterFilter = null)
        {
            UpdateMovieList(timeFilter);
            foreach (Showtime showtime in this._showtimeList)
            {
                if (ShowtimePassesFilter(showtime, titleFilter, theaterFilter, timeFilter))
                {
                    yield return showtime;
                }

            }
        }

        /// <inheritdoc />
        public string GetMovieDescription(string movieName)
        {
            foreach (Movie movie in this._movieList)
            {
                if (movie.title.Equals(movieName))
                {
                    return movie.description;
                }
            }
            return movieName + " is not in the database.";
        }

        /// <inheritdoc />
        public string GetMovieRating(string movieName)
        {
            foreach (Movie movie in this._movieList)
            {
                if (movie.title.Equals(movieName))
                {
                    return movie.rating;
                }
            }
            return "";
        }

        private static Boolean MovieIsPlayingAt(dynamic movie, String theaterName)
        {
            foreach (dynamic showtime in movie.showtimes)
            {
                if (showtime.theatre.name.Equals(theaterName))
                {
                    return true;
                }
            }
            return false;
        }

        private static Boolean ShowtimePassesFilter(Showtime showtime, String titleFilter, String theaterFilter, SamiDateTime timeFilter)
        {
            // See if the showtime is in a certain time 
            if (!timeFilter.TimeIsInRange(showtime.Time))
            {
                // The time is not in the range
                return false;
            }

            if (!String.IsNullOrEmpty(titleFilter))
            {
                if (!showtime.MovieTitle.Equals(titleFilter))
                {
                    return false;
                }
            }

            if (!String.IsNullOrEmpty(theaterFilter))
            {
                if (!showtime.Theater.Equals(theaterFilter))
                {
                    return false;
                }
            }

            // We've hit this point, so we passed the filter
            return true;
        }
    }
}
