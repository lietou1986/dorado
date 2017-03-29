using Dorado.DataExpress.Schema;
using Dorado.DataExpress.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dorado.DataExpress.Ldo
{
    public class LdoEntityInfo
    {
        private Dictionary<string, DataProperty> _fields;
        private DataProperty _primaryKey;
        private Dictionary<string, DataProperty> _properties;
        private TableAttribute _table;

        public TableAttribute Table
        {
            get
            {
                return this._table;
            }
        }

        public DataProperty PrimaryKey
        {
            get
            {
                return this._primaryKey;
            }
        }

        public Dictionary<string, DataProperty> Properties
        {
            get
            {
                return this._properties;
            }
        }

        public Dictionary<string, DataProperty> Fields
        {
            get
            {
                return this._fields;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                if (this._properties.ContainsKey(propertyName))
                {
                    return this._properties[propertyName].Field.FieldName;
                }
                return string.Empty;
            }
        }

        public TableSchema Schema
        {
            get;
            protected set;
        }

        public LdoEntityInfo()
        {
        }

        public LdoEntityInfo(Type type)
            : this()
        {
            this.FromType(type);
        }

        public void FromType(Type type)
        {
            this._table = type.GetTable();
            this.Schema = new TableSchema
            {
                Name = this._table.TableName,
                Owner = this._table.Owner
            };
            this._fields = type.GetDataFields();
            this._properties = new Dictionary<string, DataProperty>(this._fields.Count + 1, StringComparer.OrdinalIgnoreCase);
            foreach (DataProperty item in this._fields.Values)
            {
                item.Schema.Table = this.Schema;
                this._properties.Add(item.Property.Name, item);
            }
            IEnumerable<KeyValuePair<string, DataProperty>> keyField =
                from field in this._fields
                where field.Value.Field.IsPrimaryKey
                select field;
            if (keyField.Count<KeyValuePair<string, DataProperty>>() > 0)
            {
                this._primaryKey = keyField.First<KeyValuePair<string, DataProperty>>().Value;
            }
        }
    }
}