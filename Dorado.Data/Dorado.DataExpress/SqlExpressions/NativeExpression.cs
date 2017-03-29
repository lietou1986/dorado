namespace Dorado.DataExpress.SqlExpressions
{
    public class NativeExpression : BaseExpression
    {
        private readonly string _nativeSql;

        public override string Sql
        {
            get
            {
                return this._nativeSql;
            }
        }

        public NativeExpression()
        {
        }

        public NativeExpression(string sql)
            : this()
        {
            this._nativeSql = sql;
        }
    }
}