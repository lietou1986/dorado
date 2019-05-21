using Dorado.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Dorado.Configuration.ServerHost
{
    public class ConfigManagerHandler : IHttpHandler
    {
        private const string NoAppPath = "General";

        private string GetDownloadUrl(string applicationName, string sectionName, int major, int minor)
        {
            string folder = System.Configuration.ConfigurationManager.AppSettings["httpPublishFolder"];
            return folder + "/" + applicationName + "/" + sectionName + "/" + major + "/" + sectionName + "." + minor;
        }

        private void SaveConfig(string applicationName, string sectionName, int major, int minor, byte[] buffer,
            string operatorId = "len.zhang")
        {
            MemoryStream stream = new MemoryStream(buffer);
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);

            doc.DocumentElement.SetAttribute("majorVersion", major.ToString());
            doc.DocumentElement.SetAttribute("minorVersion", minor.ToString());

            string folder = System.Configuration.ConfigurationManager.AppSettings["publishFolder"];
            folder += "\\" + applicationName + "\\" + sectionName + "\\" + major + "\\";
            IOUtility.CreateDirectory(folder);
            string fileName = folder + sectionName + "." + minor;
            doc.Save(fileName);

            AddLog(applicationName, sectionName, major, minor, operatorId);
        }

        private static void AddLog(string applicationName, string sectionName, int major, int minor,
            string operatorId = "len.zhang")
        {
            string folder = System.Configuration.ConfigurationManager.AppSettings["publishFolder"];
            string fileName = folder + "\\" + applicationName + "\\" + sectionName + "\\" + major + "\\log.txt";
            var fs = File.Exists(fileName)
                ? new FileStream(fileName, FileMode.Append, FileAccess.Write)
                : new FileStream(fileName, FileMode.CreateNew, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(string.Format("minor={0};operatorid={1};datetime={2}", minor, operatorId, DateTime.Now));
            sw.Close();
            fs.Close();
        }

        private RemoteConfigManagerDto GetAllConfigs(RemoteConfigManagerDto remoteConfigManager)
        {
            RemoteConfigManagerDto result = new RemoteConfigManagerDto();
            try
            {
                result.Operation.Result = true;

                string folder = System.Configuration.ConfigurationManager.AppSettings["publishFolder"];
                folder = Path.Combine(folder, remoteConfigManager.Operation.Condition);

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

        private RemoteConfigManagerDto GetApplications()
        {
            RemoteConfigManagerDto result = new RemoteConfigManagerDto();
            try
            {
                result.Operation.Result = true;

                string folder = System.Configuration.ConfigurationManager.AppSettings["publishFolder"];
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

                string folder = System.Configuration.ConfigurationManager.AppSettings["publishFolder"];
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

                string folder = System.Configuration.ConfigurationManager.AppSettings["publishFolder"];
                folder = Path.Combine(folder, remoteConfigManager.Operation.Condition);
                if (Directory.Exists(folder))
                {
                    string[] minorVersions = Directory.GetFiles(folder);
                    if (minorVersions.Length == 0)
                        return result;
                    foreach (string minorVersion in minorVersions)
                    {
                        string[] strs = remoteConfigManager.Operation.Condition.Split("/\\".ToCharArray());
                        string applicationName = strs[0];
                        string sectionName = strs[1];
                        int major = Convert.ToInt32(strs[2]);

                        string fileName = minorVersion.Substring(folder.Length + 1);
                        strs = fileName.Split(".".ToCharArray());
                        if (int.TryParse(strs[1], out var minor))
                        {
                            string downloadUrl = GetDownloadUrl(applicationName, sectionName, major, minor);
                            result.RemoteConfigSections.AddSection(Path.Combine(applicationName, sectionName), major,
                                minor, downloadUrl);
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

                string[] strs = remoteConfigManager.Operation.Condition.Split("/\\".ToCharArray());
                string applicationName = strs[0];
                string sectionName = strs[1];
                int major = Convert.ToInt32(strs[2]);
                int maxMinor = GetMaxMinVersion(applicationName, sectionName, ref major);

                SaveConfig(applicationName, sectionName, major, maxMinor + 1, remoteConfigManager.Operation.Value,
                    remoteConfigManager.Operation.OperatorId);
                result.Operation.ResultInfo = GetDownloadUrl(applicationName, sectionName, major, maxMinor + 1);
                return result;
            }
            catch (Exception ex)
            {
                result.Operation.Result = false;
                result.Operation.ResultInfo = ex.ToString();
            }

            return result;
        }

        private int GetMaxMinVersion(String applicationName, String sectionName, ref int major)
        {
            int maxMinVersion = 0;
            string folder = System.Configuration.ConfigurationManager.AppSettings["publishFolder"] +
                            string.Format("/{0}/{1}/", NoAppPath, sectionName);
            string[] files;

            if (Directory.Exists(folder))
            {
                string[] folders = Directory.GetDirectories(folder);
                if (folders.Length == 0)
                    return maxMinVersion;

                //获得主版本号
                foreach (string str in folders)
                {
                    int maj;
                    string majorV = str.Substring(str.LastIndexOf("/") + 1);
                    if (int.TryParse(majorV, out maj))
                    {
                        major = Math.Max(major, maj);
                    }
                }

                //获得次版本号
                ////从General中查找次版本号
                folder = folder + string.Format("/{0}/{1}/{2}/", NoAppPath, sectionName, major);
                if (Directory.Exists(folder))
                {
                    files = Directory.GetFiles(folder);
                    foreach (string fileDirectory in files)
                    {
                        string filename = fileDirectory.Substring(folder.Length + 1);
                        string[] vS = filename.Split(".".ToCharArray());
                        int minor;
                        if (int.TryParse(vS[1], out minor))
                        {
                            maxMinVersion = Math.Max(maxMinVersion, minor);
                        }
                    }
                }
            }

            ////从strs[1]中查找次版本号
            if (!applicationName.ToLower().Equals(NoAppPath))
            {
                folder = folder + string.Format("/{0}/{1}/{2}", applicationName, sectionName, major);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                files = Directory.GetFiles(folder);
                foreach (string fileDirectoty in files)
                {
                    string filename = fileDirectoty.Substring(folder.Length + 1);
                    string[] vS = filename.Split(".".ToCharArray());
                    if (int.TryParse(vS[1], out var minor))
                    {
                        maxMinVersion = Math.Max(maxMinVersion, minor);
                    }
                }
            }

            folder = folder + string.Format("/{0}/{1}/{2}", applicationName, sectionName, major);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return maxMinVersion;
        }

        private RemoteConfigManagerDto DeleteConfig(RemoteConfigManagerDto remoteConfigManager)
        {
            RemoteConfigManagerDto result = new RemoteConfigManagerDto();
            try
            {
                result.Operation.Result = true;

                string folder = System.Configuration.ConfigurationManager.AppSettings["publishFolder"];
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

        private RemoteConfigManagerDto GetLastVersionBySectionName(RemoteConfigManagerDto remoteConfigManager)
        {
            RemoteConfigManagerDto rcmDto = GetAllLastVersion(remoteConfigManager);
            RemoteConfigManagerDto result = new RemoteConfigManagerDto();

            string[] tmpParam = remoteConfigManager.Operation.Condition.Split('|');
            string[] tmpAppList = tmpParam[0].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] tmpsectionList = tmpParam[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] majorList = tmpParam[2].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, int> appList = new Dictionary<string, int>();

            for (int index = 0; index < tmpsectionList.Length; index++)
            {
                appList.Add(
                    Path.Combine(tmpsectionList[index], tmpAppList.Length <= index ? NoAppPath : tmpAppList[index]),
                    tmpAppList.Length <= index ? 1 : Convert.ToInt32(majorList[index]));
            }

            foreach (RemoteConfigSectionParam param in rcmDto.RemoteConfigSections)
            {
                if (appList.ContainsKey(param.SectionName) && appList[param.SectionName] == param.MajorVersion)
                    result.RemoteConfigSections.AddSection(param.SectionName, param.MajorVersion, param.MinorVersion,
                        param.DownloadUrl);
            }

            return result;
        }

        private RemoteConfigManagerDto GetAllLastVersion(RemoteConfigManagerDto remoteConfigManager)
        {
            RemoteConfigManagerDto result = new RemoteConfigManagerDto();
            try
            {
                result.Operation.Result = true;

                string folder = System.Configuration.ConfigurationManager.AppSettings["publishFolder"];
                folder = Path.Combine(folder, remoteConfigManager.Operation.Condition);

                if (Directory.Exists(folder))
                {
                    string applicationName = remoteConfigManager.Operation.Condition;
                    string[] sections = Directory.GetDirectories(folder);
                    foreach (string section in sections)
                    {
                        string sectionName = section.Substring(section.Length + 1);
                        string[] majorVersions = Directory.GetDirectories(section);
                        foreach (string majorVersion in majorVersions)
                        {
                            if (int.TryParse(majorVersion.Substring(folder.Length + 1), out var major))
                            {
                                var maxMinor = 0;
                                string[] minorVersions = Directory.GetFiles(majorVersion);
                                foreach (string minorVersion in minorVersions)
                                {
                                    string fileName = minorVersion.Substring(majorVersion.Length + 1);
                                    string[] strs = fileName.Split(".".ToCharArray());
                                    if (int.TryParse(strs[1], out var minor))
                                        maxMinor = Math.Max(maxMinor, minor);
                                }

                                string url = GetDownloadUrl(applicationName, sectionName, major, maxMinor);
                                result.RemoteConfigSections.AddSection(Path.Combine(applicationName, sectionName),
                                    major, maxMinor, url);
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
                        return GetAllConfigs(remoteConfigManager);
                    }
                case "getApplications":
                    {
                        return GetApplications();
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
                        return GetAllLastVersion(remoteConfigManager);
                    }

                case "getLastVersionBySectionName":
                    return GetLastVersionBySectionName(remoteConfigManager);

                default:
                    {
                        var result = new RemoteConfigManagerDto { Operation = { Result = false } };
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
            RemoteConfigManagerDto remoteConfigManager =
                (RemoteConfigManagerDto)xser.Deserialize(context.Request.InputStream);

            RemoteConfigManagerDto result = Manager(remoteConfigManager);

            context.Response.ContentType = "text/xml";
            xser.Serialize(context.Response.OutputStream, result);
        }

        public bool IsReusable => false;
    }

    [XmlRoot("RemoteConfigManager")]
    public class RemoteConfigManagerDto
    {
        [XmlElement("Operation")] public RemoteConfigOperation Operation { get; set; }

        [XmlElement("RemoteConfigSections")] public RemoteConfigSectionCollection RemoteConfigSections { get; set; }

        public RemoteConfigManagerDto()
        {
            Operation = new RemoteConfigOperation();
            RemoteConfigSections = new RemoteConfigSectionCollection();
        }
    }

    public class RemoteConfigOperation
    {
        [XmlAttribute("Command")] public string Command { get; set; }

        [XmlAttribute("Result")] public bool Result { get; set; }

        [XmlAttribute("Condition")] public string Condition { get; set; }

        [XmlAttribute("ResultInfo")] public string ResultInfo { get; set; }

        [XmlAttribute("Value")] public byte[] Value { get; set; }

        [XmlAttribute("OperatorID")] public string OperatorId { get; set; }

        [XmlAttribute("Log")] public byte[] Log { get; set; }
    }
}