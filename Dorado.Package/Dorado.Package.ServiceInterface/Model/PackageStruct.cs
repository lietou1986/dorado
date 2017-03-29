using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dorado.Package.ServiceInterface.Model
{
    /// <summary>
    /// 打包的结构信息(可以自定义包中的结构)
    /// </summary>
    [DataContract(Namespace = "http://www.Dorado.com")]
    public class PackageStruct
    {
        public PackageStruct()
        {
            ListPackageCatalog = new List<PackageCatalog>();
        }

        [DataMember]
        public string PackName { get; set; }

        [DataMember]
        public PackagePriority PackagePriority { get; set; }

        [DataMember]
        public PackageType PackageType { get; set; }

        [DataMember]
        public List<PackageCatalog> ListPackageCatalog { get; set; }
    }
}