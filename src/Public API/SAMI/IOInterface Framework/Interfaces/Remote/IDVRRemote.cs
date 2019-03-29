namespace SAMI.IOInterfaces.Interfaces.Remote
{
    /// <summary>
    /// IO interface which communicates and controls a DVR, or something with similar functionality for live TV.
    /// </summary>
    public interface IDVRRemote : IIOInterface
    {
        /// <summary>
        /// Pauses the currently playing TV.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes live TV at the currently paused location.
        /// </summary>
        void Play();

        /// <summary>
        /// Moves quickly forwards in time, until pause is called, or we reach the point of showing live TV.
        /// </summary>
        void FastForward();

        /// <summary>
        /// Moves quickly backwards in time, until pause is called, or we reach the beginning of the recording.
        /// </summary>
        void Rewind();

        /// <summary>
        /// Starts to record the channel currently displayed, but continues showing live TV.
        /// This continues until the currently playing show is done.
        /// </summary>
        void Record();
    }
}
