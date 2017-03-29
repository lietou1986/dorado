using Dorado.DataExpress.Schema;

namespace Dorado.DataExpress.SqlExpressions
{
    public class AvgExpression : CommandExpression
    {
        public AvgExpression()
            : base("avg")
        {
        }

        public AvgExpression(BaseExpression expression)
            : this()
        {
            this.Parameters.Add(expression);
        }

        public AvgExpression(ColumnSchema column)
            : this(SqlExpression.Column(column))
        {
        }

        public AvgExpression(string columnName)
            : this(SqlExpression.Column(columnName))
        {
        }
    }
}