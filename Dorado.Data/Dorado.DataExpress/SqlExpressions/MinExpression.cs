using Dorado.DataExpress.Schema;

namespace Dorado.DataExpress.SqlExpressions
{
    public class MinExpression : CommandExpression
    {
        public override string Sql
        {
            get
            {
                return base.Dialect.GetKeyword(base.Sql);
            }
        }

        public MinExpression()
            : base("MIN")
        {
        }

        public MinExpression(BaseExpression expression)
            : this()
        {
            this.Parameters.Add(expression);
        }

        public MinExpression(ColumnSchema column)
            : this(SqlExpression.Column(column))
        {
        }

        public MinExpression(string columnName)
            : this(SqlExpression.Column(columnName))
        {
        }
    }
}