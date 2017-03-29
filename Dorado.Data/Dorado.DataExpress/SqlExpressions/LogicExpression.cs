namespace Dorado.DataExpress.SqlExpressions
{
    public class LogicExpression : BaseCompareExpression
    {
        private const string Template = " {0} {1} {2}";

        public BaseExpression Left
        {
            get;
            set;
        }

        public BaseExpression Right
        {
            get;
            set;
        }

        public override string Sql
        {
            get
            {
                this.Left.InheritedFrom(this);
                this.Right.InheritedFrom(this);
                return string.Format(" {0} {1} {2}", this.Left.Sql, base.Dialect.GetKeyword(base.SqlOperator), this.Right.Sql);
            }
        }

        public LogicExpression()
            : base("and")
        {
        }

        public LogicExpression(string sqlOperator)
            : base(sqlOperator)
        {
        }

        public override void ProcessParameter(SqlStatement statement)
        {
            this.Left.ProcessParameter(statement);
            this.Right.ProcessParameter(statement);
        }
    }
}