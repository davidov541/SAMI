using System;
using System.Collections.Generic;

namespace SAMI.IOInterfaces.Spotify
{
    internal interface ISpotifyWrapper : IDisposable
    {
        event EventHandler<MusicDeliveryEventArgs> MusicDataAvailable;
        event EventHandler MusicRestarting;
        event EventHandler<CheckBufferEventArgs> CheckBuffer;

        void LogIn();
        void LogOut();
        bool TryStartPlaylist(String playlistName, bool random);
        void StopMusic();
        List<String> GetPlaylists();
        void DeleteCurrentTrack();
        void SkipToNextTrack();
        String GetCurrentSongName();
        String GetCurrentArtistName();
    }
}
