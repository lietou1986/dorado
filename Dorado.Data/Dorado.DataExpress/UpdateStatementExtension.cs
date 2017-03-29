using Dorado.DataExpress.Schema;
using System;
using System.Linq.Expressions;

namespace Dorado.DataExpress
{
    public static class UpdateStatementExtension
    {
        public static UpdateStatement<T> Set<T>(this UpdateStatement<T> update, Expression<Func<T>> expression) where T : new()
        {
            MemberInitExpression body = expression.Body as MemberInitExpression;
            if (body != null)
            {
                if (body.Bindings.Count == 0)
                {
                    throw new ColumnsNotFoundExcpetion("没有指定更新的列");
                }
                LambdaExpressionHelper.GetColumns<T>(body.Bindings, delegate(ColumnSchema col, object val)
                {
                    if (col != null)
                    {
                        update.Set(col, val);
                    }
                }
                );
            }
            return update;
        }

        public static UpdateStatement<T> Update<T>(this Database db, Expression<Func<T>> expression) where T : new()
        {
            UpdateStatement<T> st = db.Update<T>();
            return st.Set(expression);
        }

        public static UpdateStatement<TEntity> Where<TEntity>(this UpdateStatement<TEntity> update, Expression<Func<TEntity, bool>> expression) where TEntity : new()
        {
            return (UpdateStatement<TEntity>)update.Where(LambdaExpressionHelper.ParseBinaryExpression<TEntity>((BinaryExpression)expression.Body));
        }
    }
}