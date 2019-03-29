using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.Apps.Jokes
{
    [ParseableElement("Jokes", ParseableElementType.App)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class JokesApp : VoiceActivatedApp<CashewJokesConversation>
    {
        private List<JokeConversation> _jokeConversationPrototypes;
        private int _currJoke = 0;
        private List<JokeConversation> _sortedConversations;

        public int NumberOfJokes
        {
            get
            {
                return _jokeConversationPrototypes.Count;
            }
        }

        public JokesApp()
            : base()
        {
        }

        public override void Initialize(IConfigurationManager configManager)
        {
            base.Initialize(configManager);
            _jokeConversationPrototypes = new List<JokeConversation>
            {
                new CashewJokesConversation(ConfigManager),
                new OneLinerConversation(ConfigManager, "What did the spider do on the computer?... Made a website!"),
                new OneLinerConversation(ConfigManager, "Where do all the cool mice live?... In their mousepads!"),
                new OneLinerConversation(ConfigManager, "What do you get when you cross a computer and a lifeguard?... A screensaver!"),
            };
            Random rand = new Random();
            _sortedConversations = _jokeConversationPrototypes.OrderBy<Conversation, int>(i => rand.Next()).OfType<JokeConversation>().ToList();

            Provider = new GrammarProvider(new XmlGrammar(ConfigManager, "JokesGrammar.grxml", GetType()),
                _jokeConversationPrototypes.SelectMany(j => j.GrammarsNeeded).ToArray());
        }

        public override bool TryCreateConversationFromPhrase(Dialog phrase, out Conversation createdConversation)
        {
            createdConversation = _sortedConversations.ElementAt(_currJoke++).Clone() as Conversation;
            if (_currJoke >= _sortedConversations.Count)
            {
                _currJoke = 0;
                Random rand = new Random();
                _sortedConversations = _jokeConversationPrototypes.OrderBy<Conversation, int>(i => rand.Next()).OfType<JokeConversation>().ToList();
            }
            if (createdConversation.TryAddDialog(phrase))
            {
                return true;
            }
            createdConversation.Dispose();
            return false;
        }
    }
}
