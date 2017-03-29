using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Dorado.Configuration.ConfigurationManager
{
    /// <summary>
    /// 配置文件管理基类
    /// </summary>
    public abstract class BaseConfigurationManager
    {
        internal Dictionary<string, ConfigEntry> configEntries;
        protected object configLocker;

        protected BaseConfigurationManager()
        {
            configEntries = new Dictionary<string, ConfigEntry>();
            configLocker = new object();
        }

        //OnCreate执行时才真正初始化配置实例达到延迟初始化的效果，子类可以重写
        //下载方法在子类实现
        protected abstract object OnCreate(string sectionName, Type type, out int major, out int minor);

        /// <summary>
        /// 获取配置条目
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <returns></returns>
        internal ConfigEntry GetEntry(string sectionName)
        {
            sectionName = sectionName.ToLower();
            ConfigEntry entry;
            lock (configLocker)
            {
                configEntries.TryGetValue(sectionName, out entry);
            }
            return entry;
        }

        /// <summary>
        /// 取得配置实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        public T GetSection<T>(string section)
        {
            string sectionName = section.ToLower();
            ConfigEntry entry = GetEntry(sectionName);
            if (entry == null)
            {
                lock (configLocker)
                {
                    if (!configEntries.TryGetValue(sectionName, out entry))
                    {
                        entry = new ConfigEntry(section, typeof(T), OnCreate);
                        configEntries.Add(sectionName, entry);
                    }
                }
            }

            //通过Value属性得到配置实例，达到延迟化的同时可以进行一些必要的预处理操作
            return (T)entry.Value;
        }

        public static void HandleException(Exception ex, string msg, string sectionName)
        {
            //防止递归调用创建ErrorTrackerConfig，一切为了性能
            if (sectionName != "ErrorTrackerConfig")
                LoggerWrapper.Logger.Error("ConfigurationManager->" + new ConfigurationErrorsException(msg, ex));
            else
                LoggerWrapper.Logger.Info(msg);
        }
    }
}