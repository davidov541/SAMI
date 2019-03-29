using System;
using System.IO;
using System.Runtime.InteropServices;
using SAMI.Configuration;

namespace SAMI.Application.Shell
{
    internal class Shell
    {
        private static ShellSAMI _sami;
        private static ConfigurationManager _configManager;
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        // A delegate type to be used as the handler routine 
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        // An enumerated type for the control messages
        // sent to the handler routine.
        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            Cleanup();
            return true;
        }

        public static void Main(String[] args)
        {
            bool isParsingDirectory = false;
            bool isParsingConfigFile = false;
            String configurationFile = "ConsoleConfiguration.xml";
            foreach (String arg in args)
            {
                if (isParsingDirectory)
                {
                    ConfigurationManager.AddPluginDirectory(arg);
                    isParsingDirectory = false;
                }
                else if (isParsingConfigFile)
                {
                    configurationFile = Path.GetFullPath(arg);
                }
                else if (arg.Equals("-PluginDirectory"))
                {
                    isParsingDirectory = true;
                }
                else if (arg.Equals("-ConfigurationFile"))
                {
                    isParsingConfigFile = true;
                }
            }

#if !DEBUG
            Directory.SetCurrentDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SAMI"));
#endif
            _configManager = new ConfigurationManager(configurationFile);
            _sami = new ShellSAMI(_configManager);
            SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);
            _sami.Run(false);
            Cleanup();
        }

        private static void Cleanup()
        {
            if (_sami != null && _configManager != null)
            {
                _sami.Dispose();
                _configManager.Dispose();
                _sami = null;
                _configManager = null;
            }
        }
    }
}
