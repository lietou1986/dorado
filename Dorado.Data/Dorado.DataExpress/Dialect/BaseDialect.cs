using Dorado.DataExpress.Schema;
using Dorado.DataExpress.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;

namespace Dorado.DataExpress.Dialect
{
    public abstract class BaseDialect
    {
        private const string _newLine = "\r\n";
        private const string _dateFunciton = "GETDATE()";
        private const string _LastIdentity = "SELECT SCOPE_IDENTITY()";
        private const string _systemNamePartten = "^[a-zA-Z-_0-9]{0,}$";
        private const string _NoCount = "SET NOCOUNT ON";
        private const string _systemNameTemplate = "[{0}]";
        private const string _valueQuote = "'{0}'";
        private readonly Dictionary<DbType, string> _types = new Dictionary<DbType, string>();

        public abstract string ParameterPrefix
        {
            get;
        }

        public virtual string NewLine
        {
            get
            {
                return "\r\n";
            }
        }

        public virtual string SystemNameTemplate
        {
            get
            {
                return "[{0}]";
            }
        }

        public virtual string DateFunction
        {
            get
            {
                return "GETDATE()";
            }
        }

        public virtual string ValueQuote
        {
            get
            {
                return "'{0}'";
            }
        }

        public virtual string NoCount
        {
            get
            {
                return "SET NOCOUNT ON";
            }
        }

        public virtual string LastIdentity
        {
            get
            {
                return "SELECT SCOPE_IDENTITY()";
            }
        }

        public virtual Regex SystemNamePartten
        {
            get
            {
                return new Regex("^[a-zA-Z-_0-9]{0,}$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            }
        }

        protected Dictionary<string, Func<string, string>> KeywordFunctions
        {
            get;
            set;
        }

        public virtual bool ValidateSystemName(string name)
        {
            return this.SystemNamePartten.IsMatch(name);
        }

        public void AddKeywordFunc(string keyword, Func<string, string> func)
        {
            this.KeywordFunctions.Add(keyword, func);
        }

        public virtual string GetKeyword(string keyWord)
        {
            if (this.KeywordFunctions == null)
            {
                return keyWord;
            }
            Func<string, string> convertFunc;
            if (this.KeywordFunctions.TryGetValue(keyWord, out convertFunc))
            {
                return convertFunc(keyWord);
            }
            return this.ConvertKeyword(keyWord);
        }

        protected virtual string ConvertKeyword(string keyword)
        {
            return keyword;
        }

        public void RegisterType(DbType type, string format)
        {
            this._types.Add(type, format);
        }

        public virtual string GetSystemName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            StringBuilder rb = new StringBuilder(96);
            string[] array = name.Split(new char[]
			{
				'.'
			});
            for (int i = 0; i < array.Length; i++)
            {
                string s = array[i];
                rb.Append(string.Format(this.SystemNameTemplate, s)).Append('.');
            }
            return rb.ToString(0, rb.Length - 1);
        }

        public virtual string GetSystemName(DatabaseSchema database)
        {
            return this.GetSystemName(database.Name);
        }

        public virtual string GetSystemName(TableSchema table)
        {
            return this.GetSystemName(table.Name);
        }

        public virtual string GetSystemName(ViewSchema view)
        {
            return this.GetSystemName(view.Name);
        }

        public virtual string GetSystemName(ColumnSchema column)
        {
            if (column.Table != null)
            {
                return this.GetSystemName(column.Table.Name) + "." + this.GetSystemName(column.Name);
            }
            return this.GetSystemName(column.Name);
        }

        public virtual string GetSystemName(BaseDbSchema dbObject)
        {
            return this.GetSystemName(dbObject.Name);
        }

        public virtual string EncodeValue(string value)
        {
            if (value.Contains("'"))
            {
                return value.Replace("'", "''");
            }
            return value;
        }

        public virtual object ToSqlValue(object obj)
        {
            if (obj == null)
            {
                return DBNull.Value;
            }
            if (obj is bool)
            {
                return ((bool)obj) ? 1 : 0;
            }
            if (obj is Enum)
            {
                return (int)obj;
            }
            return obj;
        }

        public virtual string SqlValue(object obj)
        {
            if (obj == null || obj is DBNull)
            {
                return "null";
            }
            Type type = obj.GetType();
            if (type.IsEnum)
            {
                type = Enum.GetUnderlyingType(type);
                string name;
                switch (name = type.Name)
                {
                    case "System.UInt":
                    case "System.UInt32":
                        {
                            return ((uint)obj).ToString();
                        }
                    case "System.UInt16":
                        {
                            return ((ushort)obj).ToString();
                        }
                    case "System.UInt64":
                        {
                            return ((ulong)obj).ToString();
                        }
                    case "System.Int64":
                        {
                            return ((long)obj).ToString();
                        }
                    case "System.Int16":
                        {
                            return ((short)obj).ToString();
                        }
                }
                return ((int)obj).ToString();
            }
            if (type.IsGenericType)
            {
                type = Nullable.GetUnderlyingType(type);
            }
            string typeName = type.ToString();
            string key;
            switch (key = typeName)
            {
                case "System.Double":
                case "System.Int":
                case "System.Int32":
                case "System.Int16":
                case "System.Int64":
                case "System.UInt16":
                case "System.UInt":
                case "System.UInt32":
                case "System.Decimal":
                case "System.UInt64":
                    {
                        return obj.ToString();
                    }
                case "System.Byte[]":
                    {
                        return ToolLite.BytesToHexWithLeader((byte[])obj);
                    }
                case "System.Char[]":
                    {
                        return string.Format(this.ValueQuote, this.EncodeValue(new string((char[])obj)));
                    }
                case "System.DateTime":
                    {
                        return string.Format(this.ValueQuote, ((DateTime)obj).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    }
                case "System.Boolean":
                    {
                        if (!(bool)obj)
                        {
                            return "false";
                        }
                        return "1";
                    }
            }
            return string.Format(this.ValueQuote, this.EncodeValue(obj.ToString()));
        }

        public virtual string ParametersBuilder(DbCommand cmd)
        {
            string sql = cmd.CommandText;
            for (int i = 0; i < cmd.Parameters.Count; i++)
            {
                DbParameter pm = cmd.Parameters[i];
                sql = sql.Replace(pm.ParameterName, this.SqlValue(pm.Value));
            }
            return sql;
        }
    }
}