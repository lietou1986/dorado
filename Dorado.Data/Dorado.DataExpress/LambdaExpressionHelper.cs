using Dorado.DataExpress.Ldo;
using Dorado.DataExpress.Schema;
using Dorado.DataExpress.SqlExpressions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Dorado.DataExpress
{
    internal static class LambdaExpressionHelper
    {
        internal static BaseExpression ParseMemberExpression<TEntity>(MemberExpression expression) where TEntity : new()
        {
            if (expression.Member.ReflectedType != typeof(TEntity))
            {
                return SqlExpression.Val(LambdaExpressionHelper.GetExpressionValue(expression));
            }
            ColumnSchema colschema = BinderManager<TEntity>.GetColumnSchema(expression.Member.Name);
            if (colschema != null)
            {
                return SqlExpression.Column(colschema);
            }
            return SqlExpression.Val(LambdaExpressionHelper.GetExpressionValue(expression));
        }

        internal static BaseExpression ParseBinaryExpression<TEntity>(BinaryExpression expression) where TEntity : new()
        {
            BaseExpression left = null;
            BaseExpression right = null;
            if (expression.Left is BinaryExpression)
            {
                left = LambdaExpressionHelper.ParseBinaryExpression<TEntity>((BinaryExpression)expression.Left);
            }
            else
            {
                if (expression.Left is MemberExpression)
                {
                    left = LambdaExpressionHelper.ParseMemberExpression<TEntity>((MemberExpression)expression.Left);
                }
                else
                {
                    if (expression.Left is ConstantExpression)
                    {
                        left = LambdaExpressionHelper.ParseConstantExpression((ConstantExpression)expression.Left);
                    }
                    else
                    {
                        if (expression.Left is MethodCallExpression)
                        {
                            left = LambdaExpressionHelper.ParseMethodCallExpression<TEntity>((MethodCallExpression)expression.Left);
                        }
                        else
                        {
                            if (expression.Left is UnaryExpression)
                            {
                                left = LambdaExpressionHelper.ParseUnaryExpression<TEntity>((UnaryExpression)expression.Left);
                            }
                        }
                    }
                }
            }
            if (expression.Right is BinaryExpression)
            {
                right = LambdaExpressionHelper.ParseBinaryExpression<TEntity>((BinaryExpression)expression.Right);
            }
            else
            {
                if (expression.Right is MemberExpression)
                {
                    right = LambdaExpressionHelper.ParseMemberExpression<TEntity>((MemberExpression)expression.Right);
                }
                else
                {
                    if (expression.Right is ConstantExpression)
                    {
                        ConstantExpression rightConstant = (ConstantExpression)expression.Right;
                        if (rightConstant.Value == null || rightConstant.Value is DBNull)
                        {
                            ExpressionType nodeType = expression.NodeType;
                            if (nodeType == ExpressionType.Equal)
                            {
                                return SqlExpression.IsNull(left);
                            }
                            if (nodeType != ExpressionType.NotEqual)
                            {
                                throw new NotSupportedException(string.Format("在左值为{0}，操作符为:{1}的情况下，判断是否为空不被支持。", expression.Left, expression.NodeType));
                            }
                            return SqlExpression.IsNotNull(left);
                        }
                        else
                        {
                            right = LambdaExpressionHelper.ParseConstantExpression(rightConstant);
                        }
                    }
                    else
                    {
                        if (expression.Right is MethodCallExpression)
                        {
                            right = LambdaExpressionHelper.ParseMethodCallExpression<TEntity>((MethodCallExpression)expression.Right);
                        }
                        else
                        {
                            if (expression.Right is UnaryExpression)
                            {
                                right = LambdaExpressionHelper.ParseUnaryExpression<TEntity>((UnaryExpression)expression.Right);
                            }
                        }
                    }
                }
            }
            if (left == null || right == null)
            {
                throw new ArgumentNullException(string.Format("左表达式{0}或右表达式{1}为空。", expression.Left, expression.Right));
            }
            return LambdaExpressionHelper.GetExpression<TEntity>(expression, left, right);
        }

        private static BaseExpression ParseUnaryExpression<T>(UnaryExpression expression) where T : new()
        {
            ConstantExpression exp = expression.Operand as ConstantExpression;
            if (exp != null)
            {
                return LambdaExpressionHelper.ParseConstantExpression(exp);
            }
            BinaryExpression exp2 = expression.Operand as BinaryExpression;
            if (exp2 != null)
            {
                return LambdaExpressionHelper.ParseBinaryExpression<T>(exp2);
            }
            return null;
        }

        private static BaseExpression GetExpression<T>(BinaryExpression expression, BaseExpression left, BaseExpression right)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    {
                        return SqlExpression.Compute(left, ColumnOperator.Add, right);
                    }
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    {
                        return SqlExpression.And(left, right);
                    }
                case ExpressionType.ArrayLength:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Call:
                case ExpressionType.Coalesce:
                case ExpressionType.Conditional:
                case ExpressionType.Constant:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.Invoke:
                case ExpressionType.Lambda:
                case ExpressionType.LeftShift:
                case ExpressionType.ListInit:
                case ExpressionType.MemberAccess:
                case ExpressionType.MemberInit:
                case ExpressionType.Negate:
                case ExpressionType.UnaryPlus:
                case ExpressionType.NegateChecked:
                case ExpressionType.New:
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                case ExpressionType.Not:
                case ExpressionType.Parameter:
                case ExpressionType.Power:
                case ExpressionType.Quote:
                case ExpressionType.RightShift:
                case ExpressionType.TypeAs:
                case ExpressionType.TypeIs:
                    {
                        throw new NotSupportedException(string.Format("不支持{0}类型的运算符。", expression.NodeType));
                    }
                case ExpressionType.Divide:
                    {
                        return SqlExpression.Compute(left, ColumnOperator.Div, right);
                    }
                case ExpressionType.Equal:
                    {
                        return SqlExpression.Eq(left, right);
                    }
                case ExpressionType.GreaterThan:
                    {
                        return SqlExpression.Gt(left, right);
                    }
                case ExpressionType.GreaterThanOrEqual:
                    {
                        return SqlExpression.Ge(left, right);
                    }
                case ExpressionType.LessThan:
                    {
                        return SqlExpression.Lt(left, right);
                    }
                case ExpressionType.LessThanOrEqual:
                    {
                        return SqlExpression.Le(left, right);
                    }
                case ExpressionType.Modulo:
                    {
                        return SqlExpression.Compute(left, ColumnOperator.Mod, right);
                    }
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    {
                        return SqlExpression.Compute(left, ColumnOperator.Multi, right);
                    }
                case ExpressionType.NotEqual:
                    {
                        return SqlExpression.Ne(left, right);
                    }
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    {
                        return SqlExpression.Or(left, right);
                    }
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    {
                        return SqlExpression.Compute(left, ColumnOperator.Dec, right);
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
            }
        }

        public static void GetColumns<T>(ReadOnlyCollection<MemberBinding> memberBindings, Action<ColumnSchema, object> action) where T : new()
        {
            if (memberBindings != null)
            {
                using (IEnumerator<MemberBinding> enumerator = memberBindings.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        MemberAssignment binding = (MemberAssignment)enumerator.Current;
                        ColumnSchema column = BinderManager<T>.GetColumnSchema(binding.Member.Name);
                        ConstantExpression exp = binding.Expression as ConstantExpression;
                        if (exp != null)
                        {
                            action(column, exp.Value);
                        }
                        else
                        {
                            UnaryExpression exp2 = binding.Expression as UnaryExpression;
                            if (exp2 != null)
                            {
                                ConstantExpression expVal = exp2.Operand as ConstantExpression;
                                if (expVal != null)
                                {
                                    action(column, expVal.Value);
                                    continue;
                                }
                                action(column, LambdaExpressionHelper.GetExpressionValue(exp2.Operand));
                            }
                            object dynamicValue = LambdaExpressionHelper.GetExpressionValue(binding.Expression);
                            action(column, dynamicValue);
                        }
                    }
                }
            }
        }

        internal static object GetExpressionValue(Expression expression)
        {
            Delegate compiledExpression = Expression.Lambda(expression, new ParameterExpression[0]).Compile();
            object result = compiledExpression.DynamicInvoke(new object[0]);
            return result ?? DBNull.Value;
        }

        internal static object GetMemberValue(MemberExpression memberExpression)
        {
            Delegate compile = Expression.Lambda(memberExpression, new ParameterExpression[0]).Compile();
            return compile.DynamicInvoke(new object[0]);
        }

        internal static object GetExpressionValue(BinaryExpression binaryExpression)
        {
            Delegate compile = Expression.Lambda(binaryExpression, new ParameterExpression[0]).Compile();
            return compile.DynamicInvoke(new object[0]);
        }

        internal static BaseExpression ParseConstantExpression(ConstantExpression expression)
        {
            return SqlExpression.Val(expression.Value);
        }

        internal static T GetConstantValue<T>(ConstantExpression expression)
        {
            return (T)expression.Value;
        }

        internal static T GetExpressionValue<T>(Expression expression)
        {
            if (expression is ConstantExpression)
            {
                return LambdaExpressionHelper.GetConstantValue<T>((ConstantExpression)expression);
            }
            return (T)LambdaExpressionHelper.GetExpressionValue(expression);
        }

        internal static BaseExpression ParseMethodCallExpression<TEntity>(MethodCallExpression expression) where TEntity : new()
        {
            MemberExpression mexp = expression.Object as MemberExpression;
            if (mexp != null)
            {
                string name = expression.Method.Name;
                BaseExpression column = LambdaExpressionHelper.ParseMemberExpression<TEntity>(mexp);
                string a;
                if ((a = name) != null)
                {
                    if (a == "StartsWith")
                    {
                        return SqlExpression.Like(column, LambdaExpressionHelper.GetExpressionValue<string>(expression.Arguments[0]) + "%");
                    }
                    if (a == "Contains")
                    {
                        return SqlExpression.Like(column, "%" + LambdaExpressionHelper.GetExpressionValue<string>(expression.Arguments[0]) + "%");
                    }
                    if (a == "EndsWith")
                    {
                        return SqlExpression.Like(column, "%" + LambdaExpressionHelper.GetExpressionValue<string>(expression.Arguments[0]));
                    }
                }
            }
            return SqlExpression.Val(LambdaExpressionHelper.GetExpressionValue(expression));
        }
    }
}