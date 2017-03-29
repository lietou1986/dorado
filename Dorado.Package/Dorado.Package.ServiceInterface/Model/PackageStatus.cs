using System.Runtime.Serialization;

namespace Dorado.Package.ServiceInterface.Model
{
    /// <summary>
    /// 用于生成报告时的状态与打包状态
    /// </summary>
    [DataContract(Namespace = "http://www.Dorado.com")]
    public enum PackageStatus
    {
        [EnumMember]
        UnProcessed = 0,

        [EnumMember]
        Processing = 1,

        [EnumMember]
        Processed = 2,

        [EnumMember]
        Faulted = 3,

        [EnumMember]
        FetchTask = 4
    }
}