using System.Configuration;
using System.IO;

namespace Dorado
{
    public static class AppSettings
    {
        /// <summary>
        /// 应用程序名称
        /// </summary>
        public static string ApplicationName
        {
            get { return ConfigurationManager.AppSettings["ApplicationName"]; }
        }

        /// <summary>
        /// 日志存储目录
        /// </summary>
        public static string LogPath
        {
            get
            {
                string logDir = ConfigurationManager.AppSettings["LogPath"];
                if (string.IsNullOrWhiteSpace(logDir))
                {
                    logDir = "c:\\Dorado.logs";
                    string applicationName = ApplicationName;
                    if (!string.IsNullOrWhiteSpace(applicationName))
                        logDir = Path.Combine(logDir, applicationName);
                }
                return logDir;
            }
        }

        /// <summary>
        /// 是否启用本地调试
        /// </summary>
        public static bool IsDebug
        {
            get { return ConfigurationManager.AppSettings["IsDebug"] == "1"; }
        }
    }
}