using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.DataExpress.Ldo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;

namespace Dorado.DataExpress
{
    public class SqlStatement : IDisposable
    {
        private bool _disposing;

        public Database Database
        {
            get;
            set;
        }

        public DbCommand Command
        {
            get;
            set;
        }

        public SqlStatement()
        {
            this.Command = null;
            this.Database = null;
        }

        public SqlStatement(Database db)
        {
            this.Database = db;
            this.Command = db.Driver.CreateCommand(db.Connection);
        }

        public static implicit operator DataSet(SqlStatement st)
        {
            return st.ExecuteDataSet();
        }

        public static implicit operator DataTable(SqlStatement st)
        {
            return st.ExecuteDataTable();
        }

        public static implicit operator DbDataReader(SqlStatement st)
        {
            return st.ExecuteReader();
        }

        public static implicit operator int(SqlStatement st)
        {
            return (int)st.ExecuteScalar();
        }

        public static implicit operator long(SqlStatement st)
        {
            return (long)st.ExecuteScalar();
        }

        public static implicit operator string(SqlStatement st)
        {
            return (string)st.ExecuteScalar();
        }

        public static implicit operator double(SqlStatement st)
        {
            return (double)st.ExecuteScalar();
        }

        public static implicit operator bool(SqlStatement st)
        {
            return (bool)st.ExecuteScalar();
        }

        public List<T> Bind<T>(Action<T, IDataRecord> bindAction) where T : new()
        {
            if (bindAction == null)
            {
                throw new ArgumentNullException("bindAction");
            }
            DbDataReader reader = this.ExecuteReader();
            List<T> entityList;
            if (reader.HasRows)
            {
                entityList = new List<T>();
                while (reader.Read())
                {
                    T entity = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
                    bindAction(entity, reader);
                    entityList.Add(entity);
                }
            }
            else
            {
                entityList = null;
            }
            return entityList;
        }

        public DbDataReader ExecuteReader()
        {
            this.TraceSql();
            return this.Command.ExecuteReader();
        }

        public DataTable ExecuteDataTable()
        {
            DataTable result;
            using (DbDataAdapter adp = this.Database.Driver.CreateDataAdapter())
            {
                this.TraceSql();
                adp.SelectCommand = this.Command;
                DataTable table = new DataTable();
                adp.Fill(table);
                result = table;
            }
            return result;
        }

        public void ForEach(Action<IDataRecord> elementAction)
        {
            using (DbDataReader reader = this.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        elementAction(reader);
                    }
                }
            }
        }

        protected virtual void TraceSql()
        {
            if (this.Database != null && (this.Database.EnableSqlTrace || (this.Database.Pool != null && this.Database.Pool.Logging)))
            {
                StringBuilder builder = new StringBuilder(this.Command.CommandText.Length + this.Command.Parameters.Count * 32);
                builder.Append("\r\n---------------\r\n").Append("SQL脚本:\r\n").Append(this.Command.CommandText).Append("\r\n");
                if (this.Command.Parameters.Count > 0)
                {
                    builder.Append("参数:\r\n");
                    foreach (DbParameter pm in this.Command.Parameters)
                    {
                        builder.Append(pm.ParameterName).Append("[").Append(pm.DbType).Append("]").Append("=").Append(pm.Value).Append(this.Database.Dialect.NewLine);
                    }
                }
                builder.Append("\r\n---------------\r\n");
                LoggerWrapper.Logger.Debug(builder.ToString());
            }
        }

        public DataSet ExecuteDataSet()
        {
            DataSet result;
            using (DbDataAdapter adp = this.Database.Driver.CreateDataAdapter(this.Command))
            {
                this.TraceSql();
                DataSet ds = new DataSet();
                adp.Fill(ds);
                result = ds;
            }
            return result;
        }

        public object ExecuteScalar()
        {
            this.TraceSql();
            return this.Command.ExecuteScalar();
        }

        public T ExecuteScalar<T>()
        {
            this.TraceSql();
            return (T)this.Command.ExecuteScalar();
        }

        public object[] ExecuteUnique()
        {
            this.TraceSql();
            object[] result;
            using (DbDataReader reader = this.ExecuteReader())
            {
                int count = reader.FieldCount;
                if (reader.HasRows)
                {
                    reader.Read();
                    object[] objs = new object[count];
                    reader.GetValues(objs);
                    result = objs;
                }
                else
                {
                    result = null;
                }
            }
            return result;
        }

        public T BindUnique<T>(Action<T, IDataRecord> bindAction) where T : new()
        {
            T result;
            using (DbDataReader reader = this.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    T entity = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
                    bindAction(entity, reader);
                    result = entity;
                }
                else
                {
                    result = default(T);
                }
            }
            return result;
        }

        public int Execute()
        {
            this.TraceSql();
            return this.Command.ExecuteNonQuery();
        }

        public int ExecuteNonQuery()
        {
            this.TraceSql();
            return this.Command.ExecuteNonQuery();
        }

        public SqlStatement Prepare()
        {
            this.Command.Prepare();
            return this;
        }

        public SqlStatement AddParameter(DbType type, string name, object value, ParameterDirection direction)
        {
            string pname = name;
            if (!pname.StartsWith(this.Database.Dialect.ParameterPrefix))
            {
                pname = this.Database.Dialect.ParameterPrefix + pname;
            }
            if (!this.Command.Parameters.Contains(pname))
            {
                DbParameter parameter = this.Command.CreateParameter();
                parameter.DbType = type;
                parameter.ParameterName = pname;
                parameter.Value = value;
                parameter.Direction = direction;
                this.Command.Parameters.Add(parameter);
            }
            else
            {
                this.Command.Parameters[pname].Value = value;
            }
            return this;
        }

        public SqlStatement AddParameter(DbType type, string name, object value, ParameterDirection direction, int ValueSize)
        {
            string pname = name;
            if (!pname.StartsWith(this.Database.Dialect.ParameterPrefix))
            {
                pname = this.Database.Dialect.ParameterPrefix + pname;
            }
            if (!this.Command.Parameters.Contains(pname))
            {
                DbParameter parameter = this.Command.CreateParameter();
                parameter.DbType = type;
                parameter.ParameterName = pname;
                parameter.Value = value;
                parameter.Direction = direction;
                parameter.Size = ValueSize;
                this.Command.Parameters.Add(parameter);
            }
            else
            {
                this.Command.Parameters[pname].DbType = type;
                this.Command.Parameters[pname].Size = ValueSize;
                this.Command.Parameters[pname].Value = value;
            }
            return this;
        }

        public SqlStatement AddParameter(DbType type, string name, object value)
        {
            return this.AddParameter(type, name, value, ParameterDirection.Input);
        }

        public SqlStatement AddParameter(string name, object value)
        {
            string pname = name;
            if (!pname.StartsWith(this.Database.Dialect.ParameterPrefix))
            {
                pname = this.Database.Dialect.ParameterPrefix + pname;
            }
            if (!this.Command.Parameters.Contains(pname))
            {
                DbParameter parameter = this.Command.CreateParameter();
                parameter.ParameterName = pname;
                parameter.Direction = ParameterDirection.Input;
                this.Command.Parameters.Add(parameter);
            }
            this.Command.Parameters[pname].Value = this.Database.Dialect.ToSqlValue(value);
            return this;
        }

        public SqlStatement Set(params Expression<Func<string, object>>[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                Expression<Func<string, object>> expression = parameters[i];
                if (expression.Parameters.Count == 1)
                {
                    string pname = expression.Parameters[0].Name;
                    object value = LambdaExpressionHelper.GetExpressionValue(expression.Body);
                    if (!string.IsNullOrEmpty(pname))
                    {
                        this.AddParameter(pname, value ?? DBNull.Value);
                    }
                }
            }
            return this;
        }

        public SqlStatement Set(params Expression<Func<string, DbType, object>>[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                Expression<Func<string, DbType, object>> expression = parameters[i];
                if (expression.Parameters.Count == 2)
                {
                    string pname = expression.Parameters[0].Name;
                    DbType dbType = LambdaExpressionHelper.GetExpressionValue<DbType>(expression.Parameters[1]);
                    object value = LambdaExpressionHelper.GetExpressionValue(expression.Body);
                    if (!string.IsNullOrEmpty(pname))
                    {
                        this.AddParameter(dbType, pname, value ?? DBNull.Value);
                    }
                }
            }
            return this;
        }

        public SqlStatement Add(string name, object value)
        {
            return this.AddParameter(name, value);
        }

        public List<T> List<T>() where T : new()
        {
            List<T> result2;
            using (DbDataReader reader = this.ExecuteReader())
            {
                List<T> result = new List<T>();
                List<DataReaderField> fields = reader.GetFieldsInfo();
                while (reader.Read())
                {
                    T entity = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
                    BinderManager<T>.Binder.ReadEntity(reader, fields, entity);
                    result.Add(entity);
                }
                result2 = result;
            }
            return result2;
        }

        public T First<T>() where T : new()
        {
            T result;
            using (DbDataReader reader = this.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    T entity = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
                    List<DataReaderField> fields = reader.GetFieldsInfo();
                    reader.Read();
                    BinderManager<T>.Binder.ReadEntity(reader, fields, entity);
                    result = entity;
                }
                else
                {
                    result = default(T);
                }
            }
            return result;
        }

        ~SqlStatement()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (!this._disposing)
            {
                if (this.Database.ActiveStatement.Contains(this))
                {
                    this.Database.ActiveStatement.Remove(this);
                }
                this._disposing = true;
                if (this.Command != null)
                {
                    this.Command.Dispose();
                }
            }
        }
    }
}