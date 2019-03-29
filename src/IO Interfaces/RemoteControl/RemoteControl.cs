using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Power;
using SAMI.IOInterfaces.Interfaces.Remote;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Remote
{
    [ParseableElement("IRRemoteControl", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class RemoteControl : ITVInfoRemote, IVolumeController, IDVRRemote, IPowerController
    {
        protected IRemoteCOMManager _manager;
        private IConfigurationManager _configManager;

        private readonly String[] _numericButtons = {
					"zero",
					"one",
					"two",
					"three",
					"four",
					"five",
					"six",
					"seven",
					"eight",
					"nine",
                    "period"};

        private List<Channel> _channels = new List<Channel>();
        private List<RemoteCode> _codes = new List<RemoteCode>();

        public String Name
        {
            get;
            private set;
        }

        public String COMPort
        {
            get;
            private set;
        }

        public RemoteControl()
        {
        }

        #region IVolumeController Members
        private double _volume;
        public double Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                double change = value - _volume;
                int numberOfKeys = (int)Math.Floor(Math.Abs(change) / 0.01);
                if (numberOfKeys == 0)
                {
                    // Amount = 0 when mute, so we want to send this once.
                    numberOfKeys = 1;
                }
                SendCode(ConvertDirectionToCommand(change), numberOfKeys);
                _volume = value;
            }
        }
        #endregion

        #region IParseable Members
        public IEnumerable<PersistentProperty> Properties
        {
            get
            {
                yield return new PersistentProperty("Name", () => Name, set => Name = set);
                yield return new PersistentProperty("COMPort", () => COMPort, set => COMPort = set);
            }
        }

        public IEnumerable<IParseable> Children
        {
            get
            {
                return _channels;
            }
        }

        public virtual bool IsValid
        {
            get
            {
                return _manager != null && _manager.IsValid;
            }
        }

        public virtual void Initialize(IConfigurationManager manager)
        {
            _configManager = manager;
            _manager = RemoteCOMManager.GetInstance(_configManager, COMPort);
        }

        public void Dispose()
        {
            _manager.Dispose();
        }

        public void AddChild(IParseable component)
        {
            if (component is Channel)
            {
                _channels.Add(component as Channel);
            }
            else if (component is RemoteCode)
            {
                _codes.Add(component as RemoteCode);
            }
        }
        #endregion

        #region ITVRemote Members
        public IEnumerable<String> GetChannels()
        {
            return _channels.Select(c => c.Name);
        }

        public bool TrySendChannel(String channelName)
        {
            if (_channels.Any(channel => channel.Name.Equals(channelName)))
            {
                SendChannel(_channels.Single(channel => channel.Name.Equals(channelName)).Number.ToString());
                return true;
            }
            return false;
        }

        public void SendChannel(String number)
        {
            bool successful = true;
            for (int i = 0; i < number.Length; i++)
            {
                byte[] code;
                if (number.ToCharArray()[i] == '.')
                {
                    code = _codes.Single(c => c.Name.Equals(_numericButtons[10])).Code;
                }
                else
                {
                    code = _codes.Single(c => c.Name.Equals(_numericButtons[Int32.Parse(number.ToCharArray()[i].ToString())])).Code;
                }
                successful &= _manager.TrySendCode(code);
            }
            if (!successful)
            {
                Console.WriteLine("Outputting code for channel {0}.", number);
            }
        }
        #endregion

        #region IDVRRemote Members
        public void Pause()
        {
            SendCode("pause", 1);
        }

        public void Play()
        {
            SendCode("play", 1);
        }

        public void FastForward()
        {
            SendCode("ff", 1);
        }

        public void Rewind()
        {
            SendCode("rew", 1);
        }

        public void Record()
        {
            SendCode("record", 1);
        }

        public void Stop()
        {
            SendCode("stop", 1);
        }
        #endregion

        #region ITVInfoRemote Members
        public void ToggleInfo()
        {
            SendCode("info", 1);
        }
        #endregion

        #region Helper Methods
        private void SendCode(String codeName, int repeat = 1)
        {
            Console.WriteLine("Trying to send " + codeName);
            bool success = true;
            if (_codes.Any(c => c.Name.Equals(codeName)))
            {
                for (int i = 0; i < repeat; i++)
                {
                    success &= _manager.TrySendCode(_codes.Single(c => c.Name.Equals(codeName)).Code);
                }

                if (!success)
                {
                    Console.WriteLine("Outputting code for {0} {1} times.", codeName, repeat);
                }
            }
        }

        private String ConvertDirectionToCommand(double amount)
        {
            if (amount > 0)
            {
                return "volumeup";
            }
            else if (amount < 0)
            {
                return "volumedown";
            }
            else
            {
                return "mute";
            }
        }
        #endregion

        #region IPowerController

        public void TurnOn()
        {
            SendCode("power", 1);
        }

        public void TurnOff()
        {
            SendCode("power", 1);
        }
        #endregion
    }
}
