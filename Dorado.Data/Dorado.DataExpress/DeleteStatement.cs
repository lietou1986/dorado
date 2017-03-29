using Dorado.DataExpress.Schema;
using Dorado.DataExpress.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dorado.DataExpress
{
    public class DeleteStatement : BaseQuery
    {
        private const string _sDelete = "DELETE FROM ";
        private readonly List<BaseExpression> Conditions = new List<BaseExpression>();

        public DeleteStatement()
        {
        }

        public DeleteStatement(TableSchema table, SqlStatement st)
        {
            base.Statement = st;
            base.Table = table;
        }

        public DeleteStatement AddCondition(BaseCompareExpression exp)
        {
            exp.Parent = this;
            exp.Dialect = base.Statement.Database.Dialect;
            exp.Driver = base.Statement.Database.Driver;
            this.Conditions.Add(exp);
            return this;
        }

        [Obsolete]
        public DeleteStatement AddCondition(ColumnSchema col, object value)
        {
            BaseCompareExpression colExp = SqlExpression.Eq(col, value);
            return this.AddCondition(colExp);
        }

        [Obsolete]
        public DeleteStatement AddCondition(string colName, object value)
        {
            return this.AddCondition(new ColumnSchema
            {
                Name = colName,
                FullName = colName,
                Table = base.Table
            }, value);
        }

        public DeleteStatement Where(ColumnSchema col, object value)
        {
            return this.AddCondition(col, value);
        }

        public DeleteStatement Where(string colName, object value)
        {
            return this.AddCondition(colName, value);
        }

        public DeleteStatement Where(BaseExpression exp)
        {
            this.Conditions.Add(exp);
            return this;
        }

        public DeleteStatement From(string tableName)
        {
            return this.From(new TableSchema(tableName));
        }

        public DeleteStatement From(TableSchema table)
        {
            base.Table = table;
            return this;
        }

        public DeleteStatement Where(BaseCompareExpression exp)
        {
            return this.AddCondition(exp);
        }

        public override string GenerateSql()
        {
            StringBuilder builder = new StringBuilder(64);
            builder.Append("DELETE FROM ");
            builder.Append(base.Statement.Database.Dialect.GetSystemName(base.Table));
            builder.Append(this.GenerateConditions());
            return builder.ToString();
        }

        public override void ProcessParameters()
        {
            foreach (BaseExpression exp in this.Conditions)
            {
                exp.ProcessParameter(base.Statement);
            }
        }

        private string GenerateConditions()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("\r\nWHERE\r\n");
            foreach (BaseExpression compare in this.Conditions)
            {
                builder.Append(compare.Sql);
                builder.Append(" AND");
            }
            return builder.ToString(0, builder.Length - 4);
        }

        public int Execute()
        {
            base.Statement.Command.CommandText = this.GenerateSql();
            this.ProcessParameters();
            return base.Statement.ExecuteNonQuery();
        }
    }
}