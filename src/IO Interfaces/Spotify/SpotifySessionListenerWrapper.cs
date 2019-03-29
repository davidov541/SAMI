using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using SpotifySharp;

namespace SAMI.IOInterfaces.Spotify
{
    internal class SpotifySessionListenerWrapper : SpotifySessionListener, ISpotifyWrapper
    {
        private SpotifySession _session;

        private EventWaitHandle _notifyHandle;
        private EventWaitHandle _exitHandle;
        private EventWaitHandle _loggedInHandle;

        private String _username;
        private String _password;

        private Playlist _currentlyPlayingPlaylist;
        private int _currentTrackNum = -1;
        private List<int> _playingOrder;

        public bool _isValid = true;
        private bool _isPlaying = false;

        private Thread _backgroundThread;

        private const String FavoriteName = "favorites";
        private String _apiKey = "01,2C,09,B7,24,90,7E,8D,81,1E,9C,8F,18,8B,95,48,A9,01,D9,A3,B6,71,30,4D,1F,39,4F,9C,32,98,39,03,3B,1F,83,B6,9D,0C,AF,A9,2B,AC,DA,28,50,0E,30,F3,C1,AD,C4,F5,6D,AC,81,D1,46,26,7F,6A,B9,98,44,E0,8E,F9,41,A9,FC,B9,43,05,B2,24,2C,99,69,FF,16,E2,D8,F0,6A,D2,B5,58,0B,C5,2E,D6,6B,F6,74,EA,92,D6,D0,ED,C5,E2,E1,99,9F,FE,AF,C2,B3,AB,10,E6,D5,68,C3,38,85,EC,5F,A2,44,5F,B4,1B,D6,9B,C2,25,9F,66,2C,0C,8D,9F,8D,22,5C,D3,A5,AE,35,D5,2A,5A,9F,EF,C4,EC,98,BF,F7,E2,A6,D1,6C,AD,59,E1,5A,C8,9C,25,19,EE,28,2E,91,C4,3A,68,70,42,60,50,9A,5C,8F,68,C5,AF,E2,CA,4A,BE,77,93,45,BD,77,FF,6A,42,A2,26,BA,BF,1A,61,50,AB,F1,E2,AD,E4,AE,FD,C9,F3,47,B1,A2,A9,41,33,A6,4A,16,3A,66,0D,35,F4,3F,A4,AA,19,20,A5,C5,41,A7,E1,D2,45,F7,BD,98,C0,FF,4D,44,95,6D,41,10,DA,99,2E,32,F6,92,1A,93,83,9B,90,B3,48,6C,EB,54,AA,E7,63,B9,90,ED,60,FD,36,E5,2E,AD,75,63,F2,1A,CB,86,89,58,F0,71,90,27,7E,5C,3F,7D,05,E9,E7,70,AD,9B,61,B2,41,9C,A0,F7,99,63,FF,B1,02,90,60,01,D2,84,CF,C9,58,CE,43,D3,5D,06,F3,08,FD,D6";

        public event EventHandler<MusicDeliveryEventArgs> MusicDataAvailable;
        public event EventHandler MusicRestarting;
        public event EventHandler<CheckBufferEventArgs> CheckBuffer;

        private byte[] APIKey
        {
            get
            {
                List<byte> bytes = new List<byte>();
                if (_apiKey != null)
                {
                    foreach (String b in _apiKey.Split(','))
                    {
                        bytes.Add(Byte.Parse(b, NumberStyles.HexNumber));
                    }
                }
                return bytes.ToArray();
            }
            set
            {
                _apiKey = String.Empty;
                foreach (byte b in value)
                {
                    _apiKey += b.ToString("x") + ",";
                }
                _apiKey = _apiKey.TrimEnd(',');
            }
        }

        public SpotifySessionListenerWrapper(String username, String password)
        {
            _username = username;
            _password = password;
            _backgroundThread = new Thread(RunBackgroundThread);
            _backgroundThread.IsBackground = true;
            _notifyHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            _exitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            _loggedInHandle = new EventWaitHandle(false, EventResetMode.ManualReset);

            SpotifySessionConfig config = new SpotifySessionConfig();
            config.ApiVersion = 12;
            config.CacheLocation = "tmp";
            config.SettingsLocation = "tmp";
            config.ApplicationKey = APIKey;
            config.UserAgent = "SAMI";
            config.Listener = this;

            _session = SpotifySession.Create(config);

            _backgroundThread.Start();
        }

        public void Dispose()
        {
            if (_isPlaying)
            {
                StopMusicUnlocked();
            }
            _exitHandle.Set();
            _backgroundThread.Join();
            _isValid = false;
        }

        private void RunBackgroundThread()
        {
            try
            {
                while (!_exitHandle.WaitOne(0))
                {
                    if (_notifyHandle.WaitOne(500))
                    {
                        int next_timeout = 0;
                        do
                        {
                            lock (_session)
                            {
                                _session.ProcessEvents(ref next_timeout);
                            }
                        } while (next_timeout == 0);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        #region ISpotifyWrapper Members
        public void LogIn()
        {
            lock (_session)
            {
                try
                {
                    _session.Login(_username, _password, true, null);
                }
                catch (SpotifyException e)
                {
                    Console.Error.WriteLine("Failed to log in session: {0}", e.Message);
                    throw;
                }
            }
        }

        public void LogOut()
        {
            lock (_session)
            {
                _session.Logout();
            }
        }

        public bool TryStartPlaylist(String playlistName, bool random)
        {
            if (_isValid)
            {
                _loggedInHandle.WaitOne();
                lock (_session)
                {
                    return TryStartPlaylistUnlocked(playlistName, random);
                }
            }
            return false;
        }

        private bool TryStartPlaylistUnlocked(String playlistName, bool random)
        {
            PlaylistContainer pc = GetPlaylistContainer();
            _currentlyPlayingPlaylist = null;
            if (pc == null)
            {
                return false;
            }
            if (playlistName.Equals(FavoriteName))
            {
                lock (_session)
                {
                    _currentlyPlayingPlaylist = _session.StarredCreate();
                }
            }
            else
            {
                for (int i = 0; i < pc.NumPlaylists(); i++)
                {
                    Playlist list = pc.Playlist(i);
                    if (list.Name().Equals(playlistName))
                    {
                        _currentlyPlayingPlaylist = list;
                        break;
                    }
                }
            }
            if (_currentlyPlayingPlaylist == null)
            {
                return false;
            }
            _playingOrder = Enumerable.Range(0, _currentlyPlayingPlaylist.NumTracks()).ToList();
            if (random)
            {
                List<int> straightPlayingOrder = _playingOrder.ToList();
                Random rndGen = new Random();
                _playingOrder.Clear();
                while (straightPlayingOrder.Any())
                {
                    int index = rndGen.Next(straightPlayingOrder.Count);
                    _playingOrder.Add(straightPlayingOrder[index]);
                    straightPlayingOrder.RemoveAt(index);
                }
            }
            _currentTrackNum = 0;

            ResumeUnlocked();
            return true;
        }

        private void ResumeUnlocked()
        {
            _session.PlayerLoad(_currentlyPlayingPlaylist.Track(_playingOrder[_currentTrackNum]));
            _session.PlayerPlay(true);
            _isPlaying = true;
        }

        public void StopMusic()
        {
            if (_isValid)
            {
                lock (_session)
                {
                    StopMusicUnlocked();
                }
            }
        }

        private void StopMusicUnlocked()
        {
            _session.PlayerPlay(false);
            _isPlaying = false;
        }

        public List<String> GetPlaylists()
        {
            if (_isValid)
            {
                _loggedInHandle.WaitOne();
                PlaylistContainer pc = GetPlaylistContainer();
                if (pc == null)
                {
                    return new List<String>();
                }
                lock (_session)
                {
                    return GetPlaylistsUnlocked(pc);
                }
            }
            return new List<string>();
        }

        private List<String> GetPlaylistsUnlocked(PlaylistContainer pc)
        {
            _session.FlushCaches();
            List<String> playlists = new List<String>();

            playlists.Add(FavoriteName);
            for (int i = 0; i < pc.NumPlaylists(); i++)
            {
                if (!pc.Playlist(i).Name().Equals(String.Empty))
                {
                    playlists.Add(pc.Playlist(i).Name());
                }
            }
            return playlists;
        }

        public void DeleteCurrentTrack()
        {
            if (_isValid)
            {
                lock (_session)
                {
                    DeleteCurrentTrackUnlocked();
                }
            }
        }

        private void DeleteCurrentTrackUnlocked()
        {
            _currentlyPlayingPlaylist.RemoveTracks(new int[1] { _playingOrder[_currentTrackNum] });
        }

        public void SkipToNextTrack()
        {
            if (_isValid)
            {
                lock (_session)
                {
                    SkipToNextTrackUnlocked();
                }
            }
        }

        private void SkipToNextTrackUnlocked()
        {
            StopMusicUnlocked();
            if (MusicRestarting != null)
            {
                MusicRestarting(this, new EventArgs());
            }
            StartNextTrackUnlocked();
        }

        public String GetCurrentSongName()
        {
            if (_isValid)
            {
                return _currentlyPlayingPlaylist.Track(_playingOrder[_currentTrackNum]).Name();
            }
            return String.Empty;
        }

        public String GetCurrentArtistName()
        {
            if (_isValid)
            {
                return _currentlyPlayingPlaylist.Track(_playingOrder[_currentTrackNum]).Artist(0).Name();
            }
            return String.Empty;
        }
        #endregion

        #region Overrides
        public override void NotifyMainThread(SpotifySession session)
        {
            _notifyHandle.Set();
        }

        public override void LoggedIn(SpotifySession session, SpotifyError error)
        {
            HandleSpotifyError(error);
            _loggedInHandle.Set();
        }

        public override void StreamingError(SpotifySession session, SpotifyError error)
        {
            base.StreamingError(session, error);
            HandleSpotifyError(error);
        }

        public override void ConnectionError(SpotifySession session, SpotifyError error)
        {
            base.ConnectionError(session, error);
            HandleSpotifyError(error);
        }

        public override void OfflineError(SpotifySession session, SpotifyError error)
        {
            base.OfflineError(session, error);
            HandleSpotifyError(error);
        }

        public override void ScrobbleError(SpotifySession session, SpotifyError error)
        {
            base.ScrobbleError(session, error);
            HandleSpotifyError(error);
        }

        public override int MusicDelivery(SpotifySession session, AudioFormat format, IntPtr frames, int num_frames)
        {
            if (MusicDataAvailable != null)
            {
                MusicDeliveryEventArgs args = new MusicDeliveryEventArgs(frames, num_frames, format.sample_rate, format.channels, 16);
                MusicDataAvailable(this, args);
                return args.NumFramesRead;
            }
            return 0;
        }

        public override void GetAudioBufferStats(SpotifySession session, out AudioBufferStats stats)
        {
            stats = new AudioBufferStats();
            stats.samples = 0;
            stats.stutter = 0;
            if (CheckBuffer != null)
            {
                CheckBufferEventArgs args = new CheckBufferEventArgs();
                CheckBuffer(this, args);
                stats.samples = args.NumberOfSamples / 4;
            }
        }

        public override void EndOfTrack(SpotifySession session)
        {
            StartNextTrackUnlocked();
        }
        #endregion

        #region Utility Functions
        private PlaylistContainer GetPlaylistContainer()
        {
            PlaylistContainer pc;
            lock (_session)
            {
                pc = _session.Playlistcontainer();
            }
            while ((pc == null || pc.NumPlaylists() == 0) && _isValid)
            {
                Thread.Sleep(1000);
                lock (_session)
                {
                    pc = _session.Playlistcontainer();
                }
            }

            if (!_isValid)
            {
                return null;
            }

            for (int i = 0; i < pc.NumPlaylists(); i++)
            {
                int numIterations = 0;
                while (pc.Playlist(i).Name().Equals(String.Empty) && numIterations++ < 100)
                {
                    Thread.Sleep(100);
                    lock (_session)
                    {
                        pc = _session.Playlistcontainer();
                    }
                }
            }
            return pc;
        }

        private void StartNextTrackUnlocked()
        {
            lock (_session)
            {
                _session.PlayerLoad(_currentlyPlayingPlaylist.Track(_playingOrder[++_currentTrackNum]));
                bool keepTrying = true;
                while (keepTrying)
                {
                    try
                    {
                        _session.PlayerPlay(true);
                        keepTrying = false;
                    }
                    catch (SpotifyException exp)
                    {
                        if (exp.Error != SpotifyError.TrackNotPlayable)
                        {
                            throw;
                        }
                    }
                }
            }
        }
        #endregion

        #region Error
        private void HandleSpotifyError(SpotifyError error)
        {
            switch (error)
            {
                case SpotifyError.BadUsernameOrPassword:
                    // ErrorManager.GetInstance().AddError(new SAMIUserException("Spotify's credentials were not correct. Please check Spotify's configuration and relaunch Sammie."));
                    _isValid = false;
                    break;
                case SpotifyError.ApiInitializationFailed:
                case SpotifyError.ApplicationBanned:
                case SpotifyError.BadApiVersion:
                case SpotifyError.BadApplicationKey:
                case SpotifyError.BadUserAgent:
                case SpotifyError.CantOpenTraceFile:
                case SpotifyError.ClientTooOld:
                case SpotifyError.InboxIsFull:
                case SpotifyError.IndexOutOfRange:
                case SpotifyError.InvalidArgument:
                case SpotifyError.InvalidDeviceId:
                case SpotifyError.InvalidIndata:
                case SpotifyError.IsLoading:
                case SpotifyError.LastfmAuthError:
                case SpotifyError.MissingCallback:
                case SpotifyError.NetworkDisabled:
                case SpotifyError.NoCache:
                case SpotifyError.NoCredentials:
                case SpotifyError.NoStreamAvailable:
                case SpotifyError.NoSuchUser:
                case SpotifyError.OfflineDiskCache:
                case SpotifyError.OfflineExpired:
                case SpotifyError.OfflineLicenseError:
                case SpotifyError.OfflineLicenseLost:
                case SpotifyError.OfflineNotAllowed:
                case SpotifyError.OfflineTooManyTracks:
                case SpotifyError.OtherPermanent:
                case SpotifyError.OtherTransient:
                case SpotifyError.PermissionDenied:
                case SpotifyError.SystemFailure:
                case SpotifyError.TrackNotPlayable:
                case SpotifyError.UnableToContactServer:
                case SpotifyError.UserBanned:
                case SpotifyError.UserNeedsPremium:
                    _isValid = false;
                    break;
                case SpotifyError.Ok:
                default:
                    break;
            }
        }
        #endregion
    }
}
