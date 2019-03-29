namespace SAMI.IOInterfaces.Interfaces.Audio
{
    /// <summary>
    /// Enum defining the possible states of a media player.
    /// </summary>
    public enum PlayerState
    {
        /// <summary>
        /// No media is playing, and starting it will start from the beginning.
        /// </summary>
        Stopped,
        /// <summary>
        /// No media is playing, but starting it will start from the last place.
        /// </summary>
        Paused,
        /// <summary>
        /// Media is currently playing.
        /// </summary>
        Playing
    }
}
