using System.Collections.Generic;

namespace SAMI.IOInterfaces.Interfaces.XBee
{
    internal abstract class XbeeSerialTxPacket
    {
        public byte FrameId
        {
            get;
            set;
        }

        protected abstract List<byte> FrameData
        {
            get;
        }

        public List<byte> GetPacketData()
        {
            List<byte> packetData = new List<byte>(FrameData);
            // Add checksum
            packetData.Add(CalculateChecksum(packetData));
            // Add frame size
            int frameSize = FrameData.Count;
            byte frameSizeMsb = (byte)((frameSize >> 8) & 0xFF);
            byte frameSizeLsb = (byte)(frameSize & 0xFF);
            packetData.Insert(0, frameSizeMsb);
            packetData.Insert(1, frameSizeLsb);
            
            return packetData;
        }

        public static byte CalculateChecksum(List<byte> data)
        {
            byte sum = 0;
            foreach (byte b in data)
            {
                sum += b;
            }
            return (byte) (0xFF - sum);
        }
    }
}
