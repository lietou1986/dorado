using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml.Serialization;

namespace Dorado.Web.Fileset
{
    [XmlRoot("file")]
    [Serializable]
    public class StaticFile
    {
        public const string EtagFormat = "\"XST{0:X}\"";
        protected bool _cacheInited;
        protected string _cache;
        protected string _pcache;
        protected bool _pcacheInited;
        protected object _pcacheSyncRoot = new object();

        [XmlIgnore]
        public StaticFileset Parent
        {
            get;
            set;
        }

        [XmlIgnore]
        public StaticFileDictionary Owner
        {
            get;
            set;
        }

        [XmlAttribute("name")]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute("enableMinify")]
        public bool EnableMinify
        {
            get;
            set;
        }

        [XmlAttribute("type")]
        public StaticFileType Type
        {
            get;
            set;
        }

        [XmlAttribute("version")]
        public int Version
        {
            get;
            set;
        }

        [XmlAttribute("url")]
        public string Url
        {
            get;
            set;
        }

        [XmlAttribute("debugUrl")]
        public string DebugUrl
        {
            get;
            set;
        }

        [XmlAttribute("flags")]
        public StaticFileFlag Flags
        {
            get;
            set;
        }

        [XmlAttribute("description")]
        public string Description
        {
            get;
            set;
        }

        [XmlElement("partten")]
        public string Partten
        {
            get;
            set;
        }

        [XmlArrayItem("depend"), XmlArray("depends")]
        public string[] Depends
        {
            get;
            set;
        }

        [XmlArray("attrs"), XmlArrayItem("attr")]
        public List<CustomAttribute> Attributes
        {
            get;
            set;
        }

        [XmlIgnore]
        public string ETag
        {
            get;
            set;
        }

        [XmlIgnore]
        public string Cache
        {
            get
            {
                InitCache();
                return _cache;
            }
        }

        [XmlIgnore]
        public string ParttenCache
        {
            get
            {
                try
                {
                    InitParttenCache();
                }
                catch (Exception ex)
                {
                    LoggerWrapper.Logger.Error("获取静态文件时发生异常!", ex);
                    _pcache = string.Empty;
                }
                return _pcache;
            }
        }

        public StaticFile()
        {
            Flags = StaticFileFlag.Reference;
            Type = StaticFileType.Javascript;
            EnableMinify = true;
        }

        public void ClearCache()
        {
            _cacheInited = (_pcacheInited = false);
            _cache = (_pcache = string.Empty);
        }

        protected virtual void InitCache()
        {
            if (!_cacheInited)
            {
                Monitor.Enter(this);
                try
                {
                    if (!_cacheInited)
                    {
                        if (Type == StaticFileType.Group)
                        {
                            _cacheInited = true;
                            _cache = string.Empty;
                        }
                        else
                        {
                            if (Flags.Contains(new StaticFileFlag[]
							{
								StaticFileFlag.Embed
							}))
                            {
                                _cache = LoadContent();
                            }
                            else
                            {
                                _cache = (Flags.Contains(new StaticFileFlag[]
								{
									StaticFileFlag.Local
								}) ? VirtualPathUtility.ToAbsolute(Url) : Url);
                                if (Parent.ConfigurationRoot.Parameter.VersionedUrl && Version > 0)
                                {
                                    _cache = _cache + "?v=" + Version.ToString();
                                }
                            }
                            if (_cache.Contains('$'))
                            {
                                _cache = StaticFilesetManager.ReplaceVarible(_cache);
                            }
                            ETag = string.Format("\"XST{0:X}\"", _cache.GetHashCode());
                            _cacheInited = true;
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(this);
                }
            }
        }

        public string LoadContent()
        {
            IFileLoader ldr = null;
            if (Flags.Contains(new StaticFileFlag[]
			{
				StaticFileFlag.Remote
			}))
            {
                ldr = new RemoteFileLoader();
            }
            else
            {
                if (EnableMinify && StaticFilesetManager.EnableMinify)
                {
                    ldr = new LocalMiniFileLoader(Type);
                }
                else
                {
                    ldr = new LocalFileLoader();
                }
            }
            string result;
            try
            {
                string fileContent = ldr.Load(Url);
                result = fileContent;
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error(string.Format("读取静态文件{0}({1}),失败", Name, Url), ex);
                result = string.Empty;
            }
            return result;
        }

        protected virtual void InitParttenCache()
        {
            if (!_pcacheInited)
            {
                object pcacheSyncRoot;
                Monitor.Enter(pcacheSyncRoot = _pcacheSyncRoot);
                try
                {
                    if (!_pcacheInited)
                    {
                        if (Type == StaticFileType.Group || string.IsNullOrEmpty(Cache))
                        {
                            _pcacheInited = true;
                            _pcache = string.Empty;
                        }
                        else
                        {
                            string customAttributes = string.Empty;
                            if (Attributes != null && Attributes.Count > 0)
                            {
                                StringBuilder sb = new StringBuilder(512);
                                foreach (CustomAttribute attribute in Attributes)
                                {
                                    sb.Append(attribute.AttributeName);
                                    sb.Append("=").Append("\"");
                                    if (attribute.Value != null)
                                    {
                                        sb.Append(StaticFilesetManager.ReplaceVarible(attribute.Value));
                                    }
                                    sb.Append("\" ");
                                }
                                customAttributes = sb.ToString();
                            }
                            string realPartten;
                            if (Flags.Contains(new StaticFileFlag[]
							{
								StaticFileFlag.Reference
							}))
                            {
                                switch (Type)
                                {
                                    case StaticFileType.Css:
                                        {
                                            realPartten = StaticFilesetManager.Consts.ReferenceStyleSheet;
                                            break;
                                        }
                                    case StaticFileType.Javascript:
                                        {
                                            realPartten = StaticFilesetManager.Consts.ReferenceJavascript;
                                            break;
                                        }
                                    case StaticFileType.Other:
                                        {
                                            realPartten = ((!string.IsNullOrEmpty(Partten)) ? Partten : "{0}{1}");
                                            break;
                                        }
                                    default:
                                        {
                                            realPartten = "{0}{1}";
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                switch (Type)
                                {
                                    case StaticFileType.Css:
                                        {
                                            realPartten = StaticFilesetManager.Consts.EmbedStyleSheet;
                                            break;
                                        }

                                    case StaticFileType.Javascript:
                                        {
                                            realPartten = StaticFilesetManager.Consts.EmbedJavascript;
                                            break;
                                        }
                                    case StaticFileType.Other:
                                        {
                                            realPartten = (string.IsNullOrEmpty(Partten) ? Partten : "{0}{1}");
                                            break;
                                        }
                                    default:
                                        {
                                            realPartten = "{0}{1}";
                                            break;
                                        }
                                }
                            }
                            _pcache = string.Format(realPartten, Cache, customAttributes);
                            _pcacheInited = true;
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(pcacheSyncRoot);
                }
            }
        }
    }
}