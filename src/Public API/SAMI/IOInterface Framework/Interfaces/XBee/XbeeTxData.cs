using System;
using System.Collections.Generic;

namespace SAMI.IOInterfaces.Interfaces.XBee
{
    internal class XbeeTxData : XbeeSerialTxPacket
    {
        private UInt64 _destAddress;
        private List<byte> _data;

        protected override List<byte> FrameData
        {
            get
            {
                List<byte> frameData = new List<byte>();
                frameData.Add(0x00); // API Identifier
                frameData.Add(FrameId); // Frame ID

                // Destination Address
                UInt64 addressTemp = _destAddress;
                for (int i = 0; i < 8; i++)
                {
                    // Send data MSB first
                    byte addrByte = (byte)((addressTemp >> 56) & 0xFF);
                    addressTemp <<= 8;
                    frameData.Add(addrByte);
                }
                frameData.Add(0x00); // Options

                // Data
                foreach (byte b in _data)
                {
                    frameData.Add(b);
                }
                return frameData;
            }
        }

        public XbeeTxData(UInt64 destAddress, List<byte> data)
        {
            _destAddress = destAddress;
            _data = data;
        }
    }
}
