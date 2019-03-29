using System;
using System.Collections.Generic;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Jokes
{
    internal abstract class JokeConversation : Conversation, ICloneable
    {
        protected override String CommandName
        {
            get
            {
                return "jokes";
            }
        }

        public abstract IEnumerable<XmlGrammar> GrammarsNeeded
        {
            get;
        }

        internal JokeConversation(IConfigurationManager configManager, DateTime expirationDate)
            : base(configManager, expirationDate)
        {
        }

        public abstract object Clone();
    }
}
