using Dorado.Data.Exceptions;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Dorado.Data
{
    internal class ParameterSet : IParameterSet
    {
        private SqlParameterCollection pm;

        internal ParameterSet(SqlParameterCollection sqlParameterCollection)
        {
            this.pm = sqlParameterCollection;
        }

        public object GetValue(string key)
        {
            return this.pm[key].Value;
        }

        public void AddTypedDbNull(string key, ParameterDirectionWrap direction, DbType dbType)
        {
            this.AddWithValue(key, DBNull.Value, direction, null, new DbType?(dbType));
        }

        public void AddWithValue(string key, object value)
        {
            this.AddWithValue(key, value, false);
        }

        public void AddWithValue(string key, object value, bool check)
        {
            if (value == null)
            {
                value = DBNull.Value;
            }
            if (check && !SqlHelper.CheckSqlParameter(value.ToString()))
            {
                throw new SqlParameterException(key, value.ToString());
            }
            int index = this.pm.IndexOf(key);
            if (index == -1)
            {
                this.pm.AddWithValue(key, value);
                return;
            }
            this.pm[index].Value = value;
        }

        public void AddWithValue(string key, object value, ParameterDirectionWrap direction)
        {
            this.AddWithValue(key, value, direction, null, null);
        }

        public void AddWithValue(string key, object value, ParameterDirectionWrap direction, int? size)
        {
            this.AddWithValue(key, value, direction, size, null);
        }

        public void AddWithValue(string key, object value, ParameterDirectionWrap direction, int? size, DbType? dbType)
        {
            ParameterDirection pDir;
            if (value == null)
            {
                value = DBNull.Value;
            }
            SqlParameter sp = new SqlParameter(key, value);
            if (dbType.HasValue)
            {
                sp.DbType = dbType.Value;
            }
            switch (direction)
            {
                case ParameterDirectionWrap.Input:
                    pDir = ParameterDirection.Input;
                    break;

                case ParameterDirectionWrap.Output:
                    pDir = ParameterDirection.Output;
                    break;

                case ParameterDirectionWrap.InputOutput:
                    pDir = ParameterDirection.InputOutput;
                    break;

                case ParameterDirectionWrap.ReturnValue:
                    pDir = ParameterDirection.ReturnValue;
                    break;

                default:
                    pDir = ParameterDirection.Input;
                    break;
            }
            int index = this.pm.IndexOf(key);
            if (index == -1)
            {
                sp.Direction = pDir;
                if (size.HasValue)
                {
                    sp.Size = size.Value;
                }
                this.pm.Add(sp);
            }
            else
            {
                this.pm[index].Direction = pDir;
                if (size.HasValue)
                {
                    this.pm[index].Size = size.Value;
                }
                this.pm[index].Value = value;
            }
        }
    }
}