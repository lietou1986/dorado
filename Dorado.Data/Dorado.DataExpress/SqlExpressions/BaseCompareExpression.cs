namespace Dorado.DataExpress.SqlExpressions
{
    public class BaseCompareExpression : BaseExpression
    {
        public const string sAnd = " AND ";
        public const string sOr = " OR ";
        private BaseExpression _Column;
        private string _name = string.Empty;

        public BaseExpression Column
        {
            get
            {
                return this._Column;
            }
            set
            {
                this._Column = value;
            }
        }

        public string SqlOperator
        {
            get;
            set;
        }

        public BaseExpression Value
        {
            get;
            set;
        }

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(this._name))
                {
                    this._name = base.Parent.GetParameterName();
                }
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        public override string Sql
        {
            get
            {
                this.Column.InheritedFrom(this);
                this.Value.InheritedFrom(this);
                return string.Format(base.Dialect.GetKeyword(this.SqlOperator), this.Column.Sql, this.Value.Sql);
            }
        }

        public BaseCompareExpression()
        {
            this.SqlOperator = " {0}={1} ";
        }

        public BaseCompareExpression(string sqlOperator)
            : this()
        {
            this.SqlOperator = sqlOperator;
        }

        public override void ProcessParameter(SqlStatement statement)
        {
            if (this.Column != null)
            {
                this.Column.ProcessParameter(statement);
            }
            if (this.Value != null)
            {
                this.Value.ProcessParameter(statement);
            }
        }
    }
}