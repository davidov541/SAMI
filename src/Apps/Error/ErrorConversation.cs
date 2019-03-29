using System;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Error
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

        private Exception _exception;
        private String _message;
        private bool _hasIndicatedError;

        public override string GrammarRuleName
        {
            get
            {
                return String.Empty;
            }
        }

        internal ErrorConversation(IConfigurationManager configManager, String message)
            : base(configManager, DateTime.Now.AddHours(1))
        {
            _message = message;
            _hasIndicatedError = false;
        }

        internal ErrorConversation(IConfigurationManager configManager, Exception exp)
            : base(configManager, DateTime.Now.AddHours(1))
        {
            _exception = exp;
            _hasIndicatedError = false;
        }

        public override string Speak()
        {
            base.Speak();
            if (CurrentDialog != null)
            {
                switch (CurrentDialog.GetPropertyValue("Subcommand"))
                {
                    case "message":
                        ConversationIsOver = false;
                        return _exception.Message;
                    case "callstack":
                        ConversationIsOver = false;
                        return _exception.TargetSite.Name;
                    case "type":
                        ConversationIsOver = false;
                        return _exception.GetType().Name;
                    default:
                        ConversationIsOver = true;
                        return String.Empty;
                }
            }
            else
            {
                if (_exception != null)
                {
                    ConversationIsOver = false;
                    _hasIndicatedError = true;
                    return "I encountered a " + _exception.GetType().Name + ". What would you like to do?";
                }
                else
                {
                    ConversationIsOver = true;
                    return _message;
                }
            }
        }
    }
}
