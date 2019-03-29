using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SAMI.Sensors.Movie
{
    public class MovieList
    {
        public Movie[] movies { get; set; }

        public String[] getMovieNames(String theaterName)
        {
            String[] nameList = new String[this.movies.Length];
            for (int i = 0; i < this.movies.Length; i++)
            {
                if (theaterName.Equals("Any") || movies[i].isPlayingAt(theaterName))
                {
                    nameList[i] = movies[i].getName();
                }
            }
            return nameList;
        }

        public Showtime[] getMovieShowtimes(String movieName, String theater)
        {
            List<Showtime> showtimes = new List<Showtime>();
            foreach (Movie movie in movies)
            {
                if (movie.getName().Equals(movieName))
                {
                    foreach (Showtime showtime in movie.showtimes)
                    {
                        if (theater.Equals("Any") || showtime.theatre.name.Equals(theater))
                        {
                            showtimes.Add(showtime);
                        }
                    }
                }
            }
            return showtimes.ToArray();
        }


        internal string getMovieDescription(string movieName)
        {
            foreach (Movie movie in movies)
            {
                if (movie.title.Equals(movieName))
                {
                    return movie.shortDescription;
                }
            }
            return movieName + " is not in the database.";
        }

        internal string getMovieRating(string movieName)
        {
            foreach (Movie movie in movies)
            {
                if (movie.title.Equals(movieName))
                {
                    return movie.ratings[0].code;
                }
            }
            return "";
        }
    }
}
