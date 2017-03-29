using Dorado.DataExpress.Schema;

namespace Dorado.DataExpress.SqlExpressions
{
    public class GtExpression : BaseCompareExpression
    {
        public GtExpression(ColumnSchema col, object value)
            : this(new SimpleColumnExpression(col), value)
        {
        }

        public GtExpression(BaseExpression col, object value)
            : base(" {0}>{1} ")
        {
            base.Value = ((value is BaseExpression) ? (value as BaseExpression) : new SimpleValueExpression(value));
            base.Column = col;
        }

        public GtExpression()
            : base(" {0}>{1} ")
        {
        }
    }
}