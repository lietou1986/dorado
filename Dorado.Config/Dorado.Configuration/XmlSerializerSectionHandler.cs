using Dorado.Configuration.ConfigurationManager;
using System;
using System.Configuration;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Dorado.Configuration
{
    /// <summary>
    /// ConfigutationSectionHandler 通过使用xml序列化的方式来把配置信息映射到配置节点中定义的特定类中
    ///  如何使用可以参考：
    /// http://technet.microsoft.com/zh-cn/exchange/2tw134k3(VS.80).aspx
    ///这个类在 .NET Framework 2.0中已经广泛使用
    ///代码有点乱有时间重构
    /// </summary>
    public class XmlSerializerSectionHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            object retVal = GetConfigInstance(section);

            System.Configuration.Configuration config = LocalConfigurationManager.LocalMainConfig;

            //SectionInformation info = config.GetSection(section.Name).SectionInformation;
            ConfigurationSection configSection = config.GetSection(section.Name);
            if (configSection.SectionInformation.RestartOnExternalChanges == false)
            {
                string filePath = GetConfigFilePath(config, configSection);
                ConfigWatcher.SetupWatcher(filePath, retVal);
            }

            return retVal;
        }

        private static T GetConfigInstance<T>(XmlNode section)
        {
            return (T)GetConfigInstance(section, typeof(T));
        }

        public static int GetConfigurationClassMajorVersion<T>()
        {
            return GetConfigurationClassMajorVersion(typeof(T));
        }

        internal const int DefaultMajorVersion = 1,
            DefaultMinorVersion = 1,
            DefaultUninitMinorVersion = 0;

        public static int GetConfigurationClassMajorVersion(Type type)
        {
            object[] objAttrs = type.GetCustomAttributes(typeof(ConfigurationVersionAttribute), false);
            if (objAttrs == null || objAttrs.Length == 0)
                return DefaultMajorVersion;
            else
                return ((ConfigurationVersionAttribute)objAttrs[0]).MajorVersion;
        }

        /*
        private static void GetVersion(string version,out int majorVersion,out int minorVersion)
        {
            string[] strs = version.Trim().Split('.');
            if (strs.Length < 1)
            {
                majorVersion = 1;
                minorVersion = 1;
            }
            else if (!int.TryParse(strs[0], out majorVersion))
            {
                majorVersion = 1;
                minorVersion = 1;
            }
            else
            {
                if (strs.Length <2 || !int.TryParse(strs[1], out minorVersion))
                    minorVersion = 1;
            }
        }

        internal static string GetConfigVersion(XmlElement section)
        {
            string version = section.GetAttribute("version");
            int majorVersion,minorVersion;
            GetVersion(version, out majorVersion, out minorVersion);
            return majorVersion + "." + minorVersion;
        }
         * */

        internal static void GetConfigVersion(XmlElement section, out int major, out int minor)
        {
            if (!int.TryParse(section.GetAttribute("majorVersion"), out major))
                major = DefaultMajorVersion;
            if (!int.TryParse(section.GetAttribute("minorVersion"), out minor))
                minor = DefaultMinorVersion;
        }

        /// <summary>
        /// 取得配置实例
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static object GetConfigInstance(XmlNode section, Type t)
        {
            int fileMajorVersion, fileMinorVersion;
            return GetConfigInstance(section, t, out  fileMajorVersion, out  fileMinorVersion);
        }

        public static object GetConfigInstance(XmlNode section, Type t, out int major, out int minor)
        {
            //string version = ((XmlElement)section).GetAttribute("version");
            GetConfigVersion((XmlElement)section, out major, out minor);
            int clsMajorVersion = GetConfigurationClassMajorVersion(t);
            if (major != clsMajorVersion)
                throw new VersionIncompatibleException("主版本与该文件版本不匹配", clsMajorVersion, minor);

            try
            {
                XmlSerializer ser = new XmlSerializer(t);
                object obj = ser.Deserialize(new XmlNodeReader(section));
                if (obj is IPostSerializer)
                {
                    ((IPostSerializer)obj).PostSerializer();
                }
                return obj;
            }
            catch (Exception ex)
            {
                Exception innerEx = ex;
                if (ex.InnerException != null)
                    innerEx = ex.InnerException;

                throw new ConfigurationErrorsException(string.Format(
                    "XmlSerializerSectionHandler 未能从 '{0}' 创建类型.  异常: \r\n{1}", t.FullName, innerEx.ToString()), innerEx);
            }
        }

        public static T GetConfigInstance<T>(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            return GetConfigInstance<T>(doc.DocumentElement);
        }

        private static object GetConfigInstance(XmlNode section)
        {
            XPathNavigator nav = section.CreateNavigator();
            string typeName = (string)nav.Evaluate("string(@type)");
            Type t = Type.GetType(typeName);

            if (t == null)
                throw new ConfigurationErrorsException("XmlSerializerSectionHandler未能创建类型 '" + typeName +
                    "'.  请确保这是个有效的字符串类型.", section);

            return GetConfigInstance(section, t);
        }

        private static string GetConfigFilePath(System.Configuration.Configuration confFile, ConfigurationSection section)
        {
            string configSource = section.SectionInformation.ConfigSource;
            if (configSource == String.Empty)
            {
                return Path.GetFullPath(confFile.FilePath);
            }
            else
            {
                return ConfigUtility.Combine(Path.GetDirectoryName(confFile.FilePath), configSource);
            }
        }
    }
}