using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.Apps.Podcast
{
    [ParseableElement("PodcastApp", ParseableElementType.App)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class PodcastApp : VoiceActivatedApp<PodcastConversation>
    {
        private IEnumerable<PodcastInfo> Podcasts
        {
            get
            {
                return Children.OfType<PodcastInfo>();
            }
        }

        public override bool IsValid
        {
            get
            {
                return Podcasts.Any();
            }
        }

        public override string InvalidMessage
        {
            get
            {
                return "No podcasts have been registered with this app. Please register podcasts to play.";
            }
        }

        public PodcastApp()
            : base()
        {
        }

        public override void Initialize(IConfigurationManager configManager)
        {
            base.Initialize(configManager);
            if (IsValid)
            {
                List<String> podcastNames = Podcasts.Select(info => info.Name).ToList();
                XmlGrammar grammar = GrammarUtility.CreateGrammarFromList(ConfigManager.GetPathForFile("PodcastGrammar.grxml", GetType()), "PodcastName", podcastNames);
                Provider = new GrammarProvider(grammar);
            }
        }

        public override bool TryCreateConversationFromPhrase(Dialog phrase, out Conversation createdConversation)
        {
            createdConversation = new PodcastConversation(ConfigManager, Podcasts);
            bool tryResult = createdConversation.TryAddDialog(phrase);
            if (!tryResult)
            {
                createdConversation.Dispose();
            }
            return tryResult;
        }
    }
}
