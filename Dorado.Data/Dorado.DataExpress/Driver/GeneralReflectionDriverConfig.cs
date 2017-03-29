using System.Runtime.Serialization;

namespace Dorado.DataExpress.Driver
{
    [DataContract]
    public class GeneralReflectionDriverConfig
    {
        [DataMember(Name = "ConnectionProvider")]
        public string ConnectionProviderType
        {
            get;
            set;
        }

        [DataMember(Name = "CommandProvider")]
        public string CommandProviderType
        {
            get;
            set;
        }

        [DataMember(Name = "DataAdapter")]
        public string DataAdapterProviderType
        {
            get;
            set;
        }
    }
}