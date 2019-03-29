using System;
using System.Collections.Generic;
using SAMI.Apps;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Interfaces.Movies
{
    /// <summary>
    /// An interface for components which provide information about movie times at nearby theaters.
    /// </summary>
    public interface IMovieSensor : IIOInterface
    {
        /// <summary>
        /// Gets a list of all showtimes that match the given filters.
        /// </summary>
        /// <param name="timeFilter">Indicates what time frame should be searched for movies.</param>
        /// <param name="titleFilter">Indicates what titles should be searched for. If null or String.Empty, all films will be searched.</param>
        /// <param name="theaterFilter">Indicates what theaters should be searched. If null or String.Empty, all theaters will be searched.</param>
        /// <returns>List of showtimes that match the filters</returns>
        IEnumerable<Showtime> GetMovieShowtimes(SamiDateTime timeFilter, String titleFilter = null, String theaterFilter = null);

        /// <summary>
        /// Returns a string that describes the given film.
        /// </summary>
        /// <param name="movieName">Name of the film to be described.</param>
        /// <returns>Description of the film.</returns>
        String GetMovieDescription(string movieName);

        /// <summary>
        /// Returns the MPAA rating (e.g. PG, R, etc.) of the film.
        /// </summary>
        /// <param name="movieName">Name of the film to search for.</param>
        /// <returns>The MPAA rating of the film.</returns>
        String GetMovieRating(string movieName);
    }
}
