using Dorado.DataExpress.Schema;

namespace Dorado.DataExpress.SqlExpressions
{
    public class LikeExpression : BaseCompareExpression
    {
        public LikeExpression()
            : base(" {0} like {1} ")
        {
        }

        public LikeExpression(ColumnSchema column, string value)
            : this()
        {
            base.Column = new SimpleColumnExpression(column);
            base.Value = new SimpleValueExpression(value);
        }

        public LikeExpression(BaseExpression column, string value)
            : this()
        {
            base.Column = column;
            base.Value = new SimpleValueExpression(value);
        }
    }
}