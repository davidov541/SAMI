using System;

namespace SAMI.IOInterfaces.Interfaces.Baseball
{
    /// <summary>
    /// The standings for an individual team in an individual race.
    /// </summary>
    public class BaseballTeamStanding
    {
        /// <summary>
        /// The name of the team this standings represents.
        /// </summary>
        public BaseballTeam Team
        {
            get;
            set;
        }

        /// <summary>
        /// The number of games back the team is of the leader for this race.
        /// </summary>
        public double GamesBack
        {
            get;
            set;
        }
    }
}
