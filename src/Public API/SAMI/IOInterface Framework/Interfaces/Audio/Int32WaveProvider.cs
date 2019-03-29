using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Audio;

namespace SAMI.IOInterfaces.Interfaces.Audio
{
    internal class Int32WaveProvider : BufferedWaveProvider, IAudioProvider
    {
        private Queue<short> _sound;
        private const int QueueSize = 441000;

        public int NumberOfSamplesInBuffer
        {
            get
            {
                return BufferedBytes;
            }
        }

        public Int32WaveProvider(WaveFormat format) :
            base(format)
        {
            _sound = new Queue<short>(QueueSize);
        }

        public int AddData(byte[] buffer, int offset, int size)
        {
            AddSamples(buffer, offset, size);
            return buffer.Length;
        }

        public void Reset()
        {
            _sound.Clear();
        }
    }

}
