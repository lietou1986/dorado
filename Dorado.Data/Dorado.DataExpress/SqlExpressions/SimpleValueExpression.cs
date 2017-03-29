namespace Dorado.DataExpress.SqlExpressions
{
    public class SimpleValueExpression : BaseExpression
    {
        private string _parameterName;
        private object _value;

        public object Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }

        public string ParameterName
        {
            get
            {
                return this._parameterName;
            }
            set
            {
                this._parameterName = value;
            }
        }

        public override string Sql
        {
            get
            {
                if (string.IsNullOrEmpty(this._parameterName))
                {
                    this._parameterName = base.Dialect.ParameterPrefix + base.Parent.GetParameterName();
                }
                return this._parameterName;
            }
        }

        public SimpleValueExpression(object value)
        {
            this.Value = value;
        }

        public override void ProcessParameter(SqlStatement st)
        {
            st.AddParameter(this._parameterName, this.Value);
        }
    }
}