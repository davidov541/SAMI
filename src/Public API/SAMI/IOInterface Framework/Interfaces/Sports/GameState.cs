namespace SAMI.IOInterfaces.Interfaces.Sports
{
    /// <summary>
    /// Enum containing states that a sports game can be in.
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// No game was found for this day.
        /// </summary>
        NoGame,
        /// <summary>
        /// The game hasn't yet started.
        /// </summary>
        GameHasntStarted,
        /// <summary>
        /// The game is currently in a rain delay.
        /// </summary>
        RainDelay,
        /// <summary>
        /// The game has been postponed to a future date.
        /// </summary>
        Postponed,
        /// <summary>
        /// The game has started, but not completed.
        /// </summary>
        Started,
        /// <summary>
        /// The game has been finished.
        /// </summary>
        Completed
    }
}
