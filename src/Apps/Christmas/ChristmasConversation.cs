using System;
using System.Linq;
using SAMI.Apps;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.LightSwitch;
using SAMI.IOInterfaces.Interfaces.Audio;

namespace Christmas
{
    public class ChristmasConversation : Conversation
    {
        protected override String CommandName
        {
            get
            {
                return "Christmas";
            }
        }

        public ChristmasConversation(IConfigurationManager configManager)
            : base(configManager)
        {
        }

        public override String Speak()
        {
            base.Speak();
            ConversationIsOver = true;

            // Turn on the light switch
            ILightSwitchController lightSwitchControl = ConfigManager.FindAllComponentsOfType<ILightSwitchController>().FirstOrDefault(s => s.Name.Contains("Christmas"));
            lightSwitchControl.TurnOn();


            // Start the music!
            IMusicController controller = ConfigManager.FindAllComponentsOfType<IMusicController>().FirstOrDefault(c => c.GetPlaylists().Contains("Christmas"));
            if (controller == null || !controller.TryStartPlaylist("Christmas", true))
            {
                return "I'm sorry, I could not find the music playlist.";
            }
            else
            {
                return "Merry Christmas!";
            }

        }
    }
}
