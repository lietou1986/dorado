namespace Dorado.DataExpress.SqlExpressions
{
    public class AndExpression : LogicExpression
    {
        public AndExpression()
            : base("and")
        {
        }

        public AndExpression(BaseExpression left, BaseExpression right)
            : this()
        {
            base.Left = left;
            base.Right = right;
        }
    }
}