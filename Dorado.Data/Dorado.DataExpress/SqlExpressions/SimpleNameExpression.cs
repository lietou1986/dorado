namespace Dorado.DataExpress.SqlExpressions
{
    public class SimpleNameExpression : BaseExpression
    {
        public string Name
        {
            get;
            set;
        }

        public override string Sql
        {
            get
            {
                return base.Dialect.GetSystemName(this.Name);
            }
        }

        public SimpleNameExpression(string name)
        {
            this.Name = name;
        }
    }
}