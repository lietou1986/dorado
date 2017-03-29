using Dorado.DataExpress.Schema;
using System;
using System.Linq.Expressions;

namespace Dorado.DataExpress
{
    public static class InsertStatementExtension
    {
        public static InsertStatement<T> Set<T>(this InsertStatement<T> insert, Expression<Func<T>> expression) where T : new()
        {
            MemberInitExpression body = expression.Body as MemberInitExpression;
            if (body != null)
            {
                if (body.Bindings.Count == 0)
                {
                    throw new ColumnsNotFoundExcpetion("没有指定需要新建行的数据");
                }
                LambdaExpressionHelper.GetColumns<T>(body.Bindings, delegate(ColumnSchema col, object val)
                {
                    if (col != null)
                    {
                        insert.Add(col, val);
                    }
                }
                );
            }
            return insert;
        }
    }
}