using System.Collections.Generic;
using System.Text;

namespace Dorado.DataExpress.SqlExpressions
{
    public class CommandExpression : BaseExpression
    {
        private const string sqlOperator = "{0}({1})";
        public List<BaseExpression> Parameters = new List<BaseExpression>();
        private string _functionName = string.Empty;

        public override string Sql
        {
            get
            {
                return string.Format("{0}({1})", this.FunctionName, this.GenerateParam());
            }
        }

        public string FunctionName
        {
            get
            {
                return this._functionName;
            }
            set
            {
                this._functionName = value;
            }
        }

        public CommandExpression()
        {
        }

        public CommandExpression(string functionName)
        {
            this._functionName = functionName;
        }

        public CommandExpression(string functionName, params object[] objects)
            : this(functionName)
        {
            for (int i = 0; i < objects.Length; i++)
            {
                object obj = objects[i];
                this.Parameters.Add((obj is BaseExpression) ? (obj as BaseExpression) : new SimpleValueExpression(obj));
            }
        }

        private string GenerateParam()
        {
            if (this.Parameters.Count == 0)
            {
                return "";
            }
            StringBuilder builder = new StringBuilder(32);
            foreach (BaseExpression exp in this.Parameters)
            {
                exp.InheritedFrom(this);
                builder.Append(exp.Sql);
                builder.Append(",");
            }
            return builder.ToString(0, builder.Length - 1);
        }

        public override void ProcessParameter(SqlStatement statement)
        {
            foreach (BaseExpression exp in this.Parameters)
            {
                exp.ProcessParameter(statement);
            }
        }
    }
}