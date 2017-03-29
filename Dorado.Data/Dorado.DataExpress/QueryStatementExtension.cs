using Dorado.DataExpress.Ldo;
using Dorado.DataExpress.Schema;
using Dorado.DataExpress.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dorado.DataExpress
{
    public static class QueryStatementExtension
    {
        public static List<T> ToList<T>(this QueryStatement<T> query) where T : new()
        {
            return query.All();
        }

        public static QueryStatement<TEntity> Select<TEntity>(this QueryStatement<TEntity> st, params Expression<Func<TEntity, object>>[] expressions) where TEntity : new()
        {
            if (expressions == null)
            {
                throw new ArgumentNullException("expressions");
            }
            Array.ForEach<Expression<Func<TEntity, object>>>(expressions, delegate(Expression<Func<TEntity, object>> exp)
            {
                ColumnSchema column = null;
                MemberExpression expBody = exp.Body as MemberExpression;
                if (expBody != null)
                {
                    column = BinderManager<TEntity>.GetColumnSchema(expBody.Member.Name);
                    st.AddColumn(column);
                    return;
                }
                UnaryExpression expBody2 = exp.Body as UnaryExpression;
                if (expBody2 != null)
                {
                    MemberExpression memberExp = expBody2.Operand as MemberExpression;
                    if (memberExp != null)
                    {
                        column = BinderManager<TEntity>.GetColumnSchema(memberExp.Member.Name);
                    }
                }
                st.AddColumn(column);
            }
            );
            return st;
        }

        public static QueryStatement<TEntity> Where<TEntity>(this QueryStatement<TEntity> st, Expression<Func<TEntity, bool>> expression) where TEntity : new()
        {
            return st.Where(LambdaExpressionHelper.ParseBinaryExpression<TEntity>((BinaryExpression)expression.Body));
        }

        public static QueryStatement<TEntity> OrderBy<TEntity>(this QueryStatement<TEntity> st, params Expression<Func<TEntity, object>>[] expressions) where TEntity : new()
        {
            return st.OrderBy(false, expressions);
        }

        public static QueryStatement<TEntity> OrderByDescing<TEntity>(this QueryStatement<TEntity> st, params Expression<Func<TEntity, object>>[] expressions) where TEntity : new()
        {
            return st.OrderBy(true, expressions);
        }

        internal static QueryStatement<TEntity> OrderBy<TEntity>(this QueryStatement<TEntity> st, bool descing, params Expression<Func<TEntity, object>>[] expressions) where TEntity : new()
        {
            Func<BaseExpression, OrderExpression> orderFunc = delegate(BaseExpression exp)
            {
                if (!descing)
                {
                    return SqlExpression.Order(exp);
                }
                return SqlExpression.DescOrder(exp);
            }
            ;
            Array.ForEach<Expression<Func<TEntity, object>>>(expressions, delegate(Expression<Func<TEntity, object>> exp)
            {
                MemberExpression body = exp.Body as MemberExpression;
                if (body != null)
                {
                    BaseExpression expression = LambdaExpressionHelper.ParseMemberExpression<TEntity>(body);
                    st.OrderBy(orderFunc(expression));
                    return;
                }
                UnaryExpression body2 = exp.Body as UnaryExpression;
                if (body2 != null)
                {
                    BaseExpression expression2 = LambdaExpressionHelper.ParseMemberExpression<TEntity>((MemberExpression)body2.Operand);
                    st.OrderBy(orderFunc(expression2));
                }
            }
            );
            return st;
        }
    }
}