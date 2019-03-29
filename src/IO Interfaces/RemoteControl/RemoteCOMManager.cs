using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using SAMI.Configuration;

namespace SAMI.IOInterfaces.Remote
{
    internal class RemoteCOMManager : IRemoteCOMManager
    {
        private SerialPort _comPort = null;
        private String _comPortName;
        private bool _isValid = false;
        private IConfigurationManager _configManager;
        private static Dictionary<String, RemoteCOMManager> _managers = new Dictionary<String, RemoteCOMManager>();
        public static IRemoteCOMManager GetInstance(IConfigurationManager manager, String comPort)
        {
            if (!_managers.ContainsKey(comPort))
            {
                _managers[comPort] = new RemoteCOMManager(manager, comPort);
            }
            _managers[comPort].AddReference();
            return _managers[comPort];
        }

        public int RefNumber
        {
            get;
            private set;
        }

        public bool IsValid
        {
            get
            {
                if (!_isValid)
                {
                    InitializeCOMPort();
                }
                return _isValid;
            }
        }

        public RemoteCOMManager(IConfigurationManager configManager, String comPort)
        {
            _comPortName = comPort;
            RefNumber = 0;
            _configManager = configManager;
        }

        private void AddReference()
        {
            RefNumber++;
            if (_comPort == null)
            {
                InitializeCOMPort();
            }
        }

        public void Dispose()
        {
            RefNumber--;
            if (RefNumber == 0)
            {
                _comPort.Dispose();
                _comPort = null;
            }
        }

        private void InitializeCOMPort()
        {
            try
            {
                _comPort = new SerialPort(_comPortName, 9600, Parity.None);
                _comPort.Open();
                _comPort.Write(new Byte[] { 0x20 }, 0, 1);
                _isValid = _comPort.ReadByte() == 0xBB;
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        public bool TrySendCode(byte[] codes)
        {
            try
            {
                _comPort.Write(new Byte[] { 0x30 }, 0, 1);
                _comPort.Write(codes, 0, 10);
                _comPort.ReadByte();
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("WARNING - Command could not be completed because the appropriate hardware does not seem to be connected. If unexpected, please ensure all hardware is properly plugged in.");
                return false;
            }
            return true;
        }
    }
}
