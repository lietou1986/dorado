using Dorado.DataExpress.Schema;

namespace Dorado.DataExpress.SqlExpressions
{
    public class JoinExpression : BaseExpression
    {
        public enum JoinType
        {
            Left,
            Right,
            Inner,
            Empty
        }

        private const string sTemplate = " {0} JOIN {1} ON {2}={3} ";
        private ColumnSchema _column;
        private JoinExpression.JoinType _joinOperator;
        private ColumnSchema _leftColumn;
        private TableSchema _table;

        public TableSchema Table
        {
            get
            {
                return this._table;
            }
            set
            {
                this._table = value;
            }
        }

        public ColumnSchema LeftColumn
        {
            get
            {
                return this._leftColumn;
            }
            set
            {
                this._leftColumn = value;
            }
        }

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

        public JoinExpression.JoinType JoinOperator
        {
            get
            {
                return this._joinOperator;
            }
            set
            {
                this._joinOperator = value;
            }
        }

        public override string Sql
        {
            get
            {
                if (this.JoinOperator == JoinExpression.JoinType.Empty)
                {
                    return " ," + base.Dialect.GetSystemName(this.Table);
                }
                return string.Format(" {0} JOIN {1} ON {2}={3} ", new object[]
				{
					this.JoinOperator.ToString().ToUpper(),
					base.Dialect.GetSystemName(this.Table),
					base.Dialect.GetSystemName(this.LeftColumn),
					base.Dialect.GetSystemName(this.Column)
				});
            }
        }

        public JoinExpression(TableSchema table, JoinExpression.JoinType type, ColumnSchema leftColumn, ColumnSchema column)
        {
            this.Table = table;
            this.JoinOperator = type;
            this.LeftColumn = leftColumn;
            this.Column = column;
        }
    }
}