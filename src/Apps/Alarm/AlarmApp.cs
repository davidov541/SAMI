using System.ComponentModel.Composition;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.Apps.Alarm
{
    [ParseableElement("Alarm", ParseableElementType.App)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class AlarmApp : VoiceActivatedApp<AlarmConversation>
    {
        private AlarmManager _manager;

        public AlarmApp()
            : base()
        {
        }

        public override void Initialize(IConfigurationManager configManager)
        {
            base.Initialize(configManager);
            _manager = new AlarmManager(ConfigManager);
            _manager.AlarmTriggered += (sender, args) => RaiseAsyncAlert(args.StartedConversation);
            Provider = new GrammarProvider(new XmlGrammar(ConfigManager, "AlarmGrammar.grxml", GetType()));
        }

        public override bool TryCreateConversationFromPhrase(Dialog phrase, out Conversation createdConversation)
        {
            createdConversation = new AlarmConversation(ConfigManager, _manager);
            bool tryResult = createdConversation.TryAddDialog(phrase);
            if (!tryResult)
            {
                createdConversation.Dispose();
            }
            return tryResult;
        }
    }
}
