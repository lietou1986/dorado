using System;
using System.IO;
using System.Xml.Serialization;

namespace Dorado.DataExpress.NamedQuery
{
    [XmlRoot("named-sql")]
    [Serializable]
    public class QueryFile
    {
        public const string DefualtPath = "named-queries";

        [XmlArrayItem(ElementName = "sql"), XmlArray(ElementName = "sql-collection")]
        public QueryNode[] Queries;

        public static string QueryPath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "named-queries");
            }
        }

        public static void Load(string fName)
        {
            if (fName == null)
            {
                throw new ArgumentNullException("fName");
            }
            if (!File.Exists(fName))
            {
                throw new FileNotFoundException("命名查询文件未能找到", fName);
            }
            XmlSerializer serializer = new XmlSerializer(typeof(QueryFile));
            using (FileStream fs = File.OpenRead(fName))
            {
                QueryFile qf = (QueryFile)serializer.Deserialize(fs);
                if (qf.Queries != null && qf.Queries.Length > 0)
                {
                    QueryNode[] queries = qf.Queries;
                    for (int i = 0; i < queries.Length; i++)
                    {
                        QueryNode st = queries[i];
                        if (!QueryCache.Default.ContainsKey(st.Name))
                        {
                            QueryCache.Default.Add(st.Name, st);
                        }
                        else
                        {
                            QueryCache.Default.Remove(st.Name);
                            QueryCache.Default.Add(st.Name, st);
                        }
                    }
                }
            }
        }

        public static void LoadAll()
        {
            QueryCache.Default.Clear();
            string path = QueryFile.QueryPath;
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path, "*.xml");
                string[] array = files;
                for (int i = 0; i < array.Length; i++)
                {
                    string file = array[i];
                    QueryFile.Load(file);
                }
            }
        }
    }
}