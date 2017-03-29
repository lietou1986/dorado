using Dorado.DataExpress.Schema;
using System;

namespace Dorado.DataExpress.SqlExpressions
{
    public static class SqlExpression
    {
        public static SimpleColumnExpression Column(ColumnSchema col, string alias)
        {
            return new SimpleColumnExpression(col, alias);
        }

        public static SimpleColumnExpression Column(string colName)
        {
            return SqlExpression.Column(new ColumnSchema(colName));
        }

        public static SimpleColumnExpression Column(string colName, string alias)
        {
            return SqlExpression.Column(new ColumnSchema(colName), alias);
        }

        public static SimpleColumnExpression Column(ColumnSchema col)
        {
            return SqlExpression.Column(col, string.Empty);
        }

        public static SimpleValueExpression Val(object obj)
        {
            if (obj is BaseExpression)
            {
                throw new Exception("Expressin.Val函数不允许传入BaseExpression类型的值");
            }
            return new SimpleValueExpression(obj);
        }

        public static BaseExpression Keyword(string keyword)
        {
            if (keyword == null)
            {
                throw new ArgumentNullException("keyword");
            }
            return new OriginalKeywordExpression
            {
                Keyword = keyword
            };
        }

        public static SimpleNameExpression Name(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return new SimpleNameExpression(name);
        }

        public static JoinExpression InnerJoin(TableSchema table, ColumnSchema leftColumn, ColumnSchema column)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (leftColumn == null)
            {
                throw new ArgumentNullException("leftColumn");
            }
            if (column == null)
            {
                throw new ArgumentNullException("column");
            }
            return new JoinExpression(table, JoinExpression.JoinType.Inner, leftColumn, column);
        }

        public static JoinExpression LeftJoin(TableSchema table, ColumnSchema leftColumn, ColumnSchema column)
        {
            return new JoinExpression(table, JoinExpression.JoinType.Left, leftColumn, column);
        }

        public static EqExpression Eq(string colName, object val)
        {
            if (colName == null)
            {
                throw new ArgumentNullException("colName");
            }
            if (val == null)
            {
                throw new ArgumentNullException("val");
            }
            return SqlExpression.Eq(new ColumnSchema(colName), val);
        }

        public static EqExpression Eq(ColumnSchema col, object val)
        {
            if (col == null)
            {
                throw new ArgumentNullException("col");
            }
            if (val == null)
            {
                throw new ArgumentNullException("val");
            }
            return new EqExpression(col, val);
        }

        public static EqExpression Eq(BaseExpression col, object val)
        {
            if (col == null)
            {
                throw new ArgumentNullException("col");
            }
            if (val == null)
            {
                throw new ArgumentNullException("val");
            }
            return new EqExpression(col, val);
        }

        public static EqExpression Eq(BaseExpression left, BaseExpression right)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }
            return new EqExpression(left, right);
        }

        public static NeExpression Ne(string colName, object val)
        {
            if (colName == null)
            {
                throw new ArgumentNullException("colName");
            }
            if (val == null)
            {
                throw new ArgumentNullException("val");
            }
            return SqlExpression.Ne(new ColumnSchema(colName), val);
        }

        public static NeExpression Ne(ColumnSchema col, object val)
        {
            if (col == null)
            {
                throw new ArgumentNullException("col");
            }
            if (val == null)
            {
                throw new ArgumentNullException("val");
            }
            return new NeExpression(col, val);
        }

        public static NeExpression Ne(BaseExpression col, object val)
        {
            if (col == null)
            {
                throw new ArgumentNullException("col");
            }
            if (val == null)
            {
                throw new ArgumentNullException("val");
            }
            return new NeExpression(col, val);
        }

        public static NeExpression Ne(BaseExpression left, BaseExpression right)
        {
            return new NeExpression(left, right);
        }

        public static LikeExpression Like(ColumnSchema col, string value)
        {
            if (col == null)
            {
                throw new ArgumentNullException("col");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return new LikeExpression(col, value);
        }

        public static LikeExpression Like(string colName, string value)
        {
            if (colName == null)
            {
                throw new ArgumentNullException("colName");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return new LikeExpression(new ColumnSchema(colName), value);
        }

        public static LikeExpression Like(BaseExpression col, string value)
        {
            if (col == null)
            {
                throw new ArgumentNullException("col");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return new LikeExpression(col, value);
        }

        public static IsNullExpression IsNull(ColumnSchema col)
        {
            if (col == null)
            {
                throw new ArgumentNullException("col");
            }
            return new IsNullExpression(col);
        }

        public static IsNotNullExpression IsNull(BaseExpression col)
        {
            if (col == null)
            {
                throw new ArgumentNullException("col");
            }
            return new IsNotNullExpression(col);
        }

        public static IsNotNullExpression IsNotNull(ColumnSchema col)
        {
            if (col == null)
            {
                throw new ArgumentNullException("col");
            }
            return new IsNotNullExpression(col);
        }

        public static IsNotNullExpression IsNotNull(BaseExpression col)
        {
            if (col == null)
            {
                throw new ArgumentNullException("col");
            }
            return new IsNotNullExpression(col);
        }

        public static AndExpression And(BaseCompareExpression exp1, BaseCompareExpression exp2)
        {
            if (exp1 == null)
            {
                throw new ArgumentNullException("exp1");
            }
            if (exp2 == null)
            {
                throw new ArgumentNullException("exp2");
            }
            return new AndExpression(exp1, exp2);
        }

        public static BaseExpression And(BaseExpression left, BaseExpression right)
        {
            return new AndExpression(left, right);
        }

        public static OrExpression Or(BaseCompareExpression exp1, BaseCompareExpression exp2)
        {
            if (exp1 == null)
            {
                throw new ArgumentNullException("exp1");
            }
            if (exp2 == null)
            {
                throw new ArgumentNullException("exp2");
            }
            return new OrExpression(exp1, exp2);
        }

        public static OrExpression Or(BaseExpression left, BaseExpression right)
        {
            return new OrExpression(left, right);
        }

        public static GtExpression Gt(ColumnSchema col, object value)
        {
            if (col == null)
            {
                throw new ArgumentNullException("col");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return new GtExpression(col, value);
        }

        public static GtExpression Gt(BaseExpression col, object value)
        {
            if (col == null)
            {
                throw new ArgumentNullException("col");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return new GtExpression(col, value);
        }

        public static GtExpression Gt(string colName, object value)
        {
            if (colName == null)
            {
                throw new ArgumentNullException("colName");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return SqlExpression.Gt(SqlExpression.Column(colName), value);
        }

        public static GtExpression Gt(BaseExpression left, BaseExpression right)
        {
            return new GtExpression
            {
                Column = left,
                Value = right
            };
        }

        public static GeExpression Ge(string colName, object value)
        {
            if (colName == null)
            {
                throw new ArgumentNullException("colName");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return SqlExpression.Ge(new ColumnSchema(colName), value);
        }

        public static GeExpression Ge(BaseExpression left, BaseExpression right)
        {
            return new GeExpression(left, right);
        }

        public static GeExpression Ge(ColumnSchema col, object value)
        {
            if (col == null)
            {
                throw new ArgumentNullException("col");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return new GeExpression(col, value);
        }

        public static GeExpression Ge(BaseExpression col, object value)
        {
            if (col == null)
            {
                throw new ArgumentNullException("col");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return new GeExpression(col, value);
        }

        public static LtExpression Lt(ColumnSchema col, object value)
        {
            if (col == null)
            {
                throw new ArgumentNullException("col");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return new LtExpression(col, value);
        }

        public static LtExpression Lt(BaseExpression left, BaseExpression right)
        {
            return new LtExpression(left, right);
        }

        public static LtExpression Lt(BaseExpression col, object value)
        {
            if (col == null)
            {
                throw new ArgumentNullException("col");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return new LtExpression(col, value);
        }

        public static LtExpression Lt(string colName, object value)
        {
            if (colName == null)
            {
                throw new ArgumentNullException("colName");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return SqlExpression.Lt(SqlExpression.Column(colName), value);
        }

        public static LeExpression Le(BaseExpression exp, object value)
        {
            if (exp == null)
            {
                throw new ArgumentNullException("exp");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return new LeExpression(exp, value);
        }

        public static LeExpression Le(BaseExpression left, BaseExpression right)
        {
            return new LeExpression(left, right);
        }

        public static LeExpression Le(ColumnSchema col, object value)
        {
            if (col == null)
            {
                throw new ArgumentNullException("col");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return new LeExpression(col, value);
        }

        public static LeExpression Le(string columnName, object value)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException("columnName");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return SqlExpression.Le(SqlExpression.Column(columnName), value);
        }

        public static BetweenExpression Between(ColumnSchema col, object value1, object value2)
        {
            if (col == null)
            {
                throw new ArgumentNullException("col");
            }
            if (value1 == null)
            {
                throw new ArgumentNullException("value1");
            }
            if (value2 == null)
            {
                throw new ArgumentNullException("value2");
            }
            return new BetweenExpression(col, value1, value2);
        }

        public static BetweenExpression Between(BaseExpression col, object value1, object value2)
        {
            if (col == null)
            {
                throw new ArgumentNullException("col");
            }
            if (value1 == null)
            {
                throw new ArgumentNullException("value1");
            }
            if (value2 == null)
            {
                throw new ArgumentNullException("value2");
            }
            return new BetweenExpression(col, value1, value2);
        }

        public static BetweenExpression Between(string colName, object value1, object value2)
        {
            if (colName == null)
            {
                throw new ArgumentNullException("colName");
            }
            if (value1 == null)
            {
                throw new ArgumentNullException("value1");
            }
            if (value2 == null)
            {
                throw new ArgumentNullException("value2");
            }
            return SqlExpression.Between(new ColumnSchema(colName), value1, value2);
        }

        public static ComputeExpression Compute(BaseExpression exp1, ColumnOperator op, BaseExpression exp2)
        {
            return new ComputeExpression(exp1, op, exp2);
        }

        public static ComputeExpression Compute(BaseExpression exp1)
        {
            if (exp1 == null)
            {
                throw new ArgumentNullException("exp1");
            }
            return new ComputeExpression(exp1);
        }

        public static ComputeExpression Compute(ColumnSchema col)
        {
            if (col == null)
            {
                throw new ArgumentNullException("col");
            }
            return new ComputeExpression(SqlExpression.Column(col));
        }

        public static QuoteExpressoin Quoted(BaseExpression exp)
        {
            if (exp == null)
            {
                throw new ArgumentNullException("exp");
            }
            return new QuoteExpressoin(exp);
        }

        public static OrderExpression Order(BaseExpression exp)
        {
            return new OrderExpression(exp, false);
        }

        public static OrderExpression Order(ColumnSchema col)
        {
            return new OrderExpression(col, false);
        }

        public static OrderExpression Order(string name)
        {
            return new OrderExpression(name, false);
        }

        public static OrderExpression DescOrder(BaseExpression exp)
        {
            return new OrderExpression(exp, true);
        }

        public static OrderExpression DescOrder(ColumnSchema col)
        {
            return new OrderExpression(col, true);
        }

        public static OrderExpression DescOrder(string name)
        {
            return new OrderExpression(name, true);
        }

        public static CommandExpression Command(string commandName, params BaseExpression[] parameters)
        {
            CommandExpression exp = new CommandExpression(commandName);
            for (int i = 0; i < parameters.Length; i++)
            {
                BaseExpression pm = parameters[i];
                exp.Parameters.Add(pm);
            }
            return exp;
        }

        public static CommandExpression Command(string commandName, params object[] parameters)
        {
            CommandExpression exp = new CommandExpression(commandName);
            for (int i = 0; i < parameters.Length; i++)
            {
                object pm = parameters[i];
                if (pm is BaseExpression)
                {
                    exp.Parameters.Add(pm as BaseExpression);
                }
                else
                {
                    exp.Parameters.Add(SqlExpression.Val(pm));
                }
            }
            return exp;
        }

        public static NativeExpression Sql(string sql)
        {
            return new NativeExpression(sql);
        }

        public static SumExpression Sum(BaseExpression exp)
        {
            return new SumExpression(exp);
        }

        public static SumExpression Sum(ColumnSchema column)
        {
            return SqlExpression.Sum(SqlExpression.Column(column));
        }

        public static SumExpression Sum(string columnName)
        {
            return SqlExpression.Sum(SqlExpression.Column(columnName));
        }

        public static CountExpression Count(BaseExpression exp)
        {
            return new CountExpression(exp);
        }

        public static CountExpression Count(ColumnSchema column)
        {
            return SqlExpression.Count(SqlExpression.Column(column));
        }

        public static CountExpression Count(string columnName)
        {
            return SqlExpression.Count(SqlExpression.Column(columnName));
        }

        public static CountExpression Count()
        {
            return SqlExpression.Count(SqlExpression.Sql("*"));
        }

        public static MaxExpression Max(BaseExpression expression)
        {
            return new MaxExpression(expression);
        }

        public static MaxExpression Max(ColumnSchema column)
        {
            return new MaxExpression(column);
        }

        public static MaxExpression Max(string columnName)
        {
            return new MaxExpression(columnName);
        }

        public static MinExpression Min(BaseExpression expression)
        {
            return new MinExpression(expression);
        }

        public static MinExpression Min(ColumnSchema column)
        {
            return new MinExpression(column);
        }

        public static MinExpression Min(string columnName)
        {
            return new MinExpression(columnName);
        }

        public static AvgExpression Avg(BaseExpression exp)
        {
            return new AvgExpression(exp);
        }

        public static AvgExpression Avg(ColumnSchema column)
        {
            return SqlExpression.Avg(SqlExpression.Column(column));
        }

        public static AvgExpression Avg(string columnName)
        {
            return SqlExpression.Avg(SqlExpression.Column(columnName));
        }

        public static InExpression In(string colName, params object[] values)
        {
            return SqlExpression.In(SqlExpression.Column(colName), values);
        }

        public static InExpression In(string colName, int[] values)
        {
            return SqlExpression.In(SqlExpression.Column(colName), values);
        }

        public static InExpression In(string colName, string[] values)
        {
            return SqlExpression.In(SqlExpression.Column(colName), values);
        }

        public static InExpression In(string colName, double[] values)
        {
            return SqlExpression.In(SqlExpression.Column(colName), values);
        }

        public static InExpression In(ColumnSchema col, params object[] values)
        {
            return SqlExpression.In(SqlExpression.Column(col), values);
        }

        public static InExpression In(ColumnSchema col, int[] values)
        {
            return SqlExpression.In(SqlExpression.Column(col), values);
        }

        public static InExpression In(ColumnSchema col, double[] values)
        {
            return SqlExpression.In(SqlExpression.Column(col), values);
        }

        public static InExpression In(ColumnSchema col, string[] values)
        {
            return SqlExpression.In(SqlExpression.Column(col), values);
        }

        public static InExpression In(BaseExpression col, params object[] values)
        {
            InExpression exp = new InExpression(col);
            Array args = values;
            if (values.Length == 1 && values[0] is Array)
            {
                args = (Array)values[0];
            }
            foreach (object obj in args)
            {
                exp.Values.Add((obj is BaseExpression) ? (obj as BaseExpression) : new SimpleValueExpression(obj));
            }
            return exp;
        }

        public static InExpression In(BaseExpression col, int[] values)
        {
            InExpression exp = new InExpression(col);
            for (int i = 0; i < values.Length; i++)
            {
                int obj = values[i];
                exp.Values.Add(new SimpleValueExpression(obj));
            }
            return exp;
        }

        public static InExpression In(BaseExpression col, string[] values)
        {
            InExpression exp = new InExpression(col);
            for (int i = 0; i < values.Length; i++)
            {
                string obj = values[i];
                exp.Values.Add(new SimpleValueExpression(obj));
            }
            return exp;
        }

        public static InExpression In(BaseExpression col, double[] values)
        {
            InExpression exp = new InExpression(col);
            for (int i = 0; i < values.Length; i++)
            {
                double obj = values[i];
                exp.Values.Add(new SimpleValueExpression(obj));
            }
            return exp;
        }
    }
}