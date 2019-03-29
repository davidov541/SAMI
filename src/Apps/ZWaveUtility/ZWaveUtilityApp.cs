using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.IOInterfaces.Interfaces.ZWave;
using SAMI.Persistence;

namespace SAMI.Apps.ZWaveUtility
{
    [ParseableElement("ZWaveUtility", ParseableElementType.App)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ZWaveUtilityApp : VoiceActivatedApp, IZWavePairingMonitor
    {
        private String _currentlyPairingNode;

        public override bool IsValid
        {
            get
            {
                return ConfigManager != null && (ConfigManager as IInternalConfigurationManager).FindAllComponentsOfTypeEvenInvalid<IZWaveNode>().Any();
            }
        }

        public override string InvalidMessage
        {
            get
            {
                return "No devices are available for pairing.";
            }
        }

        public ZWaveUtilityApp()
            : base()
        {
        }

        public override void Initialize(IConfigurationManager configManager)
        {
            Provider = new GrammarProvider(GetGrammar, new TimeSpan(24, 0, 0));
            base.Initialize(configManager);
        }

        private XmlGrammar GetGrammar()
        {
            if (IsValid)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(ConfigManager.GetPathForFile("ZWaveUtilityGrammar.grxml", GetType()));
                IEnumerable<String> locations = (ConfigManager as IInternalConfigurationManager).FindAllComponentsOfTypeEvenInvalid<IZWaveNode>().Select(s => s.Name);
                XmlElement oneof = GrammarUtility.CreateListOfPossibleStrings(doc, locations);
                XmlElement rule = GrammarUtility.CreateElement(doc, "rule", new Dictionary<string, string>
                {
                    {"id", "NodeName"},
                    {"scope", "public"},
                });
                rule.AppendChild(oneof);
                doc.LastChild.AppendChild(rule);
                return new XmlGrammar(doc);
            }
            else
            {
                return null;
            }
        }

        public override bool TryCreateConversationFromPhrase(Dialog phrase, out Conversation createdConversation)
        {
            createdConversation = new InformationalConversation(ConfigManager, "O K");
            if (phrase.CheckForApp("ZWaveUtility"))
            {
                String nodeName = phrase.GetPropertyValue("NodeName");
                if (String.IsNullOrEmpty(nodeName))
                {
                    (ConfigManager as IInternalConfigurationManager).FindAllComponentsOfTypeEvenInvalid<IZWaveNode>().First().ResetController();
                    return true;
                }
                IZWaveNode node = (ConfigManager as IInternalConfigurationManager).FindAllComponentsOfTypeEvenInvalid<IZWaveNode>().FirstOrDefault(s => s.Name.Equals(nodeName));
                if (node == null)
                {
                    return false;
                }
                else
                {
                    createdConversation = new InformationalConversation(ConfigManager, String.Format("Press the button on the {0} to pair it.", nodeName));
                    _currentlyPairingNode = nodeName;
                    node.TryStartPairing(this);
                }
                return true;
            }
            return false;
        }

        public void PairingStarted()
        {
        }

        public void DeviceFound()
        {
            RaiseAsyncAlert(new InformationalConversation(ConfigManager, String.Format("Now connecting to the {0}. Please wait until we are done connecting to the {0}.", _currentlyPairingNode)));
        }

        public void PairingCompleted()
        {
            RaiseAsyncAlert(new InformationalConversation(ConfigManager, String.Format("{0} has been successfully paired!", _currentlyPairingNode)));
            _currentlyPairingNode = null;
        }
    }
}
