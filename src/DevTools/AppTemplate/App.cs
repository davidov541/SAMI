using System.ComponentModel.Composition;
using SAMI.Apps;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace $projectname$
{
    [ParseableElement("$projectname$", ParseableElementType.App)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class $projectname$ : VoiceActivatedApp
    {
        public $projectname$(IConfigurationManager configManager)
        {
            Provider = new GrammarProvider(new XmlGrammar(configManager, "AppGrammar.grxml", GetType()));
        }

        public override bool TryCreateConversationFromPhrase(Dialog phrase, out Conversation createdConversation)
        {
            createdConversation = null;
            if (phrase.CheckForApp("$projectname$"))
            {
                createdConversation = new InformationalConversation(ConfigManager, "Example Phrase");
                return true;
            }
            return false;
        }
    }
}
