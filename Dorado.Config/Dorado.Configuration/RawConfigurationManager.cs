using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Serialization;

namespace Dorado.Configuration
{
    [XmlRoot("RemoteConfigManager")]
    public class RemoteConfigManagerDTO
    {
        private RemoteConfigOperation operation;

        [XmlElement("Operation")]
        public RemoteConfigOperation Operation
        {
            get { return operation; }
            set { operation = value; }
        }

        private RemoteConfigSectionCollection remoteConfigSections;

        [XmlElement("RemoteConfigSections")]
        public RemoteConfigSectionCollection RemoteConfigSections
        {
            get { return remoteConfigSections; }
            set { remoteConfigSections = value; }
        }

        public RemoteConfigManagerDTO()
        {
            operation = new RemoteConfigOperation();
            remoteConfigSections = new RemoteConfigSectionCollection();
        }
    }

    public class RemoteConfigOperation
    {
        private string command;

        [XmlAttribute("Command")]
        public string Command
        {
            get { return command; }
            set { command = value; }
        }

        private bool result;

        [XmlAttribute("Result")]
        public bool Result
        {
            get { return result; }
            set { result = value; }
        }

        private string condition;

        [XmlAttribute("Condition")]
        public string Condition
        {
            get { return condition; }
            set { condition = value; }
        }

        private string resultInfo;

        [XmlAttribute("ResultInfo")]
        public string ResultInfo
        {
            get { return resultInfo; }
            set { resultInfo = value; }
        }

        private byte[] _value;

        [XmlAttribute("Value")]
        public byte[] Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private string operatorID;

        [XmlAttribute("OperatorID")]
        public string OperatorID
        {
            get { return operatorID; }
            set { operatorID = value; }
        }

        private byte[] log;

        [XmlAttribute("Log")]
        public byte[] Log
        {
            get { return log; }
            set { log = value; }
        }
    }

    public class RawConfigurationManager
    {
        #region Singleton

        private static RawConfigurationManager instance = new RawConfigurationManager();

        public static RawConfigurationManager Instance => instance;

        private RawConfigurationManager()
        {
        }

        #endregion Singleton

        private static readonly string NoAppPath = "General";

        private DataTable GetAllLastVersion(RemoteConfigSectionCollection configs)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(Int32));
            dt.Columns.Add("SectionName", typeof(string));
            dt.Columns.Add("Major", typeof(Int32));
            dt.Columns.Add("Application", typeof(string));
            dt.Columns.Add("Minor", typeof(Int32));
            dt.Columns.Add("FileName", typeof(string));
            dt.Columns.Add("DownloadUrl", typeof(string));
            dt.Columns.Add("CanDelete", typeof(bool));
            dt.Columns.Add("ShowValue", typeof(bool));
            dt.Columns.Add("CanDeleteApp", typeof(bool));
            dt.PrimaryKey = new DataColumn[] {dt.Columns["ID"]};

            if (configs.Sections.Count == 0)
            {
                DataRow dr = dt.NewRow();
                dr["ID"] = 1;
                dr["SectionName"] = "";
                dr["Major"] = 0;
                dr["Application"] = "";
                dr["FileName"] = "";
                dr["Minor"] = 0;
                dr["DownloadUrl"] = "";
                dr["CanDelete"] = false;
                dr["ShowValue"] = false;
                dr["CanDeleteApp"] = false;
                dt.Rows.Add(dr);
            }

            string lastSectionName = null;
            for (int i = 0; i < configs.Sections.Count; i++)
            {
                RemoteConfigSectionParam section = configs.Sections[i];
                DataRow dr = dt.NewRow();
                string[] strs = section.SectionName.Split("\\".ToCharArray());
                dr["ID"] = i + 1;
                dr["SectionName"] = strs[0];
                if (string.Compare(lastSectionName, strs[0]) == 0)
                {
                    dr["CanDelete"] = false;
                }
                else
                {
                    dr["CanDelete"] = true;
                    lastSectionName = dr["SectionName"].ToString();
                }

                dr["Major"] = section.MajorVersion;
                dr["CanDeleteApp"] = GetAppCount(configs, strs[0]) > 1 ? true : false;
                if (string.Compare(strs[1], NoAppPath) == 0)
                {
                    dr["Application"] = NoAppPath;
                    dr["FileName"] = string.Format("{0}.{1}.{2}.xml", strs[0], section.MajorVersion,
                        section.MinorVersion);
                }
                else
                {
                    dr["Application"] = strs[1];
                    dr["FileName"] = string.Format("{0}.{1}.{2}.{3}.xml", strs[0], strs[1], section.MajorVersion,
                        section.MinorVersion);
                }

                dr["Minor"] = section.MinorVersion;
                dr["DownloadUrl"] = section.DownloadUrl;
                dr["ShowValue"] = true;
                dt.Rows.Add(dr);
            }

            return dt;
        }

        private DataTable GetMinors(RemoteConfigSectionCollection configs, byte[] Log)
        {
            string[] logs = new string[0];
            if ((Log != null) && (Log.Length > 0))
            {
                string logStr = Encoding.Default.GetString(Log);
                logs = logStr.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            }

            #region 对Section进行排序

            configs.Sections.Sort(CompareSection);

            #endregion 对Section进行排序

            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(Int32));
            dt.Columns.Add("SectionName", typeof(string));
            dt.Columns.Add("Major", typeof(Int32));
            dt.Columns.Add("Application", typeof(string));
            dt.Columns.Add("Minor", typeof(Int32));
            dt.Columns.Add("FileName", typeof(string));
            dt.Columns.Add("DownloadUrl", typeof(string));
            dt.Columns.Add("CanDelete", typeof(bool));
            dt.Columns.Add("OperatorID", typeof(string));
            dt.Columns.Add("OperateTime", typeof(string));
            dt.PrimaryKey = new DataColumn[] {dt.Columns["ID"]};

            string lastSectionName = null;
            for (int i = 0; i < configs.Sections.Count; i++)
            {
                RemoteConfigSectionParam section = configs.Sections[i];
                DataRow dr = dt.NewRow();
                string[] strs = section.SectionName.Split("\\".ToCharArray());
                dr["ID"] = i + 1;
                dr["SectionName"] = strs[0];
                if (string.Compare(lastSectionName, strs[0]) == 0)
                {
                    dr["CanDelete"] = false;
                }
                else
                {
                    dr["CanDelete"] = true;
                    lastSectionName = dr["SectionName"].ToString();
                }

                dr["Major"] = section.MajorVersion;
                if (string.Compare(strs[1], NoAppPath) == 0)
                {
                    dr["Application"] = NoAppPath;
                    dr["FileName"] = string.Format("{0}.{1}.{2}.xml", strs[0], section.MajorVersion,
                        section.MinorVersion);
                }
                else
                {
                    dr["Application"] = strs[1];
                    dr["FileName"] = string.Format("{0}.{1}.{2}.{3}.xml", strs[0], strs[1], section.MajorVersion,
                        section.MinorVersion);
                }

                dr["Minor"] = section.MinorVersion;
                dr["DownloadUrl"] = section.DownloadUrl;
                foreach (string log in logs)
                {
                    string[] keyValues = log.Split(";".ToCharArray());
                    string[] tmpStrs = keyValues[0].Split("=".ToCharArray());
                    if (int.Parse(tmpStrs[1]) == section.MinorVersion)
                    {
                        dr["OperatorID"] = keyValues[1].Split("=".ToCharArray())[1];
                        dr["OperateTime"] = keyValues[2].Split("=".ToCharArray())[1];
                        break;
                    }
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }

        public DataTable GetAllLastVersion(string application)
        {
            DataTable result = new DataTable();
            try
            {
                RemoteConfigManagerDTO remoteConfigManagerDto = new RemoteConfigManagerDTO
                {
                    Operation = {Command = "getAllLastVersion", Condition = application}
                };

                string URL = System.Configuration.ConfigurationManager.AppSettings["remoteConfigurationManagerUrl"] +
                             "ConfigManagerHandler.ashx";

                MemoryStream ms = new MemoryStream();
                XmlSerializer ser = new XmlSerializer(typeof(RemoteConfigManagerDTO));
                ser.Serialize(ms, remoteConfigManagerDto);

                HttpWebRequest req = (HttpWebRequest) HttpWebRequest.Create(URL);
                req.ContentType = "text/xml";
                req.Method = "POST";
                req.Timeout = 30000;
                req.ReadWriteTimeout = 30000;
                req.ContentLength = ms.Length;
                req.KeepAlive = false;

                using (Stream stream = req.GetRequestStream())
                {
                    byte[] buf = ms.ToArray();
                    stream.Write(buf, 0, buf.Length);
                    stream.Close();
                }

                RemoteConfigManagerDTO lstOutput;
                HttpWebResponse rsp = (HttpWebResponse) req.GetResponse();
                using (Stream stream = rsp.GetResponseStream())
                {
                    lstOutput = (RemoteConfigManagerDTO) ser.Deserialize(stream);
                    stream.Close();
                }

                rsp.Close();

                if (lstOutput.Operation.Result)
                {
                    result = GetAllLastVersion(lstOutput.RemoteConfigSections);
                }
                else
                {
                    LoggerWrapper.Logger.Error(lstOutput.Operation.ResultInfo);
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error(ex);
            }

            return result;
        }

        private DataTable GetRemoteConfigs(RemoteConfigSectionCollection configs)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id", typeof(string));
            dt.Columns.Add("text", typeof(string));
            foreach (RemoteConfigSectionParam section in configs.Sections)
            {
                DataRow dr = dt.NewRow();
                dr["id"] = section.SectionName;
                dr["text"] = section.SectionName;
                dt.Rows.Add(dr);
            }

            return dt;
        }

        private int CompareApplication(RemoteConfigSectionParam x, RemoteConfigSectionParam y)
        {
            return x.SectionName.CompareTo(y.SectionName);
        }

        private DataTable GetApplications(RemoteConfigSectionCollection configs)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id", typeof(string));
            dt.Columns.Add("text", typeof(string));
            bool HaveNoAppPath = false;
            foreach (RemoteConfigSectionParam section in configs.Sections)
            {
                if (string.Compare(section.SectionName, NoAppPath) == 0)
                {
                    HaveNoAppPath = true;
                    break;
                }
            }

            if (!HaveNoAppPath)
            {
                configs.AddSection(NoAppPath, 0, 0);
            }

            configs.Sections.Sort(CompareApplication);
            foreach (var t in configs.Sections)
            {
                DataRow dr = dt.NewRow();
                RemoteConfigSectionParam section = t;
                if (string.Compare(section.SectionName, NoAppPath) == 0)
                {
                    dr["id"] = section.SectionName;
                    dr["text"] = NoAppPath;
                }
                else
                {
                    dr["id"] = section.SectionName;
                    dr["text"] = section.SectionName;
                }

                dt.Rows.Add(dr);
            }

            return dt;
        }

        private int CompareMajor(RemoteConfigSectionParam x, RemoteConfigSectionParam y)
        {
            return y.MajorVersion.CompareTo(x.MajorVersion);
        }

        public DataTable GetAllConfigs(String application)
        {
            DataTable result = new DataTable();
            try
            {
                RemoteConfigManagerDTO remoteConfigManagerDto = new RemoteConfigManagerDTO
                {
                    Operation = {Command = "getAllConfigs", Condition = application}
                };

                string URL = System.Configuration.ConfigurationManager.AppSettings["remoteConfigurationManagerUrl"] +
                             "ConfigManagerHandler.ashx";

                MemoryStream ms = new MemoryStream();
                XmlSerializer ser = new XmlSerializer(typeof(RemoteConfigManagerDTO));
                ser.Serialize(ms, remoteConfigManagerDto);

                HttpWebRequest req = (HttpWebRequest) HttpWebRequest.Create(URL);
                req.ContentType = "text/xml";
                req.Method = "POST";
                req.Timeout = 30000;
                req.ReadWriteTimeout = 30000;
                req.ContentLength = ms.Length;
                req.KeepAlive = false;

                using (Stream stream = req.GetRequestStream())
                {
                    byte[] buf = ms.ToArray();
                    stream.Write(buf, 0, buf.Length);
                    stream.Close();
                }

                RemoteConfigManagerDTO lstOutput;
                HttpWebResponse rsp = (HttpWebResponse) req.GetResponse();
                using (Stream stream = rsp.GetResponseStream())
                {
                    lstOutput = (RemoteConfigManagerDTO) ser.Deserialize(stream);
                    stream.Close();
                }

                rsp.Close();

                if (lstOutput.Operation.Result)
                {
                    result = GetRemoteConfigs(lstOutput.RemoteConfigSections);
                }
                else
                {
                    LoggerWrapper.Logger.Error(lstOutput.Operation.ResultInfo);
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error(ex);
            }

            return result;
        }

        public DataTable GetApplications()
        {
            DataTable dt = new DataTable();
            try
            {
                RemoteConfigManagerDTO remoteConfigManagerDto = new RemoteConfigManagerDTO
                {
                    Operation = {Command = "getApplications"}
                };

                string URL = System.Configuration.ConfigurationManager.AppSettings["remoteConfigurationManagerUrl"] +
                             "ConfigManagerHandler.ashx";

                MemoryStream ms = new MemoryStream();
                XmlSerializer ser = new XmlSerializer(typeof(RemoteConfigManagerDTO));
                ser.Serialize(ms, remoteConfigManagerDto);

                HttpWebRequest req = (HttpWebRequest) HttpWebRequest.Create(URL);
                req.ContentType = "text/xml";
                req.Method = "POST";
                req.Timeout = 30000;
                req.ReadWriteTimeout = 30000;
                req.ContentLength = ms.Length;
                req.KeepAlive = false;

                using (Stream stream = req.GetRequestStream())
                {
                    byte[] buf = ms.ToArray();
                    stream.Write(buf, 0, buf.Length);
                    stream.Close();
                }

                RemoteConfigManagerDTO lstOutput;
                HttpWebResponse rsp = (HttpWebResponse) req.GetResponse();
                using (Stream stream = rsp.GetResponseStream())
                {
                    lstOutput = (RemoteConfigManagerDTO) ser.Deserialize(stream);
                    stream.Close();
                }

                rsp.Close();

                if (lstOutput.Operation.Result)
                {
                    dt = GetApplications(lstOutput.RemoteConfigSections);
                }
                else
                {
                    LoggerWrapper.Logger.Error(lstOutput.Operation.ResultInfo);
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error(ex);
            }

            return dt;
        }

        public DataTable GetMajors(string application)
        {
            DataTable dt = new DataTable();
            try
            {
                RemoteConfigManagerDTO remoteConfigManagerDto = new RemoteConfigManagerDTO
                {
                    Operation = {Command = "getMajors", Condition = application}
                };

                string URL = System.Configuration.ConfigurationManager.AppSettings["remoteConfigurationManagerUrl"] +
                             "ConfigManagerHandler.ashx";

                MemoryStream ms = new MemoryStream();
                XmlSerializer ser = new XmlSerializer(typeof(RemoteConfigManagerDTO));
                ser.Serialize(ms, remoteConfigManagerDto);

                HttpWebRequest req = (HttpWebRequest) HttpWebRequest.Create(URL);
                req.ContentType = "text/xml";
                req.Method = "POST";
                req.Timeout = 30000;
                req.ReadWriteTimeout = 30000;
                req.ContentLength = ms.Length;
                req.KeepAlive = false;

                using (Stream stream = req.GetRequestStream())
                {
                    byte[] buf = ms.ToArray();
                    stream.Write(buf, 0, buf.Length);
                    stream.Close();
                }

                RemoteConfigManagerDTO lstOutput;
                HttpWebResponse rsp = (HttpWebResponse) req.GetResponse();
                using (Stream stream = rsp.GetResponseStream())
                {
                    lstOutput = (RemoteConfigManagerDTO) ser.Deserialize(stream);
                    stream.Close();
                }

                rsp.Close();

                if (lstOutput.Operation.Result)
                {
                    dt = GetMajors(lstOutput.RemoteConfigSections);
                }
                else
                {
                    LoggerWrapper.Logger.Error(lstOutput.Operation.ResultInfo);
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error(ex);
            }

            return dt;
        }

        private DataTable GetMajors(RemoteConfigSectionCollection configs)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id", typeof(string));
            dt.Columns.Add("text", typeof(string));
            configs.Sections.Sort(CompareMajor);
            foreach (var t in configs.Sections)
            {
                var dr = dt.NewRow();
                RemoteConfigSectionParam section = t;
                dr["id"] = section.MajorVersion.ToString();
                dr["text"] = section.MajorVersion.ToString();
                dt.Rows.Add(dr);
            }

            return dt;
        }

        public DataTable GetMinors(string application, string sectionName, int major)
        {
            DataTable dt = new DataTable();
            RemoteConfigManagerDTO remoteConfigManagerDto = new RemoteConfigManagerDTO
            {
                Operation =
                {
                    Command = "getMinors", Condition = String.Format("{0}/{1}/{2}", application, sectionName, major),
                }
            };
            try
            {
                string URL = System.Configuration.ConfigurationManager.AppSettings["remoteConfigurationManagerUrl"] +
                             "ConfigManagerHandler.ashx";

                MemoryStream ms = new MemoryStream();
                XmlSerializer ser = new XmlSerializer(typeof(RemoteConfigManagerDTO));
                ser.Serialize(ms, remoteConfigManagerDto);

                HttpWebRequest req = (HttpWebRequest) HttpWebRequest.Create(URL);
                req.ContentType = "text/xml";
                req.Method = "POST";
                req.Timeout = 30000;
                req.ReadWriteTimeout = 30000;
                req.ContentLength = ms.Length;
                req.KeepAlive = false;

                using (Stream stream = req.GetRequestStream())
                {
                    byte[] buf = ms.ToArray();
                    stream.Write(buf, 0, buf.Length);
                    stream.Close();
                }

                RemoteConfigManagerDTO lstOutput;
                HttpWebResponse rsp = (HttpWebResponse) req.GetResponse();
                using (Stream stream = rsp.GetResponseStream())
                {
                    lstOutput = (RemoteConfigManagerDTO) ser.Deserialize(stream);
                    stream.Close();
                }

                rsp.Close();

                if (lstOutput.Operation.Result)
                {
                    dt = GetMinors(lstOutput.RemoteConfigSections, lstOutput.Operation.Log);
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error(ex);
            }

            return dt;
        }

        public bool CreateMinor(string application, string sectionName, int major, String cfg)
        {
            return CreateMinor(application, sectionName, major, Encoding.UTF8.GetBytes(cfg));
        }

        public bool CreateMinor(string application, string sectionName, int major, byte[] cfg)
        {
            try
            {
                RemoteConfigManagerDTO remoteConfigManagerDto = new RemoteConfigManagerDTO
                {
                    Operation =
                    {
                        Command = "createMinor",
                        Condition = String.Format("{0}/{1}/{2}", application, sectionName, major),
                        Value = cfg
                    }
                };

                string URL = System.Configuration.ConfigurationManager.AppSettings["remoteConfigurationManagerUrl"] +
                             "ConfigManagerHandler.ashx";

                MemoryStream ms = new MemoryStream();
                XmlSerializer ser = new XmlSerializer(typeof(RemoteConfigManagerDTO));
                ser.Serialize(ms, remoteConfigManagerDto);

                HttpWebRequest req = (HttpWebRequest) HttpWebRequest.Create(URL);
                req.ContentType = "text/xml";
                req.Method = "POST";
                req.Timeout = 30000;
                req.ReadWriteTimeout = 30000;
                req.ContentLength = ms.Length;
                req.KeepAlive = false;

                using (Stream stream = req.GetRequestStream())
                {
                    byte[] buf = ms.ToArray();
                    stream.Write(buf, 0, buf.Length);
                    stream.Close();
                }

                RemoteConfigManagerDTO lstOutput;
                HttpWebResponse rsp = (HttpWebResponse) req.GetResponse();
                using (Stream stream = rsp.GetResponseStream())
                {
                    lstOutput = (RemoteConfigManagerDTO) ser.Deserialize(stream);
                    stream.Close();
                }

                rsp.Close();

                if (lstOutput.Operation.Result)
                {
                    LoggerWrapper.Logger.Info(string.Format("新建配置：{0}", remoteConfigManagerDto.Operation.Condition));
                }
                else
                {
                    LoggerWrapper.Logger.Error(lstOutput.Operation.ResultInfo);
                }

                return true;
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error(ex);
            }

            return false;
        }

        public bool DeleteConfig(string application, String sectionName)
        {
            String condition = Path.Combine(string.Compare(application, NoAppPath) == 0 ? NoAppPath : application,
                sectionName);

            bool result = true;
            RemoteConfigManagerDTO remoteConfigManagerDto = new RemoteConfigManagerDTO();
            remoteConfigManagerDto.Operation.Command = "deleteConfig";
            remoteConfigManagerDto.Operation.Condition = condition;
            try
            {
                string URL = System.Configuration.ConfigurationManager.AppSettings["remoteConfigurationManagerUrl"] +
                             "ConfigManagerHandler.ashx";

                MemoryStream ms = new MemoryStream();
                XmlSerializer ser = new XmlSerializer(typeof(RemoteConfigManagerDTO));
                ser.Serialize(ms, remoteConfigManagerDto);

                HttpWebRequest req = (HttpWebRequest) HttpWebRequest.Create(URL);
                req.ContentType = "text/xml";
                req.Method = "POST";
                req.Timeout = 30000;
                req.ReadWriteTimeout = 30000;
                req.ContentLength = ms.Length;
                req.KeepAlive = false;

                using (Stream stream = req.GetRequestStream())
                {
                    byte[] buf = ms.ToArray();
                    stream.Write(buf, 0, buf.Length);
                    stream.Close();
                }

                RemoteConfigManagerDTO lstOutput;
                HttpWebResponse rsp = (HttpWebResponse) req.GetResponse();
                using (Stream stream = rsp.GetResponseStream())
                {
                    lstOutput = (RemoteConfigManagerDTO) ser.Deserialize(stream);
                    stream.Close();
                }

                rsp.Close();

                if (!lstOutput.Operation.Result)
                    result = false;
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error(ex);
            }

            return result;
        }

        private int CompareSection(RemoteConfigSectionParam rcsp1, RemoteConfigSectionParam rcsp2)
        {
            if (rcsp1.MajorVersion == rcsp2.MajorVersion)
                return rcsp2.MinorVersion - rcsp1.MinorVersion;
            return rcsp2.MajorVersion - rcsp1.MajorVersion;
        }

        private int GetAppCount(RemoteConfigSectionCollection configs, string curSectionName)
        {
            int num = 0;
            foreach (RemoteConfigSectionParam current in configs.Sections)
            {
                string[] array = current.SectionName.Split("\\".ToCharArray());
                if (string.Compare(curSectionName, array[0]) == 0)
                {
                    num++;
                }
            }

            return num;
        }

        public bool IsStandardXML(byte[] cfg)
        {
            try
            {
                MemoryStream stream = new MemoryStream(cfg);
                XmlDocument doc = new XmlDocument();
                doc.Load(stream);
                return true;
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error(ex);
                return false;
            }
        }
    }
}