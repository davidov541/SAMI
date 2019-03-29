using System;
using OpenZWaveDotNet;

namespace SAMI.IOInterfaces.Interfaces.ZWave
{
    internal class NotificationEventArgs : EventArgs
    {
        public IZWaveManager Manager
        {
            get;
            private set;
        }

        public byte NodeId
        {
            get;
            private set;
        }

        public ZWNotification.Type NotificationType
        {
            get;
            private set;
        }

        public ZWValueID ValueId
        {
            get;
            private set;
        }

        public UInt32 HomeId
        {
            get;
            private set;
        }

        public NotificationEventArgs(IZWaveManager manager, ZWNotification notification)
        {
            Manager = manager;
            NodeId = notification.GetNodeId();
            NotificationType = notification.GetType();
            ValueId = notification.GetValueID();
            HomeId = notification.GetHomeId();
        }

        public NotificationEventArgs(IZWaveManager manager, byte nodeId, ZWNotification.Type notificationType, ZWValueID valueId, UInt32 homeId)
        {
            Manager = manager;
            NodeId = nodeId;
            NotificationType = notificationType;
            ValueId = valueId;
            HomeId = homeId;
        }
    }
}
