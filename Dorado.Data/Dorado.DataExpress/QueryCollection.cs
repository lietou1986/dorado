using System.Collections.Generic;
using System.Data;

namespace Dorado.DataExpress
{
    public class QueryCollection
    {
        private readonly List<QueryStatement> _queries = new List<QueryStatement>();

        public QueryStatement this[int index]
        {
            get
            {
                return this._queries[index];
            }
            set
            {
                if (!this._queries.Contains(value))
                {
                    this._queries.Add(value);
                }
            }
        }

        public int Count
        {
            get
            {
                return this._queries.Count;
            }
        }

        public void Add(QueryStatement query)
        {
            this._queries.Add(query);
        }

        public void Remove(QueryStatement query)
        {
            this._queries.Remove(query);
        }

        public void RemoveAt(int index)
        {
            this._queries.RemoveAt(index);
        }

        public DataSet ExecuteAll(string name)
        {
            DataSet ds = new DataSet(name);
            foreach (QueryStatement query in this._queries)
            {
                ds.Tables.Add(query.Execute());
            }
            return ds;
        }

        public DataSet ExecuteAll()
        {
            return this.ExecuteAll(string.Empty);
        }
    }
}