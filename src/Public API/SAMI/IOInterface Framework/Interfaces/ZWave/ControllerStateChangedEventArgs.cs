using System;
using OpenZWaveDotNet;

namespace SAMI.IOInterfaces.Interfaces.ZWave
{
    internal class ControllerStateChangedEventArgs : EventArgs
    {
        public IZWaveManager Manager
        {
            get;
            private set;
        }

        public ZWControllerState ControllerState
        {
            get;
            private set;
        }

        public ControllerStateChangedEventArgs(IZWaveManager manager, ZWControllerState controllerState)
        {
            Manager = manager;
            ControllerState = controllerState;
        }
    }
}
