using System;
using System.Collections.Generic;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Interfaces.Audio
{
    /// <summary>
    /// IOInterface which provides music to be played.
    /// </summary>
    public interface IMusicController : IDisposable, IIOInterface
    {
        /// <summary>
        /// Indicates the current state of playing (e.g. whether it is playing, paused, stopped, etc).
        /// </summary>
        PlayerState State
        {
            get;
        }

        /// <summary>
        /// Logs into any services needed to provide the audio.
        /// </summary>
        void LogIn();

        /// <summary>
        /// Logs out of any services needed to provide the audio.
        /// </summary>
        void LogOut();

        /// <summary>
        /// Tries to start a playlist using a given IAudioStreamManager to output the audio to.
        /// </summary>
        /// <param name="manager">IAudioStreamManager that the audio should be output to.</param>
        /// <param name="playlistName">Name of the playlist to start playing.</param>
        /// <param name="randomize">If true, the playlist will be randomized. Otherwise, the playlist will be played in the order indicated.</param>
        /// <returns>True if successful, false otherwise.</returns>
        bool TryStartPlaylist(String playlistName, bool randomize);

        /// <summary>
        /// Stops outputting new audio data.
        /// </summary>
        void StopMusic();

        /// <summary>
        /// Returns a list of the valid playlists that can be played.
        /// </summary>
        /// <returns>A list of the names of all valid playlists.</returns>
        IEnumerable<String> GetPlaylists();

        /// <summary>
        /// Deletes the current track from the currently playing playlist.
        /// </summary>
        void DeleteCurrentTrack();

        /// <summary>
        /// Skips to the next track in the playlist.
        /// </summary>
        void SkipToNextTrack();

        /// <summary>
        /// Returns the name of the song that is currently playing.
        /// </summary>
        /// <returns>Name of the currently playing song.</returns>
        String GetCurrentSongName();

        /// <summary>
        /// Returns the name of the artist that performs the song that is currently playing.
        /// </summary>
        /// <returns>Name of the artist that performs the currently playing song.</returns>
        String GetCurrentArtistName();
    }
}
