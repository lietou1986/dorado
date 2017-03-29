using System;

namespace Dorado.DataExpress.Ldo
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class FieldAttribute : Attribute
    {
        public string FieldName
        {
            get;
            set;
        }

        public int FieldIndex
        {
            get;
            set;
        }

        public bool Lazy
        {
            get;
            set;
        }

        public Relationship Relation
        {
            get;
            set;
        }

        public string KeyField
        {
            get;
            set;
        }

        public bool IsPrimaryKey
        {
            get;
            set;
        }

        public bool AllowNull
        {
            get;
            set;
        }

        public FieldAttribute()
        {
            this.FieldIndex = -1;
            this.AllowNull = true;
            this.FieldName = string.Empty;
            this.Lazy = true;
            this.Relation = Relationship.None;
            this.KeyField = string.Empty;
            this.IsPrimaryKey = false;
        }

        public FieldAttribute(string fieldName)
        {
            this.FieldIndex = -1;
            this.AllowNull = true;
            this.Lazy = true;
            this.Relation = Relationship.None;
            this.KeyField = string.Empty;
            this.IsPrimaryKey = false;
            this.FieldName = fieldName;
        }
    }
}