using Dorado.DataExpress.Schema;

namespace Dorado.DataExpress.SqlExpressions
{
    public class IsNotNullExpression : BaseCompareExpression
    {
        private string _sqlOp = " {0} IS NOT NULL";

        public override string Sql
        {
            get
            {
                base.Column.InheritedFrom(this);
                return string.Format(base.SqlOperator, base.Column.Sql);
            }
        }

        public IsNotNullExpression()
            : base(" {0} is not NULL")
        {
        }

        public IsNotNullExpression(ColumnSchema column)
            : this()
        {
            base.Column = new SimpleColumnExpression(column);
        }

        public IsNotNullExpression(BaseExpression column)
            : this()
        {
            base.Column = column;
        }
    }
}