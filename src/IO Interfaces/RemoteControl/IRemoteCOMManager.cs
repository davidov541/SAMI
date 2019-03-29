using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;

namespace SAMI.IOInterfaces.Remote
{
    internal interface IRemoteCOMManager : IDisposable
    {
        int RefNumber
        {
            get;
        }

        bool IsValid
        {
            get;
        }

        bool TrySendCode(byte[] codes);
    }
}
