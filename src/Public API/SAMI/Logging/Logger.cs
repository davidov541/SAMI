using System;
using System.Linq;
using System.Threading.Tasks;
using SAMI.Configuration;
using SAMI.IOInterfaces.Interfaces.Database;

namespace SAMI.Logging
{
    internal static class Logger
    {
        public static void LogString(IConfigurationManager configManager, String logMessage, LogCategory category)
        {
            IDatabaseManager man = configManager.FindAllComponentsOfType<IDatabaseManager>().FirstOrDefault();
            if (man != null)
            {
                Task.Run(() =>
                    {
                        man.StartSession();
                        man.TryRunNoResultQuery(String.Format("INSERT INTO dbo.Logs (Time,Message,Category,Source) VALUES ('{0}','{1}',{2},'{3}')",
                            DateTime.Now.ToString(),
                            logMessage,
                            ((int)category).ToString(),
                            Environment.MachineName));
                        man.EndSession();
                    });
            }
        }

        public static void LogException(IConfigurationManager configManager, Exception e)
        {
            LogString(configManager, e.Message, LogCategory.Error);
        }
    }
}
