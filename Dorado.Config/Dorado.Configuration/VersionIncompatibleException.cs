using System;

namespace Dorado.Configuration
{
    /// <summary>
    /// 版本不兼容异常类
    /// </summary>
    public class VersionIncompatibleException : ApplicationException
    {
        public VersionIncompatibleException(string msg)
            : base(msg)
        {
        }

        public VersionIncompatibleException(string msg, int versionInClass, int versionInConfig)
            : base(msg)
        {
            VersionInClass = versionInClass;
            VersionInConfig = versionInConfig;
        }

        public int VersionInClass, VersionInConfig;
    }
}