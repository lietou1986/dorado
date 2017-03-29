using System;

namespace Dorado.Configuration
{
    public class RemoteConfigKeyAttribute : Attribute
    {
        public RemoteConfigKeyAttribute(string configKey)
        {
            ConfigKey = configKey;
        }

        public string ConfigKey { get; set; }
    }
}