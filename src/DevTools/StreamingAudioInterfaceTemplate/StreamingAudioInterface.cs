using System;
using System.ComponentModel.Composition;
using System.Threading;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Audio;
using SAMI.Persistence;

namespace $safeprojectname$
{
    [ParseableElement("$projectname$", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
	public class $safeprojectname$ : StreamingAudioIOInterface
	{
        private byte[] _data = new byte[]
        {
            // Replace with your real data or dynamically generate from stream. 
            0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
            0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F,
        };

        public override String Name
        {
            get
            {
                return "$safeprojectname$";
            }
        }

        public $safeprojectname$(IConfigurationManager configManager)
        {
        }

        public void PlayAndStopSound()
        {
            IAudioProvider provider;
            int streamKey = InitializeStream(44100, 2, 8, out provider);
            provider.AddData(_data, 0, _data.Length);
            StartStream(streamKey);
            Thread.Sleep(1000);
            StopStream(streamKey);
        }
	}
}
