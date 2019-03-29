using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using SAMI.IOInterfaces.Interfaces.LightSwitch;
using SAMI.IOInterfaces.Interfaces.ZWave;
using SAMI.Persistence;

namespace ZWave.Switch
{
    [ParseableElement("ZWaveDimmableSwitch", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class ZWaveDimmableSwitch : ZWaveNode, IDimmableLightSwitchController
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

        public ZWaveDimmableSwitch()
            : base()
        {
        }

        public void TurnOn()
        {
            SetLightLevel(1);
        }

        public void TurnOff()
        {
            SetLightLevel(0);
        }

        public void SetLightLevel(double level)
        {
            SetValue(0, 0x26, (byte)(level * 255));
        }
    }
}
