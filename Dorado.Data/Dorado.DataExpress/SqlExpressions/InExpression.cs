using Dorado.DataExpress.Schema;
using System.Collections.Generic;
using System.Text;

namespace Dorado.DataExpress.SqlExpressions
{
    public class InExpression : BaseCompareExpression
    {
        private string _sqlOp = " {0} IN ({1}) ";
        private List<BaseExpression> _values = new List<BaseExpression>();

        public List<BaseExpression> Values
        {
            get
            {
                return this._values;
            }
            set
            {
                this._values = value;
            }
        }

        public override string Sql
        {
            get
            {
                base.Column.InheritedFrom(this);
                return string.Format(base.SqlOperator, base.Column.Sql, this.GenerateParam());
            }
        }

        public InExpression()
            : base(" {0} in ({1}) ")
        {
        }

        public InExpression(ColumnSchema col)
            : this(new SimpleColumnExpression(col))
        {
        }

        public InExpression(BaseExpression col)
            : this()
        {
            base.Column = col;
        }

        public InExpression(ColumnSchema col, params object[] values)
            : this(col)
        {
            for (int i = 0; i < values.Length; i++)
            {
                object obj = values[i];
                this.Values.Add((obj is BaseExpression) ? (obj as BaseExpression) : new SimpleValueExpression(obj));
            }
        }

        public InExpression(BaseExpression col, params object[] values)
            : this(col)
        {
            for (int i = 0; i < values.Length; i++)
            {
                object obj = values[i];
                this.Values.Add((obj is BaseExpression) ? (obj as BaseExpression) : new SimpleValueExpression(obj));
            }
        }

        private string GenerateParam()
        {
            StringBuilder builder = new StringBuilder(32);
            for (int i = 0; i < this.Values.Count; i++)
            {
                this.Values[i].InheritedFrom(this);
                builder.Append(this.Values[i].Sql);
                builder.Append(",");
            }
            return builder.ToString(0, builder.Length - 1);
        }

        public override void ProcessParameter(SqlStatement statement)
        {
            base.Column.ProcessParameter(statement);
            foreach (BaseExpression t in this.Values)
            {
                t.ProcessParameter(statement);
            }
        }
    }
}