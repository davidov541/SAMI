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
	public class $safeprojectname$ : FileAudioIOInterface
	{
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

        public void PlayAndStopMusic(String filePath)
        {
            if (State != PlayerState.Playing)
            {
                PlayFile(filePath);
            }
            Thread.Sleep(1000);
            PauseFile();
        }
	}
}
