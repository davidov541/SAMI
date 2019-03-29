using System;
using OpenZWaveDotNet;

namespace SAMI.IOInterfaces.Interfaces.ZWave
{
    internal interface IZWaveManager
    {
        event EventHandler<NotificationEventArgs> OnNotification;
        event EventHandler<ControllerStateChangedEventArgs> OnControllerStateChanged;

        void Create();

        void AddDriver(String comPath, ZWControllerInterface controllerInterface);
        void SetValue(ZWValueID valueId, bool value);
        void SetValue(ZWValueID valueId, byte value);
        bool BeginControllerCommand(UInt32 homeId, ZWControllerCommand command, bool highPower, byte nodeId);
        void ResetController(UInt32 homeId);
    }
}
