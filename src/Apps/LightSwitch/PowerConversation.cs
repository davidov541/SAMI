using System;
using System.Linq;
using System.Threading.Tasks;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Power;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Power
{
    internal class PowerConversation : Conversation
    {
        protected override String CommandName
        {
            get
            {
                return "power";
            }
        }

        public Task CommandTask
        {
            get;
            private set;
        }

        public PowerConversation(IConfigurationManager configManager)
            : base(configManager)
        {
        }

        public override string Speak()
        {
            base.Speak();
            Dialog phrase = CurrentDialog;
            IPowerController lightSwitchControl = ConfigManager.FindAllComponentsOfType<IPowerController>().FirstOrDefault(s => s.Name.Equals(phrase.GetPropertyValue("SwitchName")));
            ConversationIsOver = true;
            if (lightSwitchControl == null)
            {
                return String.Empty;
            }
            else if (phrase.GetPropertyValue("Direction").Equals("on"))
            {
                CommandTask = Task.Run(() =>
                {
                    lightSwitchControl.TurnOn();
                });
                return "OK";
            }
            else if (phrase.GetPropertyValue("Direction").Equals("off"))
            {

                CommandTask = Task.Run(() =>
                {
                    lightSwitchControl.TurnOff();
                });
                return "OK";
            }
            return "Sorry, that is not a valid light switch command.";

        }

    }
}
