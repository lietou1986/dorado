using Dorado.DataExpress.Schema;

namespace Dorado.DataExpress.SqlExpressions
{
    public class EqExpression : BaseCompareExpression
    {
        public EqExpression(ColumnSchema column, object value)
            : this(new SimpleColumnExpression(column), value)
        {
        }

        public EqExpression(BaseExpression column, object value)
        {
            base.Column = column;
            base.Value = ((value is BaseExpression) ? (value as BaseExpression) : new SimpleValueExpression(value));
        }

        public EqExpression(BaseExpression left, BaseExpression right)
        {
            base.Column = left;
            base.Value = right;
        }
    }
}