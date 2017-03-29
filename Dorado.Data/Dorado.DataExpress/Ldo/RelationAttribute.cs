using System;

namespace Dorado.DataExpress.Ldo
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RelationAttribute : Attribute
    {
        public bool Lazy
        {
            get;
            set;
        }

        public string RelationField
        {
            get;
            set;
        }

        public RelationAttribute(string relationField)
        {
            this.Lazy = true;
            this.RelationField = relationField;
        }
    }
}