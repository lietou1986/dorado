using Dorado.DataExpress.Schema;
using System.Reflection;

namespace Dorado.DataExpress.Ldo
{
    public class DataProperty
    {
        private FieldAttribute _field;
        private ColumnSchema _schema;

        public FieldAttribute Field
        {
            get
            {
                return this._field;
            }
            set
            {
                this._schema = new ColumnSchema
                {
                    AllowNull = value.AllowNull,
                    Name = value.FieldName,
                    IsPrimaryKey = value.IsPrimaryKey
                };
                this._field = value;
            }
        }

        public PropertyInfo Property
        {
            get;
            set;
        }

        public ColumnSchema Schema
        {
            get
            {
                return this._schema;
            }
        }
    }
}