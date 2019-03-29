using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using NAudio.Wave;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Remote;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Interfaces.Audio
{
    /// <summary>
    /// Base class for any IO interfaces that need to play audio 
    /// from a file.
    /// In order to use, override this class, and call the protected methods.
    /// Currently, it is restricted to only working for local files.
    /// This limitation should be removed in the future.
    /// </summary>
    public abstract class FileAudioIOInterface : IVolumeController
    {
        #region IPersistable Properties
        /// <summary>
        /// Name of this particular instance of the IO resource.
        /// This allows for referencing from apps when determining which
        /// IO resources an app should interact with in a certain context.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Indicates if this IParseable is valid to be used.
        /// </summary>
        public virtual bool IsValid
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Contains PersitentProperty instances for each property that should be persisted by this class.
        /// </summary>
        public virtual IEnumerable<PersistentProperty> Properties
        {
            get
            {
                yield break;
            }
        }

        /// <summary>
        /// Contains all of the IParseable instances that should be persisted as
        /// children of this class.
        /// </summary>
        public virtual IEnumerable<IParseable> Children
        {
            get
            {
                yield break;
            }
        }
        #endregion

        #region IVolumeController Properties
        /// <summary>
        /// The current volume that the hardware is set to.
        /// The volume is set on a scale from 0 - 1.
        /// </summary>
        public double Volume
        {
            get;
            set;
        }
        #endregion

        public FileAudioIOInterface()
        {
            State = PlayerState.Stopped;
            Volume = 0.5;
        }

        #region IPersistable Functions
        /// <summary>
        /// Called once all IParseable instances have been created from the configuration file.
        /// This is called on the parent element before being called on the children elements.
        /// </summary>
        /// <param name="configManager">Configuration Manager that is creating this interface</param>
        public virtual void Initialize(IConfigurationManager configManager)
        {
        }

        /// <summary>
        /// Adds a child IParseable element. 
        /// This should add the element to the backing for <see cref="Children"/>,
        /// and perform any other necessary work on the child.
        /// </summary>
        /// <param name="child">Child to add to this IParseable instance.</param>
        public virtual void AddChild(IParseable child)
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if(_playThread != null && _playThread.IsAlive)
            {
                State = PlayerState.Stopped;
                _playThread.Join();
            }
        }
        #endregion

        #region File Playback Functions
        private Thread _playThread;

        /// <summary>
        /// Indicates what state the player is in currently.
        /// </summary>
        protected PlayerState State
        {
            get;
            private set;
        }

        /// <summary>
        /// Starts playing the file at the given name.
        /// If the file is not local, a NotImplementedException will be thrown.
        /// If something else is already playing, the call will be ignored.
        /// </summary>
        /// <param name="fileName">Path of the file to play.</param>
        protected void PlayFile(String fileName)
        {
            if (State == PlayerState.Stopped && (_playThread == null || !_playThread.IsAlive))
            {
                _playThread = new Thread(() => PlayFileInternal(fileName));
                _playThread.Start();
            }
            else if (State == PlayerState.Paused)
            {
                State = PlayerState.Playing;
            }
        }

        private void PlayFileInternal(String fileName)
        {
            using (Stream ms = new MemoryStream())
            {
                using (Stream stream = WebRequest.Create(fileName).GetResponse().GetResponseStream())
                {
                    byte[] buffer = new byte[32768];
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                }

                ms.Position = 0;
                using (WaveStream blockAlignedStream =
                    new BlockAlignReductionStream(
                        WaveFormatConversionStream.CreatePcmStream(
                            new Mp3FileReader(ms))))
                {
                    using (WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                    {
                        waveOut.Init(blockAlignedStream);
                        waveOut.Play();
                        State = PlayerState.Playing;
                        while (waveOut.PlaybackState != PlaybackState.Stopped)
                        {
                            if (State == PlayerState.Playing && waveOut.PlaybackState != PlaybackState.Playing)
                            {
                                waveOut.Play();
                            }
                            else if (State == PlayerState.Paused && waveOut.PlaybackState != PlaybackState.Paused)
                            {
                                waveOut.Pause();
                            }
                            else if (State == PlayerState.Stopped)
                            {
                                waveOut.Stop();
                            }
                            else if (Volume != waveOut.Volume)
                            {
                                waveOut.Volume = (float)Volume;
                            }
                            Thread.Sleep(100);
                        }
                        waveOut.Stop();
                    }
                }
            }
        }

        /// <summary>
        /// Event which is triggered once the file that was last played is finished.
        /// </summary>
        public event EventHandler<EventArgs> TrackFinished;

        private void stream_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            EventHandler<EventArgs> localCopy = TrackFinished;
            if (e.Exception == null && localCopy != null)
            {
                localCopy(this, new EventArgs());
            }
        }

        /// <summary>
        /// Pauses the currently playing file.
        /// If nothing is playing, nothing happens.
        /// </summary>
        protected void PauseFile()
        {
            if (State == PlayerState.Playing)
            {
                State = PlayerState.Paused;
            }
        }

        /// <summary>
        /// Resumes the currently playing file.
        /// If nothing is paused, nothing happens.
        /// </summary>
        protected void ResumeFile()
        {
            if (State == PlayerState.Paused)
            {
                State = PlayerState.Playing;
            }
        }

        protected void StopFile()
        {
            if(State == PlayerState.Playing ||
                State == PlayerState.Paused)
            {
                State = PlayerState.Stopped;
                _playThread.Join();
            }
        }
        #endregion
    }
}
