using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Remote;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.Persistence;

namespace SAMI.Apps.Remote
{
    [ParseableElement("Remote", ParseableElementType.App)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    internal class RemoteApp : VoiceActivatedApp<RemoteConversation>
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
                return References.Any();
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
                    return "No remote controls are connected to Sammie. Please make sure that your remote is plugged into Sammie.";
                }
            }
        }

        public RemoteApp()
            : base()
        {
        }

        public override void Initialize(IConfigurationManager configManager)
        {
            base.Initialize(configManager);
            if (ConfigManager.FindAllComponentsOfType<ITVRemote>().Any())
            {
                XmlGrammar grammar = GrammarUtility.CreateGrammarFromList(ConfigManager.GetPathForFile("RemoteGrammar.grxml", GetType()), "ChannelName", ConfigManager.FindAllComponentsOfType<ITVRemote>().SelectMany(r => r.GetChannels()).ToList());
                Provider = new GrammarProvider(grammar);
            }
        }

        public override bool TryCreateConversationFromPhrase(Dialog phrase, out Conversation createdConversation)
        {
            createdConversation = new RemoteConversation(ConfigManager, References);
            bool tryResult = createdConversation.TryAddDialog(phrase);
            if (!tryResult)
            {
                createdConversation.Dispose();
            }
            return tryResult;
        }

        public override void AddChild(IParseable component)
        {
            if (component is IOInterfaceReference)
            {
                base.AddChild(component);
            }
            else if (component is VolumeRemoteReference)
            {
                base.AddChild(new IOInterfaceReference("Volume", (component as RemoteReference).RemoteName, ConfigManager));
            }
            else if (component is ChannelRemoteReference)
            {
                base.AddChild(new IOInterfaceReference("Channel", (component as RemoteReference).RemoteName, ConfigManager));
            }
            else if (component is PowerRemoteReference)
            {
                base.AddChild(new IOInterfaceReference("Power", (component as RemoteReference).RemoteName, ConfigManager));
            }
            else if (component is RemoteReference)
            {
                base.AddChild(new IOInterfaceReference(String.Empty, (component as RemoteReference).RemoteName, ConfigManager));
            }
        }
    }
}
