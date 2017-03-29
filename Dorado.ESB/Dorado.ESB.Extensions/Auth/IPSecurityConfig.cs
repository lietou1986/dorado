using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Dorado.Configuration.ConfigurationManager;

namespace Dorado.ESB.Extensions.Auth
{
    /// <summary>
    /// ip安全配置
    /// </summary>
    public class IPSecurityConfig
    {
        /// <summary>
        /// 读取PlatformServicesIPSecurityConfig节
        /// </summary>
        private static IPSecurityConfig instance = ConfigManager.GetSection<IPSecurityConfig>("PlatformServicesIPSecurityConfig");

        public static IPSecurityConfig Instance
        {
            get
            {
                return instance;
            }
            set
            {
                instance = value;

                //调入安全
                IPSecurityManager.Instance.OnConfigReload(value, EventArgs.Empty);
            }
        }

        /// <summary>
        /// ip配置列表
        /// </summary>
        [XmlElement("IPSet")]
        public List<IPSetConfig> IPSets = new List<IPSetConfig>();
    }

    public class IPSetConfig
    {
        [XmlAttribute("Math")]
        public string Math;

        /// <summary>
        /// 允许访问的方法
        /// </summary>
        [XmlElement("AllowMethodList")]
        public string AllowMethodList;
    }
}