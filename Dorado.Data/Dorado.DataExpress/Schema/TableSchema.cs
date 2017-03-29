using System;
using System.Collections.Generic;

namespace Dorado.DataExpress.Schema
{
    public class TableSchema : BaseDbSchema
    {
        public Dictionary<string, ColumnSchema> Columns = new Dictionary<string, ColumnSchema>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, KeySchema> ForeignKeys = new Dictionary<string, KeySchema>(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, KeySchema> PrimaryKeys = new Dictionary<string, KeySchema>(StringComparer.OrdinalIgnoreCase);

        public TableSchema(string name)
        {
            base.Name = name;
        }

        public TableSchema(string name, string fullName, string desc, string alias)
        {
            base.Name = name;
            base.FullName = fullName;
            base.Description = desc;
            base.Alias = alias;
        }

        public TableSchema(string name, string fullName, string desc)
        {
            base.Name = name;
            base.FullName = fullName;
            base.Description = desc;
        }

        public TableSchema(string name, string fullName)
        {
            base.Name = name;
            base.FullName = fullName;
        }

        public TableSchema()
        {
        }

        public ColumnSchema CreateColumn(string colName)
        {
            ColumnSchema col = new ColumnSchema(this, colName);
            this.AddColumn(col);
            return col;
        }

        public void AddColumn(ColumnSchema col)
        {
            this.Columns.Add(col.Name, col);
        }
    }
}