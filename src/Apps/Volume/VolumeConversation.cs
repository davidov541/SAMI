using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Remote;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Volume
{
    internal class VolumeConversation : Conversation
    {
        private VolumeConversationState _state;
        private IEnumerable<IOInterfaceReference> _references;
        private String _source;

        protected override String CommandName
        {
            get
            {
                return "volume";
            }
        }

        public override string GrammarRuleName
        {
            get
            {
                switch (_state)
                {
                    case VolumeConversationState.Initial:
                        return GrammarUtility.MainGrammarName;
                    case VolumeConversationState.VolumeAdjustment:
                        return VolumeApp.VolumeGrammarName;
                    default:
                        break;
                }
                return base.GrammarRuleName;
            }
        }

        internal VolumeConversation(IConfigurationManager configManager, IEnumerable<IOInterfaceReference> references)
            : base(configManager, DateTime.Now.AddMinutes(1))
        {
            _state = VolumeConversationState.Initial;
            _references = references;
        }

        public override string Speak()
        {
            base.Speak();
            Dialog phrase = CurrentDialog;
            int levelNum = Int32.Parse(phrase.GetPropertyValue("levelNum"));
            switch (_state)
            {
                case VolumeConversationState.Initial:
                    if (levelNum == -1)
                    {
                        // If we are asking to do volume and we didn't indicate a repeat volume, we should use the interactive mode.
                        ConversationIsOver = false;
                        _state = VolumeConversationState.VolumeAdjustment;
                        _source = phrase.GetPropertyValue("source");
                        AdjustVolume(phrase.GetPropertyValue("direction"), 0.05, _source);
                        return "How is that?";
                    }
                    else
                    {
                        Console.WriteLine("About to call adjust volume with " + phrase.GetPropertyValue("direction"));
                        ConversationIsOver = true;
                        Task.Run(() => AdjustVolume(phrase.GetPropertyValue("direction"), levelNum / 100.0, phrase.GetPropertyValue("source")));
                        return "OK";
                    }
                case VolumeConversationState.VolumeAdjustment:
                    if (levelNum == -1)
                    {
                        ConversationIsOver = true;
                        return "Good";
                    }
                    else
                    {
                        ConversationIsOver = false;
                        AdjustVolume(phrase.GetPropertyValue("direction"), levelNum / 100.0, _source);
                        return "How is that?";
                    }
                default:
                    break;
            }
            return String.Empty;
        }

        private void AdjustVolume(String direction, double amount, String source)
        {
            switch (direction)
            {
                case "up":
                    break;
                case "down":
                    amount = amount * -1;
                    break;
                case "mute":
                    amount = 0.0;
                    break;
            }
            IEnumerable<IVolumeController> controllers = ConfigManager.FindAllComponentsOfType<IVolumeController>();
            foreach (IVolumeController controller in controllers)
            {
                if ((String.IsNullOrEmpty(source) && _references.Any(r => r.Name.Equals(controller.Name))) ||
                    (!String.IsNullOrEmpty(source) && controller.Name.Equals(source)))
                {
                    if (amount == 0)
                    {
                        controller.Volume = 0;
                    }
                    else
                    {
                        controller.Volume = controller.Volume + amount;
                    }
                }
            }
        }
    }
}
