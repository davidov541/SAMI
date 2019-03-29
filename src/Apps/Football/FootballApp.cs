using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Football;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.Apps.Football
{
    [ParseableElement("Football", ParseableElementType.App)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class FootballApp : VoiceActivatedApp<FootballConversation>
    {
        private bool _foundTeams = true;
        public override bool IsValid
        {
            get
            {
                return ConfigManager != null && ConfigManager.FindAllComponentsOfType<IFootballSensor>().Any() && _foundTeams;
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
                    return "We cannot connect to the Football score server currently.";
                }
            }
        }

        public FootballApp()
            : base()
        {
        }

        public override void Initialize(IConfigurationManager configManager)
        {
            base.Initialize(configManager);
            if (ConfigManager.FindAllComponentsOfType<IFootballSensor>().Any())
            {
                IEnumerable<FootballTeam> teams = ConfigManager.FindAllComponentsOfType<IFootballSensor>().SelectMany(s => s.Teams).ToList();
                if (teams.Any())
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(ConfigManager.GetPathForFile("FootballGrammar.grxml", GetType()));

                    XmlElement oneof = GrammarUtility.CreateElement(doc, "one-of", new Dictionary<String, String>());
                    foreach (FootballTeam possibleVal in teams)
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
                        {"id", "FootballTeams"},
                        {"scope", "public"},
                    });
                    rule.AppendChild(oneof);
                    doc.LastChild.AppendChild(rule);
                    Provider = new GrammarProvider(new XmlGrammar(doc));
                }
                else
                {
                    _foundTeams = false;
                }
            }
        }
    }
}
