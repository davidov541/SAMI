using System;
using System.Collections.Generic;
using System.Linq;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.LightSwitch;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.Apps.Dimmer
{
    [ParseableElement("Dimmer", ParseableElementType.App)]
    public class DimmerApp : VoiceActivatedApp<DimmerConversation>
    {
        public override bool IsValid
        {
            get
            {
                return base.IsValid && ConfigManager != null && ConfigManager.FindAllComponentsOfType<IDimmableLightSwitchController>().Any();
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

        public override void Initialize(IConfigurationManager configManager)
        {
            base.Initialize(configManager);
            if (ConfigManager.FindAllComponentsOfType<IDimmableLightSwitchController>().Any())
            {
                IEnumerable<String> locations = ConfigManager.FindAllComponentsOfType<IDimmableLightSwitchController>().Select(s => s.Name);
                XmlGrammar grammar = GrammarUtility.CreateGrammarFromList(ConfigManager.GetPathForFile("DimmerGrammar.grxml", GetType()), "DimmableSwitchName", locations.ToList());
                Provider = new GrammarProvider(grammar);
            }
        }
    }
}
