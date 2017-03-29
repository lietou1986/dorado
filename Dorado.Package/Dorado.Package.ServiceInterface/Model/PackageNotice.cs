using System.Runtime.Serialization;

namespace Dorado.Package.ServiceInterface.Model
{
    [DataContract(Namespace = "http://www.Dorado.com")]
    public class PackageNotice
    {
        [DataMember]
        public string Subject { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public string SubjectFailed
        {
            get;
            set;
        }

        [DataMember]
        public string ContentFailed
        {
            get;
            set;
        }

        [DataMember]
        public string ReceiverId { get; set; }

        [DataMember]
        public int TenantID { get; set; }
    }
}