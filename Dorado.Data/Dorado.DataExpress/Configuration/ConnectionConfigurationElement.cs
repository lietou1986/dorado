using Dorado.DataExpress.Utility;
using System.Configuration;

namespace Dorado.DataExpress.Configuration
{
    public class ConnectionConfigurationElement : ConfigurationElement
    {
        private const string EncrypteKey = "xw_xud_&^(__";

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get
            {
                return base["value"] as string;
            }
        }

        [ConfigurationProperty("encrypted", DefaultValue = false)]
        public bool Encrypted
        {
            get
            {
                return (bool)base["encrypted"];
            }
        }

        public override string ToString()
        {
            if (this.Encrypted)
            {
                return TripleDes.Decrypt(this.Value, "xw_xud_&^(__");
            }
            return this.Value;
        }
    }
}