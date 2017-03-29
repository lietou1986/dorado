namespace Dorado.DataExpress.SqlExpressions
{
    public class QuoteExpressoin : BaseExpression
    {
        private string _SQLOperator = "({0})";
        private BaseExpression _expression;

        public override string Sql
        {
            get
            {
                this.Expression.InheritedFrom(this);
                return string.Format(this.SQLOperator, this.Expression.Sql);
            }
        }

        public string SQLOperator
        {
            get
            {
                return this._SQLOperator;
            }
            set
            {
                this._SQLOperator = value;
            }
        }

        public BaseExpression Expression
        {
            get
            {
                return this._expression;
            }
            set
            {
                this._expression = value;
            }
        }

        public QuoteExpressoin(BaseExpression exp)
        {
            this.Expression = exp;
        }

        public override void ProcessParameter(SqlStatement st)
        {
            this.Expression.ProcessParameter(st);
        }
    }
}