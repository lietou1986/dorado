using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Dorado.Configuration.ServerHost
{
    public class ResourceManagerHandler : IHttpHandler
    {
        private const string NoAppPath = "General";

        private string GetDownloadUrl(string sectionName, string applicationName, int major, int minor)
        {
            string folder = System.Configuration.ConfigurationManager.AppSettings["httpResourceFolder"];
            return folder + "/" + sectionName + "/" + applicationName + "/" + major + "/" + sectionName + "." + minor;
        }

        private void SaveConfig(string sectionName, string applicationName, int major, int minor, byte[] buffer, string operatorId)
        {
            MemoryStream stream = new MemoryStream(buffer);
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);

            string[] names = sectionName.Split(".".ToCharArray());
            doc.DocumentElement.SetAttribute("resourceSetName", names[0]);
            doc.DocumentElement.SetAttribute("culture", names[1]);
            doc.DocumentElement.SetAttribute("majorVersion", major.ToString());
            doc.DocumentElement.SetAttribute("minorVersion", minor.ToString());

            string folder = System.Configuration.ConfigurationManager.AppSettings["resourceFolder"];
            string fileName = folder + "\\" + sectionName + "\\" + applicationName + "\\" + major + "\\" + sectionName + "." + minor;
            doc.Save(fileName);

            AddLog(sectionName, applicationName, major, minor, operatorId);
        }

        private void AddLog(string sectionName, string applicationName, int major, int minor, string operatorId)
        {
            string folder = System.Configuration.ConfigurationManager.AppSettings["resourceFolder"];
            string fileName = folder + "\\" + sectionName + "\\" + applicationName + "\\" + major + "\\log.txt";
            var fs = File.Exists(fileName) ? new FileStream(fileName, FileMode.Append, FileAccess.Write) : new FileStream(fileName, FileMode.CreateNew, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(string.Format("minor={0};operatorid={1};datetime={2}", minor, operatorId, DateTime.Now));
            sw.Close();
            fs.Close();
        }

        private RemoteConfigManagerDto GetAllConfigs()
        {
            RemoteConfigManagerDto result = new RemoteConfigManagerDto();
            try
            {
                result.Operation.Result = true;

                string folder = System.Configuration.ConfigurationManager.AppSettings["resourceFolder"];
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                string[] configs = Directory.GetDirectories(folder);
                if (configs.Length == 0)
                    return result;
                foreach (string config in configs)
                {
                    string sectionName = config.Substring(folder.Length + 1);
                    result.RemoteConfigSections.AddSection(sectionName, 0, 0);
                }
            }
            catch (Exception ex)
            {
                result.Operation.Result = false;
                result.Operation.ResultInfo = ex.ToString();
            }
            return result;
        }

        private RemoteConfigManagerDto GetApplicatinos(RemoteConfigManagerDto remoteConfigManager)
        {
            RemoteConfigManagerDto result = new RemoteConfigManagerDto();
            try
            {
                result.Operation.Result = true;

                string folder = System.Configuration.ConfigurationManager.AppSettings["resourceFolder"];
                folder = Path.Combine(folder, remoteConfigManager.Operation.Condition);
                if (!Directory.Exists(folder))
                {
                    return result;
                }
                string[] applications = Directory.GetDirectories(folder);
                if (applications.Length == 0)
                    return result;
                foreach (string application in applications)
                {
                    string sectionName = application.Substring(folder.Length + 1);
                    result.RemoteConfigSections.AddSection(sectionName, 0, 0);
                }
            }
            catch (Exception ex)
            {
                result.Operation.Result = false;
                result.Operation.ResultInfo = ex.ToString();
            }
            return result;
        }

        private RemoteConfigManagerDto GetMajors(RemoteConfigManagerDto remoteConfigManager)
        {
            RemoteConfigManagerDto result = new RemoteConfigManagerDto();
            try
            {
                result.Operation.Result = true;

                string folder = System.Configuration.ConfigurationManager.AppSettings["resourceFolder"];
                folder = Path.Combine(folder, remoteConfigManager.Operation.Condition);
                if (!Directory.Exists(folder))
                {
                    return result;
                }
                string[] majorVersions = Directory.GetDirectories(folder);
                if (majorVersions.Length == 0)
                    return result;
                foreach (string majorVersion in majorVersions)
                {
                    int major;
                    if (int.TryParse(majorVersion.Substring(folder.Length + 1), out major))
                    {
                        result.RemoteConfigSections.AddSection(remoteConfigManager.Operation.Condition, major, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Operation.Result = false;
                result.Operation.ResultInfo = ex.ToString();
            }
            return result;
        }

        private RemoteConfigManagerDto GetMinors(RemoteConfigManagerDto remoteConfigManager)
        {
            RemoteConfigManagerDto result = new RemoteConfigManagerDto();
            try
            {
                result.Operation.Result = true;

                string folder = System.Configuration.ConfigurationManager.AppSettings["resourceFolder"];
                folder = Path.Combine(folder, remoteConfigManager.Operation.Condition);
                if (Directory.Exists(folder))
                {
                    string[] minorVersions = Directory.GetFiles(folder);
                    if (minorVersions.Length == 0)
                        return result;
                    foreach (string minorVersion in minorVersions)
                    {
                        string[] strs = remoteConfigManager.Operation.Condition.Split("\\".ToCharArray());
                        string sectionName = strs[0];
                        string applicationName = strs[1];
                        int major = Convert.ToInt32(strs[2]);

                        string fileName = minorVersion.Substring(folder.Length + 1);
                        strs = fileName.Split(".".ToCharArray());
                        int minor;
                        if (int.TryParse(strs[strs.Length - 1], out minor))
                        {
                            string downloadUrl = GetDownloadUrl(sectionName, applicationName, major, minor);
                            result.RemoteConfigSections.AddSection(Path.Combine(sectionName, applicationName), major, minor, downloadUrl);
                        }
                    }

                    string logFileName = folder + @"\log.txt";
                    if (File.Exists(logFileName))
                    {
                        FileStream fs = new FileStream(logFileName, FileMode.Open, FileAccess.Read);
                        StreamReader sr = new StreamReader(fs);
                        result.Operation.Log = Encoding.Default.GetBytes(sr.ReadToEnd());
                        sr.Close();
                        fs.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                result.Operation.Result = false;
                result.Operation.ResultInfo = ex.ToString();
            }
            return result;
        }

        private RemoteConfigManagerDto CreateMinor(RemoteConfigManagerDto remoteConfigManager)
        {
            RemoteConfigManagerDto result = new RemoteConfigManagerDto();
            try
            {
                result.Operation.Result = true;

                string folder = System.Configuration.ConfigurationManager.AppSettings["resourceFolder"];
                folder = Path.Combine(folder, remoteConfigManager.Operation.Condition);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string[] strs = remoteConfigManager.Operation.Condition.Split("\\".ToCharArray());
                string sectionName = strs[0];
                string applicationName = strs[1];
                int major = Convert.ToInt32(strs[2]);

                string[] minorVersions = Directory.GetFiles(folder);
                if (minorVersions.Length == 0)
                {
                    SaveConfig(sectionName, applicationName, major, 1, remoteConfigManager.Operation.Value, remoteConfigManager.Operation.OperatorId);
                    result.Operation.ResultInfo = GetDownloadUrl(sectionName, applicationName, major, 1);
                    return result;
                }

                int maxMinor = 1;
                foreach (string minorVersion in minorVersions)
                {
                    string fileName = minorVersion.Substring(folder.Length + 1);
                    strs = fileName.Split(".".ToCharArray());
                    int minor;
                    if (int.TryParse(strs[strs.Length - 1], out minor))
                    {
                        maxMinor = Math.Max(maxMinor, minor);
                    }
                }

                SaveConfig(sectionName, applicationName, major, maxMinor + 1, remoteConfigManager.Operation.Value, remoteConfigManager.Operation.OperatorId);
                result.Operation.ResultInfo = GetDownloadUrl(sectionName, applicationName, major, maxMinor + 1);
                return result;
            }
            catch (Exception ex)
            {
                result.Operation.Result = false;
                result.Operation.ResultInfo = ex.ToString();
            }
            return result;
        }

        private RemoteConfigManagerDto DeleteConfig(RemoteConfigManagerDto remoteConfigManager)
        {
            RemoteConfigManagerDto result = new RemoteConfigManagerDto();
            try
            {
                result.Operation.Result = true;

                string folder = System.Configuration.ConfigurationManager.AppSettings["resourceFolder"];
                folder = Path.Combine(folder, remoteConfigManager.Operation.Condition);
                if (Directory.Exists(folder))
                {
                    string backupFolder = System.Configuration.ConfigurationManager.AppSettings["backupFolder"];
                    backupFolder = Path.Combine(backupFolder, remoteConfigManager.Operation.Condition);
                    if (!Directory.Exists(backupFolder))
                    {
                        Directory.CreateDirectory(backupFolder);
                    }
                    backupFolder = Path.Combine(backupFolder, DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss"));
                    Directory.Move(folder, backupFolder);
                }
            }
            catch (Exception ex)
            {
                result.Operation.Result = false;
                result.Operation.ResultInfo = ex.ToString();
            }
            return result;
        }

        private RemoteConfigManagerDto GetAllLastVersion()
        {
            RemoteConfigManagerDto result = new RemoteConfigManagerDto();
            try
            {
                result.Operation.Result = true;

                string folder = System.Configuration.ConfigurationManager.AppSettings["resourceFolder"];
                if (Directory.Exists(folder))
                {
                    string[] sections = Directory.GetDirectories(folder);
                    foreach (string section in sections)
                    {
                        string sectionName = section.Substring(folder.Length + 1);
                        string[] applications = Directory.GetDirectories(section);
                        foreach (string application in applications)
                        {
                            string applicationName = application.Substring(section.Length + 1);
                            string[] majorVersions = Directory.GetDirectories(application);
                            foreach (string majorVersion in majorVersions)
                            {
                                int major;
                                if (int.TryParse(majorVersion.Substring(application.Length + 1), out major))
                                {
                                    int maxMinor = 0;
                                    string[] minorVersions = Directory.GetFiles(majorVersion);
                                    foreach (string minorVersion in minorVersions)
                                    {
                                        string fileName = minorVersion.Substring(majorVersion.Length + 1);
                                        string[] strs = fileName.Split(".".ToCharArray());
                                        int minor;
                                        if (int.TryParse(strs[strs.Length - 1], out minor))
                                        {
                                            maxMinor = Math.Max(maxMinor, minor);
                                        }
                                    }
                                    string url = GetDownloadUrl(sectionName, applicationName, major, maxMinor);
                                    result.RemoteConfigSections.AddSection(Path.Combine(sectionName, applicationName), major, maxMinor, url);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Operation.Result = false;
                result.Operation.ResultInfo = ex.ToString();
            }
            return result;
        }

        private RemoteConfigManagerDto Manager(RemoteConfigManagerDto remoteConfigManager)
        {
            switch (remoteConfigManager.Operation.Command)
            {
                case "getAllConfigs":
                    {
                        return GetAllConfigs();
                    }
                case "getApplications":
                    {
                        return GetApplicatinos(remoteConfigManager);
                    }
                case "getMajors":
                    {
                        return GetMajors(remoteConfigManager);
                    }
                case "getMinors":
                    {
                        return GetMinors(remoteConfigManager);
                    }
                case "createMinor":
                    {
                        return CreateMinor(remoteConfigManager);
                    }
                case "deleteConfig":
                    {
                        return DeleteConfig(remoteConfigManager);
                    }
                case "getAllLastVersion":
                    {
                        return GetAllLastVersion();
                    }
                default:
                    {
                        var result = new RemoteConfigManagerDto {Operation = {Result = false}};
                        return result;
                    }
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.InputStream.Length == 0)
            {
                return;
            }
            XmlSerializer xser = new XmlSerializer(typeof(RemoteConfigManagerDto));
            RemoteConfigManagerDto remoteConfigManager = (RemoteConfigManagerDto)xser.Deserialize(context.Request.InputStream);

            RemoteConfigManagerDto result = Manager(remoteConfigManager);

            context.Response.ContentType = "text/xml";
            xser.Serialize(context.Response.OutputStream, result);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}