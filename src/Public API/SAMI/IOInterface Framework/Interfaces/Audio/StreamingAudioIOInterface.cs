using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Remote;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Interfaces.Audio
{
    /// <summary>
    /// Base class for any IO interfaces that need to play audio
    /// from a byte array or byte stream.
    /// In order to use, override this class and use the protected methods.
    /// </summary>
    public abstract class StreamingAudioIOInterface : IVolumeController
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
        private double _volume = 0.5;

        /// <summary>
        /// The current volume that the hardware is set to.
        /// The volume is set on a scale from 0 - 1.
        /// </summary>
        public double Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                _volume = Math.Min(Math.Max(value, 0), 1);
                foreach (WaveOut stream in _streams)
                {
                    stream.Volume = (float)_volume;
                }
            }
        }
        #endregion

        private List<WaveOut> _streams = new List<WaveOut>();
        /// <summary>
        /// Indicates whether any streams are currently playing for this IO interface.
        /// </summary>
        public bool AreStreamsPlaying
        {
            get
            {
                return _streams.Any();
            }
        }

        #region IPersistable Functions
        /// <summary>
        /// Called once all IParseable instances have been created from the configuration file.
        /// This is called on the parent element before being called on the children elements.
        /// </summary>
        /// <param name="configManager">Configuration manager creating this interface</param>
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
            foreach (WaveOut stream in _streams)
            {
                stream.Stop();
                stream.Dispose();
            }
            _streams.Clear();
        }
        #endregion

        public StreamingAudioIOInterface()
        {
        }

        #region Binary Playback Functions
        /// <summary>
        /// Creates and initializes a stream to the given specifications.
        /// Call this before calling any other functions.
        /// </summary>
        /// <param name="samplingRate">Sampling rate that the data will be using.</param>
        /// <param name="numberOfChannels">Number of channels that the data will contain.</param>
        /// <param name="numberOfBits">Number of bits each packet of data contains.</param>
        /// <param name="provider">An IAudioProvider instance for you to put data into for it to write out to the speaker.</param>
        /// <returns>Stream key if successful that represents the new stream that was created.</returns>
        protected int InitializeStream(int samplingRate, int numberOfChannels, int numberOfBits, out IAudioProvider provider)
        {
            provider = new Int32WaveProvider(new WaveFormat(samplingRate, numberOfBits, numberOfChannels));
            (provider as Int32WaveProvider).BufferDuration = TimeSpan.FromSeconds(120);

            int nextIndex = _streams.Count;
            _streams.Insert(nextIndex, new WaveOut());
            _streams[nextIndex].Init(provider as Int32WaveProvider);
            return nextIndex;
        }

        /// <summary>
        /// Starts the given stream.
        /// </summary>
        /// <param name="streamKey">Stream to start.</param>
        protected void StartStream(int streamKey)
        {
            if (_streams.Count == 1)
            {
                Volume = 0.25;
            }
            _streams[streamKey].Play();
        }

        /// <summary>
        /// Stops the given stream.
        /// </summary>
        /// <param name="streamKey">Stream to stop.</param>
        protected void StopStream(int streamKey)
        {
            _streams[streamKey].Stop();
            // HACK: Turn volume back up to 50% so that SAMI is audible.
            if(_streams.Count == 1)
            {
                Volume = 0.5;
            }
            _streams[streamKey].Dispose();
            _streams.RemoveAt(streamKey);
        }
        #endregion
    }
}
