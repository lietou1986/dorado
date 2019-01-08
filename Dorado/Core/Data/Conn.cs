using Dorado.Core.Logger;
using Dorado.Extensions;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Dorado.Core.Data
{
    /// <summary>
    /// Sql Sever 数据库连接类。
    /// </summary>
    public class Conn : IConn
    {
        private static string _default;
        private SqlConnection _conn;
        private SqlTransaction _trans;
        private string _name = "mlist";
        private string _tablename;
        private int _top;
        private int _page = 1;
        private int _pagesize = 10;
        private int _maxcount;
        private ArrayList _field;
        private ArrayList _idx;
        private StringBuilder _join;
        private StringBuilder _where;
        private ArrayList _sort;
        private ArrayList _batchName;
        private ArrayList _batchSql;
        private string _keyid;
        private int _commandTimeOut;

        public Conn(string connectionString, int commandTimeOut = 60)
        {
            if (!connectionString.HasValue())
            {
                throw new CoreException("数据库连接串不能为空");
            }
            _commandTimeOut = commandTimeOut;
            _default = connectionString;
            _conn = new SqlConnection(connectionString);
        }

        /// <summary>
        /// 当前查询集名称
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value == null) throw new CoreException("对不起，Conn连接名称不能为空！");
                _name = value;
            }
        }

        public int CommandTimeOut
        {
            get { return _commandTimeOut; }
            set { _commandTimeOut = value; }
        }

        public int Top
        {
            get { return _top; }
            set
            {
                if (value <= 0) throw new CoreException("Top不能为负值和零!");
                _top = value;
            }
        }

        public int Page
        {
            get { return _page; }
            set
            {
                _page = value <= 0 ? 1 : value;
            }
        }

        public int PageSize
        {
            get { return _pagesize; }
            set
            {
                if (value >= 0)
                    _pagesize = value;
                else
                    throw new CoreException("每页显示条数必须为正值!");
            }
        }

        public int MaxCount
        {
            get { return _maxcount; }
            set
            {
                if (value >= 0)
                    _maxcount = value;
                else
                    throw new CoreException("记录集总数不能为负值!");
            }
        }

        public string DataBaseName
        {
            get
            {
                Open();
                return _conn.Database;
            }
            set { ChangeDatabase(value); }
        }

        public Conn SetTop(int top)
        {
            Top = top;
            return this;
        }

        public Conn SetPage(int page)
        {
            Page = page;
            return this;
        }

        public Conn SetPageSize(int pageSize)
        {
            PageSize = pageSize;
            return this;
        }

        public Conn SetMaxCount(int maxCount)
        {
            MaxCount = maxCount;
            return this;
        }

        public Conn SetDataBaseName(string dataBaseName)
        {
            DataBaseName = dataBaseName;
            return this;
        }

        public Conn SetName(string name)
        {
            Name = name;
            return this;
        }

        public Conn SetCommandTimeOut(int commandTimeOut)
        {
            CommandTimeOut = commandTimeOut;
            return this;
        }

        public Conn Add()
        {
            Add(Name, Sql());
            return this;
        }

        public Conn Add(string name)
        {
            Add(name, Sql());
            return this;
        }

        public Conn Add(string name, string sql)
        {
            if (name == null) throw new CoreException("对不起，添加数据集时，名称不能为空！");
            if (_batchName == null)
            {
                _batchName = new ArrayList();
                _batchSql = new ArrayList();
            }
            _batchName.Add(name);
            _batchSql.Add(sql);

            Clear();
            return this;
        }

        public Conn Add(string name, string sql, params object[] para)
        {
            Add(name, string.Format(sql, para));
            return this;
        }

        /// <summary>
        /// 改成连接的数据库
        /// </summary>
        /// <param name="databaseName">数据库名</param>
        public Conn ChangeDatabase(string databaseName)
        {
            Open();
            _conn.ChangeDatabase(databaseName);
            return this;
        }

        public SqlConnection Connection
        {
            get { return _conn; }
        }

        public SqlTransaction Transaction
        {
            get
            {
                return _trans;
            }
            set
            {
                _trans = value;
            }
        }

        public Conn Open()
        {
            if (_conn == null) _conn = new SqlConnection(_default);
            if (_conn.State != ConnectionState.Open) _conn.Open();
            return this;
        }

        public Conn BeginTrans()
        {
            if (_trans != null) return this;
            if (_conn == null) _conn = new SqlConnection(_default);
            if (_conn.State != ConnectionState.Open) _conn.Open();
            _trans = _conn.BeginTransaction(IsolationLevel.ReadUncommitted);
            return this;
        }

        public Conn BeginTrans(IsolationLevel isolationLevel)
        {
            if (_trans != null) return this;
            if (_conn == null) _conn = new SqlConnection(_default);
            if (_conn.State != ConnectionState.Open) _conn.Open();
            _trans = _conn.BeginTransaction(isolationLevel);
            return this;
        }

        public Conn Rollback()
        {
            if (_conn == null || _conn.State == ConnectionState.Closed) return this;
            if (_trans == null) return this;
            _trans.Rollback();
            _trans.Dispose();
            _trans = null;
            return this;
        }

        public Conn Close()
        {
            if (_conn == null || _conn.State == ConnectionState.Closed) return this;

            if (_trans != null)
            {
                _trans.Commit();
                _trans.Dispose();
                _trans = null;
            }

            _conn.Close();
            _conn.Dispose();
            _conn = null;
            return this;
        }

        public Conn From(string tableName)
        {
            _tablename = tableName;
            return this;
        }

        public Conn Fields(params string[] name)
        {
            if (_field == null) _field = new ArrayList();
            foreach (string t in name)
            {
                string[] tmp = t.Trim().ToLower().Split(' ');
                _field.Add(tmp.Length > 1
                           ? new string[] { tmp[0], tmp[1], null }
                           : new string[] { tmp[0], string.Empty, null });
            }
            return this;
        }

        public Conn KeyId(string keyid)
        {
            _keyid = keyid;
            return this;
        }

        public Conn Field(string func, string name, string alias)
        {
            if (_field == null) _field = new ArrayList();
            _field.Add(new string[] { "dbo." + func + "(" + name + ")", alias.ToLower(), null });
            return this;
        }

        public Conn Field(string name)
        {
            if (_field == null) _field = new ArrayList();
            _field.Add(new string[] { name.ToLower(), string.Empty, null });
            return this;
        }

        public Conn Field(string name, string alias)
        {
            if (_field == null) _field = new ArrayList();
            _field.Add(new string[] { name, alias.ToLower(), null });
            return this;
        }

        public Conn Field(string name, int length)
        {
            if (_field == null) _field = new ArrayList();
            _field.Add(new string[] { name.ToLower(), string.Empty, length.ToString() });
            return this;
        }

        public Conn Field(string name, string alias, int length)
        {
            if (_field == null) _field = new ArrayList();
            _field.Add(new string[] { name, alias.ToLower(), length.ToString() });
            return this;
        }

        public Conn Field(string name, bool isIndex)
        {
            if (isIndex)
            {
                if (_idx == null) _idx = new ArrayList();
                _idx.Add(new string[] { name.ToLower(), string.Empty, null });
            }
            else
            {
                if (_field == null) _field = new ArrayList();
                _field.Add(new string[] { name.ToLower(), string.Empty, null });
            }
            return this;
        }

        public Conn Field(string name, bool isIndex, int length)
        {
            if (isIndex)
            {
                if (_idx == null) _idx = new ArrayList();
                _idx.Add(new string[] { name.ToLower(), string.Empty, length.ToString() });
            }
            else
            {
                if (_field == null) _field = new ArrayList();
                _field.Add(new string[] { name.ToLower(), string.Empty, length.ToString() });
            }
            return this;
        }

        public Conn Field(string name, string alias, bool isIndex)
        {
            if (isIndex)
            {
                if (_idx == null) _idx = new ArrayList();
                _idx.Add(new string[] { name.ToLower(), alias, null });
            }
            else
            {
                if (_field == null) _field = new ArrayList();
                _field.Add(new string[] { name.ToLower(), alias, null });
            }
            return this;
        }

        public Conn Field(string name, string alias, bool isIndex, int length)
        {
            if (isIndex)
            {
                if (_idx == null) _idx = new ArrayList();

                _idx.Add(new string[] { name.ToLower(), alias, length.ToString() });
            }
            else
            {
                if (_field == null) _field = new ArrayList();
                _field.Add(new string[] { name.ToLower(), alias, length.ToString() });
            }
            return this;
        }

        public Conn Join(string tableName, string source, string dest)
        {
            if (_join == null) _join = new StringBuilder();
            _join.Append(" inner join " + tableName + " (nolock) on " + source + " = " + dest);
            return this;
        }

        public Conn Join(string tableName, string sql)
        {
            if (_join == null) _join = new StringBuilder();
            _join.Append(" inner join " + tableName + " (nolock) on " + sql);
            return this;
        }

        public Conn Join(string sql)
        {
            if (_join == null) _join = new StringBuilder();
            _join.Append(" inner join " + sql);
            return this;
        }

        public Conn LeftJoin(string tableName, string source, string dest)
        {
            if (_join == null) _join = new StringBuilder();
            _join.Append(" left join " + tableName + " (nolock) on " + source + " = " + dest);
            return this;
        }

        public Conn LeftJoin(string tableName, string sql)
        {
            if (_join == null) _join = new StringBuilder();
            _join.Append(" left join " + tableName + " (nolock) on " + sql);
            return this;
        }

        public Conn LeftJoin(string sql)
        {
            if (_join == null) _join = new StringBuilder();
            _join.Append(" left join " + sql);
            return this;
        }

        public Conn RightJoin(string tableName, string source, string dest)
        {
            if (_join == null) _join = new StringBuilder();
            _join.Append(" right join " + tableName + " (nolock) on " + source + " = " + dest);
            return this;
        }

        public Conn RightJoin(string tableName, string sql)
        {
            if (_join == null) _join = new StringBuilder();
            _join.Append(" right join " + tableName + " (nolock) on " + sql);
            return this;
        }

        public Conn RightJoin(string sql)
        {
            if (_join == null) _join = new StringBuilder();
            _join.Append(" right join " + sql);
            return this;
        }

        public Conn Clear()
        {
            _tablename = null;

            _top = 0;
            _page = 1;
            _pagesize = 20;
            _maxcount = 0;

            if (_idx != null) _idx.Clear();
            if (_field != null) _field.Clear();
            if (_join != null) _join.Remove(0, _join.Length);
            if (_where != null) _where.Remove(0, _where.Length);
            if (_sort != null) _sort.Clear();
            return this;
        }

        public Conn ClearField()
        {
            if (_idx != null) _idx.Clear();
            if (_field != null) _field.Clear();
            return this;
        }

        public Conn ClearWhere()
        {
            if (_where != null) _where.Remove(0, _where.Length);
            return this;
        }

        public Conn ClearJoin()
        {
            if (_join != null) _join.Remove(0, _join.Length);
            return this;
        }

        public Conn ClearOrder()
        {
            if (_order != null) _sort.Clear();
            return this;
        }

        public Conn Where(string sql)
        {
            if (_where == null) _where = new StringBuilder();
            _where.Append(_where.Length > 0 ? " and " : " where ");
            _where.Append("(" + sql + ")");
            return this;
        }

        public Conn Where(string[] fields, object[] data)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < fields.Length; i++)
            {
                if (sb.Length > 0) sb.Append(" and ");
                sb.Append(fields[i] + "=" + DataTypeExtensions.ToSql(data[i]));
            }
            Where(sb.ToString());
            return this;
        }

        public Conn Where(string sql, params object[] para)
        {
            for (int i = 0; i < para.Length; i++)
            {
                if (para[i] is string) para[i] = para[i].ToString();
            }
            Where(string.Format(sql, para));
            return this;
        }

        public Conn WhereIn(string sql, params object[] para)
        {
            if (_where == null) _where = new StringBuilder();
            _where.Append(_where.Length > 0 ? " and " : " where ");
            _where.Append("(" + string.Format(sql, para) + ")");
            return this;
        }

        public Conn WhereOr(string sql, params object[] para)
        {
            if (_where == null) _where = new StringBuilder();
            _where.Append(_where.Length > 0 ? " or " : " where ");
            for (int i = 0; i < para.Length; i++)
            {
                if (para[i] is string) para[i] = para[i].ToString();
            }
            _where.Append("(" + string.Format(sql, para) + ")");
            return this;
        }

        public Conn Order(string field)
        {
            if (_sort == null) _sort = new ArrayList(3);
            field = field.Trim().ToLower();
            string[] tmp = field.Split(',');
            foreach (string t in tmp)
            {
                string[] s = new string[3];
                if (t.IndexOf(" desc") >= 0)
                    s[1] = "desc";
                else
                    s[1] = "asc";
                s[0] = t.Replace(" desc", string.Empty).Replace(" asc", string.Empty).Trim();
                s[2] = null;
                _sort.Add(s);
            }
            return this;
        }

        //生成排序Sql字符串
        private string _order
        {
            get
            {
                if (_sort == null) return string.Empty;
                StringBuilder sort = new StringBuilder();
                foreach (object t1 in _sort)
                {
                    string[] t = (string[])t1;
                    if (sort.Length > 0) sort.Append(','); else sort.Append(" order by ");
                    sort.Append(t[0] + " " + t[1]);
                }
                return sort.ToString();
            }
        }

        public int ExecuteNonQuery(string sql)
        {
            Open();
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        public int ExecuteNonQuery(string sql, params object[] para)
        {
            for (int i = 0; i < para.Length; i++)
            {
                if (para[i] is string) para[i] = para[i].ToString();
            }
            Open();
            using (SqlCommand cmd = new SqlCommand(string.Format(sql, para), _conn))
            {
                if (Transaction != null) cmd.Transaction = Transaction;
                return cmd.ExecuteNonQuery();
            }
        }

        public object ExecuteScalar(string sql)
        {
            Open();
            using (SqlCommand cmd = new SqlCommand(sql, _conn))
            {
                if (Transaction != null) cmd.Transaction = Transaction;
                return cmd.ExecuteScalar();
            }
        }

        public object ExecuteScalar(string sql, params object[] para)
        {
            for (int i = 0; i < para.Length; i++)
            {
                if (para[i] is string) para[i] = para[i].ToString();
            }
            Open();
            using (SqlCommand cmd = new SqlCommand(string.Format(sql, para), _conn))
            {
                if (Transaction != null) cmd.Transaction = Transaction;
                return cmd.ExecuteScalar();
            }
        }

        public DataArrayList SelectBatch()
        {
            return SelectBatch(null);
        }

        public DataArrayList SelectBatch(DataArrayList list)
        {
            if (_batchName == null) throw new CoreException("您还没有指定任何查询语句！");
            if (list == null) list = new DataArrayList();
            StringBuilder sb = new StringBuilder();
            foreach (object t in _batchSql)
            {
                sb.Append(t + ";\n");
            }
            Open();
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                cmd = new SqlCommand(sb.ToString(), Connection) { CommandTimeout = _commandTimeOut };
                reader = cmd.ExecuteReader();

                int index = 0;
                do
                {
                    string name = _batchName[index].ToString();
                    DataArrayColumn[] columns = new DataArrayColumn[reader.FieldCount];
                    var data = new DataArray(name);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        columns[i] = data.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                    }

                    while (reader.Read())
                    {
                        data.AddRow();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            columns[i].Set(reader.GetValue(i), data.Count - 1);
                        }
                    }

                    list.Add(data);

                    index++;
                } while (reader.NextResult());
                reader.Close();
            }
            catch (SqlException ex)
            {
                LoggerWrapper.Logger.Error("Conn.SelectBacth发生错误：", ex);
            }
            finally
            {
                if (reader != null) reader.Close();
                if (cmd != null) cmd.Dispose();
            }
            _batchName.Clear();
            _batchSql.Clear();
            return list;
        }

        public DataArrayList SelectAll()
        {
            return SelectAll(null);
        }

        public DataArrayList SelectAll(DataArrayList list)
        {
            if (_batchName == null) throw new CoreException("您还没有指定任何查询语句！");
            if (list == null) list = new DataArrayList();
            Open();
            StringBuilder error = new StringBuilder();
            for (int i = 0; i < _batchSql.Count; i++)
            {
                SqlCommand cmd = null;
                SqlDataReader reader = null;
                try
                {
                    cmd = new SqlCommand(_batchSql.ToString(), Connection) { CommandTimeout = _commandTimeOut };
                    reader = cmd.ExecuteReader();

                    list.Add(_batchName[i].ToString(), reader.Exec(null, false));
                    reader.Close();
                }
                catch (SqlException ex)
                {
                    error.Append("错误：" + ex.Message + "\n" + _batchSql[i].ToString() + "\n");
                }
                finally
                {
                    if (reader != null) reader.Close();
                    if (cmd != null) cmd.Dispose();
                }
            }
            if (error.Length > 0)
            {
                LoggerWrapper.Logger.Error("Conn.SelectAll发生错误：\r\n" + error.ToString());
            }
            _batchName.Clear();
            _batchSql.Clear();
            return list;
        }

        private string Fld()
        {
            StringBuilder ret = new StringBuilder();
            if (_idx != null)
            {
                foreach (string[] tmp in from object t in _idx select t as string[])
                {
                    if (ret.Length > 0) ret.Append(",");
                    if (tmp[2] == null)
                        ret.Append(tmp[0] + " " + tmp[1]);
                    else
                        ret.Append("cast(" + tmp[0] + " as nvarchar(" + tmp[2] + "))+'..' " + (tmp[1] == string.Empty ? tmp[0] : tmp[1]));
                }
            }
            if (_field == null) return ret.ToString();
            foreach (string[] tmp in from object t in _field select t as string[])
            {
                if (ret.Length > 0) ret.Append(",");
                if (tmp[2] == null)
                    ret.Append(tmp[0] + " " + tmp[1]);
                else
                    ret.Append("cast(" + tmp[0] + " as nvarchar(" + tmp[2] + "))+'..' " + (tmp[1] == string.Empty ? tmp[0] : tmp[1]));
            }
            return ret.ToString();
        }

        #region 简单查询 Select

        public string Sql()
        {
            if (_tablename == null) throw new CoreException("对不起，您没有指定表名！");
            return "select"
                + (_top > 0 ? " top " + _top : string.Empty) + " "
                + Fld()
                + " from "
                + _tablename + " (nolock) "
                + (_join == null ? string.Empty : _join.ToString()) + " "
                + (_where == null ? string.Empty : _where.ToString()) + " "
                + _order;
        }

        public DataArray Select(string sql, params object[] para)
        {
            for (int i = 0; i < para.Length; i++)
            {
                if (para[i] is string) para[i] = para[i].ToString();
            }
            return Select(string.Format(sql, para));
        }

        public DataArray Select(string sql = "")
        {
            sql = string.IsNullOrWhiteSpace(sql) ? Sql() : sql;

            DataArray data = new DataArray(_name);
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                Open();
                cmd = new SqlCommand(sql, Connection) { CommandTimeout = _commandTimeOut };
                reader = cmd.ExecuteReader();
                reader.Exec(data, false);
            }
            catch (SqlException ex)
            {
                LoggerWrapper.Logger.Error("Conn.Select发生错误：" + sql, ex);
                throw;
            }
            finally
            {
                if (reader != null) reader.Close();
                if (cmd != null) cmd.Dispose();
            }
            data.PageSize = PageSize;
            return data;
        }

        #endregion 简单查询 Select

        #region 游标分页查询 SelectCursor

        private string SqlCursor(string sql, bool isLimitPageIndex)
        {
            if (_pagesize <= 0) _pagesize = 20;
            if (_page <= 0) _page = 1;
            StringBuilder sb = new StringBuilder();
            sb.Append("declare @P1 int, @rowcount int, @page int, @pagesize int,@sqlstr nvarchar(4000);\n");
            sb.Append("set @page=" + _page.ToString() + ";\n");
            sb.Append("set @pagesize=" + _pagesize.ToString() + ";\n");
            sb.Append("set @sqlstr=N'" + sql.Replace("'", "''") + "';\n");
            sb.Append("exec sp_cursoropen @P1 output,@sqlstr,@scrollopt=1,@ccopt=1, @rowcount=@rowcount output;\n");
            if (isLimitPageIndex)
            {
                sb.Append("if @page*" + _pagesize.ToString() + "> @rowcount + " + _pagesize.ToString() + " ");
                sb.Append("set @page=(@rowcount-1)/" + _pagesize.ToString() + "+1;\n");
            }
            sb.Append("select @page,@rowcount,ceiling(1.0*@rowcount/@pagesize);\n");
            sb.Append("set @page=(@page-1)*@pagesize+1;\n");
            sb.Append("exec sp_cursorfetch @P1,16,@page," + _pagesize.ToString() + ";\n");
            sb.Append("exec sp_cursorclose @P1;\n");
            return sb.ToString();
        }

        public DataArray SelectCursor(bool isLimitPageIndex = true)
        {
            return SelectCursor(null, isLimitPageIndex);
        }

        public DataArray SelectCursor(string sql, bool isLimitPageIndex = true)
        {
            sql = SqlCursor(sql ?? Sql(), isLimitPageIndex);

            if (_pagesize == 0) _pagesize = 20;
            DataArray data = new DataArray(_name, _pagesize) { PageSize = _pagesize };

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                Open();
                cmd = new SqlCommand(sql, _conn) { CommandTimeout = _commandTimeOut };

                reader = cmd.ExecuteReader();

                reader.NextResult();
                reader.Read();
                _page = reader.GetInt32(0);
                _maxcount = reader.GetInt32(1);
                data.Page = _page;
                data.MaxCount = _maxcount;

                reader.NextResult();

                reader.Exec(data, false);
            }
            catch (SqlException ex)
            {
                LoggerWrapper.Logger.Error("Conn.SelectCursor发生错误：" + sql, ex);
                throw ex;
            }
            finally
            {
                if (reader != null) reader.Close();
                if (cmd != null) cmd.Dispose();
            }
            return data;
        }

        #endregion 游标分页查询 SelectCursor

        #region RowNumber分页查询 SelectPage

        private string SqlRowNumber()
        {
            if (_pagesize <= 0) _pagesize = 20;
            if (_page <= 0) _page = 1;
            string fld = Fld().Replace("'", "''");
            string join = _join == null ? string.Empty : _join.ToString().Replace("'", "''");
            string where = _where == null ? string.Empty : _where.ToString().Replace("'", "''");
            StringBuilder sb = new StringBuilder();
            sb.Append("declare @select varchar(8000);\n");
            sb.Append("set @select ='select count(1)  from " + _tablename + " " + (join) + " " + (where) + ";");
            sb.Append("select * from (select " + fld + ",ROW_NUMBER() over(" + _order + ") as rowNumber  from " + _tablename + " " + (join) + " " + (where) + " )  " + _tablename + " where  rowNumber between (((" + _page + " - 1) * " + _pagesize + ")+1) and (" + _page + "*" + _pagesize + ")';\n");
            sb.Append("exec(@select);\n");
            return sb.ToString();
        }

        public DataArray SelectPage(string sql = "")
        {
            if (_page == 1 && _pagesize == 1) return Select(sql);

            sql = string.IsNullOrWhiteSpace(sql) ? SqlRowNumber() : sql;

            if (_pagesize == 0) _pagesize = 20;
            DataArray data = new DataArray(_name, _pagesize) { PageSize = _pagesize };

            SqlCommand cmd = null;
            SqlDataReader reader = null;
            try
            {
                Open();
                cmd = new SqlCommand(sql, _conn) { CommandTimeout = _commandTimeOut };

                reader = cmd.ExecuteReader();
                reader.Read();
                _maxcount = reader.GetInt32(0);
                data.Page = _page;
                data.MaxCount = _maxcount;
                reader.NextResult();

                reader.Exec(data, false);
            }
            catch (SqlException ex)
            {
                LoggerWrapper.Logger.Error("Conn.SelectPage发生错误：" + sql, ex);
                throw;
            }
            finally
            {
                if (reader != null) reader.Close();
                if (cmd != null) cmd.Dispose();
            }
            return data;
        }

        #endregion RowNumber分页查询 SelectPage

        public void Dispose()
        {
            Close();
        }
    }
}