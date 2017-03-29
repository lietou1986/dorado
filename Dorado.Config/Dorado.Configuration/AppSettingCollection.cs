using System;
using System.Collections.Specialized;
using System.Xml.Serialization;
using Dorado.Configuration.ConfigurationManager;

namespace Dorado.Configuration
{
    public class AppSettingEntry
    {
        [XmlAttribute("key")]
        public string Key;

        [XmlAttribute("value")]
        public string Value;
    }

    /// <summary>
    /// 处理配置文件中的AppSetting节点键值对
    /// </summary>
    [XmlRoot(AppSettingCollection.SectionName)]
    public class AppSettingCollection : IPostSerializer
    {
        private const string SectionName = "appSettings";

        public static AppSettingCollection Using(string sectionName = SectionName)
        {
            instance = RemoteConfigurationManager.Instance.GetSection<AppSettingCollection>(sectionName);
            return instance;
        }

        private static AppSettingCollection instance;

        public static AppSettingCollection Instance
        {
            get
            {
                if (instance == null)
                    Using();
                return instance;
            }
            set
            {
                instance = value;
                if (_handler != null)
                    _handler(value, EventArgs.Empty);
            }
        }

        private static EventHandler _handler;

        public static void RegisterConfigChangedNotification(EventHandler handler)
        {
            _handler += handler;
        }

        private NameValueCollection _collection = new NameValueCollection();

        [XmlIgnore]
        public NameValueCollection Collection
        {
            get { return _collection; }
            set { _collection = value; }
        }

        [XmlElement("add")]
        public AppSettingEntry[] Entries;

        [XmlIgnore]
        public string this[string key]
        {
            get
            {
                return _collection[key];
            }
        }

        #region IPostSerializer 成员

        public void PostSerializer()
        {
            if (Entries != null)
            {
                foreach (AppSettingEntry entry in Entries)
                {
                    _collection[entry.Key] = entry.Value;
                }
            }

            //与本地的配置文件中的AppSetting合并在一起
            foreach (string key in System.Web.Configuration.WebConfigurationManager.AppSettings)
            {
                _collection[key] = System.Web.Configuration.WebConfigurationManager.AppSettings[key];
            }
        }

        #endregion IPostSerializer 成员
    }
}