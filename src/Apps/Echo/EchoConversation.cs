using System;
using SAMI.Configuration;

namespace SAMI.Apps.Echo
{
    internal class EchoConversation : Conversation
    {
        protected override String CommandName
        {
            get
            {
                return "echo";
            }
        }

        public EchoConversation(IConfigurationManager configManager)
            : base(configManager)
        {
        }

        public override string Speak()
        {
            base.Speak();
            ConversationIsOver = true;
            return CurrentDialog.GetPropertyValue("Param");
        }
    }
}
