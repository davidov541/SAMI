using System.ComponentModel.Composition;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.Apps.Echo
{
    [ParseableElement("Echo", ParseableElementType.App)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class EchoApp : VoiceActivatedApp<EchoConversation>
    {
        public EchoApp()
            : base()
        {
        }

        public override void Initialize(IConfigurationManager configManager)
        {
            base.Initialize(configManager);
            Provider = new GrammarProvider(new XmlGrammar(ConfigManager, "EchoGrammar.grxml", GetType()));
        }
    }
}
