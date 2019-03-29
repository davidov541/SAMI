using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Movies;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Movie
{
    internal class MovieConversation : Conversation
    {
        protected override string CommandName
        {
            get
            {
                return "Movie";
            }
        }

        public MovieConversation(IConfigurationManager configManager)
            : base(configManager)
        {
        }

        public override string Speak()
        {
            String returnString = String.Empty;
            Dialog phrase = CurrentDialog;
            String title = phrase.GetPropertyValue("Title");
            String theater = phrase.GetPropertyValue("Theater");
            SamiDateTime samiTime = ParseTime(phrase.GetPropertyValue("Time"));

            base.Speak();
            ConversationIsOver = true;
            IEnumerable<IMovieSensor> movieDatabases = ConfigManager.FindAllComponentsOfType<IMovieSensor>();
            switch (phrase.GetPropertyValue("SubCommand"))
            {
                case "Names":
                    // Return a list of movie titles
                    List<String> movieTitles = movieDatabases.SelectMany(s => s.GetMovieShowtimes(samiTime, theaterFilter: theater).Select(showtime => showtime.MovieTitle)).Distinct().ToList();
                    if (movieTitles.Count > 0)
                    {
                        returnString = "On " + samiTime.Time.ToString("MMMM d", CultureInfo.InvariantCulture);
                        returnString += ", these movies will be playing. ";

                        returnString += SayList(movieTitles);
                        return returnString;
                    }
                    else
                    {
                        return "I'm sorry, no movies will be shown then.";
                    }

                case "Showtimes":
                    // Return a list of showtimes
                    IEnumerable<Showtime> showtimes = movieDatabases.SelectMany(s => s.GetMovieShowtimes(samiTime, title, theater)).ToList();

                    if (showtimes.Any())
                    {
                        String lastTheater = "";
                        returnString = "On " + samiTime.Time.ToString("MMMM d", CultureInfo.InvariantCulture) + ", ";
                        foreach (Showtime showtime in showtimes)
                        {
                            if (!showtime.Theater.Equals(lastTheater))
                            {
                                returnString += showtime.MovieTitle + " will be shown at " + showtime.Theater + " at ";
                            }

                            returnString += showtime.Time.ToString("hh:mm") + " . ";

                            lastTheater = showtime.Theater;
                        }
                        return returnString;
                    }
                    else
                    {
                        return "I'm sorry, " + title + " will not be shown then.";
                    }

                case "Description":
                    IEnumerable<String> descriptions = movieDatabases.Select(s => s.GetMovieDescription(title)).Where(d => !String.IsNullOrEmpty(d)).ToList();
                    if (descriptions.Any())
                    {
                        return descriptions.FirstOrDefault();
                    }
                    else
                    {
                        return String.Format("I can't find the description for {0}.", title);
                    }
                case "Rating":
                    IEnumerable<String> ratings = movieDatabases.Select(s => s.GetMovieRating(title)).Where(d => !String.IsNullOrEmpty(d)).ToList();
                    if (ratings.Any())
                    {
                        return String.Format("{0} is rated {1}.", title, ratings.FirstOrDefault());
                    }
                    else
                    {
                        return String.Format("I can't find the rating for {0}.", title);
                    }
                default:
                    return "The movie sub command " + phrase.GetPropertyValue("Subcommand") + " is not recognized.";
            }
        }
    }
}
