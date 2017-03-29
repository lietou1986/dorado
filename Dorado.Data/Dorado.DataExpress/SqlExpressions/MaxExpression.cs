using Dorado.DataExpress.Schema;

namespace Dorado.DataExpress.SqlExpressions
{
    public class MaxExpression : CommandExpression
    {
        public MaxExpression()
            : base("MAX")
        {
        }

        public MaxExpression(BaseExpression expression)
            : this()
        {
            this.Parameters.Add(expression);
        }

        public MaxExpression(ColumnSchema column)
            : this(SqlExpression.Column(column))
        {
        }

        public MaxExpression(string column)
            : this(SqlExpression.Column(column))
        {
        }
    }
}