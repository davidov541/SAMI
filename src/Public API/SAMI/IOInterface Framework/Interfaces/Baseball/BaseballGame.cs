using System;
using System.Collections.Generic;
using SAMI.IOInterfaces.Interfaces.Sports;

namespace SAMI.IOInterfaces.Interfaces.Baseball
{
    /// <summary>
    /// Class representing a game of baseball.
    /// </summary>
    public class BaseballGame
    {
        /// <summary>
        /// The information about the score for the winning team.
        /// </summary>
        public BaseballTeamScore WinningTeamScore
        {
            get;
            set;
        }

        /// <summary>
        /// The information about the score for the losing team.
        /// </summary>
        public BaseballTeamScore LosingTeamScore
        {
            get;
            set;
        }

        /// <summary>
        /// The information about the score for the home team.
        /// </summary>
        public BaseballTeamScore HomeTeamScore
        {
            get;
            set;
        }

        /// <summary>
        /// The information about the score for the away team.
        /// </summary>
        public BaseballTeamScore AwayTeamScore
        {
            get;
            set;
        }

        /// <summary>
        /// A list of keys representing the channels on which this game is being shown.
        /// </summary>
        public IEnumerable<String> Channels
        {
            get;
            set;
        }

        /// <summary>
        /// The current state of the game.
        /// </summary>
        public GameState State
        {
            get;
            set;
        }

        /// <summary>
        /// How many outs have been recorded in this half of the inning.
        /// If the game is not currently underway, the value of this is undefined.
        /// </summary>
        public int NumberOfOuts
        {
            get;
            set;
        }

        /// <summary>
        /// If true, the game is currently in the top of the inning. Otherwise, the game is in the bottom of the inning.
        /// If the game is not currently underway, the value of this is undefined.
        /// </summary>
        public bool TopOfInning
        {
            get;
            set;
        }

        /// <summary>
        /// The number inning that is currently happening.
        /// If the game is not currently underway, the value of this is undefined.
        /// </summary>
        public int InningNumber
        {
            get;
            set;
        }

        /// <summary>
        /// The number of players on base currently.
        /// If the game is not currently underway, the value of this is undefined.
        /// </summary>
        public int NumberOnBase
        {
            get;
            set;
        }

        /// <summary>
        /// The time at which this game starts or has started.
        /// </summary>
        public DateTime StartingTime
        {
            get;
            set;
        }
    }
}
