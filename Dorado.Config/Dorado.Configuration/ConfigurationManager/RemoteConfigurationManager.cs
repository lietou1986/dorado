using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Dorado.Configuration.ConfigurationManager
{
    [XmlRoot(Namespace = "Dorado.Configuration")]
    public class RemoteConfigurationManager : BaseConfigurationManager
    {
        #region Singleton

        private RemoteConfigurationManagerConfiguration config;

        private static RemoteConfigurationManager instance = new RemoteConfigurationManager();

        public static RemoteConfigurationManager Instance
        {
            get
            {
                return instance;
            }
        }

        protected RemoteConfigurationManager()
            : base()
        {
            //取得远程配置管理文件RemoteConfigurationManager.config
            string configFile = GetRemoteConfigFile();
            try
            {
                config = ConfigWatcher.CreateAndSetupWatcher<RemoteConfigurationManagerConfiguration>(configFile);

                if (config.CheckRemoteConfig)
                {
                    System.Timers.Timer timer = new System.Timers.Timer(config.TimerInterval);
                    timer.Elapsed += new System.Timers.ElapsedEventHandler(TimerCallback);
                    timer.Start();

                    // TimerCallback(null,null);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex,
                    "无法加载 RemoteConfigurationManager 配置文件, 请在appSettings中设置'" + RemoteConfigFileAppSettingKey + "' ",
                    "RemoteConfigurationManager");
                throw ex;
            }
        }

        #endregion Singleton

        private const string RemoteConfigFileAppSettingKey = "RemoteConfigFile";
        private const string RemoteConfigurationManagerConfigFileName = "RemoteConfigurationManager.config";

        private static string GetRemoteConfigFile()
        {
            string remoteFile = System.Web.Configuration.WebConfigurationManager.AppSettings[RemoteConfigFileAppSettingKey];
            if (remoteFile == null)
                remoteFile = LocalConfigurationManager.MapConfigPath(RemoteConfigurationManagerConfigFileName);
            else
                remoteFile = LocalConfigurationManager.MapConfigPath(remoteFile);
            if (!File.Exists(remoteFile))
            {
                LoggerWrapper.Logger.Info("配置文件" + remoteFile + "' 不存在, 将会在 '" + ConfigUtility.DefaultApplicationConfigFolder + "'创建新的默认配置文件");
                remoteFile = ConfigUtility.Combine(ConfigUtility.DefaultApplicationConfigFolder, RemoteConfigurationManagerConfigFileName);

                // 如果 "RemoteConfigurationManager.config" 不存在时必须在此创建默认配置
                if (!File.Exists(remoteFile))
                {
                    Directory.CreateDirectory(ConfigUtility.DefaultApplicationConfigFolder);
                    using (XmlTextWriter writer = new XmlTextWriter(remoteFile, Encoding.UTF8))
                    {
                        writer.WriteStartElement(RemoteConfigurationManagerConfiguration.TagName);
                        RemoteConfigurationManagerConfiguration.DefaultConfig.WriteXml(writer);
                        writer.WriteEndElement();
                    }
                }
            }
            return remoteFile;
        }

        private static string GetSectionName(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        private string GetFileName(string sectionName)
        {
            return sectionName + ".config";
        }

        private string GetPath(string sectionName)
        {
            return ConfigUtility.Combine(config.LocalApplicationFolder, GetFileName(sectionName));
        }

        /// <summary>
        ///   如果超出了最大备份文件数量则删除掉超出限制的最老的文件
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        private static void RemoveOldBackupFiles(string sectionName)
        {
            string[] files = Directory.GetFiles(instance.config.LocalApplicationFolder, sectionName + ".*.config");
            if (files.Length > instance.config.MaxBackupFiles)
            {
                List<string> lst = new List<string>(files);
                lst.Sort();//排序是为了删除掉最老的备份文件

                while (lst.Count > instance.config.MaxBackupFiles)
                {
                    File.Delete(lst[0]);
                    lst.RemoveAt(0);
                }
            }
        }

        private static string GetTempFileName(string filePath)
        {
            return filePath + "." + Guid.NewGuid().ToString("N");
        }

        private static string GetBackupFileName(string filePath)
        {
            return filePath.Substring(0, filePath.LastIndexOf('.') + 1) + DateTime.Now.ToString("yyyyMMddHHmmss") + ".config";
        }

        private void OnConfigFileChanged(object sender, EventArgs args)
        {
            string filePath = ((FileChangedEventArgs)args).FileName;
            string sectionName = GetSectionName(filePath);

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            int major, minor;
            XmlSerializerSectionHandler.GetConfigVersion(doc.DocumentElement, out major, out minor);

            ConfigEntry entry = GetEntry(sectionName);
            if (entry != null)
                entry.MinorVersion = minor;
        }

        private object CreateLocalObject(Type type, string path, out int major, out int minor)
        {
            try
            {
                if (File.Exists(path))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(path);
                    object obj = ConfigWatcher.CreateAndSetupWatcher(doc.DocumentElement,
                        path, type, OnConfigFileChanged);

                    XmlSerializerSectionHandler.GetConfigVersion(doc.DocumentElement, out major, out minor);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "RemoteConfigurationManager.CreateLocalObject,type=" + type.Name + ",path=" + path, type.Name);
            }
            major = XmlSerializerSectionHandler.GetConfigurationClassMajorVersion(type);
            minor = XmlSerializerSectionHandler.DefaultUninitMinorVersion;
            return null;
        }

        #region Override entry

        /// <summary>
        /// 重写基类OnCreate方法来创建远程配置实例（原理其实还是把远程配置下载到本地创建本地配置实例）
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="type">The type.</param>
        /// <param name="major">The major.</param>
        /// <param name="minor">The minor.</param>
        /// <returns></returns>
        protected override object OnCreate(string sectionName, Type type, out int major, out int minor)
        {
            string fileName = GetFileName(sectionName);

            //string path = GetPath( sectionName );
            string path = ConfigUtility.Combine(config.LocalApplicationFolder, fileName);
            object obj = CreateLocalObject(type, path, out major, out minor);
            if (obj != null)
                return obj;

            major = XmlSerializerSectionHandler.GetConfigurationClassMajorVersion(type);
            minor = XmlSerializerSectionHandler.DefaultUninitMinorVersion;
            try
            {
                RemoteConfigSectionParam newParams = GetServerVersion(sectionName, major);
                if (newParams != null)
                {
                    //从远程下载！
                    if (Download(newParams.SectionName, newParams.DownloadUrl, path, CheckDownloadStream))
                        obj = CreateLocalObject(type, path, out major, out minor);//下载远程配置到本地后创建配置实例
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "从远程服务器下载配置 '" + sectionName + "' 时出错", sectionName);
            }

            //如果对象为null通过反射默认对象替代
            if (obj == null)
            {
                LoggerWrapper.Logger.Info("无法从 RemoteConfiguration 取得拥有类型'" + type.Name + "' 的'" + sectionName + "' 节点，通将过反射创建空实例替代");
                obj = Activator.CreateInstance(type);

                //为配置添加文件更改监控
                ConfigWatcher.SetupWatcher(path, obj);
                ConfigWatcher.RegisterReloadNotification(path, OnConfigFileChanged);
            }
            return obj;
        }

        #endregion Override entry

        /// <summary>
        /// 取得服务器上的配置版本
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="majorVersion">The major version.</param>
        /// <returns></returns>
        private RemoteConfigSectionParam GetServerVersion(string name, int majorVersion)
        {
            RemoteConfigSectionCollection lstParams = new RemoteConfigSectionCollection(config.ApplicationName, config.Environment);
            lstParams.AddSection(name, majorVersion, XmlSerializerSectionHandler.DefaultUninitMinorVersion);
            RemoteConfigSectionCollection newParams = GetServerVersions(lstParams);
            if (newParams.Count == 0)
                return null;
            else
                return newParams[0];
        }

        private RemoteConfigSectionCollection GetServerVersions(RemoteConfigSectionCollection lstParams)
        {
            // send
            // <config machine=''application=''>
            //      <section name='' majorVerion='' minorVersion='' />
            //      <section name='' majorVerion='' minorVersion='' />
            //      <section name='' majorVerion='' minorVersion='' />
            // </config>
            //
            // receive
            // <config>
            //      <section name='' majorVerion='' minorVersion='' downloadUrl='' />
            //      <section name='' majorVerion='' minorVersion='' downloadUrl='' />
            //      <section name='' majorVerion='' minorVersion='' downloadUrl='' />
            // </config>

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    XmlSerializer ser = new XmlSerializer(typeof(RemoteConfigSectionCollection));
                    ser.Serialize(ms, lstParams);

                    HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(config.RemoteConfigurationUrl);
                    req.ContentType = "text/xml";
                    req.Method = "POST";
                    req.Timeout = config.Timeout;
                    req.ReadWriteTimeout = config.ReadWriteTimeout;
                    req.ContentLength = ms.Length;
                    req.ServicePoint.Expect100Continue = false;
                    req.ServicePoint.UseNagleAlgorithm = false;
                    req.KeepAlive = false;

                    using (Stream stream = req.GetRequestStream())
                    {
                        byte[] buf = ms.ToArray();
                        stream.Write(buf, 0, buf.Length);
                    }
                    RemoteConfigSectionCollection newParams;
                    using (HttpWebResponse rsp = (HttpWebResponse)req.GetResponse())
                    {
                        using (Stream stream = rsp.GetResponseStream())
                        {
                            newParams = (RemoteConfigSectionCollection)ser.Deserialize(stream);
                        }
                    }
                    return newParams;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, "无法从 '" + config.RemoteConfigurationUrl + "'获得远程服务器上的版本", "RemoteConfigurationManager");
                return new RemoteConfigSectionCollection();
            }
        }

        private static void WriteStreamToFile(Stream stream, string file)
        {
            using (FileStream fout = new FileStream(file, FileMode.Create, FileAccess.Write))
            {
                byte[] buf = new byte[4096];
                int length;
                while ((length = stream.Read(buf, 0, buf.Length)) > 0)
                    fout.Write(buf, 0, length);
            }
        }

        /// <summary>
        /// 下载远程配置到本地
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="url">The URL.</param>
        /// <param name="targetPath">The target path.</param>
        /// <param name="checker">The checker.</param>
        /// <returns></returns>
        private static bool Download(string resourceName, string url, string targetPath, DownloadChecker checker)
        {
            try
            {
                //因为window的问题!!
                //WebClient client = new WebClient();
                //client.DownloadFile(url, targetPath);
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
                req.Method = "GET";
                req.KeepAlive = false;

                HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();
                string tmpFile = GetTempFileName(targetPath);
                using (Stream rspStream = rsp.GetResponseStream())
                {
                    //using (FileStream fout = new FileStream(tmpFile, FileMode.Create, FileAccess.Write))
                    {
                        try
                        {
                            if (checker != null)
                            {
                                //下载前检验
                                //如果使用缓冲模式那么Response.ContentLength是不靠谱的
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    byte[] buf = new byte[4096];
                                    int length;
                                    while ((length = rspStream.Read(buf, 0, buf.Length)) > 0)
                                        ms.Write(buf, 0, length);
                                    ms.Position = 0;

                                    //验证配置文件有效性
                                    checker(resourceName, ms);
                                    ms.Position = 0;
                                    WriteStreamToFile(ms, tmpFile);
                                }
                            }
                            else
                                WriteStreamToFile(rspStream, tmpFile);

                            //首先需要检查版本!

                            //为了解决读写冲突
                            // 因为window拷贝文件是非事物的
                            // 必须在更改文件名之前删除文件!!!
                            if (File.Exists(targetPath))
                            {
                                if (!Instance.config.BackupConfig)
                                    File.Delete(targetPath);
                                else
                                {
                                    //删除老的备份文件
                                    RemoveOldBackupFiles(GetSectionName(targetPath));
                                    File.Move(targetPath, GetBackupFileName(targetPath));
                                }
                            }
                            File.Move(tmpFile, targetPath);

                            //File.Replace(tmpFile, targetPath, null);
                        }
                        finally
                        {
                            File.Delete(tmpFile);
                        }
                    }
                }
                rsp.Close();
                return true;
            }
            catch (Exception ex)
            {
                HandleException(ex, "无法下载 '" + url + "'到 '" + targetPath + "'", resourceName);
                return false;
            }
        }

        private class DownloadParam
        {
            public string Name;
            public string LocalPath;
            public string Url;
            public DownloadChecker Checker;

            public DownloadParam(string name, string url, string path, DownloadChecker checker)
            {
                this.Name = name;
                this.Url = url;
                this.LocalPath = path;
                this.Checker = checker;
            }
        }

        #region 刷新本地配置 暂时废弃

        public void RefreshAllConfigs()
        {
            RemoteConfigSectionCollection lstParams = new RemoteConfigSectionCollection(config.ApplicationName);
            lock (configLocker)
            {
                foreach (ConfigEntry entry in configEntries.Values)
                {
                    if (entry.IsSet)
                        lstParams.AddSection(entry.Name, entry.MajorVersion, entry.MinorVersion);
                }
            }
            RefreshConfigs(lstParams);
        }

        public void RefreshConfig(string name, int majorVersion)
        {
            RemoteConfigSectionCollection lstParams = new RemoteConfigSectionCollection(config.ApplicationName);
            int orgMinorVersion = 0;
            lock (configLocker)
            {
                foreach (ConfigEntry entry in configEntries.Values)
                {
                    if (entry.Name == name)
                    {
                        if (entry.IsSet && entry.MajorVersion == majorVersion)
                        {
                            orgMinorVersion = entry.MinorVersion;
                            lstParams.AddSection(entry.Name, entry.MajorVersion, entry.MinorVersion);
                        }
                        break;
                    }
                }
            }
            RefreshConfigs(lstParams);
        }

        private void RefreshConfigs(RemoteConfigSectionCollection lstParams)
        {
            if (lstParams.Count == 0)
                return;
            RemoteConfigSectionCollection newParams = GetServerVersions(lstParams);//取得服务器上的版本信息
            if (newParams.Count == 0)//没有新版本直接返回
                return;

            //用来记录日志
            Dictionary<string, RemoteConfigSectionParam> tblOldParam = new Dictionary<string, RemoteConfigSectionParam>(lstParams.Count);
            foreach (RemoteConfigSectionParam item in lstParams)
                tblOldParam.Add(item.SectionName, item);

            foreach (RemoteConfigSectionParam param in newParams.Sections)
            {
                string localPath = GetPath(param.SectionName);

                //下载远程配置到本地
                if (!Download(param.SectionName, param.DownloadUrl, localPath, CheckDownloadStream))
                {
                    throw new System.Configuration.ConfigurationErrorsException("无法下载 '" + param.DownloadUrl + "' 到 '" + localPath + "'");
                }

                //执行本地配置更改监听事件
                FileWatcher.Instance.ProcessFileChanged(localPath);

                LoggerWrapper.Logger.Info(string.Format("配置 '{0}({1})' 已经从版本({2})更新到版本({3})",
                            param.SectionName, param.MajorVersion,
                            tblOldParam[param.SectionName].MinorVersion, param.MinorVersion));
            }
        }

        #endregion 刷新本地配置 暂时废弃

        private void TimerCallback(object sender, System.Timers.ElapsedEventArgs args)
        {
            RemoteConfigSectionCollection lstParams = new RemoteConfigSectionCollection(config.ApplicationName);

            //configLocker.AcquireReaderLock(-1);
            //using (configLocker.AcquireReaderLock())
            lock (configLocker)
            {
                foreach (ConfigEntry entry in configEntries.Values)
                {
                    if (entry.IsSet)
                        lstParams.AddSection(entry.Name, entry.MajorVersion, entry.MinorVersion);
                }

                //configLocker.ReleaseReaderLock();
            }
            if (lstParams.Count == 0)
                return;

            //发送本地版本信息到服务器获取更新信息
            lstParams = GetServerVersions(lstParams);
            if (lstParams.Count == 0)
                return;

            //如果有更新则下载新的配置文件
            foreach (RemoteConfigSectionParam param in lstParams.Sections)
            {
                ThreadPool.QueueUserWorkItem(
                    delegate(object obj)
                    {
                        DownloadParam dp = (DownloadParam)obj;
                        Download(dp.Name, dp.Url, dp.LocalPath, dp.Checker);
                    },
                    new DownloadParam(param.SectionName,
                        param.DownloadUrl,
                        GetPath(param.SectionName),
                        CheckDownloadStream)
                        );
            }
        }

        private delegate void DownloadChecker(string sectionName, Stream stream);

        /// <summary>
        /// 验证远程配置文件的有效性
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <param name="stream">The stream.</param>
        private void CheckDownloadStream(string sectionName, Stream stream)
        {
            ConfigEntry entry = this.GetEntry(sectionName);
            if (entry == null)
                throw new System.Configuration.ConfigurationErrorsException("在 RemoteConfigurationManager 没有找到 '" + sectionName + "' ");

            XmlDocument doc = new XmlDocument();
            doc.Load(stream);
            XmlSerializerSectionHandler.GetConfigInstance(doc.DocumentElement, entry.EntryType);
        }
    }
}