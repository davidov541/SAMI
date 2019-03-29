using System;
using SAMI.Configuration;

namespace SAMI.Apps.Current
{
    internal class CurrentConversation : Conversation
    {
        protected override String CommandName
        {
            get
            {
                return "current";
            }
        }

        public CurrentConversation(IConfigurationManager configManager)
            : base(configManager)
        {
        }

        public override string Speak()
        {
            base.Speak();
            ConversationIsOver = true;
            switch (CurrentDialog.GetPropertyValue("Param").ToLower())
            {
                case "time":
                    return String.Format("It is {0}.", DateTime.Now.ToShortTimeString());
                case "date":
                    return String.Format("Today is {0}.", DateTime.Now.ToLongDateString());
                case "day":
                    return String.Format("Today is a {0}.", DateTime.Now.DayOfWeek.ToString());
                default:
                    return "Could not decifer what you are asking for.";
            }
        }
    }
}
