using System;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Configuration;
using System.Xml;

namespace Dorado.Configuration.ConfigurationManager
{
    /// <summary>
    /// 本地配置文件管理类，用来管理本地配置文件
    /// </summary>
    public class LocalConfigurationManager : BaseConfigurationManager
    {
        #region Singleton

        private LocalConfigurationManager()
        {
        }

        private static LocalConfigurationManager instance;

        public static LocalConfigurationManager Instance
        {
            get { return instance; }
        }

        static LocalConfigurationManager()
        {
            _systemConfig = GetExeConfig();
            localBaseConfigFolder = Path.GetDirectoryName(_systemConfig.FilePath);
            instance = new LocalConfigurationManager();
            XmlDocument doc = new XmlDocument();
            if (File.Exists(_systemConfig.FilePath))
            {
                doc.Load(_systemConfig.FilePath);
            }
            else
            {
                doc.LoadXml("<configuration/>");
            }
            _systemConfigXml = doc;
        }

        #endregion Singleton

        private static string localBaseConfigFolder;

        public static string LocalBaseConfigFolder
        {
            get
            {
                return localBaseConfigFolder;
            }
        }

        public static string Combine(string folder, string file)
        {
            return ConfigUtility.Combine(folder, file);
        }

        public static string MapConfigPath(string fileName)
        {
            return Combine(localBaseConfigFolder, fileName);
        }

        #region 本地主配置文件

        private static System.Configuration.Configuration _systemConfig;

        private static System.Configuration.Configuration GetExeConfig()
        {
            string AppVirtualPath = HttpRuntime.AppDomainAppVirtualPath;
            if ((AppVirtualPath != null) && (0 < AppVirtualPath.Length))
            {
                //使用根web.config文件
                return WebConfigurationManager.OpenWebConfiguration(AppVirtualPath);
            }
            else
            {
                return System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }
        }

        internal static System.Configuration.Configuration LocalMainConfig
        {
            get
            {
                return _systemConfig;
            }
        }

        private static XmlDocument _systemConfigXml;

        internal static XmlDocument SystemConfigXml
        {
            get
            {
                return _systemConfigXml;
            }
        }

        #endregion 本地主配置文件

        private static string GetSectionConfigSource(string name)
        {
            XmlNodeList nodeList = _systemConfigXml.DocumentElement.GetElementsByTagName(name);
            if (nodeList.Count == 0)
                return string.Empty;

            //XmlElement elm =(XmlElement)_systemConfigXml.DocumentElement.SelectSingleNode(name);
            //if (elm == null) return null;
            XmlElement elm = (XmlElement)nodeList[0];
            return elm.GetAttribute("configSource");
        }

        private static string GetConfigSectionFileName(string name)
        {
            string configSource = GetSectionConfigSource(name);

            //string folder = Path.GetDirectoryName( _systemConfig.FilePath );
            if (configSource.Length == 0)
                return string.Empty;
            else

                //return Combine( folder , configSource );
                return Combine(localBaseConfigFolder, configSource);
        }

        /// <summary>
        /// 重写基类OnCreate方法来创建本地配置实例
        /// </summary>
        /// <param name="sectionName">节点名称</param>
        /// <param name="type">The type.</param>
        /// <param name="major">主版本号</param>
        /// <param name="minor">次版本号</param>
        /// <returns></returns>
        protected override object OnCreate(string sectionName, Type type, out int major, out int minor)
        {
            major = XmlSerializerSectionHandler.GetConfigurationClassMajorVersion(type);
            minor = XmlSerializerSectionHandler.DefaultUninitMinorVersion;
            string configPath = GetConfigSectionFileName(sectionName);

            if (configPath.Length == 0)
                return null;

            object retVal;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configPath);

                //初始化配置实例
                retVal = XmlSerializerSectionHandler.GetConfigInstance(doc.DocumentElement, type, out major, out minor);

                // XmlSerializerSectionHandler.GetConfigVersion( doc.DocumentElement , out major , out minor );
            }
            catch (Exception ex)
            {
                HandleException(ex, "创建本地配置时出错: sectionName=" + sectionName + ",type=" + type.Name + ", 会创建空实例替代", sectionName);

                //出现异常通过反射初始化为空配置实例
                retVal = Activator.CreateInstance(type);
            }

            //为配置文件添加FileWatcher
            ConfigWatcher.SetupWatcher(configPath, retVal);
            return retVal;
        }
    }
}