using System.ComponentModel.Composition;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace Christmas
{
    [ParseableElement("Christmas", ParseableElementType.App)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Christmas : VoiceActivatedApp<ChristmasConversation>
    {
        public Christmas()
            : base()
        {
        }

        public override void Initialize(IConfigurationManager configManager)
        {
            base.Initialize(configManager);
            Provider = new GrammarProvider(new XmlGrammar(configManager, "ChristmasGrammar.grxml", GetType()));
        }
    }
}
