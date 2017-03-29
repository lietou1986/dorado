using System;

namespace Dorado.Configuration
{
    /// <summary>
    /// 保持主版本兼容
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ConfigurationVersionAttribute : Attribute
    {
        public ConfigurationVersionAttribute()
        {
            majorVersion = 1;
        }

        public ConfigurationVersionAttribute(int majorVersion)
        {
            this.majorVersion = majorVersion;
        }

        private int majorVersion;

        /// <summary>
        /// 这是主版本
        /// </summary>
        public int MajorVersion
        {
            get { return majorVersion; }
            set { majorVersion = value; }
        }
    }
}