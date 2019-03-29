using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Bartender;
using SAMI.IOInterfaces.Interfaces.XBee;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Bartender
{
    [ParseableElement("Bartender", ParseableElementType.IOInterface)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class BartenderController : XBeeInterface, IBartenderController
    {
        private String _name;
        public override String Name
        {
            get
            {
                return _name;
            }
        }

        public override IEnumerable<PersistentProperty> Properties
        {
            get
            {
                foreach(PersistentProperty prop in base.Properties)
                {
                    yield return prop;
                }
                yield return new PersistentProperty("Name", () => Name, s => _name = s);
            }
        }

        private IEnumerable<Dispenser> Dispensers
        {
            get
            {
                return Children.OfType<Dispenser>();
            }
        }

        public IEnumerable<String> AvailableLiquids
        {
            get
            {
                return Dispensers.Select(d => d.Key);
            }
        }

        public BartenderController()
            : base()
        {
        }

        public void DispenseLiquid(String pumpKey, double milliLiters)
        {
            Dispenser dispenser = GetDispenser(pumpKey);
            int dispenseTimeMs = (int)(milliLiters * Double.Parse(dispenser.MillilitersPerSecond) * 1000);
            ActivatePump(GetDispenser(pumpKey).DispenserIndex, dispenseTimeMs);
        }

        private void ActivatePump(int pumpIndex, int ms)
        {
            byte[] msArray = IntToByteArray(ms);
            List<byte> command = new List<byte>();
            command.Add((byte)pumpIndex);
            command.AddRange(msArray);
            StartXbeeSession();
            SendXbeeData(command);
            EndXbeeSession();
            Console.WriteLine("Pumping:");
            Console.WriteLine("PumpIndex: " + pumpIndex + " Ms: " + ms);
        }

        private Dispenser GetDispenser(String key)
        {
            foreach (Dispenser dispenser in Dispensers)
            {
                if (dispenser.Key.Equals(key))
                {
                    return dispenser;
                }
            }
            return null;
        }
    }
}
