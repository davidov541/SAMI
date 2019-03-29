using System;

namespace SAMI.Apps.Podcast
{
    internal class Episode
    {
        public Uri AudioFile
        {
            get;
            set;
        }

        public DateTime Timestamp
        {
            get;
            set;
        }

        public bool PreviouslySeen
        {
            get;
            set;
        }

        public Episode(Uri audioFile, DateTime timestamp)
        {
            this.AudioFile = audioFile;
            this.Timestamp = timestamp;
            this.PreviouslySeen = false;
        }
    }
}
