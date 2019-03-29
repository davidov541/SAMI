using System.Collections.Generic;

namespace SAMI.IOInterfaces.Interfaces.XBee
{
    internal class XbeeTxDataResponse : XbeeSerialRxPacket
    {
        private int _status;
        
        public XbeeTxDataResponse(List<byte> frameData)
        {
            FrameId = frameData[1];
            _status = frameData[2];
        }

        public int GetStatus()
        {
            return _status;
        }
    }
}

