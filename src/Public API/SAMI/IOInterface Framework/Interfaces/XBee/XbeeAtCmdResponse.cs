using System;
using System.Collections.Generic;

namespace SAMI.IOInterfaces.Interfaces.XBee
{
    internal class XbeeAtCmdResponse : XbeeSerialRxPacket
    {
        private String _atCommand;
        private CommandStatus _status;

        public UInt32 Response
        {
            get;
            private set;
        }

        public XbeeAtCmdResponse(List<byte> frameData)
        {
            FrameId = frameData[1];
            _atCommand = Convert.ToChar(frameData[2]).ToString() + Convert.ToChar(frameData[3]).ToString();
            _status = ConvertCommandStatus(frameData[4]);
            Response = 0;
            for (int i = 5; i < frameData.Count; i++)
            {
                Response = (Response << 8) | frameData[i];
            }
        }
    }
}
