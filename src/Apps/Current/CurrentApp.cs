using System.ComponentModel.Composition;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.Apps.Current
{
    [ParseableElement("Current", ParseableElementType.App)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class CurrentApp : VoiceActivatedApp<CurrentConversation>
    {
        public CurrentApp()
            : base()
        {
        }

        public override void Initialize(IConfigurationManager configManager)
        {
            base.Initialize(configManager);
            Provider = new GrammarProvider(new XmlGrammar(ConfigManager, "CurrentGrammar.grxml", GetType()));
        }
    }
}
