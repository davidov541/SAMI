using System;
using System.Diagnostics;
using System.IO;

namespace SAMI.Application.Launcher
{
    internal class Program
    {
        private const int RestartReturnValue = 5;
        private const String TemporaryZipFilePath = "tmpZip";

        public static void Main(string[] args)
        {
            int lastReturnValue = RestartReturnValue;
            while(lastReturnValue == RestartReturnValue)
            {
                Process proc = Process.Start("SAMIApplication.exe");
                proc.WaitForExit();
                lastReturnValue = proc.ExitCode;

                DirectoryInfo di = new DirectoryInfo(TemporaryZipFilePath);
                if(lastReturnValue == RestartReturnValue && di.Exists)
                {
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.CopyTo(file.Name, true);
                    }
                    di.Delete(true);
                }
            }
        }
    }
}
