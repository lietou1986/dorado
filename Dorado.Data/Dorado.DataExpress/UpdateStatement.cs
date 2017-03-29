using Dorado.DataExpress.Ldo;
using Dorado.DataExpress.Schema;
using Dorado.DataExpress.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dorado.DataExpress
{
    public class UpdateStatement : BaseQuery
    {
        private readonly List<BaseExpression> _conditions = new List<BaseExpression>();
        private readonly List<ColumnWithValue> _keyValues = new List<ColumnWithValue>();

        public UpdateStatement()
        {
        }

        public UpdateStatement(TableSchema table, SqlStatement st)
        {
            base.Table = table;
            base.Statement = st;
        }

        [Obsolete]
        public UpdateStatement Add(ColumnSchema col, object value)
        {
            ColumnWithValue kv = new ColumnWithValue
            {
                Column = col,
                Value = value
            };
            this._keyValues.Add(kv);
            return this;
        }

        [Obsolete]
        public UpdateStatement Add(string col, object value)
        {
            ColumnSchema schema = this.GetColumnSchema(col);
            return this.Add(schema, value);
        }

        public UpdateStatement Set(string col, object value)
        {
            return this.Set(this.GetColumnSchema(col), value);
        }

        public UpdateStatement Set(ColumnSchema col, object value)
        {
            ColumnWithValue kv = new ColumnWithValue
            {
                Column = col,
                Value = value
            };
            this._keyValues.Add(kv);
            return this;
        }

        [Obsolete]
        public UpdateStatement AddCondition(BaseCompareExpression exp)
        {
            exp.Parent = this;
            exp.Dialect = base.Statement.Database.Dialect;
            exp.Driver = base.Statement.Database.Driver;
            this._conditions.Add(exp);
            return this;
        }

        [Obsolete]
        public UpdateStatement AddCondition(ColumnSchema col, object value)
        {
            BaseCompareExpression colExp = SqlExpression.Eq(col, value);
            return this.AddCondition(colExp);
        }

        [Obsolete]
        public UpdateStatement AddCondition(string colName, object value)
        {
            ColumnSchema col = new ColumnSchema
            {
                Name = colName,
                FullName = colName,
                Table = base.Table
            };
            return this.AddCondition(col, value);
        }

        public UpdateStatement Where(BaseCompareExpression exp)
        {
            this._conditions.Add(exp);
            return this;
        }

        public UpdateStatement Where(BaseExpression exp)
        {
            this._conditions.Add(exp);
            return this;
        }

        public UpdateStatement Where(ColumnSchema col, object val)
        {
            this._conditions.Add(SqlExpression.Eq(col, val));
            return this;
        }

        public UpdateStatement Where(string colName, object value)
        {
            ColumnSchema col = this.GetColumnSchema(colName);
            this._conditions.Add(SqlExpression.Eq(col, value));
            return this;
        }

        protected ColumnSchema GetColumnSchema(string columnName)
        {
            return this.GetColumnSchema(columnName, null);
        }

        protected virtual ColumnSchema GetColumnSchema(string columnName, string fullName)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException("columnName");
            }
            return new ColumnSchema(columnName)
            {
                Table = base.Table,
                FullName = fullName
            };
        }

        public override string GenerateSql()
        {
            StringBuilder builder = new StringBuilder(128);
            builder.Append(base.Dialect.GetKeyword("update")).Append(base.Dialect.NewLine).Append(base.Dialect.GetSystemName(base.Table)).Append(base.Dialect.NewLine).Append(base.Dialect.GetKeyword("set")).Append(base.Dialect.NewLine).Append(this.GenerateValueParameters());
            if (this._conditions.Count > 0)
            {
                builder.Append(this.GenerateConditions());
            }
            return builder.ToString();
        }

        private string GenerateConditions()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(base.Dialect.NewLine).Append(base.Dialect.GetKeyword("where")).Append(base.Dialect.NewLine);
            foreach (BaseExpression compare in this._conditions)
            {
                compare.Dialect = base.Dialect;
                compare.Driver = base.Driver;
                compare.Parent = this;
                builder.Append(compare.Sql).Append(" ").Append(base.Dialect.GetKeyword("and"));
            }
            return builder.ToString(0, builder.Length - 4);
        }

        private string GenerateValueParameters()
        {
            StringBuilder builder = new StringBuilder(32);
            int count = 0;
            foreach (ColumnWithValue col in this._keyValues)
            {
                count++;
                builder.Append(base.Statement.Database.Dialect.GetSystemName(col.Column)).Append("=").Append(base.Statement.Database.Dialect.ParameterPrefix + "v" + count).Append(",");
            }
            return builder.ToString(0, builder.Length - 1);
        }

        public override void ProcessParameters()
        {
            int count = 0;
            foreach (ColumnWithValue col in this._keyValues)
            {
                count++;
                base.Statement.AddParameter("v" + count, col.Value);
            }
            foreach (BaseExpression exp in this._conditions)
            {
                exp.ProcessParameter(base.Statement);
            }
        }

        public virtual int Execute()
        {
            base.Statement.Command.CommandText = this.GenerateSql();
            this.ProcessParameters();
            return base.Statement.ExecuteNonQuery();
        }
    }

    public class UpdateStatement<TEntity> : UpdateStatement where TEntity : new()
    {
        public UpdateStatement(SqlStatement st)
            : base(null, st)
        {
            base.Table = BinderManager<TEntity>.Binder.EntifyInfo.Schema;
        }

        protected override ColumnSchema GetColumnSchema(string columnName, string fullName)
        {
            ColumnSchema column = BinderManager<TEntity>.GetColumnSchema(columnName);
            if (column != null)
            {
                return column;
            }
            return base.GetColumnSchema(columnName, fullName);
        }
    }
}