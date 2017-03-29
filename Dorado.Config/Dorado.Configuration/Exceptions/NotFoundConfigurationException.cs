using System;

namespace Dorado.Configuration.Exceptions
{
    [Serializable]
    public class NotFoundConfigurationException : CoreException
    {
        public NotFoundConfigurationException()
            : base("找不到配置文件！")
        {
        }

        public NotFoundConfigurationException(string configName)
            : base(string.Format("找不到配置文件->{0}！", configName))
        {
        }
    }
}