
using System;
using SAMI.IOInterfaces.Interfaces.Sports;

namespace SAMI.IOInterfaces.Interfaces.Football
{
    /// <summary>
    /// Represents a single game of American football at any level.
    /// </summary>
    public class FootballGame
    {
        /// <summary>
        /// The current state of this game.
        /// </summary>
        public GameState State
        {
            get;
            set;
        }

        /// <summary>
        /// The score for the home team.
        /// </summary>
        public FootballTeamScore HomeTeamScore
        {
            get;
            set;
        }

        /// <summary>
        /// The score for the away team.
        /// </summary>
        public FootballTeamScore AwayTeamScore
        {
            get;
            set;
        }

        /// <summary>
        /// The score for the winning team.
        /// </summary>
        public FootballTeamScore WinningTeamScore
        {
            get
            {
                if (HomeTeamScore == null || AwayTeamScore == null)
                {
                    return null;
                }
                else if (HomeTeamScore.Score > AwayTeamScore.Score)
                {
                    return HomeTeamScore;
                }
                else
                {
                    return AwayTeamScore;
                }
            }
        }

        /// <summary>
        /// The score for the losing team.
        /// </summary>
        public FootballTeamScore LosingTeamScore
        {
            get
            {
                if (HomeTeamScore == null || AwayTeamScore == null)
                {
                    return null;
                }
                else if (HomeTeamScore.Score <= AwayTeamScore.Score)
                {
                    return HomeTeamScore;
                }
                else
                {
                    return AwayTeamScore;
                }
            }
        }

        /// <summary>
        /// The quarter the game is currently in, if the game is currently going on.
        /// If not, the value for this property is undefined.
        /// </summary>
        public int Quarter
        {
            get;
            set;
        }

        /// <summary>
        /// How much time is left in the quarter, if the game is currently going on.
        /// If not, the value for this property is undefined.
        /// </summary>
        public TimeSpan TimeLeft
        {
            get;
            set;
        }

        /// <summary>
        /// If the game has not started yet, this is the absolute time when the game will start.
        /// Otherwise, this is the time when the game did start.
        /// </summary>
        public DateTime StartingTime
        {
            get;
            set;
        }
    }
}
