using System;
using SAMI.Apps;
using SAMI.Configuration;

namespace SAMI.Error
{
    internal class ErrorConversation : Conversation
    {
        protected override String CommandName
        {
            get
            {
                return "error";
            }
        }

        private SAMIUserException _exception;

        public override string GrammarRuleName
        {
            get
            {
                return "errorConversationGrammar";
            }
        }

        public ErrorConversation(IConfigurationManager configManager, SAMIUserException exp)
            : base(configManager)
        {
            _exception = exp;
        }

        public override string Speak()
        {
            base.Speak();
            ConversationIsOver = true;
            return _exception.MessageToUser;
        }
    }
}
