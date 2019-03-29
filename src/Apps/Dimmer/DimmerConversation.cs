using System;
using System.Linq;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.LightSwitch;

namespace SAMI.Apps.Dimmer
{
    public class DimmerConversation : Conversation
    {
        protected override String CommandName
        {
            get
            {
                return "Dimmer";
            }
        }

        public DimmerConversation(IConfigurationManager configManager)
            : base(configManager)
        {
        }

        public override String Speak()
        {
            base.Speak();
            ConversationIsOver = true;
            double level = Double.Parse(CurrentDialog.GetPropertyValue("Level")) / 100.0;
            IDimmableLightSwitchController lightSwitch = ConfigManager.FindAllComponentsOfType<IDimmableLightSwitchController>().FirstOrDefault(l => l.Name.Equals(CurrentDialog.GetPropertyValue("Switch")));
            if (lightSwitch != null)
            {
                lightSwitch.SetLightLevel(level);
            }
            return "OK";
        }
    }
}
