using Dorado.DataExpress.Schema;

namespace Dorado.DataExpress.SqlExpressions
{
    public class LtExpression : BaseCompareExpression
    {
        private string _sqlOp = " {0}<{1} ";

        public LtExpression()
            : base(" {0}<{1} ")
        {
        }

        public LtExpression(ColumnSchema col, object value)
            : this()
        {
            base.Value = new SimpleValueExpression(value);
            base.Column = new SimpleColumnExpression(col);
        }

        public LtExpression(BaseExpression col, object value)
            : this()
        {
            base.Value = new SimpleValueExpression(value);
            base.Column = col;
        }

        public LtExpression(ColumnSchema col, BaseExpression value)
            : this()
        {
            base.Value = value;
            base.Column = new SimpleColumnExpression(col);
        }

        public LtExpression(BaseExpression col, BaseExpression value)
            : this()
        {
            base.Value = value;
            base.Column = col;
        }
    }
}