using Dorado.DataExpress.Ldo;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Dorado.DataExpress.PagerProvider
{
    public class MySqlPager : BasePager
    {
        private const string SqlLimit = " limit {0},{1}";

        public override PagedDataTable Execute()
        {
            PagedDataTable result;
            using (SqlStatement st = base.Query.Statement.Database.NewStatement())
            {
                string limit = string.Format(" limit {0},{1}", base.Start, base.PageSize);
                base.Query.Statement.Command.CommandText = base.Query.GenerateSql() + limit + ";" + base.Query.GenerateCountSql();
                base.Query.ProcessParameters();
                string sql = st.Database.Dialect.ParametersBuilder(base.Query.Statement.Command);
                st.Command.CommandText = sql;
                DataSet ds = st.ExecuteDataSet();
                ds.Tables[0].TableName = (string.IsNullOrEmpty(base.Query.Table.Alias) ? base.Query.Table.Name : base.Query.Table.Alias);
                PagedDataTable pagedTable = new PagedDataTable
                {
                    Descriptor =
                    {
                        Total = (long)ds.Tables[1].Rows[0][0]
                    },
                    Data = ds.Tables[0]
                };
                result = pagedTable;
            }
            return result;
        }

        public override PagedEntity<T> Execute<T>()
        {
            PagedEntity<T> result;
            using (SqlStatement st = base.Query.Statement.Database.NewStatement())
            {
                string limit = string.Format(" limit {0},{1}", base.Start, base.PageSize);
                base.Query.Statement.Command.CommandText = base.Query.GenerateSql() + limit + ";\n" + base.Query.GenerateCountSql();
                base.Query.ProcessParameters();
                string sql = st.Database.Dialect.ParametersBuilder(base.Query.Statement.Command);
                st.Command.CommandText = sql;
                PagedEntity<T> pagedEntity = new PagedEntity<T>();
                using (DbDataReader reader = st.ExecuteReader())
                {
                    IEntityBinder<T> binder = BinderManager<T>.Binder;
                    if (reader.HasRows)
                    {
                        List<DataReaderField> fields = reader.GetFieldsInfo();
                        pagedEntity.Rows = new List<T>(reader.RecordsAffected + 1);
                        while (reader.Read())
                        {
                            T entity = binder.CreateInstance();
                            binder.ReadEntity(reader, fields, entity);
                            pagedEntity.Rows.Add(entity);
                        }
                    }
                    else
                    {
                        pagedEntity.Rows = new List<T>();
                    }
                    if (reader.NextResult() && reader.HasRows)
                    {
                        reader.Read();
                        pagedEntity.Descriptor.Total = reader.GetInt64(0);
                    }
                }
                result = pagedEntity;
            }
            return result;
        }
    }
}