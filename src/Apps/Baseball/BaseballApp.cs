using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Baseball;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.Apps.Baseball
{
    [ParseableElement("Baseball", ParseableElementType.App)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class BaseballApp : VoiceActivatedApp<BaseballConversation>
    {
        public IEnumerable<IOInterfaceReference> References
        {
            get
            {
                return Children.OfType<IOInterfaceReference>();
            }
        }

        public override bool IsValid
        {
            get
            {
                return ConfigManager != null && ConfigManager.FindAllComponentsOfType<IBaseballSensor>().Any();
            }
        }

        public override String InvalidMessage
        {
            get
            {
                if (IsValid)
                {
                    return String.Empty;
                }
                else
                {
                    return "We cannot connect to the baseball score server currently.";
                }
            }
        }

        public BaseballApp()
            : base()
        {
        }

        public override void Initialize(IConfigurationManager configManager)
        {
            base.Initialize(configManager);
            if (ConfigManager.FindAllComponentsOfType<IBaseballSensor>().Any())
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(ConfigManager.GetPathForFile("BaseballGrammar.grxml", GetType()));

                IEnumerable<BaseballTeam> teams = ConfigManager.FindAllComponentsOfType<IBaseballSensor>().SelectMany(s => s.Teams);
                XmlElement oneof = GrammarUtility.CreateElement(doc, "one-of", new Dictionary<String, String>());
                foreach (BaseballTeam possibleVal in teams)
                {
                    XmlElement item = GrammarUtility.CreateElement(doc, "item", new Dictionary<String, String>());
                    XmlElement teamCityItem = GrammarUtility.CreateElement(doc, "item", new Dictionary<String, String> { { "repeat", "0-1" } });
                    teamCityItem.InnerText = possibleVal.LocationName;
                    XmlElement teamNameItem = GrammarUtility.CreateElement(doc, "item", new Dictionary<String, String>());
                    teamNameItem.InnerText = possibleVal.Name;
                    XmlElement tag = GrammarUtility.CreateElement(doc, "tag", new Dictionary<String, String>());
                    tag.InnerText = "out = \"" + possibleVal.Key + "\";";
                    item.AppendChild(teamCityItem);
                    item.AppendChild(teamNameItem);
                    item.AppendChild(tag);
                    oneof.AppendChild(item);
                }
                XmlElement rule = GrammarUtility.CreateElement(doc, "rule", new Dictionary<string, string>
                {
                    {"id", "baseballTeams"},
                    {"scope", "public"},
                });
                rule.AppendChild(oneof);
                doc.LastChild.AppendChild(rule);
                Provider = new GrammarProvider(new XmlGrammar(doc));
            }
        }

        public override bool TryCreateConversationFromPhrase(Dialog phrase, out Conversation createdConversation)
        {
            createdConversation = new BaseballConversation(ConfigManager, References);
            bool tryResult = createdConversation.TryAddDialog(phrase);
            if (!tryResult)
            {
                createdConversation.Dispose();
            }
            return tryResult;
        }
    }
}
