using System;

namespace SAMI.IOInterfaces.Spotify
{
    /// <summary>
    /// Event arguments passed when new audio is available from an IMusicProvider component.
    /// </summary>
    internal class MusicDeliveryEventArgs : EventArgs
    {
        /// <summary>
        /// IntPtr to a native memory allocation that contains the data.
        /// </summary>
        public IntPtr Frames
        {
            get;
            private set;
        }

        /// <summary>
        /// The number of frames that Frames contains.
        /// </summary>
        public int NumFrames
        {
            get;
            private set;
        }

        public int SampleRate
        {
            get;
            private set;
        }

        public int NumberOfChannels
        {
            get;
            private set;
        }

        public int NumberOfBits
        {
            get;
            private set;
        }

        private int _numFramesRead;
        /// <summary>
        /// Indicates how many of the frames in this event were read.
        /// Any frames that are not read will be contained in the next event.
        /// </summary>
        public int NumFramesRead
        {
            get
            {
                return _numFramesRead;
            }
            set
            {
                if(NumFramesRead < value)
                {
                    _numFramesRead = value;
                }
            }
        }

        /// <summary>
        /// Constructor for MusicDeliveryEventArgs.
        /// </summary>
        /// <param name="frames">The initial value for <see cref="Frames"/>.</param>
        /// <param name="numFrames">The initial value for <see cref="NumFrames"/>.</param>
        public MusicDeliveryEventArgs(IntPtr frames, int numFrames, int sampleRate, int channelNum, int numberOfBits)
        {
            Frames = frames;
            NumFrames = numFrames;
            NumFramesRead = 0;
            SampleRate = sampleRate;
            NumberOfChannels = channelNum;
            NumberOfBits = numberOfBits;
        }
    }
}
