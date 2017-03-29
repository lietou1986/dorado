using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Dorado.Configuration
{
    public class RemoteConfigSectionParam
    {
        [XmlAttribute("name")]
        public string SectionName;

        [XmlAttribute("majorVerion")]
        public int MajorVersion;

        [XmlAttribute("minorVerion")]
        public int MinorVersion;

        [XmlAttribute("downloadUrl")]
        public string DownloadUrl;
    }

    public class RemoteConfigSectionCollection
    {
        [XmlAttribute("machine")]
        public string Machine;

        [XmlAttribute("application")]
        public string Application;

        [XmlElement("section")]
        public List<RemoteConfigSectionParam> Sections;

        public string CurrEnvironment { get; set; }

        public int Count
        {
            get
            {
                return Sections.Count;
            }
        }

        public RemoteConfigSectionParam this[int index]
        {
            get
            {
                return Sections[index];
            }
        }

        public void AddSection(string sectionName, int major, int minor)
        {
            AddSection(sectionName, major, minor, null);
        }

        public void AddSection(string sectionName, int major, int minor, string url)
        {
            RemoteConfigSectionParam param = new RemoteConfigSectionParam();
            param.SectionName = sectionName;
            param.MajorVersion = major;
            param.MinorVersion = minor;
            param.DownloadUrl = url;
            Sections.Add(param);
        }

        public void AddSection(string sectionName, int major, int minor, string url, string currentSetting)
        {
            RemoteConfigSectionParam param = new RemoteConfigSectionParam();
            param.SectionName = sectionName;
            param.MajorVersion = major;
            param.MinorVersion = minor;
            param.DownloadUrl = url;
            Sections.Add(param);
        }

        public RemoteConfigSectionCollection()
        {
            Machine = Environment.MachineName;
            Sections = new List<RemoteConfigSectionParam>();
        }

        public RemoteConfigSectionCollection(string appName)
            : this()
        {
            this.Application = appName;
        }

        public RemoteConfigSectionCollection(string appName, string environment)
            : this()
        {
            this.Application = appName;
            this.CurrEnvironment = environment;
        }

        public IEnumerator<RemoteConfigSectionParam> GetEnumerator()
        {
            return Sections.GetEnumerator();
        }
    }

    /// <summary>
    /// 权限
    /// </summary>
    public class PermissionsDTO
    {
        private string userID;

        [XmlAttribute("UserID")]
        public string UserID
        {
            get { return userID; }
            set { userID = value; }
        }
    }
}