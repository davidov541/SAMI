using System;

namespace SAMI.IOInterfaces.Interfaces.Baseball
{
    /// <summary>
    /// The score for an individual team in a single game.
    /// </summary>
    public class BaseballTeamScore
    {
        /// <summary>
        /// The name of the team this score is for.
        /// </summary>
        public BaseballTeam Team
        {
            get;
            set;
        }

        /// <summary>
        /// The score this team has in the given game.
        /// </summary>
        public int Score
        {
            get;
            set;
        }
    }
}
