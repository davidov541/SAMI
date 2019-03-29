using System;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Interfaces.Audio
{
    /// <summary>
    /// IOInterface which plays audio from a file.
    /// The file may be local or remote, and may be any common file format.
    /// </summary>
    public interface IMediaFilePlayer : IIOInterface
    {
        /// <summary>
        /// Indicates the current state of the player.
        /// </summary>
        PlayerState CurrentState
        {
            get;
        }

        /// <summary>
        /// Event which is triggered once the file that was last played is finished.
        /// </summary>
        event EventHandler<EventArgs> TrackFinished;

        /// <summary>
        /// Starts playing the audio in the file specified.
        /// </summary>
        /// <param name="fileName">Audio file to play from</param>
        void PlayAudioFile(String fileName);

        /// <summary>
        /// Pauses the currently playing track, but does not lose the current position.
        /// </summary>
        void PauseAudioFile();

        /// <summary>
        /// Resumes the paused track at the last played position.
        /// </summary>
        void ResumeAudioFile();

        /// <summary>
        /// Stops the current track.
        /// </summary>
        void StopAudioFile();
    }
}
