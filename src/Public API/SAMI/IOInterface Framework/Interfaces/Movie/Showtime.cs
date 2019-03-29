using System;

namespace SAMI.IOInterfaces.Interfaces.Movies
{
    /// <summary>
    /// Represents a showtime of a movie at a theater.
    /// </summary>
    public class Showtime
    {
        /// <summary>
        /// The name of the movie that is being shown.
        /// </summary>
        public String MovieTitle
        { 
            get; 
            set; 
        }

        /// <summary>
        /// The name of the theater at which this movie is being shown.
        /// </summary>
        public String Theater 
        { 
            get;
            set;
        }
        
        /// <summary>
        /// The time at which this movie is being shown.
        /// </summary>
        public DateTime Time 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Full constructor for <see cref="Showtime"/>
        /// </summary>
        /// <param name="name">Value for <see cref="MovieTitle"/></param>
        /// <param name="theater">Value for <see cref="Theather"/></param>
        /// <param name="time">Value for <see cref="Time"/></param>
        public Showtime(String name, String theater, String time)
        {
            MovieTitle = name;
            Theater = theater;
            Time = DateTime.Parse(time);
        }

        /// <summary>
        /// Returns true if this object equals the value passed in.
        /// </summary>
        /// <param name="obj">Other object to compare to.</param>
        /// <returns>True if this == obj, false otherwise.</returns>
        public override bool Equals(Object obj)
        {
            // If parameter cannot be cast to ThreeDPoint return false:
            Showtime s = obj as Showtime;
            if (s == null)
            {
                return false;
            }

            return Equals(s);
        }

        /// <summary>
        /// Returns true if this object equals the value passed in.
        /// </summary>
        /// <param name="s">Other object to compare to.</param>
        /// <returns>True if this == obj, false otherwise.</returns>
        public bool Equals(Showtime s)
        {
            // Return true if all fields match:
            return MovieTitle.Equals(s.MovieTitle) &&
                Theater.Equals(s.Theater) &&
                Time.Equals(s.Time);
        }
    }
}
