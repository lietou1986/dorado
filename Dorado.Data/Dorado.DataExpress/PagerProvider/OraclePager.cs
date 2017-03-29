using Dorado.DataExpress.Ldo;
using Dorado.DataExpress.SqlExpressions;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Dorado.DataExpress.PagerProvider
{
    public class OraclePager : BasePager
    {
        private const string PagedSqlStart = "SELECT * FROM(SELECT ROWNUM RowNumber,PagedContent.* FROM(";
        private const string PagedSqlEnd = ")PagedContent WHERE ROWNUM<= :MAXROWNUMBER ) WHERE RowNumber>= :MINROWNUMBER";
        internal readonly List<OrderExpression> EmptyOrders = new List<OrderExpression>();

        public override PagedDataTable Execute()
        {
            long totalCount = base.Query.Count();
            this.CreatePageSql();
            PagedDataTable result;
            using (DbDataReader reader = base.Query.ExecuteReader())
            {
                DataTable table = new DataTable();
                table.Load(reader);
                PagedDataTable pagedEntity = new PagedDataTable
                {
                    Descriptor =
                    {
                        Total = totalCount
                    },
                    Data = table
                };
                result = pagedEntity;
            }
            return result;
        }

        public override PagedEntity<T> Execute<T>()
        {
            long totalCount = base.Query.Count();
            this.CreatePageSql();
            PagedEntity<T> result;
            using (DbDataReader reader = base.Query.ExecuteReader())
            {
                PagedEntity<T> pagedEntity = new PagedEntity<T>
                {
                    Rows = reader.ReadEntities<T>(),
                    Descriptor =
                    {
                        Total = totalCount
                    }
                };
                result = pagedEntity;
            }
            return result;
        }

        private void CreatePageSql()
        {
            base.Query.SetInitialSql("SELECT * FROM(SELECT ROWNUM RowNumber,PagedContent.* FROM(");
            base.Query.SetFinalizeSql(")PagedContent WHERE ROWNUM<= :MAXROWNUMBER ) WHERE RowNumber>= :MINROWNUMBER");
            base.Query.Statement.Command.Parameters.Clear();
            base.Query.Statement.AddParameter("MINROWNUMBER", base.Start + 1).AddParameter("MAXROWNUMBER", base.Start + base.PageSize);
        }
    }
}