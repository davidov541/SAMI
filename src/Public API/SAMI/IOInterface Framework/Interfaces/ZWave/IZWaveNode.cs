using System;
using System.Collections.Generic;
using OpenZWaveDotNet;
using SAMI.Persistence;

namespace SAMI.IOInterfaces.Interfaces.ZWave
{
    internal interface IZWaveNode : IIOInterface
    {
        int NodeId
        {
            get;
            set;
        }

        void AddValueId(ZWValueID valueId);

        void RemoveValueId(ZWValueID valueId);

        bool TryStartPairing(IZWavePairingMonitor monitor);

        void ResetController();
    }
}
