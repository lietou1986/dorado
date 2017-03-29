using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace Dorado.Configuration
{
    internal class ConfigInstances
    {
        private Dictionary<string, object> configInstances = new Dictionary<string, object>();
        private static readonly RwLocker rwLock = new RwLocker();

        public object this[string path]
        {
            get
            {
                if (path == null)
                    return null;
                else
                    path = path.ToLower();
                object objRet;

                using (rwLock.Read())
                {
                    configInstances.TryGetValue(path, out objRet);
                }
                return objRet;
            }
        }

        public bool ContainsKey(string path)
        {
            if (path == null)
                return false;
            else
                path = path.ToLower();

            bool contains;
            using (rwLock.Read())
            {
                contains = configInstances.ContainsKey(path);
            }
            return contains;
        }

        public bool Add(string path, object obj)
        {
            if (path == null)
                return false;
            else
                path = path.ToLower();

            bool added;
            using (rwLock.Write())
            {
                if (configInstances.ContainsKey(path))
                    added = false;
                else
                {
                    configInstances.Add(path, obj);
                    added = true;
                }
            }
            Counter(path);
            return added;
        }

        [Conditional("DEBUG")]
        private void Counter(string path)
        {
            LoggerWrapper.Logger.Debug("新添加配置实例：" + path);
            LoggerWrapper.Logger.Debug("配置实例数量：" + configInstances.Count);
        }
    }

    internal class FileChangedEventArgs : EventArgs
    {
        public string FileName;

        public FileChangedEventArgs(string fileName)
        {
            this.FileName = fileName;
        }
    }

    /// <summary>
    /// 配置更改监测类
    /// </summary>
    public class ConfigWatcher
    {
        #region Singleton

        private ConfigWatcher()
        {
        }

        private static ConfigWatcher instance = new ConfigWatcher();

        public static ConfigWatcher Instance
        {
            get { return instance; }
        }

        static ConfigWatcher()
        {
        }

        #endregion Singleton

        private const int ChangeConfigDelay = 5000;

        //private static readonly Dictionary<string, object> configInstances = new Dictionary<string, object>();
        private static readonly ConfigInstances configInstances = new ConfigInstances();

        private static readonly Dictionary<Type, List<EventHandler>> reloadTypeDelegates = new Dictionary<Type, List<EventHandler>>();
        private static readonly Dictionary<string, List<EventHandler>> reloadFileDelegates = new Dictionary<string, List<EventHandler>>();

        #region CreateAndSetupWatcher

        public static T CreateAndSetupWatcher<T>(string path)
        {
            return CreateAndSetupWatcher<T>(path, null);
        }

        public static T CreateAndSetupWatcher<T>(string path, EventHandler OnConfigFileChangedByFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            return CreateAndSetupWatcher<T>(doc.DocumentElement, path, OnConfigFileChangedByFile);
        }

        /// <summary>
        /// 创建并设置监测器
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="path">The path.</param>
        /// <param name="type">The type.</param>
        /// <param name="OnConfigFileChangedByFile">The on config file changed by file.</param>
        /// <returns></returns>
        public static object CreateAndSetupWatcher(XmlNode section,
            string path, Type type,
            EventHandler OnConfigFileChangedByFile
            )
        {
            object obj = XmlSerializerSectionHandler.GetConfigInstance(section, type);
            SetupWatcher(path, obj);

            if (OnConfigFileChangedByFile != null)
                RegisterReloadNotification(path, OnConfigFileChangedByFile);

            return obj;
        }

        public static T CreateAndSetupWatcher<T>(XmlNode section, string path, EventHandler OnConfigFileChangedByFile)
        {
            return (T)CreateAndSetupWatcher(section, path, typeof(T), OnConfigFileChangedByFile);
        }

        #endregion CreateAndSetupWatcher

        private static readonly RwLocker rwLock = new RwLocker();

        /// <summary>
        /// 设置文件更改监测
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="configInstance">The config instance.</param>
        public static void SetupWatcher(string filePath, object configInstance)
        {
            string fileName = Path.GetFileName(filePath);

            //防止重复为文件添加监测事件
            if (configInstances.Add(fileName, configInstance))
            {
                FileWatcher.Instance.AddFile(filePath, DelayedProcessConfigChange);
            }
        }

        private static void CloneObject(object srcObject, object targetObject)
        {
            Type type = targetObject.GetType();

            PropertyInfo propInstance = type.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.FlattenHierarchy);
            if (propInstance != null && propInstance.CanRead && propInstance.CanWrite)
            {
                propInstance.SetValue(null, srcObject, null);
                return;//如果设置了实例就不设置原始对象
            }

            ICopyable srcCopyable = srcObject as ICopyable;
            if (srcCopyable != null)
            {
                srcCopyable.CopyTo(targetObject);
                return;
            }

            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (prop.CanRead && prop.CanWrite)
                    prop.SetValue(targetObject, prop.GetValue(srcObject, null), null);
            }

            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                field.SetValue(targetObject, field.GetValue(srcObject));
            }
        }

        private class EventObject
        {
            private EventHandler handler;
            private object sender;
            private EventArgs args;

            public EventObject(EventHandler handler, object sender, EventArgs args)
            {
                this.handler = handler;
                this.sender = sender;
                this.args = args;
            }

            public void Execute()
            {
                handler(sender, args);
            }
        }

        private static void CallEventHandler(object obj)
        {
            EventObject evtObj = (EventObject)obj;
            evtObj.Execute();
        }

        private static void DelayedProcessConfigChange(object sender, EventArgs args)
        {
            string filePath = ((string)sender).ToLower();
            string fileName = Path.GetFileName(filePath);

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            //刷新节点以取得最新节点信息
            System.Configuration.ConfigurationManager.RefreshSection(doc.DocumentElement.Name);

            object configInstance = configInstances[fileName];
            Type newSettingsType = configInstance.GetType();

            //取得新配置实例
            object newSettings = XmlSerializerSectionHandler.GetConfigInstance(doc.DocumentElement, newSettingsType);

            //拷贝新配置实例到老配置实例中
            CloneObject(newSettings, configInstance);

            List<EventHandler> typeHandlers = new List<EventHandler>();

            using (rwLock.Read())
            {
                List<EventHandler> delegateMethods;
                if (reloadTypeDelegates.TryGetValue(newSettingsType, out delegateMethods))
                {
                    typeHandlers.AddRange(delegateMethods);
                }

                if (reloadFileDelegates.TryGetValue(filePath, out delegateMethods))
                {
                    typeHandlers.AddRange(delegateMethods);
                }
            }

            FileChangedEventArgs eventArgs = new FileChangedEventArgs(filePath);

            foreach (EventHandler delegateMethod in typeHandlers)
            {
                //使用线程池的方式调用替代连续调用
                ThreadPool.QueueUserWorkItem(CallEventHandler,
                    new EventObject(delegateMethod, newSettings, eventArgs));
            }
        }

        /// <summary>
        /// 为配置实例更新注册通知
        /// </summary>
        /// <param name="type">Type to monitor for.</param>
        /// <param name="delegateMethod">Delegate method to call.</param>
        public static void RegisterReloadNotification(Type type, EventHandler delegateMethod)
        {
            RegisterReloadNotification(type, delegateMethod, true);
        }

        public static void RegisterReloadNotification(Type type, EventHandler delegateMethod, bool allowMultiple)
        {
            List<EventHandler> delegateMethods;

            using (rwLock.Write())
            {
                if (!allowMultiple || !reloadTypeDelegates.TryGetValue(type, out delegateMethods))
                {
                    delegateMethods = new List<EventHandler>();
                    delegateMethods.Add(delegateMethod);
                    reloadTypeDelegates[type] = delegateMethods;
                }
                else
                {
                    delegateMethods.Add(delegateMethod);
                }
            }
        }

        /// <summary>
        /// 为配置文件更新注册通知
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="delegateMethod">The delegate method.</param>
        public static void RegisterReloadNotification(string filePath, EventHandler delegateMethod)
        {
            RegisterReloadNotification(filePath, delegateMethod, true);
        }

        public static void RegisterReloadNotification(string filePath, EventHandler delegateMethod, bool allowMultiple)
        {
            List<EventHandler> delegateMethods;
            filePath = filePath.ToLower();

            using (rwLock.Write())
            {
                if (!allowMultiple || !reloadFileDelegates.TryGetValue(filePath, out delegateMethods))
                {
                    delegateMethods = new List<EventHandler>();
                    delegateMethods.Add(delegateMethod);
                    reloadFileDelegates[filePath] = delegateMethods;
                }
                else
                {
                    delegateMethods.Add(delegateMethod);
                }
            }
        }
    }
}