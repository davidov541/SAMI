using System;
using System.Collections.Generic;
using System.Threading;
using SAMI.Configuration;

namespace SAMI.IOInterfaces.Interfaces.XBee
{
    internal class XBee : IXBee
    {
        private const byte StartingByte = 0x7E;
        private const byte EscapeByte = 0x7D;

        private ComManager _comManager;
        private Semaphore _xbeeLock;
        private bool _useEscapeCharacters;
        private byte _currentFrameId;

        public bool IsInitialized
        {
            get
            {
                return _comManager.IsConnected;
            }
        }

        internal XBee(IInternalConfigurationManager configManager, bool useEscapeCharacters)
        {
            _comManager = new ComManager(configManager.XBeeCOM, 9600);
            _useEscapeCharacters = useEscapeCharacters;
            _currentFrameId = 0x01;
            _xbeeLock = new Semaphore(1, 1);
            InitializeSerial();
        }

        private void InitializeSerial()
        {
            if (!_comManager.IsConnected)
            {
                _comManager.Initialize();
            }

            if (_comManager.IsConnected)
            {
                SendPacket(new XbeeAtQueryCmd("AP"));
                XbeeAtCmdResponse response = (XbeeAtCmdResponse)ReceivePacket();
                if (response.Response != 2)
                {
                    Console.WriteLine("XBee is not set to API mode 2");
                }
            }
        }

        public void Dispose()
        {
            _comManager.Dispose();
        }

        public void StartXbeeSession()
        {
            _xbeeLock.WaitOne();
            InitializeSerial();
        }

        public void EndXbeeSession()
        {
            _xbeeLock.Release(1);
        }

        private void SendPacket(XbeeSerialTxPacket packet)
        {
            packet.FrameId = _currentFrameId;
            List<byte> packetData = packet.GetPacketData();
            if (_useEscapeCharacters)
            {
                packetData = EscapeData(packetData);
            }
            packetData.Insert(0, StartingByte); // Add starting byte

            _comManager.Write(packetData);

            IncrementCurrentFrameId();
        }

        private XbeeSerialRxPacket ReceivePacket()
        {
            List<byte> packet = new List<byte>();

            // Make sure first byte is the start delimeter
            if (_comManager.ReadByte() != StartingByte)
            {
                return null;
            }

            // Get the count of bits
            int countMsb = ReadEscapedByte() & 0xFF;
            int countLsb = ReadEscapedByte() & 0xFF;
            int count = (countMsb << 8) | countLsb;

            for (int i = 0; i < count; i++)
            {
                packet.Add(ReadEscapedByte());
            }

            // Read checksum
            packet.Add(ReadEscapedByte());

            // Verify checksum
            if (!XbeeSerialRxPacket.VerifyChecksum(packet))
            {
                // The checksum failed.
                return null;
            }

            byte apiCmd = packet[0];
            List<byte> frame = packet.GetRange(0, count);

            switch (apiCmd)
            {
                case 0x88:
                    // AT Command Response
                    return new XbeeAtCmdResponse(frame);
                case 0x89:
                    // TX Response
                    return new XbeeTxDataResponse(frame);
                case 0x80:
                    // RX Data
                    return new XbeeRxData(frame);
                default:
                    break;
            }

            return null;
        }

        private List<byte> EscapeData(List<byte> data)
        {
            List<byte> escapedData = new List<byte>();
            foreach (byte b in data)
            {
                if (b == StartingByte || b == EscapeByte || b == 0x11 || b == 0x13)
                {
                    escapedData.Add(EscapeByte);
                    byte xoredByte = (byte)(b ^ 0x20);
                    escapedData.Add(xoredByte);
                }
                else
                {
                    escapedData.Add(b);
                }
            }
            return escapedData;
        }

        private byte ReadEscapedByte()
        {
            byte b = _comManager.ReadByte();
            if (b == EscapeByte && _useEscapeCharacters)
            {
                b = (byte)(_comManager.ReadByte() ^ 0x20);
            }
            return b;
        }

        private void IncrementCurrentFrameId()
        {
            _currentFrameId++;
            if (_currentFrameId == 0x00)
            {
                _currentFrameId = 0x01;
            }
        }

        public void Send(UInt64 xbeeId, List<byte> data)
        {
            SendPacket(new XbeeTxData(xbeeId, data));
            XbeeTxDataResponse response = (XbeeTxDataResponse)(ReceivePacket());
        }

        public List<byte> Receive()
        {
            XbeeRxData response = (XbeeRxData)(ReceivePacket());
            if (response != null)
            {
                return response.GetData();
            }
            else
            {
                return null;
            }
        }
    }
}