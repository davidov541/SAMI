using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAMI.IOInterfaces.Interfaces.XBee
{
    internal abstract class XbeeSerialRxPacket
    {
        internal enum CommandStatus 
        { 
            OK, 
            Error,
            InvalidCommand, 
            InvalidParameter, 
            NoResponse 
        };

        public static bool VerifyChecksum(List<byte> data)
        {
            byte sum = 0;
            foreach (byte b in data)
            {
                sum += b;
            }
            return sum == 0xFF;
        }

        protected static CommandStatus ConvertCommandStatus(byte b)
        {
            switch (b)
            {
                case 0x00:
                    return CommandStatus.OK;
                case 0x01:
                    return CommandStatus.Error;
                case 0x02:
                    return CommandStatus.InvalidCommand;
                case 0x03:
                    return CommandStatus.InvalidParameter;
                default:
                    return CommandStatus.NoResponse;
            }
        }
        
        public byte FrameId
        {
            get;
            protected set;
        }
    }
}
