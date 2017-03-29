using Dorado.DataExpress.Ldo;
using Dorado.DataExpress.Schema;
using System.Collections.Generic;
using System.Text;

namespace Dorado.DataExpress
{
    public class InsertStatement : BaseQuery
    {
        private readonly List<ColumnWithValue> _keyValues = new List<ColumnWithValue>();

        public InsertStatement()
        {
        }

        public InsertStatement(TableSchema table, SqlStatement st)
        {
            base.Table = table;
            base.Statement = st;
        }

        public override string GenerateSql()
        {
            StringBuilder builder = new StringBuilder(128);
            builder.Append(base.Dialect.GetKeyword("insert into")).Append(base.Dialect.NewLine).Append(base.Dialect.GetSystemName(base.Table)).Append(base.Dialect.NewLine).Append(base.Dialect.GetKeyword("(")).Append(this.GenerateColumn()).Append(base.Dialect.GetKeyword(")")).Append(base.Dialect.NewLine).Append(base.Dialect.GetKeyword("values")).Append(base.Dialect.NewLine).Append(base.Dialect.GetKeyword("(")).Append(this.GenerateParameterName()).Append(base.Dialect.GetKeyword(")"));
            return builder.ToString();
        }

        public InsertStatement Add(ColumnSchema col, object value)
        {
            ColumnWithValue kv = new ColumnWithValue
            {
                Column = col,
                Value = value
            };
            this._keyValues.Add(kv);
            return this;
        }

        public InsertStatement Add(string col, object value)
        {
            ColumnSchema schema = this.GetColumnSchema(col);
            return this.Add(schema, value);
        }

        protected virtual ColumnSchema GetColumnSchema(string name)
        {
            return this.GetColumnSchema(name, null);
        }

        protected virtual ColumnSchema GetColumnSchema(string name, string fullName)
        {
            return new ColumnSchema
            {
                Table = base.Table,
                Name = name,
                FullName = fullName
            };
        }

        public override void ProcessParameters()
        {
            int count = 0;
            foreach (ColumnWithValue col in this._keyValues)
            {
                count++;
                base.Statement.AddParameter("p" + count, col.Value);
            }
        }

        private string GenerateParameterName()
        {
            StringBuilder builder = new StringBuilder(32);
            int count = 0;
            foreach (ColumnWithValue arg_1F_0 in this._keyValues)
            {
                count++;
                builder.Append(base.Statement.Database.Dialect.ParameterPrefix).Append("p").Append(count).Append(",");
            }
            return builder.ToString(0, builder.Length - 1);
        }

        private string GenerateColumn()
        {
            StringBuilder builder = new StringBuilder(64);
            foreach (ColumnWithValue col in this._keyValues)
            {
                builder.Append(base.Dialect.GetSystemName(col.Column));
                builder.Append(",");
            }
            return builder.ToString(0, builder.Length - 1);
        }

        public int Execute()
        {
            base.Statement.Command.CommandText = this.GenerateSql();
            this.ProcessParameters();
            return base.Statement.Execute();
        }

        public object ExecuteId()
        {
            base.Statement.Command.CommandText = string.Concat(new string[]
			{
				base.Dialect.NoCount,
				base.Dialect.NewLine,
				this.GenerateSql(),
				base.Dialect.NewLine,
				base.Dialect.LastIdentity,
				base.Dialect.NewLine
			});
            this.ProcessParameters();
            return base.Statement.ExecuteScalar();
        }
    }

    public class InsertStatement<T> : InsertStatement where T : new()
    {
        protected override ColumnSchema GetColumnSchema(string name)
        {
            ColumnSchema column = BinderManager<T>.GetColumnSchema(name);
            if (column != null)
            {
                return column;
            }
            return base.GetColumnSchema(name);
        }
    }
}