namespace Dorado.DataExpress.SqlExpressions
{
    public class SubQueryExpression : BaseExpression
    {
        private QueryStatement _innerQuery;

        public QueryStatement InnerQuery
        {
            get
            {
                return this._innerQuery;
            }
            set
            {
                this._innerQuery = value;
            }
        }

        public override string Sql
        {
            get
            {
                this.InnerQuery.Parent = base.Parent;
                return "(" + this.InnerQuery.GenerateSql() + ")";
            }
        }

        public override void ProcessParameter(SqlStatement st)
        {
            this.InnerQuery.Statement = st;
            this.InnerQuery.ProcessParameters();
        }
    }
}