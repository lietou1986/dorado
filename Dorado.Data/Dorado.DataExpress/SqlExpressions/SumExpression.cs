using Dorado.DataExpress.Schema;

namespace Dorado.DataExpress.SqlExpressions
{
    public class SumExpression : CommandExpression
    {
        public SumExpression()
            : base("sum")
        {
        }

        public SumExpression(BaseExpression exp)
            : this()
        {
            this.Parameters.Add(exp);
        }

        public SumExpression(ColumnSchema column)
            : this(SqlExpression.Column(column))
        {
        }

        public SumExpression(string columnName)
            : this(SqlExpression.Column(columnName))
        {
        }
    }
}