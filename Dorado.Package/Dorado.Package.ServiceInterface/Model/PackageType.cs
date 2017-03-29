using System.Runtime.Serialization;

namespace Dorado.Package.ServiceInterface.Model
{
    [DataContract(Namespace = "http://www.Dorado.com")]
    public enum PackageType
    {
        [EnumMember]
        Report = 0,

        [EnumMember]
        Recruit = 1,

        [EnumMember]
        Assessment = 2,
    }
}