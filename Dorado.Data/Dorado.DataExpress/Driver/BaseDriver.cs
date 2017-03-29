using System;
using System.Data.Common;
using System.Linq.Expressions;

namespace Dorado.DataExpress.Driver
{
    public abstract class BaseDriver
    {
        private Func<object> _newCommandFunc;
        private Func<object> _newConnectionFunc;
        private Func<object> _newDataAdapterFunc;

        public abstract Type ConnectionProvider
        {
            get;
        }

        public abstract Type CommandProvider
        {
            get;
        }

        public abstract Type DataAdapterProvider
        {
            get;
        }

        public virtual bool SupportMuliReader
        {
            get
            {
                return false;
            }
        }

        public virtual string Prefix
        {
            get
            {
                return "";
            }
        }

        public virtual string Suffix
        {
            get
            {
                return "";
            }
        }

        public virtual DbConnection CreateConnection()
        {
            if (this._newConnectionFunc == null)
            {
                NewExpression exp = Expression.New(this.ConnectionProvider);
                Expression<Func<object>> lambdaExp = Expression.Lambda<Func<object>>(exp, null);
                this._newConnectionFunc = lambdaExp.Compile();
            }
            return (DbConnection)this._newConnectionFunc();
        }

        public virtual DbCommand CreateCommand()
        {
            if (this._newCommandFunc == null)
            {
                NewExpression exp = Expression.New(this.CommandProvider);
                Expression<Func<object>> lambdaExp = Expression.Lambda<Func<object>>(exp, null);
                this._newCommandFunc = lambdaExp.Compile();
            }
            return (DbCommand)this._newCommandFunc();
        }

        public virtual DbDataAdapter CreateDataAdapter()
        {
            if (this._newDataAdapterFunc == null)
            {
                NewExpression exp = Expression.New(this.DataAdapterProvider);
                Expression<Func<object>> lambdaExp = Expression.Lambda<Func<object>>(exp, null);
                this._newDataAdapterFunc = lambdaExp.Compile();
            }
            return (DbDataAdapter)this._newDataAdapterFunc();
        }

        public virtual DbConnection CreateConnection(string connectionString)
        {
            DbConnection conn = this.CreateConnection();
            conn.ConnectionString = connectionString;
            return conn;
        }

        public virtual DbCommand CreateCommand(DbConnection conn)
        {
            DbCommand cmd = this.CreateCommand();
            cmd.Connection = conn;
            return cmd;
        }

        public virtual DbCommand CreateCommand(DbConnection conn, DbTransaction trans)
        {
            DbCommand cmd = this.CreateCommand(conn);
            cmd.Transaction = trans;
            return cmd;
        }

        public virtual DbCommand CreateCommand(string sql)
        {
            DbCommand cmd = this.CreateCommand();
            cmd.CommandText = sql;
            return cmd;
        }

        public virtual DbCommand CreateCommand(string sql, DbConnection conn)
        {
            DbCommand cmd = this.CreateCommand(sql);
            cmd.Connection = conn;
            return cmd;
        }

        public virtual DbCommand CreateCommand(string sql, DbConnection conn, DbTransaction trans)
        {
            DbCommand cmd = this.CreateCommand(sql, conn);
            cmd.Transaction = trans;
            return cmd;
        }

        public virtual DbDataAdapter CreateDataAdapter(DbCommand cmd)
        {
            DbDataAdapter adp = this.CreateDataAdapter();
            adp.SelectCommand = cmd;
            return adp;
        }

        public virtual string BackupSql(string databse, string dir)
        {
            throw new NotImplementedException("尚未实现该驱动的备份SQL脚本");
        }
    }
}