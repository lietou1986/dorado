using System;
using System.Collections.Generic;
using System.Web;

namespace Dorado.DataExpress
{
    internal class DbSession
    {
        private const string ConfigError = "数据库会话管理模块未设置，请在Web.config的httpModules中写入<add name=\"DatabaseBinder\" type=\"{0}\"/>";
        public const string DbKey = "DatabaseContainer";
        public const string DefatultKey = "DefaultDatabase";

        public static Database Current
        {
            get
            {
                if (!HttpBindModule.Inited)
                {
                    throw new Exception(string.Format("数据库会话管理模块未设置，请在Web.config的httpModules中写入<add name=\"DatabaseBinder\" type=\"{0}\"/>", typeof(HttpBindModule).AssemblyQualifiedName));
                }
                if (HttpContext.Current.Items["DefaultDatabase"] == null)
                {
                    HttpContext.Current.Items["DefaultDatabase"] = DatabaseManager.GetDatabase(HttpContext.Current.Request.Url.ToString());
                }
                return HttpContext.Current.Items["DefaultDatabase"] as Database;
            }
        }

        public static Database Get(string name)
        {
            if (!HttpBindModule.Inited)
            {
                throw new Exception(string.Format("数据库会话管理模块未设置，请在Web.config的httpModules中写入<add name=\"DatabaseBinder\" type=\"{0}\"/>", typeof(HttpBindModule).AssemblyQualifiedName));
            }
            Dictionary<string, Database> dbCache = DbSession.GetDbTable();
            if (dbCache[name] == null)
            {
                dbCache[name] = DatabaseManager.GetDatabase(name, HttpContext.Current.Request.Url.ToString());
            }
            return dbCache[name];
        }

        private static Dictionary<string, Database> GetDbTable()
        {
            Dictionary<string, Database> dbCache = null;
            if (HttpContext.Current.Items["DatabaseContainer"] == null)
            {
                dbCache = new Dictionary<string, Database>(StringComparer.OrdinalIgnoreCase);
                HttpContext.Current.Items["DatabaseContainer"] = dbCache;
            }
            else
            {
                dbCache = (Dictionary<string, Database>)HttpContext.Current.Items["DatabaseContainer"];
            }
            return dbCache;
        }
    }
}