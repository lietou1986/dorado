using Dorado.DataExpress.Dialect;
using Dorado.DataExpress.Driver;

namespace Dorado.DataExpress.SqlExpressions
{
    public abstract class BaseExpression
    {
        public BaseDriver Driver
        {
            get;
            set;
        }

        public BaseDialect Dialect
        {
            get;
            set;
        }

        public BaseQuery Parent
        {
            get;
            set;
        }

        public string Alias
        {
            get;
            set;
        }

        public abstract string Sql
        {
            get;
        }

        public PreviousOperator PreRelation
        {
            get;
            set;
        }

        public BaseExpression()
        {
            this.PreRelation = PreviousOperator.And;
            this.Alias = string.Empty;
        }

        public virtual void ProcessParameter(SqlStatement st)
        {
        }

        public override string ToString()
        {
            return this.Sql;
        }

        public void InheritedFrom(BaseExpression parent)
        {
            this.Dialect = parent.Dialect;
            this.Driver = parent.Driver;
            this.Parent = parent.Parent;
        }
    }
}