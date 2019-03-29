using System.Collections.Generic;
using System.Linq;
using OpenZWaveDotNet;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.ZWave;

namespace SAMI.Test.ZWave
{
    internal class MockZWaveNode : ZWaveNode
    {
        public override string Name
        {
            get
            {
                return "Testable Z-Wave Node";
            }
        }

        public IEnumerable<ZWValueID> MockValueIds
        {
            get
            {
                return this.ValueIds;
            }
        }

        public MockZWaveNode()
            : base()
        {
        }

        public void MockSetValue(byte index, byte commandClassId, bool value)
        {
            SetValue(index, commandClassId, value);
        }

        public void MockSetValue(byte index, byte commandClassId, byte value)
        {
            SetValue(index, commandClassId, value);
        }
    }
}
