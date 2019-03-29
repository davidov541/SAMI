using System;
using System.Collections.Generic;

namespace SAMI.IOInterfaces.Interfaces.XBee
{
    internal class XbeeAtQueryCmd : XbeeSerialTxPacket
    {
        private String _apiCmd;

        protected override List<byte> FrameData
        {
            get
            {
                if (_apiCmd.Length != 2)
                {
                    // This should through an exception or something
                    Console.WriteLine("API Command was not 2 characters long!");
                    return new List<byte>();
                }

                List<byte> frameData = new List<byte>();
                frameData.Add(0x08); // API Identifier
                frameData.Add(FrameId); // Frame ID

                // API Command
                frameData.Add((byte)(_apiCmd[0]));
                frameData.Add((byte)(_apiCmd[1]));
                return frameData;
            }
        }

        public XbeeAtQueryCmd(String apiCmd)
        {
            _apiCmd = apiCmd;
        }
    }
}
