using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Remote;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.Apps.Volume
{
    [ParseableElement("Volume", ParseableElementType.App)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class VolumeApp : VoiceActivatedApp<VolumeConversation>
    {
        internal static String VolumeGrammarName = "Adjustment";

        public override bool IsValid
        {
            get
            {
                return ConfigManager != null && ConfigManager.FindAllComponentsOfType<IVolumeController>().Any();
            }
        }

        public VolumeApp()
            : base()
        {
        }

        public override void Initialize(IConfigurationManager configManager)
        {
            base.Initialize(configManager);
            if (!Children.OfType<IOInterfaceReference>().Any())
            {
                foreach (IVolumeController controller in ConfigManager.FindAllComponentsOfType<IVolumeController>())
                {
                    AddChild(new IOInterfaceReference("", controller.Name, ConfigManager));
                }
            }
            IEnumerable<String> references = Children.OfType<IOInterfaceReference>().Select(s => s.Name);
            XmlGrammar grammar = GrammarUtility.CreateGrammarFromList(ConfigManager.GetPathForFile("VolumeGrammar.grxml", GetType()), "Source", references.ToList());
            Provider = new GrammarProvider(grammar, new XmlGrammar(ConfigManager, VolumeGrammarName, "AdjustmentGrammar.grxml", GetType()));
        }

        public override bool TryCreateConversationFromPhrase(Dialog phrase, out Conversation createdConversation)
        {
            createdConversation = new VolumeConversation(ConfigManager, Children.OfType<IOInterfaceReference>());
            bool tryResult = createdConversation.TryAddDialog(phrase);
            if (!tryResult)
            {
                createdConversation.Dispose();
            }
            return tryResult;
        }
    }
}
