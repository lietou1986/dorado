using Dorado.DataExpress.Schema;

namespace Dorado.DataExpress.SqlExpressions
{
    public class GeExpression : BaseCompareExpression
    {
        public GeExpression(ColumnSchema col, object value)
            : this(SqlExpression.Column(col), value)
        {
        }

        public GeExpression(BaseExpression col, object value)
            : base(" {0}>={1} ")
        {
            base.Value = ((value is BaseExpression) ? (value as BaseExpression) : new SimpleValueExpression(value));
            base.Column = col;
        }

        public GeExpression(BaseExpression left, BaseExpression right)
            : base(" {0}>={1} ")
        {
            base.Column = left;
            base.Value = right;
        }
    }
}