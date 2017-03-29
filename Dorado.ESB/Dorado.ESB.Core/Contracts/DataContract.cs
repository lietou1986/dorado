using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dorado.ESB.Core.Contracts
{
    [Serializable]
    [DataContract(Namespace = "urn:Dorado.platformservices")]
    public sealed class ServiceConfigData
    {
        /// <summary>
        /// 名称code
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 服务类型
        /// </summary>
        [DataMember]
        public string ServiceTypes { get; set; }

        /// <summary>
        /// 服务namespace
        /// </summary>
        [DataMember]
        public string ServiceNameSpaces { get; set; }

        /// <summary>
        /// 产生代码服务类型
        /// </summary>
        [DataMember]
        public string GenerateWcfServiceType { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        [DataMember]
        public string Topic { get; set; }

        /// <summary>
        /// 配置
        /// </summary>
        [DataMember]
        public string Config { get; set; }

        /// <summary>
        /// Assembly目录
        /// </summary>
        [DataMember]
        public string AssemblyFolderName { get; set; }

        /// <summary>
        /// Assembly names
        /// </summary>
        [DataMember]
        public string AssemblyNames { get; set; }

        /// <summary>
        /// wcf服务类型
        /// </summary>
        [DataMember]
        public string WcfType { get; set; }

        [DataMember]
        public bool Wrapper { get; set; }

        [DataMember]
        public string ServiceInterfaceType { get; set; }

        [DataMember]
        public bool IsAuthorization { get; set; }
    }

    [DataContract(Namespace = "urn:Dorado.platformservices")]
    public class ServiceMetadataBase
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ServiceType { get; set; }

        [DataMember]
        public string ServiceNamespaces { get; set; }

        [DataMember]
        public string GenerateWcfServiceType { get; set; }

        [DataMember]
        public string Topic { get; set; }

        [DataMember]
        public string AppDomainHostName { get; set; }

        [DataMember]
        public string BaseAddresses { get; set; }

        [DataMember]
        public string AssemblyFolderName { get; set; }

        [DataMember]
        public string AssemblyNames { get; set; }

        [DataMember]
        public string WcfType { get; set; }

        [DataMember]
        public bool Wrapper { get; set; }

        [DataMember]
        public string ServiceInterfaceType { get; set; }

        [DataMember]
        public bool IsAuthorization { get; set; }
    }

    [DataContract(Namespace = "urn:Dorado.platformservices")]
    public class ServiceMetadata : ServiceMetadataBase
    {
        [DataMember]
        public string Config { get; set; }
    }

    [CollectionDataContract(Namespace = "urn:Dorado.platformservices", ItemName = "ServiceMetadata")]
    [KnownType(typeof(ServiceMetadata))]
    public class ServiceMetadataCollection : List<ServiceMetadata>
    {
    }

    [DataContract(Namespace = "urn:Dorado.platformservices")]
    public class HostMetadata
    {
        private ServiceMetadataCollection _list;

        /// <summary>
        /// host名称
        /// </summary>
        [DataMember]
        public string HostName { get; set; }

        /// <summary>
        /// 应用程序名称
        /// </summary>
        [DataMember]
        public string ApplicationName { get; set; }

        /// <summary>
        /// 计算机名称
        /// </summary>
        [DataMember]
        public string MachineName { get; set; }

        /// <summary>
        /// AssemblyNames
        /// </summary>
        [DataMember]
        public string AssemblyNames { get; set; }

        /// <summary>
        /// 元数据列表
        /// </summary>
        [DataMember]
        public ServiceMetadataCollection List
        {
            get { return _list; }
            set { _list = value; }
        }

        public HostMetadata()
        {
            _list = new ServiceMetadataCollection();
        }
    }
}