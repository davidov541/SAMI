using System;
using System.ComponentModel.Composition;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.Apps.Error
{
    [ParseableElement("Error", ParseableElementType.App)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class ErrorApp : VoiceActivatedApp<ErrorConversation>
    {
        public override bool IsValid
        {
            get
            {
                return true;
            }
        }

        public override String InvalidMessage
        {
            get
            {
                return String.Empty;
            }
        }

        public ErrorApp()
            : base()
        {
        }

        public override void Initialize(IConfigurationManager configManager)
        {
            base.Initialize(configManager);
            Provider = new GrammarProvider(new XmlGrammar(ConfigManager, "UtilityGrammar.grxml", GetType()), new XmlGrammar(ConfigManager, "DateTimeGrammar.grxml", GetType()));
        }

        public override bool TryCreateConversationFromPhrase(Dialog phrase, out Conversation createdConversation)
        {
            createdConversation = null;
            return false;
        }
    }
}
