
namespace SAMI.IOInterfaces.Interfaces.ZWave
{
    internal interface IZWavePairingMonitor
    {
        void PairingStarted();

        void DeviceFound();

        void PairingCompleted();
    }
}
