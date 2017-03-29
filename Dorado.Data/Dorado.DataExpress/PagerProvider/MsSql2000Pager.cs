using Dorado.DataExpress.Ldo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Dorado.DataExpress.PagerProvider
{
    public class MsSql2000Pager : BasePager
    {
        private const string sSqlPager = "\r\nCREATE PROCEDURE SqlPager\r\n@sql nvarchar(4000),\r\n@start int,\r\n@limit int\r\nAS\r\nSET NOCOUNT ON\r\nDECLARE @cur int,@rowCount int\r\nEXECUTE sp_cursoropen @cur OUTPUT,@sql,@scrollopt=1,@ccopt=1,@rowCount=@rowCount OUTPUT\r\nEXECUTE sp_cursorfetch @cur,16,@start,@limit\r\nIF (@rowCount-@start)<@limit \r\nBEGIN\r\n\tSET @Limit=@rowCount-@start\r\nEND \r\nSELECT @start AS 'Start',@limit AS 'RowCount',@rowCount AS 'TotalRowCount'\r\nEXECUTE sp_cursorclose @cur\r\nSET NOCOUNT OFF\r\n";

        public override PagedDataTable Execute()
        {
            PagedDataTable result;
            using (SqlStatement st = base.Query.Statement.Database.NewProcedure("SqlPager"))
            {
                try
                {
                    base.Query.Statement.Command.CommandText = base.Query.GenerateSql();
                    base.Query.ProcessParameters();
                    string sql = st.Database.Dialect.ParametersBuilder(base.Query.Statement.Command);
                    st.Add("sql", sql).Add("start", base.Start + 1).Add("limit", base.PageSize);
                    DataSet ds = st.ExecuteDataSet();
                    int tableStart = (ds.Tables.Count > 2) ? 1 : 0;
                    PagedDataTable table = new PagedDataTable
                    {
                        Data = ds.Tables[tableStart],
                        Descriptor =
                        {
                            Total = (long)((int)ds.Tables[tableStart + 1].Rows[0][2])
                        }
                    };
                    table.Data.TableName = (string.IsNullOrEmpty(base.Query.Table.Alias) ? base.Query.Table.Name : base.Query.Table.Alias);
                    result = table;
                }
                catch (Exception err)
                {
                    if (err is SqlException && (err as SqlException).Number == 2812)
                    {
                        using (SqlStatement proc = st.Database.NewStatement("\r\nCREATE PROCEDURE SqlPager\r\n@sql nvarchar(4000),\r\n@start int,\r\n@limit int\r\nAS\r\nSET NOCOUNT ON\r\nDECLARE @cur int,@rowCount int\r\nEXECUTE sp_cursoropen @cur OUTPUT,@sql,@scrollopt=1,@ccopt=1,@rowCount=@rowCount OUTPUT\r\nEXECUTE sp_cursorfetch @cur,16,@start,@limit\r\nIF (@rowCount-@start)<@limit \r\nBEGIN\r\n\tSET @Limit=@rowCount-@start\r\nEND \r\nSELECT @start AS 'Start',@limit AS 'RowCount',@rowCount AS 'TotalRowCount'\r\nEXECUTE sp_cursorclose @cur\r\nSET NOCOUNT OFF\r\n"))
                        {
                            proc.Execute();
                            result = this.Execute();
                            return result;
                        }
                    }
                    throw new Exception("执行分页查询时发生错误！", err);
                }
            }
            return result;
        }

        public override PagedEntity<T> Execute<T>()
        {
            PagedEntity<T> result;
            using (SqlStatement st = base.Query.Statement.Database.NewProcedure("SqlPager"))
            {
                try
                {
                    base.Query.Statement.Command.CommandText = base.Query.GenerateSql();
                    base.Query.ProcessParameters();
                    string sql = st.Database.Dialect.ParametersBuilder(base.Query.Statement.Command);
                    st.Add("sql", sql).Add("start", base.Start + 1).Add("limit", base.PageSize);
                    using (DbDataReader reader = st.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            reader.NextResult();
                        }
                        PagedEntity<T> pagedEntity = new PagedEntity<T>();
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
                        if (reader.NextResult() && reader.Read())
                        {
                            pagedEntity.Descriptor.Total = (long)reader.GetInt32(2);
                        }
                        result = pagedEntity;
                    }
                }
                catch (Exception err)
                {
                    if (err is SqlException && (err as SqlException).Number == 2812)
                    {
                        using (SqlStatement proc = st.Database.NewStatement("\r\nCREATE PROCEDURE SqlPager\r\n@sql nvarchar(4000),\r\n@start int,\r\n@limit int\r\nAS\r\nSET NOCOUNT ON\r\nDECLARE @cur int,@rowCount int\r\nEXECUTE sp_cursoropen @cur OUTPUT,@sql,@scrollopt=1,@ccopt=1,@rowCount=@rowCount OUTPUT\r\nEXECUTE sp_cursorfetch @cur,16,@start,@limit\r\nIF (@rowCount-@start)<@limit \r\nBEGIN\r\n\tSET @Limit=@rowCount-@start\r\nEND \r\nSELECT @start AS 'Start',@limit AS 'RowCount',@rowCount AS 'TotalRowCount'\r\nEXECUTE sp_cursorclose @cur\r\nSET NOCOUNT OFF\r\n"))
                        {
                            proc.Execute();
                            result = this.Execute<T>();
                            return result;
                        }
                    }
                    throw new Exception("执行分页查询时发生错误！", err);
                }
            }
            return result;
        }
    }
}