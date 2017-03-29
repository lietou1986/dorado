using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml.Serialization;
using Dorado.Configuration.ConfigurationManager;

namespace Dorado.Configuration
{
    public class ConnectionStringEntry
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("connectionString")]
        public string ConnectionString;

        [XmlAttribute("providerName")]
        public string ProviderName;
    }

    /// <summary>
    /// 数据库链接字符串集合
    /// </summary>
    [XmlRoot(ConnectionStringCollection.SectionName)]
    public class ConnectionStringCollection : IPostSerializer
    {
        private const string SectionName = "connectionStrings";
        private static ConnectionStringCollection instance;

        public static ConnectionStringCollection Using(string sectionName = SectionName)
        {
            instance = RemoteConfigurationManager.Instance.GetSection<ConnectionStringCollection>(sectionName);
            return instance;
        }

        private static EventHandler _handler;

        public static void RegisterConfigChangedNotification(EventHandler handler)
        {
            _handler += handler;
        }

        public static ConnectionStringCollection Instance
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

        [XmlElement]
        public bool EnabledDatabaseConnectivityState = false;

        [XmlElement("add")]
        public ConnectionStringEntry[] Entries;

        private NameValueCollection _collection = new NameValueCollection();

        [XmlIgnore]
        public NameValueCollection Collection
        {
            get { return _collection; }
            set { _collection = value; }
        }

        [XmlIgnore]
        public string this[string name]
        {
            get
            {
                return _collection[name];
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        #region IPostSerializer 成员

        /// <summary>
        /// 与本地数据库连接字符串集合合并，如果名称冲突则以本地为准
        /// </summary>
        public void PostSerializer()
        {
            if (Entries != null)
            {
                foreach (ConnectionStringEntry entry in Entries)
                {
                    _collection[entry.Name] = entry.ConnectionString;
                }
            }

            foreach (ConnectionStringSettings entry in System.Configuration.ConfigurationManager.ConnectionStrings)
            {
                _collection[entry.Name] = entry.ConnectionString;
            }
        }

        #endregion IPostSerializer 成员
    }
}