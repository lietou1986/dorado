using Dorado.DataExpress.Schema;

namespace Dorado.DataExpress.SqlExpressions
{
    public class SimpleColumnExpression : BaseColumnExpression
    {
        private ColumnSchema _column;

        public ColumnSchema Column
        {
            get
            {
                return this._column;
            }
            set
            {
                this._column = value;
            }
        }

        public override string Sql
        {
            get
            {
                return base.Dialect.GetSystemName(this.Column);
            }
        }

        public SimpleColumnExpression()
        {
        }

        public SimpleColumnExpression(ColumnSchema col)
            : this(col, null)
        {
        }

        public SimpleColumnExpression(ColumnSchema col, string alias)
        {
            this.Column = col;
            base.Alias = alias;
        }
    }
}