using System;
using System.Collections.Generic;
using System.Data;

namespace Dorado.DataExpress.Schema
{
    public class CommandSchema : BaseDbSchema
    {
        public Dictionary<string, ColumnSchema> Parameters = new Dictionary<string, ColumnSchema>(StringComparer.OrdinalIgnoreCase);
        private DbType _resultDataType = DbType.String;
        private Type _resultSystemType = typeof(string);

        public DbType ResultDataType
        {
            get
            {
                return this._resultDataType;
            }
            set
            {
                this._resultDataType = value;
            }
        }

        public Type ResultSystemType
        {
            get
            {
                return this._resultSystemType;
            }
            set
            {
                this._resultSystemType = value;
            }
        }

        public CommandSchema()
        {
        }

        public CommandSchema(string name, string fullName, string desc, DbType resultType, Type resultSystemType)
        {
            base.Name = name;
            base.FullName = fullName;
            base.Description = desc;
            this.ResultDataType = resultType;
            this.ResultSystemType = resultSystemType;
        }

        public CommandSchema(string name, string fullName, string desc)
        {
            base.Name = name;
            base.FullName = fullName;
            base.Description = desc;
        }
    }
}