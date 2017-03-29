using System.Runtime.Serialization;

namespace Dorado.ESB.Common.Utility
{
    [DataContract(Namespace = "http://Dorado.com/CustomFault")]
    public class CustomFault
    {
        [DataMember]
        public int Code { get; set; }

        [DataMember]
        public string Reason { get; set; }
    }
}