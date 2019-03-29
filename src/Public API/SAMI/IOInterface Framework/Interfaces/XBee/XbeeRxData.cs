using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAMI.IOInterfaces.Interfaces.XBee
{
    internal class XbeeRxData : XbeeSerialRxPacket
    {

        private UInt64 _sourceAddress = 0;
        private byte _signalStrength;
        private byte _options;
        private List<byte> _data;
        
        public XbeeRxData(List<byte> frameData)
        {

            for (int i = 1; i <= 8; i++)
            {
                _sourceAddress = (_sourceAddress << 8) | frameData[i];
            }
            _signalStrength = frameData[9];
            _options = frameData[10];
            _data = frameData.GetRange(11, frameData.Count - 11);

        }

        public UInt64 GetSourceAddress()
        {
            return _sourceAddress;
        }

        public List<byte> GetData()
        {
            return _data;
        }

    }
}

