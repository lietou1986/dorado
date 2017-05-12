using System;
using System.IO;

namespace ClientUpdate
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Update();
        }

        private static void Update()
        {
            try
            {
                WinServiceHelper.StopService("Dorado.VWS.ClientHost");
                System.Diagnostics.Process[] process = System.Diagnostics.Process.GetProcessesByName("Dorado.VWS.ClientHost");
                if (process != null)
                {
                    foreach (System.Diagnostics.Process p in process)
                    {
                        p.Kill();
                    }
                }
                DownLoadAndUpdate();
                WinServiceHelper.StartService("Dorado.VWS.ClientHost");
            }
            catch (Exception ex)
            {
                //Logger.Log("updater", LogLevel.Local, ex.ToString()+"sfdsfsfsf");
            }
        }

        private static void DownLoadAndUpdate()
        {
            try
            {
                string updateFileName = Environment.CurrentDirectory + "\\Update.zip";
                HttpHelper.DownloadFile("http://zsync.zpidc.com/readme/update.zip", Environment.CurrentDirectory + "\\Update.zip");
                if (File.Exists(updateFileName))
                {
                    ZipHelper.UnZip(updateFileName, Environment.CurrentDirectory);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}