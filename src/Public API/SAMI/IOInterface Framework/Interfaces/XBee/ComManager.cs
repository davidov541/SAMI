using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace SAMI.IOInterfaces
{
    internal class ComManager : IDisposable
    {
        private String _comName;
        private int _baudRate;
        private SerialPort _comPort;

        public bool IsConnected
        {
            get
            {
                return _comPort != null;
            }
        }

        public ComManager(String comName, int baudRate)
        {
            _comName = comName;
            _baudRate = baudRate;
        }

        public void Initialize()
        {
            if (_comPort == null)
            {
                try
                {
                    _comPort = new SerialPort(_comName, _baudRate);
                    _comPort.Open();
                }
                catch (Exception)
                {
                    _comPort.Close();
                    _comPort = null;
                }
            }
        }

        public void Dispose()
        {
            if (_comPort != null)
            {
                _comPort.Close();
                _comPort = null;
            }
        }

        public void Write(List<byte> data)
        {
            if (_comPort != null)
            {
                try
                {
                    _comPort.Write(data.ToArray(), 0, data.Count);
                }
                catch (Exception)
                {
                    _comPort.Close();
                    _comPort = null;
                }
            }
        }

        public byte ReadByte()
        {
            if (_comPort != null)
            {
                try
                {
                    return (byte)(_comPort.ReadByte());
                }
                catch (Exception)
                {
                    _comPort.Close();
                    _comPort = null;
                }
            }
            // If the com port is unconnected or if an exception
            // occurs, return 0 by default
            return 0;
        }
    }
}


























