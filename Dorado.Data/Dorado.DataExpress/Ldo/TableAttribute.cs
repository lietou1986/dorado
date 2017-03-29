using System;

namespace Dorado.DataExpress.Ldo
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class TableAttribute : Attribute
    {
        public string TableName
        {
            get;
            set;
        }

        public string Owner
        {
            get;
            set;
        }

        public TableAttribute()
        {
            this.TableName = string.Empty;
        }

        public TableAttribute(string tableName)
        {
            this.TableName = tableName;
        }
    }
}