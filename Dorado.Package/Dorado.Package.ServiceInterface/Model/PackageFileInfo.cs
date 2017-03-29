using System.Runtime.Serialization;

namespace Dorado.Package.ServiceInterface.Model
{
    /// <summary>
    /// 需要打包的编号,根据此编号到DFS、数据库、磁盘等提取文件
    /// </summary>
    [DataContract(Namespace = "http://www.Dorado.com")]
    public class PackageFileInfo
    {
        [DataMember]
        public string IdentID { get; set; }

        [DataMember]
        public string Path { get; set; }
    }
}