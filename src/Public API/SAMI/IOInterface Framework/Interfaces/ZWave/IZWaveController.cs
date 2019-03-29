using OpenZWaveDotNet;

namespace SAMI.IOInterfaces.Interfaces.ZWave
{
    internal interface IZWaveController
    {
        bool IsInitialized
        {
            get;
        }

        void SetValue(ZWValueID valueId, bool value);

        void SetValue(ZWValueID valueId, byte value);

        bool TryStartPairing(ZWaveNode node, IZWavePairingMonitor monitor);

        void ResetController();
    }
}
