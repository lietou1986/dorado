using System;
using System.Xml.Serialization;

namespace Dorado.Web.Fileset
{
    [Flags]
    [Serializable]
    public enum StaticFileType
    {
        ///<summary>
        ///</summary>
        [XmlEnum("css")]
        Css = 1 << 1,

        ///<summary>
        ///</summary>
        [XmlEnum("javascript")]
        Javascript = 1 << 2,

        ///<summary>
        ///</summary>
        [XmlEnum("group")]
        Group = 1 << 3,

        ///<summary>
        ///</summary>
        [XmlEnum("other")]
        Other = 1 << 4
    }
}