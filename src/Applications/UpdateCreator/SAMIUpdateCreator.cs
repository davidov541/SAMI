using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using SAMI.IOInterfaces;
using SAMI.IOInterfaces.Interfaces.Database;

namespace SAMI.Application.UpdateCreator
{
    internal class SAMIUpdateCreator
    {
        private const string BlobName = "CoreSAMI";
        private const string AppName = "Core SAMI";
        private const string AppDescription = "Core libraries for SAMI.";

        public static void Main(string[] args)
        {
            // Get paths to the current directory, and any old updates.
            String path = Directory.GetCurrentDirectory();
            String updateFilePath = String.Format(@"{0}\..\CoreSAMI.zip", path);

            // Delete the configuration file and old updates.
            FileInfo configFile = new FileInfo(String.Format(@"{0}\..\SAMI\bin\Debug\SAMIConfiguration.xml", path));
            configFile.Delete();
            FileInfo updateFileInfo = new FileInfo(updateFilePath);
            updateFileInfo.Delete();

            // Zip up the new update and move to the place to use.
            ZipFile.CreateFromDirectory(String.Format(@"{0}\..\SAMI\bin\Debug", path), updateFilePath, CompressionLevel.Optimal, false);

            // Get the version of the executable.
            Version latestVersion = new Version(FileVersionInfo.GetVersionInfo(String.Format(@"{0}\..\SAMI\bin\Debug\SAMI.exe", path)).FileVersion);

            // Update database with new information.
            IDatabaseManager man = ConfigurationManager.GetInstance().FindAllComponentsOfType<IDatabaseManager>().FirstOrDefault();
            if(man != null)
            {
                man.StartSession();
                // If the app isn't in the database yet, add it.
                int appId = GetAppId(man);
                if (appId == -1)
                {
                    man.TryRunNoResultQuery(string.Format("INSERT INTO Apps (Name, Description, Type) VALUES ('{0}', '{1}', 0)", AppName, AppDescription));
                    appId = GetAppId(man);
                }

                List<Version> versions = GetAvailableVersions(man, appId).ToList();

                Version latestCurrentVersion = new Version();
                if (versions.Any())
                {
                    latestCurrentVersion = versions.Max();
                }

                if (latestVersion <= latestCurrentVersion)
                {
                    Console.WriteLine("ERROR -- File version was not higher than the current version. Make sure you updated the version of the app you are updating.");
                }
                else
                {
                    String modifiedBlobName = BlobName + latestVersion.ToString();

                    // Upload file to Azure.
                    CloudStorageAccount account = CloudStorageAccount.Parse(ConfigurationManager.GetInstance().FindAllComponentsOfType<IDatabaseManager>().First().BlobConnectionId);
                    CloudBlobClient client = account.CreateCloudBlobClient();
                    CloudBlobContainer container = client.GetContainerReference("updatestorage");
                    CloudBlockBlob update = container.GetBlockBlobReference(modifiedBlobName);
                    update.UploadFile(updateFilePath);

                    // Insert version number into database if we have reached this point.
                    man.TryRunNoResultQuery(string.Format("INSERT INTO Versions (AppId, MajorVersion, MinorVersion, BuildNumber, BlobName) VALUES ('{0}', {1}, {2}, {3}, '{4}')", appId, latestVersion.Major, latestVersion.Minor, latestVersion.Build, modifiedBlobName));
                }
                man.EndSession();
            }
            else
            {
                Console.WriteLine("ERROR -- No database manager was available.");
            }
        }

        private static int GetAppId(IDatabaseManager man)
        {
            int appId = -1;
            IDataReader reader;
            if (man.TryRunResultQuery(String.Format("SELECT Id,Name FROM Apps WHERE Name = '{0}'", AppName), out reader))
            {
                while (reader.Read())
                {
                    appId = (int)reader[0];
                }
                reader.Close();
            }
            return appId;
        }

        private static IEnumerable<Version> GetAvailableVersions(IDatabaseManager man, int appId)
        {
            IDataReader reader;
            if (man.TryRunResultQuery(String.Format("Select MajorVersion,MinorVersion,BuildNumber FROM Versions WHERE AppId = {0}", appId), out reader))
            {
                while (reader.Read())
                {
                    yield return new Version((short)reader[0], (short)reader[1], (short)reader[2], 0);
                }
                reader.Close();
            }
        }
    }
}
