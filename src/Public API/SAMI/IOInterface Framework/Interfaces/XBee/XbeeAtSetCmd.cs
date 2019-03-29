﻿using System;
using System.Collections.Generic;

namespace SAMI.IOInterfaces.Interfaces.XBee
{
    internal class XbeeAtSetCmd : XbeeSerialTxPacket
    {
        private String _apiCmd;
        private UInt32 _setValue;
        private int _bytes;

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

                // Set value
                UInt32 setValue = _setValue;
                for (int i = 4; i > 0; i--)
                {
                    if (i <= _bytes)
                    {
                        frameData.Add((byte)((setValue >> 24) & 0xFF));
                    }
                    setValue <<= 8;
                }
                return frameData;
            }
        }

        public XbeeAtSetCmd(String apiCmd, UInt32 setValue, int bytes)
        {
            _apiCmd = apiCmd;
            _setValue = setValue;
            _bytes = bytes;
        }
    }
}
