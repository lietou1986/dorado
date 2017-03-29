using System;
using System.Xml.Serialization;

namespace Dorado.Web.Fileset
{
    [Flags]
    [Serializable]
    public enum StaticFileFlag
    {
        /// <summary>
        /// 未指定(既指定到参考)
        /// </summary>
        [XmlEnum("none")]
        None = 0,

        /// <summary>
        /// 参考（外部引用）
        /// </summary>
        [XmlEnum("ref")]
        Reference = 1 << 1,

        /// <summary>
        /// 嵌入
        /// </summary>
        [XmlEnum("embed")]
        Embed = 1 << 2,

        /// <summary>
        /// 本地文件(Web应用内可直接读取的文件)
        /// </summary>
        [XmlEnum("local")]
        Local = 1 << 3,

        /// <summary>
        /// 远程文件
        /// </summary>
        [XmlEnum("remote")]
        Remote = 1 << 4,
    }
}