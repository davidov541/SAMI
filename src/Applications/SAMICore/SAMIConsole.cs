using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Database;
using SAMI.Logging;

namespace SAMI.Application
{
    internal class SAMIConsole
    {
        private static KinectSAMIFacade _sami;
        private static bool _isStopping;
        private const String UpdateLocation = "tmpZip";
        private const int UpdateReturnValue = 5;
        private static String _updateBlobName = String.Empty;
        private static ConfigurationManager _configManager;
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

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

        // A delegate type to be used as the handler routine 
        // for SetConsoleCtrlHandler.
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            Cleanup();
            return true;
        }

        internal static int Main(string[] args)
        {
            bool isParsingDirectory = false;
            bool isParsingConfigFile = false;
            String configurationFile = "KinectConfiguration.xml";
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
                    isParsingConfigFile = false;
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

            bool updating = false;
            Thread updateThread = null;
            ConfigurationManager configManager = new ConfigurationManager(configurationFile);

            try
            {
                // Create thread.
                updateThread = new Thread(() => UpdateThread(configManager));
                _isStopping = false;
                updating = false;

                _sami = new KinectSAMIFacade(configManager);

                // Start the update thread.
                updateThread.Start();

                SetConsoleCtrlHandler(new HandlerRoutine(ConsoleCtrlCheck), true);
                // Run SAMI until either she quits or we need to update.
                _sami.Run(!args.Contains("-NoIntro"));

                // Record whether we are updating or not.
                updating = _sami.CanBeUpdated;
            }
            catch (Exception e)
            {
                Logger.LogException(configManager, e);
                if (e.InnerException != null)
                {
                    Console.WriteLine(e.InnerException.ToString());
                }
                else
                {
                    Console.WriteLine(e.ToString());
                }
            }
            finally
            {
                // Stop everything.
                Cleanup();
                _isStopping = true;
                if (updateThread != null && updateThread.IsAlive)
                {
                    updateThread.Join();
                }
            }
            if (updating)
            {
                return UpdateReturnValue;
            }
            return 0;
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

        private static void UpdateThread(IConfigurationManager configManager)
        {
            while (!_isStopping)
            {
                Thread.Sleep(1000 * 60);
                if (IsUpdateAvailable(configManager))
                {
                    UpdateSAMI();
                    _sami.UpdateWaiting = true;
                    break;
                }
            }
        }

        private static IDatabaseManager _updateServer = null;
        private static bool IsUpdateAvailable(IConfigurationManager configManager)
        {
            bool updateIsAvailable = false;
            Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

            IEnumerable<IDatabaseManager> mans = configManager.FindAllComponentsOfType<IDatabaseManager>().Where(s => s.ServerName.Equals(""));
            if (mans.Any())
            {
                foreach (IDatabaseManager man in mans)
                {
                    man.StartSession();
                    // Get newest version number available from database.
                    IDataReader result;
                    Version newestVersion = new Version();
                    if (man.TryRunResultQuery("SELECT MajorVersion,MinorVersion,BuildNumber,BlobName FROM Versions as V, Apps as A WHERE A.Name = 'Core SAMI' AND V.AppId = A.Id", out result))
                    {
                        while (result.Read())
                        {
                            Version version = new Version((short)result[0], (short)result[1], (short)result[2]);
                            _updateBlobName = (String)result[3];
                            if (version.CompareTo(newestVersion) > 0)
                            {
                                newestVersion = version;
                            }
                        }
                        result.Close();
                    }

                    if (newestVersion.CompareTo(currentVersion) > 0)
                    {
                        updateIsAvailable = true;
                        _updateServer = man;
                    }
                    man.EndSession();
                }
            }
            return updateIsAvailable;
        }

        private static void UpdateSAMI()
        {
            IDatabaseManager dbms = _updateServer;
            if (dbms != null)
            {
                String zipPath = "Update.zip";
                // Retrieve storage account from connection string.
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(dbms.BlobConnectionId);

                // Create the blob client.
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                // Retrieve reference to a previously created container.
                CloudBlobContainer container = blobClient.GetContainerReference("updatestorage");

                // Retrieve reference to a blob named "photo1.jpg".
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(_updateBlobName);

                // Save blob contents to a file.
                using (var fileStream = File.OpenWrite(zipPath))
                {
                    blockBlob.DownloadToStream(fileStream);
                }

                DirectoryInfo dir = new DirectoryInfo(UpdateLocation);
                if (dir.Exists)
                {
                    dir.Delete(true);
                }

                ZipFile.ExtractToDirectory(zipPath, Path.GetFullPath(UpdateLocation));

                FileInfo zipFile = new FileInfo(zipPath);
                zipFile.Delete();
            }
        }
    }
}
