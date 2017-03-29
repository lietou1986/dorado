using Dorado.DataExpress.Schema;

namespace Dorado.DataExpress.SqlExpressions
{
    public class OrderExpression : BaseExpression
    {
        private BaseExpression _column;
        private bool _desc;

        public bool Desc
        {
            get
            {
                return this._desc;
            }
            set
            {
                this._desc = value;
            }
        }

        public BaseExpression Column
        {
            get
            {
                return this._column;
            }
            set
            {
                this._column = value;
            }
        }

        public override string Sql
        {
            get
            {
                this.Column.InheritedFrom(this);
                return this.Column.Sql + (this.Desc ? " DESC " : "");
            }
        }

        public OrderExpression(ColumnSchema col, bool desc)
        {
            this.Desc = desc;
            this.Column = new SimpleColumnExpression(col);
        }

        public OrderExpression(BaseExpression exp, bool desc)
        {
            this.Column = exp;
            this.Desc = desc;
        }

        public OrderExpression(string name, bool desc)
        {
            this.Column = new SimpleNameExpression(name);
            this.Desc = desc;
        }
    }
}