namespace Dorado.DataExpress.SqlExpressions
{
    public class OrExpression : LogicExpression
    {
        private const string Operator = "or";

        public OrExpression()
            : base("or")
        {
        }

        public OrExpression(BaseCompareExpression left, BaseCompareExpression right)
            : this()
        {
            base.Left = left;
            base.Right = right;
        }

        public OrExpression(BaseExpression left, BaseExpression right)
            : this()
        {
            base.Left = left;
            base.Right = right;
        }
    }
}