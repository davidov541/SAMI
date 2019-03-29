using OpenZWaveDotNet;

namespace SAMI.IOInterfaces.Interfaces.ZWave
{
    internal class ZWaveOptions : IZWaveOptions
    {
        private ZWOptions _options;

        public ZWaveOptions()
        {
            _options = new ZWOptions();
        }

        public void Create(string configDirectory, string userDirectory, string commandLineParams)
        {
            _options.Create(configDirectory, userDirectory, commandLineParams);
        }

        public void Lock()
        {
            _options.Lock();
        }
    }
}
