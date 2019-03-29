using System;
using System.Collections.Generic;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Interfaces.Baseball
{
    /// <summary>
    /// Interface for a component which supplies scores from any baseball games.
    /// </summary>
    public interface IBaseballSensor : IIOInterface
    {
        /// <summary>
        /// A list of all of the teams which this sensor knows about.
        /// </summary>
        IEnumerable<BaseballTeam> Teams
        {
            get;
        }

        /// <summary>
        /// The name of the league which this sensor gets information for.
        /// </summary>
        String League
        {
            get;
        }

        /// <summary>
        /// Returns a list of all scores for the given date.
        /// </summary>
        /// <param name="date">Date to query for.</param>
        /// <returns>List of all baseball games on the given day.</returns>
        IEnumerable<BaseballGame> LoadAllScores(DateTime date);

        /// <summary>
        /// Returns a list of all scores for the given date involving the given team.
        /// </summary>
        /// <param name="date">Date to query for.</param>
        /// <param name="teamName">Name of the team to query for.</param>
        /// <returns>List of all baseball games on the given day involving the team.</returns>
        IEnumerable<BaseballGame> LoadScoresForTeam(DateTime date, BaseballTeam teamName);

        /// <summary>
        /// Returns the current standings in the league.
        /// </summary>
        /// <returns>The current standings in the league.</returns>
        BaseballStandings LoadStandings();
    }
}
