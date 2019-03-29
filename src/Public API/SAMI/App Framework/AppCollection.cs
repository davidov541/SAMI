using System;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Voice;

namespace SAMI.Apps
{
    internal class AppCollection
    {
        private IConfigurationManager _configManager;

        public AppCollection(IConfigurationManager configManager)
        {
            _configManager = configManager;
        }

        public void Init(EventHandler<AsyncAlertEventArgs> asyncAlertHandler)
        {
            foreach (BaseSAMIApp app in _configManager.FindAllComponentsOfType<BaseSAMIApp>())
            {
                app.AsyncAlertRaised += asyncAlertHandler;
            }
        }

        public bool TryGetConversation(Dialog inputPhrase, out Conversation convo)
        {
            convo = null;
            foreach (IApp app in _configManager.FindAllComponentsOfType<IApp>())
            {
                if (app.TryCreateConversationFromPhrase(inputPhrase, out convo))
                {
                    if (app.IsValid)
                    {
                        return true;
                    }
                    else
                    {
                        convo = new InformationalConversation(_configManager, app.InvalidMessage);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
