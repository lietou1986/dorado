using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.DataExpress.Dialect;
using Dorado.DataExpress.Driver;
using Dorado.DataExpress.Ldo;
using Dorado.DataExpress.NamedQuery;
using Dorado.DataExpress.Schema;
using Dorado.DataExpress.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace Dorado.DataExpress
{
    public class Database : IDisposable
    {
        private const string ParameterPrefix = "@(\\w+)";
        internal static BaseDriver DefaultDriver = new MsSql2000();
        internal static BaseDialect DefaultDialect = new MsSql();
        internal List<SqlStatement> ActiveStatement = new List<SqlStatement>();
        internal List<Transaction> ActiveTransactions = new List<Transaction>();
        private DbConnection _connection;
        private string _connectionString = "";
        private DateTime _lastActive = DateTime.MinValue;
        private DatabasePool _pool;
        private bool _isDisposing;

        public bool EnableSqlTrace
        {
            get;
            set;
        }

        public DbConnection Connection
        {
            get
            {
                return this._connection;
            }
            set
            {
                this._connection = value;
            }
        }

        public BaseDriver Driver
        {
            get;
            set;
        }

        public BaseDialect Dialect
        {
            get;
            set;
        }

        public DatabasePool Pool
        {
            get
            {
                return this._pool;
            }
            set
            {
                this._pool = value;
            }
        }

        public string HostSource
        {
            get;
            set;
        }

        public string ConnectionString
        {
            get
            {
                return this._connectionString;
            }
            set
            {
                this._connectionString = value;
            }
        }

        public DateTime LastActive
        {
            get
            {
                return this._lastActive;
            }
            set
            {
                this._lastActive = value;
            }
        }

        public bool Closed
        {
            get;
            set;
        }

        public SqlStatement this[string queryName]
        {
            get
            {
                LoggerWrapper.Logger.Debug(string.Format("获取命名查询{0}", new object[]
				{
					queryName
				}));
                return this.NewNamedStatement(queryName);
            }
        }

        public Database()
        {
        }

        public Database(BaseDriver driver, BaseDialect dialect, string connectionString)
        {
            this.Driver = driver;
            this.Dialect = dialect;
            this.ConnectionString = connectionString;
            this.InitConnection();
        }

        public Database(string connectionString)
        {
            this.Driver = Database.DefaultDriver;
            this.Dialect = Database.DefaultDialect;
            this.ConnectionString = connectionString;
            this.InitConnection();
        }

        public static Database Open<TDriver, TDialect>(string connection)
            where TDriver : BaseDriver, new()
            where TDialect : BaseDialect, new()
        {
            return new Database(Activator.CreateInstance<TDriver>(), Activator.CreateInstance<TDialect>(), connection);
        }

        public DeleteStatement Delete()
        {
            return new DeleteStatement(null, this.NewStatement());
        }

        internal void UpdateHostSource()
        {
            if (HttpContext.Current != null)
            {
                this.HostSource = HttpContext.Current.Request.RawUrl;
            }
        }

        public void InitConnection()
        {
            if (this.Driver != null && this.Dialect != null)
            {
                this._connection = this.Driver.CreateConnection();
                this._connection.ConnectionString = this.ConnectionString;
                this._connection.StateChange += new StateChangeEventHandler(this.OnConnectionStateChange);
                this._connection.Open();
                this.UpdateIdle();
            }
        }

        private void OnConnectionStateChange(object sender, StateChangeEventArgs e)
        {
        }

        public void Check()
        {
            if (this._connection.State == ConnectionState.Closed)
            {
                this._connection.Open();
            }
        }

        public void UpdateIdle()
        {
            Monitor.Enter(this);
            try
            {
                this._lastActive = DateTime.Now;
            }
            finally
            {
                Monitor.Exit(this);
            }
        }

        internal SqlStatement NewProcedure(string command)
        {
            SqlStatement st = this.NewStatement();
            st.Command.CommandType = CommandType.StoredProcedure;
            st.Command.CommandText = command;
            return st;
        }

        public SqlStatement Procedure(string command)
        {
            return this.NewProcedure(command);
        }

        public QueryStatement Select(params ColumnSchema[] columns)
        {
            QueryStatement qry = new QueryStatement
            {
                Statement = this.NewStatement()
            };
            for (int i = 0; i < columns.Length; i++)
            {
                ColumnSchema col = columns[i];
                qry.AddColumn(col);
            }
            return qry;
        }

        public QueryStatement Select(params BaseExpression[] exps)
        {
            QueryStatement qry = new QueryStatement
            {
                Statement = this.NewStatement()
            };
            for (int i = 0; i < exps.Length; i++)
            {
                BaseExpression col = exps[i];
                qry.AddColumn(col);
            }
            return qry;
        }

        public QueryStatement Select()
        {
            return new QueryStatement
            {
                Statement = this.NewStatement()
            };
        }

        [Obsolete("建议使用from方法，这样更符合LINQ规范")]
        public QueryStatement<TEntity> Select<TEntity>() where TEntity : new()
        {
            return new QueryStatement<TEntity>
            {
                Statement = this.NewStatement()
            };
        }

        public QueryStatement<TEntity> From<TEntity>() where TEntity : new()
        {
            return new QueryStatement<TEntity>
            {
                Statement = this.NewStatement()
            };
        }

        public QueryStatement<TEntity> Select<TEntity>(params string[] columns) where TEntity : new()
        {
            QueryStatement<TEntity> qry = new QueryStatement<TEntity>
            {
                Statement = this.NewStatement()
            };
            for (int i = 0; i < columns.Length; i++)
            {
                string item = columns[i];
                qry.AddColumn(item);
            }
            return qry;
        }

        public QueryStatement<TEntity> Select<TEntity>(params BaseExpression[] exps) where TEntity : new()
        {
            QueryStatement<TEntity> qry = new QueryStatement<TEntity>
            {
                Statement = this.NewStatement()
            };
            for (int i = 0; i < exps.Length; i++)
            {
                BaseExpression item = exps[i];
                qry.AddColumn(item);
            }
            return qry;
        }

        public TEntity Get<TEntity>(object keyValue) where TEntity : new()
        {
            QueryStatement<TEntity> qry = new QueryStatement<TEntity>
            {
                Statement = this.NewStatement()
            };
            qry.Where(SqlExpression.Eq(BinderManager<TEntity>.EntityInfo.PrimaryKey.Schema, keyValue));
            return qry.First();
        }

        public int Delete<TEntity>(object keyValue) where TEntity : new()
        {
            DeleteStatement del = this.DeleteFrom(BinderManager<TEntity>.EntityInfo.Table.TableName);
            del.Where(SqlExpression.Eq(BinderManager<TEntity>.EntityInfo.PrimaryKey.Schema, keyValue));
            return del.Execute();
        }

        public QueryStatement SelectAll()
        {
            return this.Select();
        }

        public QueryStatement Select(params string[] columns)
        {
            QueryStatement qry = new QueryStatement
            {
                Statement = this.NewStatement()
            };
            for (int i = 0; i < columns.Length; i++)
            {
                string col = columns[i];
                qry.AddColumn(col);
            }
            return qry;
        }

        public SqlStatement Execute(string funcName)
        {
            return this.NewProcedure(funcName);
        }

        internal SqlStatement NewFunction(string command)
        {
            return this.NewProcedure(command);
        }

        public SqlStatement Command(string command)
        {
            return this.NewProcedure(command);
        }

        internal SqlStatement NewStatement()
        {
            SqlStatement st = new SqlStatement(this);
            this.UpdateIdle();
            if (this.ActiveTransactions.Count > 0)
            {
                st.Command.Transaction = this.ActiveTransactions[0].InnerTransaction;
            }
            this.ActiveStatement.Add(st);
            return st;
        }

        internal SqlStatement NewStatement(string query)
        {
            SqlStatement st = this.NewStatement();
            st.Command.CommandText = query;
            return st;
        }

        public SqlStatement Sql(string query)
        {
            return this.NewStatement(query);
        }

        internal QueryStatement NewQuery(TableSchema table)
        {
            return new QueryStatement(table, this.NewStatement());
        }

        internal QueryStatement NewQuery(string tableName)
        {
            return this.NewQuery(tableName, tableName);
        }

        internal QueryStatement NewQuery(string tableName, string alias)
        {
            if (!this.Dialect.ValidateSystemName(tableName))
            {
                LoggerWrapper.Logger.Debug(string.Format("表名称[{0}]错误，表中不允许包含特殊字符!", new object[]
				{
					tableName
				}));
                throw new Exception(string.Format("表名称[{0}]错误，表中不允许包含特殊字符!", tableName));
            }
            return this.NewQuery(new TableSchema(tableName)
            {
                Alias = alias
            });
        }

        public QueryStatement Query(TableSchema table)
        {
            return this.NewQuery(table);
        }

        public QueryStatement Query(string tableName)
        {
            return this.NewQuery(tableName);
        }

        public QueryStatement Query(string tableName, string alias)
        {
            return this.NewQuery(tableName, alias);
        }

        internal InsertStatement NewInsert(TableSchema table)
        {
            return new InsertStatement(table, this.NewStatement());
        }

        internal InsertStatement NewInsert(string tableName)
        {
            if (!this.Dialect.ValidateSystemName(tableName))
            {
                throw new Exception(string.Format("表名称[{0}]错误，表中不允许包含特殊字符!", tableName));
            }
            TableSchema table = new TableSchema(tableName);
            return this.NewInsert(table);
        }

        public InsertStatement Insert(TableSchema table)
        {
            return this.NewInsert(table);
        }

        public InsertStatement InsertInto(TableSchema table)
        {
            return this.NewInsert(table);
        }

        public InsertStatement InsertInto(string tableName)
        {
            return this.NewInsert(tableName);
        }

        public InsertStatement Insert(string tableName)
        {
            return this.NewInsert(tableName);
        }

        public InsertStatement<T> InsertInto<T>() where T : new()
        {
            return new InsertStatement<T>
            {
                Statement = this.NewStatement(),
                Table = BinderManager<T>.EntityInfo.Schema
            };
        }

        public InsertStatement<T> InsertInto<T>(Expression<Func<T>> expression) where T : new()
        {
            InsertStatement<T> insert = this.InsertInto<T>();
            return insert.Set(expression);
        }

        public InsertStatement<T> Insert<T>(Expression<Func<T>> expression) where T : new()
        {
            return this.InsertInto<T>(expression);
        }

        internal UpdateStatement NewUpdate(TableSchema table)
        {
            return new UpdateStatement(table, this.NewStatement());
        }

        internal UpdateStatement NewUpdate(string tableName)
        {
            if (!this.Dialect.ValidateSystemName(tableName))
            {
                throw new Exception(string.Format("表名称[{0}]错误，表中不允许包含特殊字符!", tableName));
            }
            return this.NewUpdate(new TableSchema(tableName));
        }

        public UpdateStatement Update(TableSchema table)
        {
            return this.NewUpdate(table);
        }

        public UpdateStatement<TEntity> Update<TEntity>() where TEntity : new()
        {
            return new UpdateStatement<TEntity>(this.NewStatement());
        }

        public UpdateStatement Update(string tableName)
        {
            return this.NewUpdate(tableName);
        }

        internal DeleteStatement NewDelete(TableSchema table)
        {
            return new DeleteStatement(table, this.NewStatement());
        }

        internal DeleteStatement NewDelete(string tableName)
        {
            if (!this.Dialect.ValidateSystemName(tableName))
            {
                throw new Exception(string.Format("表名称[{0}]错误，表中不允许包含特殊字符!", tableName));
            }
            return this.NewDelete(new TableSchema(tableName));
        }

        public DeleteStatement Delete(TableSchema table)
        {
            return this.NewDelete(table);
        }

        public DeleteStatement DeleteFrom(TableSchema table)
        {
            return this.NewDelete(table);
        }

        public DeleteStatement Delete(string tableName)
        {
            return this.NewDelete(tableName);
        }

        public DeleteStatement DeleteFrom(string tableName)
        {
            return this.NewDelete(tableName);
        }

        internal SqlStatement NewStatement(string query, Transaction transaction)
        {
            SqlStatement st = this.NewStatement(query);
            if (transaction != null)
            {
                st.Command.Transaction = transaction.InnerTransaction;
            }
            return st;
        }

        public SqlStatement NewNamedStatement(string name)
        {
            QueryNode st = QueryCache.Default[name];
            if (st != null)
            {
                string qry = st.Query;
                Regex reg = new Regex("@(\\w+)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                qry = reg.Replace(qry, this.Dialect.ParameterPrefix + "$1");
                return this.NewStatement(qry);
            }
            throw new Exception(string.Format("未能找到名为[{0}]的查询", name));
        }

        public Transaction BeginTransaction()
        {
            Transaction trans = new Transaction(this);
            this.ActiveTransactions.Add(trans);
            return trans;
        }

        public void Clear()
        {
            while (this.ActiveTransactions.Count != 0)
            {
                Transaction trans = this.ActiveTransactions[0];
                try
                {
                    trans.Dispose();
                }
                catch
                {
                    this.ActiveTransactions.Remove(trans);
                }
            }
            while (this.ActiveStatement.Count != 0)
            {
                SqlStatement stmt = this.ActiveStatement[0];
                try
                {
                    stmt.Dispose();
                }
                catch
                {
                    this.ActiveStatement.Remove(stmt);
                }
            }
        }

        public void Commit(Transaction trans)
        {
            trans.Commit();
            if (this.ActiveTransactions.Contains(trans))
            {
                this.ActiveTransactions.Remove(trans);
            }
            trans.Dispose();
        }

        public void Commit()
        {
            if (this.ActiveTransactions.Count != 0)
            {
                Transaction trans = this.ActiveTransactions[0];
                trans.Commit();
                this.ActiveTransactions.Remove(trans);
                trans.Dispose();
            }
        }

        public void RollBack(Transaction trans)
        {
            trans.Rollback();
            if (this.ActiveTransactions.Contains(trans))
            {
                LoggerWrapper.Logger.Debug(string.Format("数据库[Name:{0};ID:{1}]开始事务回滚", new object[]
				{
					(this.Pool == null) ? "" : this.Pool.Name,
					this.GetHashCode()
				}));
                this.ActiveTransactions.Remove(trans);
            }
            trans.Dispose();
        }

        public void RollBack()
        {
            if (this.ActiveTransactions.Count != 0)
            {
                LoggerWrapper.Logger.Debug(string.Format("数据库[Name:{0};ID:{1}]开始事务回滚", new object[]
				{
					(this.Pool == null) ? "" : this.Pool.Name,
					this.GetHashCode()
				}));
                Transaction trans = this.ActiveTransactions[0];
                trans.Rollback();
                this.ActiveTransactions.Remove(trans);
                trans.Dispose();
            }
        }

        internal void ReallyClose()
        {
            this.Clear();
            if (!this.Closed && this._pool != null)
            {
                this._pool.Return(this);
            }
            if (this.Connection != null && this.Connection.State == ConnectionState.Open)
            {
                this.Connection.Close();
            }
            if (this.Connection != null)
            {
                this.Connection.Dispose();
            }
        }

        internal void Close()
        {
            if (this.Closed)
            {
                return;
            }
            if (this._pool == null)
            {
                this.ReallyClose();
                return;
            }
            if (this.Pool.UseNativePool)
            {
                this.Connection.Close();
                this.Connection.Dispose();
                return;
            }
            this.Clear();
            this._pool.Return(this);
        }

        public string[] Backup(string dir, bool zip, string password)
        {
            LoggerWrapper.Logger.Debug(string.Format("开始备份数据库[{0}]，目录:{1} 启用压缩:{2}", new object[]
			{
				this.Connection.Database,
				dir,
				zip
			}));
            string sql = this.Driver.BackupSql(this.Connection.Database, dir);
            DataTable table = this.Sql(sql).ExecuteDataTable();
            List<string> files = new List<string>(table.Rows.Count);
            foreach (DataRow row in table.Rows)
            {
                files.Add(row[0] as string);
            }
            return files.ToArray();
        }

        public string[] Backup(string dir)
        {
            return this.Backup(dir, true, string.Empty);
        }

        public string[] Backup(string dir, string password)
        {
            return this.Backup(dir, true, password);
        }

        public override int GetHashCode()
        {
            return this.Connection.GetHashCode();
        }

        public void Dispose()
        {
            if (this._isDisposing)
            {
                return;
            }
            this._isDisposing = true;
            this.Close();
        }
    }
}