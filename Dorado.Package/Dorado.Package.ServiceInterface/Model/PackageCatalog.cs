using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dorado.Package.ServiceInterface.Model
{
    /// <summary>
    /// 目录，支持A/B/C/D,n级多层目录
    /// </summary>
    [DataContract(Namespace = "http://www.Dorado.com")]
    public class PackageCatalog
    {
        public PackageCatalog()
        {
            ListPackageFileInfo = new List<PackageFileInfo>();
        }

        /// <summary>
        /// 包内目录文件夹结构
        /// </summary>
        [DataMember]
        public string CatalogLevelStringName { get; set; }

        /// <summary>
        /// 需要打包的编号
        /// </summary>
        [DataMember]
        public List<PackageFileInfo> ListPackageFileInfo { get; set; }
    }
}