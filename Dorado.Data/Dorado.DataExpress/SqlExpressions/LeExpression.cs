using Dorado.DataExpress.Schema;

namespace Dorado.DataExpress.SqlExpressions
{
    public class LeExpression : BaseCompareExpression
    {
        private string _sqlOp = " {0}<={1} ";

        public LeExpression()
            : base(" {0}<={1} ")
        {
        }

        public LeExpression(ColumnSchema col, object value)
            : this()
        {
            base.Value = ((value is BaseExpression) ? (value as BaseExpression) : new SimpleValueExpression(value));
            base.Column = new SimpleColumnExpression(col);
        }

        public LeExpression(BaseExpression col, object value)
            : this()
        {
            base.Value = ((value is BaseExpression) ? (value as BaseExpression) : new SimpleValueExpression(value));
            base.Column = col;
        }

        public LeExpression(BaseExpression left, BaseExpression right)
            : this()
        {
            base.Column = left;
            base.Value = right;
        }
    }
}