using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Power;
using SAMI.IOInterfaces.Interfaces.Remote;
using SAMI.IOInterfaces.Interfaces.Voice;
using SAMI.IOInterfaces.Remote;

namespace SAMI.Apps.Remote
{
    internal class RemoteConversation : Conversation
    {
        private IEnumerable<IOInterfaceReference> _references;

        protected override String CommandName
        {
            get
            {
                return "remote";
            }
        }

        internal RemoteConversation(IConfigurationManager configManager, IEnumerable<IOInterfaceReference> references)
            : base(configManager, DateTime.Now.AddMinutes(1))
        {
            _references = references;
        }

        public override string Speak()
        {
            base.Speak();
            Dialog phrase = CurrentDialog;
            if (phrase.GetPropertyValue("remoteButton").Equals("powerup"))
            {
                ConversationIsOver = true;
                IEnumerable<String> powerNames = _references.Where(r => r.Tag.Equals("Power")).Select(r => r.Name);
                IEnumerable<IPowerController> remotes = ConfigManager.FindAllComponentsOfType<IPowerController>();
                foreach (IPowerController remote in remotes)
                {
                    if (powerNames.Contains(remote.Name))
                    {
                        Task.Run(() =>
                        {
                            remote.TurnOn();
                            Thread.Sleep(500); // Wait half a second between powering up dvr and sending channel number
                            (remote as ITVRemote).TrySendChannel(phrase.GetPropertyValue("channel"));
                        });
                    }
                }
                return "OK";
            }
            else if (phrase.GetPropertyValue("remoteButton").Equals("pause"))
            {
                ConversationIsOver = true;
                IEnumerable<IDVRRemote> remotes = ConfigManager.FindAllComponentsOfType<IDVRRemote>();
                foreach (IDVRRemote remote in remotes)
                {
                    Task.Run(() => remote.Pause());
                }
                return "OK";
            }
            else if (phrase.GetPropertyValue("remoteButton").Equals("play"))
            {
                ConversationIsOver = true;
                IEnumerable<IDVRRemote> remotes = ConfigManager.FindAllComponentsOfType<IDVRRemote>();
                foreach (IDVRRemote remote in remotes)
                {
                    Task.Run(() => remote.Play());
                }
                return "OK";
            }
            else if (phrase.GetPropertyValue("remoteButton").Equals("record"))
            {
                ConversationIsOver = true;
                IEnumerable<IDVRRemote> remotes = ConfigManager.FindAllComponentsOfType<IDVRRemote>();
                foreach (IDVRRemote remote in remotes)
                {
                    Task.Run(() => remote.Record());
                }
                return "OK";
            }
            else if (phrase.GetPropertyValue("remoteButton").Equals("info"))
            {
                ConversationIsOver = true;
                IEnumerable<ITVInfoRemote> remotes = ConfigManager.FindAllComponentsOfType<ITVInfoRemote>();
                foreach (ITVInfoRemote remote in remotes)
                {
                    Task.Run(() => remote.ToggleInfo());
                }
                return "OK";
            }
            else if (phrase.GetPropertyValue("channel").ToCharArray().All(c => char.IsDigit(c) || c == '.'))
            {
                ConversationIsOver = true;
                IEnumerable<String> channelRemoteNames = _references.Where(r => r.Tag.Equals("Channel")).Select(r => r.Name);
                IEnumerable<ITVRemote> remotes = ConfigManager.FindAllComponentsOfType<ITVRemote>();
                foreach (ITVRemote remote in remotes)
                {
                    if (channelRemoteNames.Contains(remote.Name))
                    {
                        Task.Run(() => remote.SendChannel(phrase.GetPropertyValue("channel")));
                    }
                }
                return "OK";
            }
            else
            {
                ConversationIsOver = true;
                IEnumerable<String> channelRemoteNames = _references.Where(r => r.Tag.Equals("Channel")).Select(r => r.Name);
                IEnumerable<ITVRemote> remotes = ConfigManager.FindAllComponentsOfType<ITVRemote>();
                foreach (ITVRemote remote in remotes)
                {
                    if (channelRemoteNames.Contains(remote.Name))
                    {
                        Task.Run(() => remote.TrySendChannel(phrase.GetPropertyValue("channel")));
                    }
                }
                return "OK";
            }

        }
    }
}
