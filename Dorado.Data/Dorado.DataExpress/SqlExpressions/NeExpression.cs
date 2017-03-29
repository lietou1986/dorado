using Dorado.DataExpress.Schema;

namespace Dorado.DataExpress.SqlExpressions
{
    public class NeExpression : BaseCompareExpression
    {
        private string _sqlOp = " {0}<>{1} ";

        public NeExpression()
            : base(" {0}<>{1} ")
        {
        }

        public NeExpression(ColumnSchema column, object value)
            : this()
        {
            base.Column = new SimpleColumnExpression(column);
            base.Value = new SimpleValueExpression(value);
        }

        public NeExpression(BaseExpression column, object value)
            : this()
        {
            base.Column = column;
            base.Value = new SimpleValueExpression(value);
        }

        public NeExpression(ColumnSchema column, BaseExpression value)
            : this()
        {
            base.Column = new SimpleColumnExpression(column);
            base.Value = value;
        }

        public NeExpression(BaseExpression column, BaseExpression value)
            : this()
        {
            base.Column = column;
            base.Value = value;
        }
    }
}