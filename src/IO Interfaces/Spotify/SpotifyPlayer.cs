using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Audio;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Spotify
{
    [ParseableElement("Spotify", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class SpotifyPlayer : StreamingAudioIOInterface, IMusicController
    {
        private const int StartPlaying = 10;
        private SpotifyState _currentState;
        private int _numberOfIterations;
        private int _streamKey;
        private IAudioProvider _provider;
        private String _username;
        private String _password;

        internal ISpotifyWrapper Spotify
        {
            get;
            set;
        }

        public event EventHandler<MusicDeliveryEventArgs> MusicDataAvailable;

        public override string Name
        {
            get
            {
                return "Spotify";
            }
        }

        public override IEnumerable<PersistentProperty> Properties
        {
            get
            {
                foreach (PersistentProperty prop in base.Properties)
                {
                    yield return prop;
                }
                yield return new PersistentProperty("Username", () => _username, un => _username = un);
                yield return new PersistentProperty("Password", () => _password, pw => _password = pw);
            }
        }

        public PlayerState State
        {
            get
            {
                switch (_currentState)
                {
                    case SpotifyState.Initialized:
                    case SpotifyState.LoggedIn:
                    case SpotifyState.Disposed:
                    case SpotifyState.Invalid:
                    default:
                        return PlayerState.Stopped;
                    case SpotifyState.Playing:
                        return PlayerState.Playing;
                }
            }
        }

        public override bool IsValid
        {
            get
            {
                LogIn();
                return base.IsValid && (_currentState == SpotifyState.LoggedIn || _currentState == SpotifyState.Playing);
            }
        }

        public override void Initialize(IConfigurationManager manager)
        {
            base.Initialize(manager);
            if (Spotify == null)
            {
                Spotify = new SpotifySessionListenerWrapper(_username, _password);
            }
            Spotify.MusicDataAvailable += OnMusicDataAvailable;
            Spotify.CheckBuffer += OnCheckBuffer;
            MusicDataAvailable += SpotifyWrapper_MusicDataAvailable;
            _currentState = SpotifyState.Initialized;
        }

        private void OnMusicDataAvailable(Object sender, MusicDeliveryEventArgs args)
        {
            if (MusicDataAvailable != null)
            {
                MusicDataAvailable(sender, args);
            }
        }

        private void OnCheckBuffer(object sender, CheckBufferEventArgs e)
        {
            e.NumberOfSamples = 0;
            if (_provider != null)
            {
                e.NumberOfSamples = _provider.NumberOfSamplesInBuffer;
            }
        }

        public override void Dispose()
        {
            if (_currentState == SpotifyState.Playing)
            {
                StopMusic();
            }
            if (_currentState == SpotifyState.LoggedIn)
            {
                LogOut();
            }
            if (_currentState == SpotifyState.Initialized)
            {
                try
                {
                    Spotify.MusicDataAvailable -= OnMusicDataAvailable;
                    Spotify.CheckBuffer -= OnCheckBuffer;
                    Spotify.Dispose();
                    Spotify = null;
                }
                catch (Exception e)
                {
                    // TODO: We should probably figure out what is causing that exception, 
                    //       in case it is something we can fix.
                    Console.WriteLine(e.ToString());
                }
                _currentState = SpotifyState.Disposed;
            }
            base.Dispose();
        }

        public void LogIn()
        {
            if (_currentState == SpotifyState.Initialized)
            {
                Spotify.LogIn();
                _currentState = SpotifyState.LoggedIn;
            }
        }

        public void LogOut()
        {
            if (_currentState == SpotifyState.Playing)
            {
                StopMusic();
            }
            if (_currentState == SpotifyState.LoggedIn)
            {
                Spotify.LogOut();
                _currentState = SpotifyState.Initialized;
            }
        }

        public bool TryStartPlaylist(String playlistName, bool randomize)
        {
            _numberOfIterations = 0;
            if (_currentState == SpotifyState.Initialized)
            {
                LogIn();
            }
            bool returnVal = Spotify.TryStartPlaylist(playlistName, randomize);
            if (returnVal)
            {
                _currentState = SpotifyState.Playing;
            }
            return returnVal;
        }

        private void SpotifyWrapper_MusicDataAvailable(object sender, MusicDeliveryEventArgs e)
        {
            if (e.NumFrames > 0 && _currentState == SpotifyState.Playing)
            {
                if (_numberOfIterations == StartPlaying)
                {
                    StartStream(_streamKey);
                }
                else if (_provider == null)
                {
                    _streamKey = InitializeStream(e.SampleRate, e.NumberOfChannels, e.NumberOfBits, out _provider);
                }

                if (_numberOfIterations <= StartPlaying)
                {
                    _numberOfIterations++;
                }

                var size = e.NumFrames * e.NumberOfChannels * (e.NumberOfBits / 8);
                byte[] copiedFrames = new byte[size];
                Marshal.Copy(e.Frames, copiedFrames, 0, size);   //Copy Pointer Bytes to _copiedFrames
                _provider.AddData(copiedFrames, 0, size);    //adding bytes from _copiedFrames as samples
                e.NumFramesRead = e.NumFrames;
            }
        }

        public void StopMusic()
        {
            if (_currentState == SpotifyState.Playing)
            {
                _currentState = SpotifyState.LoggedIn;
                Spotify.StopMusic();
                StopStream(_streamKey);
                LogOut();
                _provider = null;
            }
        }

        public IEnumerable<string> GetPlaylists()
        {
            bool wasLoggedOut = false;
            if (_currentState == SpotifyState.Initialized)
            {
                LogIn();
                wasLoggedOut = true;
            }
            List<String> playlists = Spotify.GetPlaylists();
            if (wasLoggedOut)
            {
                LogOut();
            }
            return playlists;
        }

        public void DeleteCurrentTrack()
        {
            if (_currentState == SpotifyState.Playing)
            {
                Spotify.DeleteCurrentTrack();
            }
        }

        public void SkipToNextTrack()
        {
            if (_currentState == SpotifyState.Playing)
            {
                Spotify.SkipToNextTrack();
            }
        }

        public string GetCurrentSongName()
        {
            if (_currentState == SpotifyState.Playing)
            {
                return Spotify.GetCurrentSongName();
            }
            else
            {
                return null;
            }
        }

        public String GetCurrentArtistName()
        {
            if (_currentState == SpotifyState.Playing)
            {
                return Spotify.GetCurrentArtistName();
            }
            else
            {
                return null;
            }
        }
    }
}
