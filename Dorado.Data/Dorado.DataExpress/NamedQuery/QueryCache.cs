using System;
using System.Collections.Generic;
using System.IO;

namespace Dorado.DataExpress.NamedQuery
{
    public class QueryCache
    {
        public static readonly Dictionary<string, QueryNode> Default;

        static QueryCache()
        {
            QueryCache.Default = new Dictionary<string, QueryNode>(255, StringComparer.OrdinalIgnoreCase);
            QueryFile.LoadAll();
            FileSystemWatcher fw = new FileSystemWatcher(QueryFile.QueryPath, "*.xml");
            fw.Changed += new FileSystemEventHandler(QueryCache.QueriesChanged);
            fw.EnableRaisingEvents = true;
        }

        private QueryCache()
        {
        }

        private static void QueriesChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                QueryFile.Load(e.FullPath);
            }
        }
    }
}