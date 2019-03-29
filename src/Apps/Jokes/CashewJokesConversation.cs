using System;
using System.Collections.Generic;
using System.Linq;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps.Jokes
{
    internal class CashewJokesConversation : JokeConversation
    {
        private JokeState _currState;

        private static Dictionary<JokeState, String> JokeRules = new Dictionary<JokeState, string>
            {
                {JokeState.KnockKnock1, "whosThereRule"},
                {JokeState.KnockKnock2, "whosThereRule"},
                {JokeState.KnockKnock3, "cashWhoRule"},
            };

        private enum JokeState
        {
            KnockKnock1,
            KnockKnock2,
            KnockKnock3,
        };

        public override string GrammarRuleName
        {
            get
            {
                return JokeRules[_currState];
            }
        }

        public override IEnumerable<XmlGrammar> GrammarsNeeded
        {
            get
            {
                return JokeRules.Select(rule => rule.Value).Distinct().Select(ruleName => new XmlGrammar(ConfigManager, ruleName, ruleName + ".grxml", GetType()));
            }
        }

        public CashewJokesConversation(IConfigurationManager configManager)
            : base(configManager, DateTime.Now.Add(TimeSpan.FromMinutes(5)))
        {
        }

        public override string Speak()
        {
            base.Speak();
            switch (_currState)
            {
                case JokeState.KnockKnock1:
                    ConversationIsOver = false;
                    _currState = JokeState.KnockKnock2;
                    return "Knock knock";
                case JokeState.KnockKnock2:
                    ConversationIsOver = false;
                    _currState = JokeState.KnockKnock3;
                    return "Cash";
                case JokeState.KnockKnock3:
                    ConversationIsOver = true;
                    return "No thanks, but I’d like some peanuts!";
                default:
                    ConversationIsOver = true;
                    return "I forgot what joke I was telling!";
            }
        }

        public override object Clone()
        {
            return new CashewJokesConversation(ConfigManager);
        }

        public override bool Equals(object obj)
        {
            if(!(obj is CashewJokesConversation))
            {
                return false;
            }
            return true;
        }
    }
}
