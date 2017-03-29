using System;

namespace Dorado.DataExpress.SqlExpressions
{
    public class OriginalKeywordExpression : BaseExpression
    {
        public string Keyword
        {
            get;
            set;
        }

        public override string Sql
        {
            get
            {
                if (string.IsNullOrEmpty(this.Keyword))
                {
                    throw new ArgumentNullException("Keyword");
                }
                if (base.Dialect.SystemNamePartten.Match(this.Keyword).Success)
                {
                    return this.Keyword;
                }
                throw new ArgumentException(string.Format("Keyword[{0}]不符合名称格式", this.Keyword));
            }
        }
    }
}