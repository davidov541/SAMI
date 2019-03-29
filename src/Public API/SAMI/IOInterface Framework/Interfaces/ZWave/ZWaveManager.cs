using System;
using OpenZWaveDotNet;

namespace SAMI.IOInterfaces.Interfaces.ZWave
{
    internal class ZWaveManager : IZWaveManager
    {
        public event EventHandler<NotificationEventArgs> OnNotification;

        public event EventHandler<ControllerStateChangedEventArgs> OnControllerStateChanged;

        private ZWManager _manager;
        public ZWaveManager()
        {
            _manager = new ZWManager();
            _manager.OnNotification = new ManagedNotificationsHandler(notification => NotificationHandler(notification, this));
        }

        private static void NotificationHandler(ZWNotification notification, ZWaveManager facade)
        {
            if(facade.OnNotification != null)
            {
                facade.OnNotification(facade, new NotificationEventArgs(facade, notification));
            }
        }

        private static void ControllerStateChangedHandler(ZWControllerState state, ZWaveManager facade)
        {
            if (facade.OnNotification != null)
            {
                facade.OnControllerStateChanged(facade, new ControllerStateChangedEventArgs(facade, state));
            }
        }

        public void Create()
        {
            _manager.Create();
        }

        public void AddDriver(string comPath, ZWControllerInterface controllerInterface)
        {
            _manager.AddDriver(comPath, controllerInterface);
        }

        public void SetValue(ZWValueID valueId, bool value)
        {
            _manager.SetValue(valueId, value);
        }

        public void SetValue(ZWValueID valueId, byte value)
        {
            _manager.SetValue(valueId, value);
        }
        
        public bool BeginControllerCommand(uint homeId, ZWControllerCommand command, bool highPower, byte nodeId)
        {
            return _manager.BeginControllerCommand(homeId, command, highPower, nodeId);
        }

        public void ResetController(UInt32 homeId)
        {
            _manager.ResetController(homeId);
        }
    }
}
