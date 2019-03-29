using System;

namespace SAMI.IOInterfaces.Interfaces.ZWave
{
    internal interface IZWaveOptions
    {
        void Create(String configDirectory, String userDirectory, String commandLineParams);

        void Lock();
    }
}
