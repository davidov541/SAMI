using System;
using System.Collections.Generic;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Interfaces.Football
{
    /// <summary>
    /// An IO interface which returns the scores for all teams in a given american football league.
    /// </summary>
    public interface IFootballSensor : IIOInterface
    {
        /// <summary>
        /// Name of the league that this interface represents.
        /// </summary>
        String League
        {
            get;
        }

        /// <summary>
        /// A list of all of the teams which are in this league.
        /// </summary>
        IEnumerable<FootballTeam> Teams
        {
            get;
        }

        /// <summary>
        /// All of the scores currently available for this week in the league.
        /// </summary>
        /// <returns>An enumerable of all of the games going on this week in the league.</returns>
        IEnumerable<FootballGame> LoadAllScores();

        /// <summary>
        /// Returns the game which the given team is in for this week. If no game is available for the team, it should return null.
        /// </summary>
        /// <param name="teamName">Key for the team which is being queried for.</param>
        /// <returns>A football game instance if one exists for that team this week, or null otherwise.</returns>
        FootballGame LoadLatestScoreForTeam(String teamName);
    }
}
