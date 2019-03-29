using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using SAMI.IOInterfaces.Interfaces.LightSwitch;
using SAMI.IOInterfaces.Interfaces.XBee;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.LightSwitch
{
    [ParseableElement("LightSwitch", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class LightSwitchController : XBeeInterface, ILightSwitchController
    {
        private String _name;
        private String _room;

        public override String Name
        {
            get
            {
                return _name;
            }
        }

        public SwitchType Type
        {
            get;
            private set;
        }

        public byte ServoId
        {
            get;
            private set;
        }

        public override IEnumerable<PersistentProperty> Properties
        {
            get
            {
                foreach (PersistentProperty prop in base.Properties)
                {
                    yield return prop;
                }
                yield return new PersistentProperty("Name", () => Name, n => _name = n);
                yield return new PersistentProperty("Type", () => Type.ToString(), t => Type = (SwitchType)Enum.Parse(typeof(SwitchType), t));
                yield return new PersistentProperty("ServoId", () => ServoId.ToString(), id => ServoId = Byte.Parse(id));
                yield return new PersistentProperty("Room", () => _room, r => _room = r);
            }
        }

        public LightSwitchController()
            : base()
        {
        }

        public void TurnOn()
        {
            List<byte> cmdList = new List<byte>();
            cmdList.Add(0x06);
            cmdList.Add(ServoId);
            StartXbeeSession();
            SendXbeeData(cmdList);
            EndXbeeSession();
        }

        public void TurnOff()
        {
            List<byte> cmdList = new List<byte>();
            cmdList.Add(0x07);
            cmdList.Add(ServoId);
            StartXbeeSession();
            SendXbeeData(cmdList);
            EndXbeeSession();
        }
    }
}
