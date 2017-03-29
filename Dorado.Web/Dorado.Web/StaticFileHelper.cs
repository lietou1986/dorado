using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.Web.Fileset;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml.Linq;

namespace Dorado.Web
{
    public static class StaticFileHelper
    {
        private const string StaticFileContextIdentity = "StaticFileContainer";
        private const string LocalScriptMarkIdentity = "LocalEmbedMark";
        private static readonly object _syncRoot;
        private static readonly Dictionary<string, LocalJsCacheItem> _localJsCache;
        private static readonly LocalFileLoader loader;
        private static readonly string[] JsLocationFormats;
        private static readonly string[] AreaJsLocationFormats;
        private static readonly string[] CssLocationFormats;
        private static readonly string[] AreaCssLocationFormats;

        public static Dictionary<string, bool> StaticFileContainer
        {
            get
            {
                if (HttpContext.Current.Items.Contains("StaticFileContainer"))
                    return (Dictionary<string, bool>)HttpContext.Current.Items["StaticFileContainer"];
                Dictionary<string, bool> container = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
                HttpContext.Current.Items.Add("StaticFileContainer", container);
                return container;
            }
        }

        internal static Dictionary<string, object> LocalEmbedMark
        {
            get
            {
                Dictionary<string, object> marks = null;
                if (HttpContext.Current.Items.Contains("LocalEmbedMark"))
                    marks = (Dictionary<string, object>)HttpContext.Current.Items["LocalEmbedMark"];
                else
                {
                    marks = new Dictionary<string, object>();
                    HttpContext.Current.Items.Add("LocalEmbedMark", marks);
                }
                return marks;
            }
        }

        static StaticFileHelper()
        {
            _syncRoot = new object();
            loader = new LocalFileLoader();
            JsLocationFormats = new string[]
            {
                "~/Views/{1}/{0}.js",
                "~/Views/Shared/{0}.js"
            };
            AreaJsLocationFormats = new string[]
            {
                "~/Areas/{2}/Views/{1}/{0}.js",
                "~/Areas/{2}/Views/Shared/{0}.js"
            };
            CssLocationFormats = new string[]
            {
                "~/Views/{1}/{0}.css",
                "~/Views/Shared/{0}.css"
            };
            AreaCssLocationFormats = new string[]
            {
                "~/Areas/{2}/Views/{1}/{0}.css",
                "~/Areas/{2}/Views/Shared/{0}.css"
            };
            _localJsCache = new Dictionary<string, LocalJsCacheItem>(255, StringComparer.OrdinalIgnoreCase);
        }

        private static void WriteTips(this TextWriter writer, string prefix, params string[] comments)
        {
            if (comments == null || comments.Length == 0)
                return;
            string output = string.Join(",", comments);
            XComment xcomment = new XComment(prefix + ":" + output);
            writer.WriteLine(xcomment);
        }

        private static void WriteComment(this TextWriter writer, params string[] comments)
        {
            if (comments == null || comments.Length == 0)
                return;
            string output = string.Join(",", comments);
            XComment xcomment = new XComment(output);
            writer.WriteLine(xcomment);
        }

        private static void WriteComment(this TextWriter writer, string comments)
        {
            writer.WriteLine(new XComment(comments).ToString());
        }

        internal static void ImportInternal(TextWriter writer, string fileIdentity)
        {
            try
            {
                if (!StaticFileContainer.ContainsKey(fileIdentity))
                {
                    StaticFile file = StaticFilesetManager.GetStaticFile(fileIdentity);
                    if (file.NotNull() && file.Depends != null)
                    {
                        string[] depends = file.Depends;
                        for (int i = 0; i < depends.Length; i++)
                        {
                            string item = depends[i];
                            ImportInternal(writer, item);
                        }
                    }
                    if (file.IsNull())
                    {
                        writer.WriteComment(string.Format("静态文件[{0}]不存在", fileIdentity));
                        LoggerWrapper.Logger.Warn(string.Format("引用了一个不存在的文件[{0}]", fileIdentity));
                    }
                    else
                    {
                        StaticFileContainer.Add(fileIdentity, true);
                        writer.Write(file.GetParttenCache());
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("引入静态文件时发生错误。", ex);
            }
        }

        public static string ImportUrl(this HtmlHelper html, string fileName)
        {
            StaticFile staticFile = StaticFilesetManager.GetStaticFile(fileName);
            if (staticFile != null)
                return staticFile.Cache;
            return string.Empty;
        }

        public static void Import(this HtmlHelper helper, string fileIdentity)
        {
            helper.ViewContext.Writer.WriteTips("引用静态文件", new string[]
            {
                fileIdentity
            });
            ImportInternal(helper.ViewContext.Writer, fileIdentity);
        }

        public static void Import(this UserControl control, string fileIdentity)
        {
            control.Response.Output.WriteTips("引用静态文件", new string[]
            {
                fileIdentity
            });
            ImportInternal(control.Response.Output, fileIdentity);
        }

        public static void Import(this UserControl control, params string[] fileIdentities)
        {
            control.Response.Output.WriteTips("引用静态文件", fileIdentities);
            foreach (string item in fileIdentities)
            {
                ImportInternal(control.Response.Output, item);
            }
        }

        public static void Import(this Page page, string fileIdentity)
        {
            page.Response.Output.WriteTips("引用静态文件", new string[]
            {
                fileIdentity
            });
            ImportInternal(page.Response.Output, fileIdentity);
        }

        public static void Import(this Page page, params string[] fileIdenties)
        {
            page.Response.Output.WriteTips("引用静态文件", fileIdenties);
            foreach (string item in fileIdenties)
            {
                ImportInternal(page.Response.Output, item);
            }
        }

        public static void Import(this HtmlHelper helper, params string[] fileIdentities)
        {
            helper.ViewContext.Writer.WriteTips("引用静态文件", fileIdentities);
            foreach (string item in fileIdentities)
            {
                ImportInternal(helper.ViewContext.Writer, item);
            }
        }

        public static string ImportFrom(this HtmlHelper helper, params string[] fileIdentities)
        {
            StringBuilder sb = new StringBuilder(fileIdentities.Length * 96);
            using (StringWriter writer = new StringWriter(sb))
            {
                writer.WriteTips("引用静态文件", fileIdentities);
                foreach (string fileIdentity in fileIdentities)
                {
                    ImportInternal(writer, fileIdentity);
                }
            }
            return sb.ToString();
        }

        public static string ImportFrom(this HtmlHelper helper, string fileIdentity)
        {
            StringBuilder sb = new StringBuilder(96);
            using (StringWriter writer = new StringWriter(sb))
            {
                writer.WriteTips("引用静态文件", new string[]
                {
                    fileIdentity
                });
                ImportInternal(writer, fileIdentity);
            }
            return sb.ToString();
        }

        public static string RenderCss(this HtmlHelper helper)
        {
            string viewPath = string.Empty;

            if (helper.ViewContext.View is RazorView)
            {
                RazorView view = (RazorView)helper.ViewContext.View;
                viewPath = view.ViewPath;
            }
            else if (helper.ViewContext.View is WebFormView)
            {
                WebFormView view = (WebFormView)helper.ViewContext.View;
                viewPath = view.ViewPath;
            }
            else
            {
                return string.Empty;
            }
            string cssPath = Path.ChangeExtension(viewPath, "css");
            if (HostingEnvironment.VirtualPathProvider.FileExists(cssPath))
            {
                return RenderFileInternal(cssPath, StaticFileType.Css);
            }
            return string.Empty;
        }

        public static string RenderCss(this UserControl page)
        {
            MasterPage master = page as MasterPage;
            if (master != null)
            {
                if (page.Page.Master != null && page.Page.Master.GetType() != master.GetType())
                {
                    master = page.Page.Master;
                }
                return RenderMasterCss(master) + GetLocalCss(page.AppRelativeVirtualPath);
            }
            return GetLocalCss(page.AppRelativeVirtualPath);
        }

        private static string RenderMasterCss(MasterPage master)
        {
            return ((master.Master != null) ? RenderMasterCss(master.Master) : string.Empty) + GetLocalCss(master.AppRelativeVirtualPath);
        }

        public static string RenderCss(this Page page)
        {
            return GetLocalCss(page.AppRelativeVirtualPath);
        }

        private static string GetLocalCss(string aspxFile)
        {
            string cssPath = Path.ChangeExtension(aspxFile, "css");
            if (HostingEnvironment.VirtualPathProvider.FileExists(cssPath))
            {
                return RenderFileInternal(cssPath, StaticFileType.Css);
            }
            return string.Empty;
        }

        public static void RenderCss(this HtmlHelper helper, params string[] cssNames)
        {
            helper.RenderCss(null, cssNames);
        }

        public static void RenderCss(this HtmlHelper helper, TextWriter writer, params string[] cssNames)
        {
            TextWriter localWriter = writer ?? helper.ViewContext.Writer;
            string areaName = AreaHelpers.GetAreaName(helper.ViewContext.RouteData);
            string controllerName = helper.ViewContext.RouteData.GetRequiredString("controller");
            foreach (string css in cssNames)
            {
                if (css.StartsWith("~") && HostingEnvironment.VirtualPathProvider.FileExists(css))
                {
                    localWriter.WriteLine(RenderFileInternal(css, StaticFileType.Css));
                    return;
                }
                if (!string.IsNullOrEmpty(areaName))
                {
                    string[] areaCssLocationFormats = AreaCssLocationFormats;
                    foreach (string partten in areaCssLocationFormats)
                    {
                        string vfile = string.Format(partten, css, controllerName, areaName);
                        if (HostingEnvironment.VirtualPathProvider.FileExists(vfile))
                        {
                            localWriter.WriteLine(RenderFileInternal(vfile, StaticFileType.Css));
                            break;
                        }
                    }
                }
                else
                {
                    string[] cssLocationFormats = CssLocationFormats;
                    foreach (string partten2 in cssLocationFormats)
                    {
                        string vfile2 = string.Format(partten2, css, controllerName);
                        if (HostingEnvironment.VirtualPathProvider.FileExists(vfile2))
                        {
                            localWriter.WriteLine(RenderFileInternal(vfile2, StaticFileType.Javascript));
                            break;
                        }
                    }
                }
            }
        }

        public static string RenderJs(this UserControl page)
        {
            MasterPage master = page as MasterPage;
            if (master == null) return GetLocalJs(page.AppRelativeVirtualPath);
            if (page.Page.Master != null && page.Page.Master.GetType() != master.GetType())
            {
                master = page.Page.Master;
            }
            return RenderMasterJs(master) + GetLocalJs(master.Page.AppRelativeVirtualPath);
        }

        internal static string RenderMasterJs(MasterPage page)
        {
            string result = string.Empty;
            if (page.Master != null)
            {
                result += RenderMasterJs(page.Master);
            }
            return result + GetLocalJs(page.AppRelativeVirtualPath);
        }

        public static string RenderJs(this Page page)
        {
            return GetLocalJs(page.AppRelativeVirtualPath);
        }

        private static string GetLocalJs(string aspxFile)
        {
            string jsPath = Path.ChangeExtension(aspxFile, "js");
            if (HostingEnvironment.VirtualPathProvider.FileExists(jsPath))
                return RenderFileInternal(jsPath, StaticFileType.Javascript);
            return string.Empty;
        }

        public static string RenderJs(this HtmlHelper helper)
        {
            string viewPath = string.Empty;
            if (helper.ViewContext.View is RazorView)
            {
                RazorView view = (RazorView)helper.ViewContext.View;
                viewPath = view.ViewPath;
            }
            else if (helper.ViewContext.View is WebFormView)
            {
                WebFormView view = (WebFormView)helper.ViewContext.View;
                viewPath = view.ViewPath;
            }
            else
            {
                return string.Empty;
            }
            string jsPath = Path.ChangeExtension(viewPath, "js");
            if (HostingEnvironment.VirtualPathProvider.FileExists(jsPath))
            {
                return RenderFileInternal(jsPath, StaticFileType.Javascript);
            }
            return string.Empty;
        }

        public static void RenderJs(this HtmlHelper helper, params string[] jsNames)
        {
            helper.RenderJs(null, jsNames);
        }

        public static void RenderJs(this HtmlHelper helper, TextWriter writer, params string[] jsNames)
        {
            TextWriter localWriter = writer ?? helper.ViewContext.Writer;
            string areaName = AreaHelpers.GetAreaName(helper.ViewContext.RouteData);
            string controllerName = helper.ViewContext.RouteData.GetRequiredString("controller");
            foreach (string js in jsNames)
            {
                if (js.StartsWith("~") && HostingEnvironment.VirtualPathProvider.FileExists(js))
                {
                    localWriter.WriteLine(RenderFileInternal(js, StaticFileType.Javascript));
                    return;
                }
                if (!string.IsNullOrEmpty(areaName))
                {
                    string[] areaJsLocationFormats = AreaJsLocationFormats;
                    foreach (string partten in areaJsLocationFormats)
                    {
                        string vfile = string.Format(partten, js, controllerName, areaName);
                        if (HostingEnvironment.VirtualPathProvider.FileExists(vfile))
                        {
                            localWriter.WriteLine(RenderFileInternal(vfile, StaticFileType.Javascript));
                            break;
                        }
                    }
                }
                else
                {
                    string[] jsLocationFormats = JsLocationFormats;
                    foreach (string partten2 in jsLocationFormats)
                    {
                        string vfile2 = string.Format(partten2, js, controllerName);
                        if (HostingEnvironment.VirtualPathProvider.FileExists(vfile2))
                        {
                            localWriter.WriteLine(RenderFileInternal(vfile2, StaticFileType.Javascript));
                            break;
                        }
                    }
                }
            }
        }

        internal static string RenderFileInternal(string filePath, StaticFileType fileType)
        {
            object syncRoot;
            Monitor.Enter(syncRoot = _syncRoot);
            try
            {
                Dictionary<string, object> marks = LocalEmbedMark;
                if (marks.ContainsKey(filePath))
                    return string.Empty;
                else
                {
                    string fileName = Path.Combine(HttpRuntime.AppDomainAppPath, VirtualPathUtility.ToAbsolute(filePath).Substring(1).Replace('/', Path.DirectorySeparatorChar));
                    marks.Add(filePath, null);
                    DateTime lastModify = File.GetLastWriteTime(fileName);
                    if (_localJsCache.ContainsKey(filePath))
                    {
                        LocalJsCacheItem cacheItem = _localJsCache[filePath];
                        if (cacheItem.LastModifyDate < lastModify)
                        {
                            string content = LoadAndMinifiter(fileName, fileType);
                            switch (fileType)
                            {
                                case StaticFileType.Css:
                                    {
                                        cacheItem.FileContent = string.Format("<style type=\"text/css\" {1}>{0}</style>", content, string.Empty);
                                        break;
                                    }
                                case StaticFileType.Javascript:
                                    {
                                        cacheItem.FileContent = string.Format("<script type=\"text/javascript\" {1}>{0}</script>", content, string.Empty);
                                        break;
                                    }
                                default:
                                    cacheItem.FileContent = content;
                                    break;
                            }
                        }
                        return cacheItem.FileContent;
                    }
                    else
                    {
                        string result = LoadAndMinifiter(fileName, fileType);
                        if (string.IsNullOrEmpty(result))
                            return string.Empty;
                        else
                        {
                            switch (fileType)
                            {
                                case StaticFileType.Css:
                                    {
                                        result = string.Format("<style type=\"text/css\" {1}>{0}</style>", result, string.Empty);
                                        break;
                                    }
                                case StaticFileType.Javascript:
                                    {
                                        result = string.Format("<script type=\"text/javascript\" {1}>{0}</script>", result, string.Empty);
                                        break;
                                    }
                            }
                            _localJsCache.Add(filePath, new LocalJsCacheItem
                            {
                                FileContent = result,
                                LastModifyDate = lastModify
                            });
                            return result;
                        }
                    }
                }
            }
            finally
            {
                Monitor.Exit(syncRoot);
            }
        }

        private static string LoadAndMinifiter(string fileName, StaticFileType fileType)
        {
            string minifierContent = string.Empty;
            try
            {
                string content = loader.Load(fileName);
                if (StaticFilesetManager.EnableMinify)
                {
                    switch (fileType)
                    {
                        case StaticFileType.Css:
                            minifierContent = MicrosoftAjaxMinifer.MinifyCss(content);
                            break;

                        case StaticFileType.Javascript:
                            minifierContent = MicrosoftAjaxMinifer.MinifyJavaScript(content);
                            break;

                        default:
                            minifierContent = content;
                            break;
                    }
                }
                else
                {
                    minifierContent = content;
                }

                return minifierContent;
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error(string.Format("读取脚本文件[{0}]时发生异常", fileName), ex);
                return string.Empty;
            }
        }
    }
}