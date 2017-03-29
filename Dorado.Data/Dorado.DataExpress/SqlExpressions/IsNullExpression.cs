using Dorado.DataExpress.Schema;

namespace Dorado.DataExpress.SqlExpressions
{
    public class IsNullExpression : BaseCompareExpression
    {
        public override string Sql
        {
            get
            {
                base.Column.InheritedFrom(this);
                return string.Format(base.SqlOperator, base.Column);
            }
        }

        public IsNullExpression()
            : base(" {0} is NULL ")
        {
        }

        public IsNullExpression(ColumnSchema column)
            : this()
        {
            base.Column = new SimpleColumnExpression(column);
        }

        public IsNullExpression(BaseExpression column)
            : this()
        {
            base.Column = column;
        }
    }
}