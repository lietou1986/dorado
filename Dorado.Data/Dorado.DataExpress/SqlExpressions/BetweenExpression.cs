using Dorado.DataExpress.Schema;

namespace Dorado.DataExpress.SqlExpressions
{
    public class BetweenExpression : BaseCompareExpression
    {
        private BaseExpression _leftValue;

        public BaseExpression LeftValue
        {
            get
            {
                return this._leftValue;
            }
            set
            {
                this._leftValue = value;
            }
        }

        public override string Sql
        {
            get
            {
                base.Column.InheritedFrom(this);
                base.Value.InheritedFrom(this);
                this.LeftValue.InheritedFrom(this);
                return string.Format(base.SqlOperator, base.Column.Sql, this.LeftValue.Sql, base.Value.Sql);
            }
        }

        public BetweenExpression(BaseExpression col, object leftValue, object value)
            : base(" {0} BETWEEN {1} AND {2} ")
        {
            base.Column = col;
            this.LeftValue = ((leftValue is BaseExpression) ? (leftValue as BaseExpression) : new SimpleValueExpression(leftValue));
            base.Value = ((value is BaseExpression) ? (value as BaseExpression) : new SimpleValueExpression(value));
        }

        public BetweenExpression(ColumnSchema col, object leftValue, object value)
            : this(SqlExpression.Column(col), leftValue, value)
        {
        }

        public override void ProcessParameter(SqlStatement statement)
        {
            base.Column.ProcessParameter(statement);
            this.LeftValue.ProcessParameter(statement);
            base.Value.ProcessParameter(statement);
        }
    }
}