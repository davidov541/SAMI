using System;
using System.Collections.Generic;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Jokes
{
    internal class OneLinerConversation : JokeConversation
    {
        internal String Message
        {
            get;
            private set;
        }

        public override IEnumerable<XmlGrammar> GrammarsNeeded
        {
            get
            {
                yield break;
            }
        }

        public OneLinerConversation(IConfigurationManager configManager, String message)
            : base(configManager, DateTime.Now.Add(TimeSpan.FromMinutes(1)))
        {
            Message = message;
        }

        public override object Clone()
        {
            return new OneLinerConversation(ConfigManager, Message);
        }

        public override bool Equals(object obj)
        {
            if(!(obj is OneLinerConversation))
            {
                return false;
            }
            OneLinerConversation other = obj as OneLinerConversation;
            return other.Message.Equals(Message);
        }

        public override string Speak()
        {
            ConversationIsOver = true;
            return Message;
        }
    }
}
