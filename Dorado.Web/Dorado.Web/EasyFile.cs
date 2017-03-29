using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Dorado.Core;
using Dorado.Core.Logger;

namespace Dorado.Web
{
    /// <summary>
    /// 文件读写类，简化文本文件的常用读写操作。
    /// </summary>
    public class EasyFile
    {
        public static string FileName(string path)
        {
            path = path.Replace("\\", "/");
            int begin = path.LastIndexOf("/");
            return path.Substring(begin + 1);
        }

        public static string Name(string path)
        {
            path = path.Replace("\\", "/");
            int begin = path.LastIndexOf("/");
            int end = path.LastIndexOf(".");
            return path.Substring(begin + 1, end - begin - 1);
        }

        public static string Path(string path)
        {
            path = path.Replace("\\", "/");
            int begin = path.LastIndexOf("/");
            return path.Substring(0, begin);
        }

        public static string Ext(string path)
        {
            int pos = path.LastIndexOf('.');
            if (pos == -1) return string.Empty;
            return path.Substring(pos + 1).ToLower();
        }

        public static string Trim(string path)
        {
            int pos = path.LastIndexOf('.');
            return path.Substring(0, pos);
        }

        public static DateTime GetTime(string url)
        {
            return File.GetLastWriteTime(MapPath(url));
        }

        public static string MapPath(params string[] paths)
        {
            if (paths.Length == 0) return HttpContext.Current.Server.MapPath("/");
            string path = System.IO.Path.Combine(paths);
            if (File.Exists(path)) return path;
            if (path[0] == '/') return HttpContext.Current.Server.MapPath("/") + path.Substring(1);
            return HttpContext.Current.Server.MapPath("/") + path;
        }

        public static string Combine(string dir, string path)
        {
            return dir + "/" + path;
        }

        public static void Write(string str, string url)
        {
            Write(str, url, Encoding.UTF8, false);
        }

        public static void Write(string str, string url, bool append)
        {
            Write(str, url, Encoding.Default, append);
        }

        public static void Write(string str, string url, Encoding encode, bool append)
        {
            url = MapPath(url);
            int pos = url.LastIndexOf("/");
            if (pos > -1)
            {
                string dir = url.Substring(0, pos);
                if (!dir.Equals(String.Empty) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(url, append, encode);
                writer.Write(str);
            }
            finally
            {
                if (writer != null) writer.Close();
            }
        }

        public static string Read(string url)
        {
            return Read(url, true);
        }

        public static string Read(string url, bool include)
        {
            url = MapPath(url);
            StreamReader sr = null;
            try
            {
                if (!File.Exists(url)) return string.Empty;
                sr = new StreamReader(url, Encoding.UTF8);
                if (include)
                {
                    return Regex.Replace(sr.ReadToEnd(), @"<!--\s*\#include\s+virtual=""\s*/([^""]+?)\s*""\s*-->", new MatchEvaluator(Include), RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
                }
                else
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
            finally
            {
                if (sr != null) sr.Close();
            }
        }

        private static string Include(Match m)
        {
            try
            {
                return Read(m.Groups[1].Value);
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error(ex);
                return string.Empty;
            }
        }

        public static void Move(string src, string dest)
        {
            src = MapPath(src);
            dest = MapPath(dest);
            string dir = dest.Substring(0, dest.LastIndexOf("/"));
            if (!dir.Equals(String.Empty) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (File.Exists(dest)) File.Delete(dest);
            File.Move(src, dest);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="url">文件路径</param>
        public static void Delete(string url)
        {
            url = MapPath(url);
            if (File.Exists(url)) File.Delete(url);
        }

        public static void ReName(string src, string dest)
        {
            src = MapPath(src);
            if (File.Exists(src))
            {
                File.Copy(src, MapPath(dest));
                File.Delete(src);
            }
        }

        public static bool Exists(string url)
        {
            return File.Exists(MapPath(url));
        }

        public static bool ExistsDir(string dir)
        {
            return Directory.Exists(MapPath(dir));
        }

        public static void DirCreate(string dir)
        {
            string path = MapPath(dir);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void CreateDir(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        public static string[] GetFiles(string url)
        {
            url = MapPath(url);
            if (!Directory.Exists(url))
                return new string[0];
            return Directory.GetFiles(url);
        }

        public static FileInfo[] GetFileInfo(string url)
        {
            url = MapPath(url);
            if (!Directory.Exists(url))
                return new FileInfo[0];
            DirectoryInfo di = new DirectoryInfo(url);
            return di.GetFiles();
        }

        public static void DirMove(string src, string dest)
        {
            src = MapPath(src);
            dest = MapPath(dest);

            Directory.Move(src, dest);
        }

        public static void DirCopy(string src, string dest)
        {
            src = MapPath(src);
            dest = MapPath(dest);
            if (src[src.Length - 1] != '/') src += "/";
            if (dest[dest.Length - 1] != '/') dest += "/";

            if (!Directory.Exists(dest))
                Directory.CreateDirectory(dest);
            DirectoryInfo di = new DirectoryInfo(src);
            FileInfo[] list = di.GetFiles();
            foreach (FileInfo info in list)
            {
                File.Copy(src + info.Name, dest + info.Name, true);
            }
        }

        public static void DirDelete(string url)
        {
            DeleteFolder(MapPath(url));
        }

        public static void DirDelete(string url, bool self)
        {
            DeleteFolder(MapPath(url), self);
        }

        private static void DeleteFolder(string dir)
        {
            DeleteFolder(dir, false);
        }

        private static void DeleteFolder(string dir, bool self)
        {
            try
            {
                if (!Directory.Exists(dir)) return;
                if (self) Directory.Delete(dir, true);
                else
                {
                    foreach (string d in Directory.GetDirectories(dir))
                    {
                        Directory.Delete(d, true);
                    }
                    foreach (string f in Directory.GetFiles(dir))
                    {
                        File.Delete(f);
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error(ex);
            }
        }

        public static void DelTmpDir(string str)
        {
            if (str == "") return;
            string[] arr = str.Split('|');
            for (int i = 0; i < arr.Length; i++)
            {
                DirDelete(arr[i]);
            }
        }
    }
}