using System;

namespace Dorado.Configuration.Exceptions
{
    [Serializable]
    public class InvalidConfigurationException : CoreException
    {
        public InvalidConfigurationException()
            : base("无效的配置文件")
        {
        }

        public InvalidConfigurationException(string message)
            : base(message)
        {
        }
    }
}