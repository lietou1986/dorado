using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace Dorado.Web.Fileset
{
    [Serializable]
    public class MergedStaticFile : StaticFile
    {
        public const string MergedFilePartten = "runtime/merged/@Model.filename.@Model.version.@Model.extension";

        private static string UrlPartten;

        [XmlArray("merge"), XmlArrayItem("from")]
        public string[] Merges
        {
            get;
            set;
        }

        static MergedStaticFile()
        {
            UrlPartten = "/" + Engine.Razor.RunCompile(MergedFilePartten, "MergedFilePartten", null, new
            {
                filename = "0",
                version = "1",
                extension = "2"
            });
        }

        protected override void InitCache()
        {
            if (_cacheInited)
                return;
            Monitor.Enter(this);
            try
            {
                if (!_cacheInited)
                {
                    _cacheInited = true;
                    if (Merges != null && Merges.Length != 0)
                    {
                        if (Merges != null && Merges.Length > 0)
                        {
                            StringBuilder builder = new StringBuilder(Merges.Length * 512);
                            string[] merges = Merges;
                            for (int i = 0; i < merges.Length; i++)
                            {
                                string fileName = merges[i];
                                StaticFile file = StaticFilesetManager.GetStaticFile(fileName);
                                if (file != null)
                                {
                                    builder.Append(file.LoadContent());
                                    builder.Append(Environment.NewLine);
                                }
                            }
                            _cache = builder.ToString();
                        }
                        else
                            _cache = string.Empty;
                        base.ETag = string.Format("\"XST{0:X}\"", _cache.GetHashCode());
                    }
                }
            }
            finally
            {
                Monitor.Exit(this);
            }
        }

        protected override void InitParttenCache()
        {
            if (_pcacheInited)
                return;
            object pcacheSyncRoot;
            Monitor.Enter(pcacheSyncRoot = _pcacheSyncRoot);
            try
            {
                if (!_pcacheInited)
                {
                    _pcacheInited = true;
                    StaticFileType type = base.Type;
                    switch (type)
                    {
                        case StaticFileType.Css:
                            {
                                string cssUrl = string.Format(MergedStaticFile.UrlPartten, base.Name, "css", base.Version);
                                _pcache = string.Format("<link href=\"{0}\" rel=\"stylesheet\" type=\"text/css\" {1}/>", cssUrl);
                                break;
                            }
                        case StaticFileType.Javascript:
                            {
                                string jsUrl = string.Format(MergedStaticFile.UrlPartten, base.Name, "js", base.Version);
                                _pcache = string.Format("<script type=\"text/javascript\" src=\"{0}\" {1}></script>", jsUrl);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                    _pcache = string.Empty;
                }
            }
            finally
            {
                Monitor.Exit(pcacheSyncRoot);
            }
        }
    }
}