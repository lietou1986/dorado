using Dorado.DataExpress.Ldo;
using Dorado.DataExpress.SqlExpressions;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Dorado.DataExpress.PagerProvider
{
    public class MsSql2005Pager : BasePager
    {
        private const string PagedSqlStart = "WITH PagedContent AS\r\n(\r\n";
        private const string PagedSqlEnd = "\r\n)\r\nselect * from PagedContent\r\nwhere RowNumber Between @StartRow and @EndRow";
        private const string PagedSqlOver = " ROW_NUMBER () OVER ( {0} ) AS 'RowNumber'";
        internal readonly List<OrderExpression> EmptyOrders = new List<OrderExpression>();

        public override PagedDataTable Execute()
        {
            this.CreatePageSql();
            PagedDataTable result;
            using (DbDataReader reader = base.Query.ExecuteReader())
            {
                int totalCount = 0;
                if (reader.HasRows && reader.Read())
                {
                    totalCount = reader.GetInt32(0);
                }
                reader.NextResult();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                PagedDataTable pagedDataTable = new PagedDataTable
                {
                    Descriptor =
                    {
                        Total = (long)totalCount
                    },
                    Data = dataTable
                };
                result = pagedDataTable;
            }
            return result;
        }

        public override PagedEntity<T> Execute<T>()
        {
            this.CreatePageSql();
            PagedEntity<T> result;
            using (DbDataReader reader = base.Query.ExecuteReader())
            {
                long totalCount = 0L;
                if (reader.HasRows && reader.Read())
                {
                    totalCount = (long)reader.GetInt32(0);
                }
                reader.NextResult();
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
            List<BaseExpression> savedExp = base.Query.Columns;
            base.Query.SetInitialSql(base.Query.GenerateCountSql() + ";\r\nWITH PagedContent AS\r\n(\r\n");
            savedExp.Insert(0, SqlExpression.Sql(string.Format(" ROW_NUMBER () OVER ( {0} ) AS 'RowNumber'", base.Query.GenerateOrders())));
            if (savedExp.Count == 1)
            {
                savedExp.Add(SqlExpression.Sql("*"));
            }
            base.Query.Orders = this.EmptyOrders;
            base.Query.Columns = savedExp;
            base.Query.SetFinalizeSql("\r\n)\r\nselect * from PagedContent\r\nwhere RowNumber Between @StartRow and @EndRow");
            base.Query.Statement.AddParameter("StartRow", base.Start + 1).AddParameter("EndRow", base.Start + base.PageSize);
        }
    }
}