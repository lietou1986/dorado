using System;

namespace Dorado.Configuration
{
    /// <summary>
    /// 配置实体
    /// </summary>
    internal class ConfigEntry
    {
        //通过委托延迟初始化配置对象
        public delegate object CreateObjectDelegate(string sectionName, Type type,
            out int majorVersion, out int minorVersion);

        public string Name;

        private int major;

        public int MajorVersion
        {
            get
            {
                int ret;
                ret = major;
                return ret;
            }
            set
            {
                major = value;
            }
        }

        private int minor;

        public int MinorVersion
        {
            get
            {
                int ret;
                ret = minor;
                return ret;
            }
            set
            {
                minor = value;
            }
        }

        public Type EntryType
        {
            get
            {
                return type;
            }
        }

        private bool isSet;

        /// <summary>
        /// 判断是否已经设置配置实例
        /// </summary>
        public bool IsSet
        {
            get
            {
                bool ret;
                ret = isSet;
                return ret;
            }
        }

        private object locker;
        private Type type;
        private CreateObjectDelegate OnCreate;

        public ConfigEntry(string sectionName, Type type, CreateObjectDelegate creater)
        {
            this.Name = sectionName;
            this.type = type;

            isSet = false;
            locker = new object();
            OnCreate = creater;
        }

        private object val;

        /// <summary>
        /// 延迟初始化配置对象
        /// </summary>
        public object Value
        {
            get
            {
                object ret;
                if (isSet)
                    ret = val;
                else
                {
                    lock (locker)
                    {
                        if (isSet) //双重确认还是为了性能
                            ret = val;
                        else
                        {
                            val = OnCreate(this.Name, this.type, out major, out minor);
                            isSet = true;
                        }
                    }
                }
                return val;
            }
        }
    }
}