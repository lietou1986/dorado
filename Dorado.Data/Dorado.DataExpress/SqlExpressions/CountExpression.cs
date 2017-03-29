using Dorado.DataExpress.Schema;

namespace Dorado.DataExpress.SqlExpressions
{
    public class CountExpression : CommandExpression
    {
        public CountExpression()
            : base("count")
        {
        }

        public CountExpression(BaseExpression exp)
            : this()
        {
            this.Parameters.Add(exp);
        }

        public CountExpression(ColumnSchema column)
            : this(SqlExpression.Column(column))
        {
        }

        public CountExpression(string columnName)
            : this(SqlExpression.Column(columnName))
        {
        }
    }
}