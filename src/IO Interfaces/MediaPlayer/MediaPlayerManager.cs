using System;
using System.ComponentModel.Composition;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Audio;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Media
{
    [ParseableElement("MediaPlayer", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class MediaPlayerManager : FileAudioIOInterface, IMediaFilePlayer
    {
        public override String Name
        {
            get
            {
                return "VLC";
            }
        }

        public PlayerState CurrentState
        {
            get 
            {
                return State;
            }
        }

        public MediaPlayerManager()
            : base()
        {
        }
        
        public void PlayAudioFile(string fileName)
        {
            PlayFile(fileName);
        }

        public void PauseAudioFile()
        {
            PauseFile();
        }

        public void ResumeAudioFile()
        {
            ResumeFile();
        }

        public void StopAudioFile()
        {
            StopFile();
        }
    }
}
