using System;
using System.Collections.Generic;

namespace Dorado.DataExpress.Dialect
{
    public class MsSql : BaseDialect
    {
        private const string sParameterPrefix = "@";
        private static string sFunctionFormat = "SELECT RESULT FROM {0}";
        private static string sProcudureFormat = "EXECUTE {0}";

        public override string ParameterPrefix
        {
            get
            {
                return "@";
            }
        }

        public MsSql()
        {
            base.KeywordFunctions = new Dictionary<string, Func<string, string>>(StringComparer.OrdinalIgnoreCase);
            this.InitKeywordFunctions();
        }

        private void InitKeywordFunctions()
        {
            base.AddKeywordFunc("select", (string k) => "SELECT");
            base.AddKeywordFunc("from", (string k) => "FROM");
            base.AddKeywordFunc("where", (string k) => "WHERE");
            base.AddKeywordFunc("union", (string k) => "UNION");
            base.AddKeywordFunc("all", (string k) => "ALL");
            base.AddKeywordFunc("count", (string k) => "COUNT");
            base.AddKeywordFunc("group by", (string k) => "GROUP BY");
            base.AddKeywordFunc("not", (string k) => "NOT");
            base.AddKeywordFunc("in", (string k) => "IN");
            base.AddKeywordFunc("order by", (string k) => "ORDER BY");
            base.AddKeywordFunc("desc", (string k) => "DESC");
            base.AddKeywordFunc("left join", (string k) => "LEFT JOIN");
        }
    }
}