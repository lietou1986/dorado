using System.Runtime.Serialization;

namespace Dorado.Package.ServiceInterface.Model
{
    [DataContract(Namespace = "http://www.Dorado.com")]
    public enum PackagePriority
    {
        [EnumMember]
        Lowest = 1,

        [EnumMember]
        Normal = 2,

        [EnumMember]
        Highest = 3
    }
}