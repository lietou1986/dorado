using System.Text;

namespace Dorado.DataExpress.SqlExpressions
{
    public class ComputeExpression : BaseColumnExpression
    {
        private BaseExpression _NextColumn;
        private BaseExpression _column;
        private ColumnOperator _oP;
        private ComputeExpression _parentExpression;

        public ComputeExpression ParentExpression
        {
            get
            {
                return this._parentExpression;
            }
            set
            {
                this._parentExpression = value;
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

        public BaseExpression NextColumn
        {
            get
            {
                return this._NextColumn;
            }
            set
            {
                this._NextColumn = value;
            }
        }

        public ColumnOperator OP
        {
            get
            {
                return this._oP;
            }
            set
            {
                this._oP = value;
            }
        }

        public override string Sql
        {
            get
            {
                StringBuilder builder = new StringBuilder(32);
                if (this.NextColumn != null)
                {
                    builder.Append(" (");
                }
                this.Column.InheritedFrom(this);
                builder.Append(this.Column.Sql);
                if (this.NextColumn != null)
                {
                    switch (this.OP)
                    {
                        case ColumnOperator.Add:
                            {
                                builder.Append("+");
                                break;
                            }
                        case ColumnOperator.Dec:
                            {
                                builder.Append("-");
                                break;
                            }
                        case ColumnOperator.Multi:
                            {
                                builder.Append("*");
                                break;
                            }
                        case ColumnOperator.Div:
                            {
                                builder.Append("/");
                                break;
                            }
                        case ColumnOperator.Mod:
                            {
                                builder.Append("%");
                                break;
                            }
                        default:
                            {
                                builder.Append("+");
                                break;
                            }
                    }
                    this.NextColumn.InheritedFrom(this);
                    builder.Append(this.NextColumn.Sql);
                }
                if (this.NextColumn != null)
                {
                    builder.Append(") ");
                }
                return builder.ToString();
            }
        }

        public ComputeExpression(BaseExpression column)
        {
            this.Column = column;
        }

        public ComputeExpression(BaseExpression column, ColumnOperator op, BaseExpression nextcol)
            : this(column)
        {
            this.NextColumn = nextcol;
            this.OP = op;
        }

        public static ComputeExpression operator +(ComputeExpression exp1, object obj)
        {
            BaseExpression exp2;
            if (obj is BaseExpression)
            {
                exp2 = (obj as BaseExpression);
            }
            else
            {
                exp2 = SqlExpression.Val(obj);
            }
            if (exp1.NextColumn == null)
            {
                exp1.OP = ColumnOperator.Add;
                exp1.NextColumn = exp2;
                return exp1;
            }
            return ComputeExpression.NewExpression(exp1, exp2, ColumnOperator.Add);
        }

        public static ComputeExpression operator -(ComputeExpression exp1, object obj)
        {
            BaseExpression exp2;
            if (obj is BaseExpression)
            {
                exp2 = (obj as BaseExpression);
            }
            else
            {
                exp2 = SqlExpression.Val(obj);
            }
            if (exp1.NextColumn == null)
            {
                exp1.OP = ColumnOperator.Dec;
                exp1.NextColumn = exp2;
                return exp1;
            }
            return ComputeExpression.NewExpression(exp1, exp2, ColumnOperator.Dec);
        }

        public static ComputeExpression operator *(ComputeExpression exp1, object obj)
        {
            BaseExpression exp2;
            if (obj is BaseExpression)
            {
                exp2 = (obj as BaseExpression);
            }
            else
            {
                exp2 = SqlExpression.Val(obj);
            }
            if (exp1.NextColumn == null)
            {
                exp1.OP = ColumnOperator.Multi;
                exp1.NextColumn = exp2;
                return exp1;
            }
            return ComputeExpression.NewExpression(exp1, exp2, ColumnOperator.Multi);
        }

        public static ComputeExpression operator /(ComputeExpression exp1, object obj)
        {
            BaseExpression exp2;
            if (obj is BaseExpression)
            {
                exp2 = (obj as BaseExpression);
            }
            else
            {
                exp2 = SqlExpression.Val(obj);
            }
            if (exp1.NextColumn == null)
            {
                exp1.OP = ColumnOperator.Div;
                exp1.NextColumn = exp2;
                return exp1;
            }
            return ComputeExpression.NewExpression(exp1, exp2, ColumnOperator.Div);
        }

        public static ComputeExpression operator %(ComputeExpression exp1, BaseExpression exp2)
        {
            if (exp1.NextColumn == null)
            {
                exp1.OP = ColumnOperator.Mod;
                exp1.NextColumn = exp2;
                return exp1;
            }
            return ComputeExpression.NewExpression(exp1, exp2, ColumnOperator.Mod);
        }

        public static ComputeExpression NewExpression(ComputeExpression exp1, BaseExpression exp2, ColumnOperator op)
        {
            ComputeExpression comp = new ComputeExpression(exp1, op, exp2);
            exp1.ParentExpression = comp;
            return comp;
        }

        public override void ProcessParameter(SqlStatement st)
        {
            if (this.Column != null)
            {
                this.Column.ProcessParameter(st);
            }
            if (this.NextColumn != null)
            {
                this.NextColumn.ProcessParameter(st);
            }
        }
    }
}