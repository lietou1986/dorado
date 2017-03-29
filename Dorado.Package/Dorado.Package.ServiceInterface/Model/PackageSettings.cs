using System.Xml.Serialization;
using Dorado.Configuration;

namespace Dorado.Package.ServiceInterface.Model
{
    [XmlRoot("PackageSettings")]
    public class PackageSettings : BaseConfig<PackageSettings>
    {
        [XmlElement("PackageTempPath")]
        public string PackageTempPath;

        [XmlElement("PackageQueue")]
        public string PackageQueue;

        [XmlElement("PackagePath")]
        public string PackagePath;

        [XmlElement("HttpBaseAddress")]
        public string HttpBaseAddress;

        [XmlElement("TaskInterval")]
        public int TaskInterval;

        [XmlElement("MaxPackFile")]
        public int MaxPackFile;
    }

    public class Provider
    {
        /// <summary>
        /// 组件名称,一个产品下可以有多个打包组件
        /// </summary>
        [XmlAttribute("Name")]
        public string Name;

        /// <summary>
        /// 打包组件主实体类
        /// </summary>
        [XmlAttribute("Type")]
        public string Type;

        /// <summary>
        /// 打包组件名字空间
        /// </summary>
        [XmlAttribute("Namespace")]
        public string Namespace;
    }
}