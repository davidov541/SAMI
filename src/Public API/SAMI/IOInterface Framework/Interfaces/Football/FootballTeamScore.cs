using System;

namespace SAMI.IOInterfaces.Interfaces.Football
{
    /// <summary>
    /// The current score for a single team in an American football game.
    /// </summary>
    public class FootballTeamScore
    {
        /// <summary>
        /// The team whose score this represents.
        /// </summary>
        public FootballTeam Team
        {
            get;
            set;
        }

        /// <summary>
        /// The score that <see cref="Team"/> has achived in the game.
        /// </summary>
        public int Score
        {
            get;
            set;
        }
    }
}
