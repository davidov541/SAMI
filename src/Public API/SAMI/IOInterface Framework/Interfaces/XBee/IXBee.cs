using System;
using System.Collections.Generic;
using System.Threading;

namespace SAMI.IOInterfaces.Interfaces.XBee
{
    internal interface IXBee : IDisposable
    {
        bool IsInitialized
        {
            get;
        }

        void Dispose();

        void StartXbeeSession();

        void EndXbeeSession();

        void Send(UInt64 xbeeId, List<byte> data);

        List<byte> Receive();
    }
}