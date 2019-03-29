using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.LightSwitch;
using SAMI.IOInterfaces.Interfaces.ZWave;
using SAMI.Persistence;

namespace ZWave.Switch
{
    [ParseableElement("ZWaveSwitch", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class ZWaveSwitch : ZWaveNode, ILightSwitchController
    {
        public SwitchType Type
        {
            get 
            {
                return SwitchType.Misc;
            }
        }

        private String _name;
        public override String Name
        {
            get
            {
                return _name;
            }
        }

        public IEnumerable<PersistentProperty> Properties
        {
            get 
            {
                foreach (PersistentProperty prop in base.Properties)
                {
                    yield return prop;
                }
                yield return new PersistentProperty("Name", () => Name, n => _name = n);
            }
        }

        public ZWaveSwitch()
            : base()
        {
        }

        public void TurnOn()
        {
            SetValue(0, 0x25, true);
        }

        public void TurnOff()
        {
            SetValue(0, 0x25, false);
        }
    }
}
