using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;
using SAMI.IOInterfaces.Interfaces.Audio;
using SAMI.IOInterfaces.Interfaces.Remote;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Audio
{
    [ParseableElement("AudioStream", ParseableElementType.Component)]
    internal class AudioStreamManager : IAudioStreamManager, IDisposable, IVolumeController
    {
        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        public String Name
        {
            get;
            private set;
        }

        public IEnumerable<PersistentProperty> Properties
        {
            get
            {
                yield return new PersistentProperty("Name", () => Name, n => Name = n);
            }
        }

        public IEnumerable<IParseable> Children
        {
            get
            {
                yield break;
            }
        }

        public AudioStreamManager()
        {
            Volume = 0.5f;
        }

        public void Initialize()
        {
        }

        public void AddChild(IParseable component)
        {
        }

        private List<WaveOut> _streams = new List<WaveOut>();

        public bool AreStreamsPlaying
        {
            get
            {
                return _streams.Any();
            }
        }

        private double _currentVolume;
        public double Volume
        {
            get
            {
                return _currentVolume;
            }
            set
            {
                _currentVolume = Math.Max(0.0, Math.Min(value, 1.0));
                foreach (WaveOut stream in _streams)
                {
                    stream.Volume = (float)_currentVolume;
                }
            }
        }

        public bool IsConnected
        {
            get
            {
                return AreStreamsPlaying;
            }
        }

        public void StartStream(int streamKey)
        {
            _streams[streamKey].Volume = (float)Volume;
            _streams[streamKey].Play();
        }

        public int InitializeStream(int samplingRate, int numberOfChannels, int numberOfBits, out IAudioProvider provider)
        {
            provider = new Int32WaveProvider(new WaveFormat(samplingRate, numberOfBits, numberOfChannels));
            (provider as Int32WaveProvider).BufferDuration = TimeSpan.FromSeconds(120);

            int nextIndex = _streams.Count;
            _streams.Insert(nextIndex, new WaveOut());
            _streams[nextIndex].Init(provider as Int32WaveProvider);
            return nextIndex;
        }

        public void StopStream(int streamKey)
        {
            _streams[streamKey].Stop();
            _streams[streamKey].Dispose();
            _streams.RemoveAt(streamKey);
        }

        public void Dispose()
        {
            foreach (WaveOut stream in _streams)
            {
                stream.Stop();
                stream.Dispose();
            }
            _streams.Clear();
        }
    }
}
