using System;
using System.Data;

namespace Dorado.DataExpress.Schema
{
    public class ColumnSchema : BaseDbSchema
    {
        private bool _allowNull = true;
        private DbType _dataType = DbType.String;
        private bool _isPrimaryKey;
        private string _nativeType = "varchar";
        private int _size = 255;
        private Type _systemType = typeof(string);

        public DbType DataType
        {
            get
            {
                return this._dataType;
            }
            set
            {
                this._dataType = value;
            }
        }

        public bool IsPrimaryKey
        {
            get
            {
                return this._isPrimaryKey;
            }
            set
            {
                this._isPrimaryKey = value;
            }
        }

        public Type SystemType
        {
            get
            {
                return this._systemType;
            }
            set
            {
                this._systemType = value;
            }
        }

        public string NativeType
        {
            get
            {
                return this._nativeType;
            }
            set
            {
                this._nativeType = value;
            }
        }

        public int Size
        {
            get
            {
                return this._size;
            }
            set
            {
                this._size = value;
            }
        }

        public int Order
        {
            get;
            set;
        }

        public TableSchema Table
        {
            get;
            set;
        }

        public bool AllowNull
        {
            get
            {
                return this._allowNull;
            }
            set
            {
                this._allowNull = value;
            }
        }

        public ColumnSchema(string columnName)
        {
            base.Name = columnName;
        }

        public ColumnSchema(string tableName, string columnName)
            : this(columnName)
        {
            this.Table = new TableSchema(tableName);
        }

        public ColumnSchema(TableSchema table, string columnName)
        {
            this.Table = table;
            base.Name = columnName;
        }

        public ColumnSchema(TableSchema table, DbType dataType, Type systemType, string nativeType, int size, int order, bool allowNull, string name, string fullName, string desc, string alias)
        {
            this.Table = table;
            this.DataType = dataType;
            this.SystemType = systemType;
            this.NativeType = nativeType;
            this.Size = size;
            this.Order = order;
            this.AllowNull = allowNull;
            base.Name = name;
            base.FullName = fullName;
            base.Description = desc;
            base.Alias = alias;
        }

        public ColumnSchema(TableSchema table, DbType dataType, Type systemType, string nativeType, int size, int order, bool allowNull, string name, string fullName, string desc)
        {
            this.Table = table;
            this.DataType = dataType;
            this.SystemType = systemType;
            this.NativeType = nativeType;
            this.Size = size;
            this.Order = order;
            this.AllowNull = allowNull;
            base.Name = name;
            base.FullName = fullName;
            base.Description = desc;
        }

        public ColumnSchema(TableSchema table, DbType dataType, Type systemType, string nativeType, int size, bool allowNull, string name, string fullName, string desc)
        {
            this.Table = table;
            this.DataType = dataType;
            this.SystemType = systemType;
            this.NativeType = nativeType;
            this.Size = size;
            base.Name = name;
            base.FullName = fullName;
            base.Description = desc;
        }

        public ColumnSchema(DbType dataType, Type systemType, string nativeType, int size, bool allowNull, string name, string fullName, string desc)
        {
            this.DataType = dataType;
            this.SystemType = systemType;
            this.NativeType = nativeType;
            this.Size = size;
            base.Name = name;
            base.FullName = fullName;
            base.Description = desc;
        }

        public ColumnSchema()
        {
        }
    }
}