using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Xml.Serialization;

namespace Dorado.Configuration.ServerHost
{
    public class ConfigVersionHandler : IHttpHandler
    {
        private const string NoAppPath = "General";

        private static string GetDownloadUrl(string sectionName, string applicationName, int major, int minor, int configType)
        {
            string folder = string.Empty;
            switch (configType)
            {
                case 1:
                    {
                        folder = System.Configuration.ConfigurationManager.AppSettings["httpPublishFolder"];
                        break;
                    }
                case 2:
                    {
                        folder = System.Configuration.ConfigurationManager.AppSettings["httpResourceFolder"];
                        break;
                    }
            }
            return folder + "/" + sectionName + "/" + applicationName + "/" + major + "/" + sectionName + "." + minor;
        }

        private int GetLastVersion(string sectionName, string applicationName, int major, out int configType, out bool exitAppConfig)
        {
            exitAppConfig = false;
            string folder = System.Configuration.ConfigurationManager.AppSettings["publishFolder"];

            folder = Path.Combine(folder, sectionName);
            folder = Path.Combine(folder, applicationName);
            folder = Path.Combine(folder, major.ToString());

            if (!Directory.Exists(folder))
            {
                folder = System.Configuration.ConfigurationManager.AppSettings["resourceFolder"];

                folder = Path.Combine(folder, sectionName);
                folder = Path.Combine(folder, applicationName);
                folder = Path.Combine(folder, major.ToString());
                if (!Directory.Exists(folder))
                {
                    configType = 0;
                    return -1;
                }

                configType = 2;
            }
            else
            {
                configType = 1;
            }

            int maxMinor = -1;
            string[] minorVersions = Directory.GetFiles(folder);
            foreach (string minorVersion in minorVersions)
            {
                string fileName = minorVersion.Substring(folder.Length + 1);
                string[] strs = fileName.Split(".".ToCharArray());
                int minor;
                if (int.TryParse(strs[strs.Length - 1], out minor))
                {
                    exitAppConfig = true;
                    maxMinor = Math.Max(maxMinor, minor);
                }
            }
            if (maxMinor == -1)
            {
                return -1;
            }

            return maxMinor;
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.InputStream.Length == 0)
            {
                return;
            }
            XmlSerializer xser = new XmlSerializer(typeof(RemoteConfigSectionCollection));
            RemoteConfigSectionCollection rcc = (RemoteConfigSectionCollection)xser.Deserialize(context.Request.InputStream);

            RemoteConfigSectionCollection ret = new RemoteConfigSectionCollection();
            bool exitAppConfig = false;
            foreach (RemoteConfigSectionParam param in rcc)
            {
                int configType;
                if (!string.IsNullOrEmpty(rcc.Application))
                {
                    int minor = GetLastVersion(param.SectionName, rcc.Application, param.MajorVersion, out configType, out exitAppConfig);
                    if (minor > param.MinorVersion)
                    {
                        string url = GetDownloadUrl(param.SectionName, rcc.Application, param.MajorVersion, minor, configType);
                        ret.AddSection(param.SectionName, param.MajorVersion, minor, url);
                        continue;
                    }
                }
                //如果指定了应用程序，且应用程序有配置文件，就不再处理默认配置文件
                if (exitAppConfig) continue;
                int minor2 = GetLastVersion(param.SectionName, NoAppPath, param.MajorVersion, out configType, out exitAppConfig);
                if (minor2 > param.MinorVersion)
                {
                    string url = GetDownloadUrl(param.SectionName, NoAppPath, param.MajorVersion, minor2, configType);
                    ret.AddSection(param.SectionName, param.MajorVersion, minor2, url);
                }
            }

            context.Response.ContentType = "text/xml";
            xser.Serialize(context.Response.OutputStream, ret);
        }

        public bool IsReusable => false;
    }
}