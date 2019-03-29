using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Power;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.Apps.Power
{
    [ParseableElement("Power", ParseableElementType.App)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class PowerApp : VoiceActivatedApp<PowerConversation>
    {
        public override bool IsValid
        {
            get
            {
                return ConfigManager != null && ConfigManager.FindAllComponentsOfType<IPowerController>().Any();
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
                    return "No valid modules are currently connected to Sammie.";
                }
            }
        }
        public PowerApp()
            : base()
        {
        }

        public override void Initialize(IConfigurationManager configManager)
        {
            base.Initialize(configManager);
            if (ConfigManager.FindAllComponentsOfType<IPowerController>().Any())
            {
                IEnumerable<String> locations = ConfigManager.FindAllComponentsOfType<IPowerController>().Select(s => s.Name);
                XmlGrammar grammar = GrammarUtility.CreateGrammarFromList(ConfigManager.GetPathForFile("PowerGrammar.grxml", GetType()), "SwitchName", locations.ToList());
                Provider = new GrammarProvider(grammar);
            }
        }
    }
}
