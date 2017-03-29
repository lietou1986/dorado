using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Dorado.DataExpress.Ldo
{
    internal static class ReflectionBinderExtension
    {
        internal static class SetterActionCache<T>
        {
            internal static readonly object SyncRoot;
            private static readonly ManualResetEvent WaitActionAdded;
            private static readonly Dictionary<PropertyInfo, Action<T, object>> SetActionCache;

            static SetterActionCache()
            {
                ReflectionBinderExtension.SetterActionCache<T>.SyncRoot = new object();
                ReflectionBinderExtension.SetterActionCache<T>.WaitActionAdded = new ManualResetEvent(true);
                ReflectionBinderExtension.SetterActionCache<T>.SetActionCache = new Dictionary<PropertyInfo, Action<T, object>>();
            }

            internal static bool TryGetAction(PropertyInfo propertyInfo, out Action<T, object> action)
            {
                ReflectionBinderExtension.SetterActionCache<T>.WaitActionAdded.WaitOne();
                return ReflectionBinderExtension.SetterActionCache<T>.SetActionCache.TryGetValue(propertyInfo, out action);
            }

            internal static void AddAction(PropertyInfo propertyInfo, Action<T, object> action)
            {
                if (ReflectionBinderExtension.SetterActionCache<T>.SetActionCache.ContainsKey(propertyInfo))
                {
                    return;
                }
                try
                {
                    ReflectionBinderExtension.SetterActionCache<T>.WaitActionAdded.Reset();
                    ReflectionBinderExtension.SetterActionCache<T>.SetActionCache.Add(propertyInfo, action);
                }
                finally
                {
                    ReflectionBinderExtension.SetterActionCache<T>.WaitActionAdded.Set();
                }
            }
        }

        private static Action<T, object> GetSetFunc<T>(PropertyInfo propertyInfo)
        {
            Action<T, object> set;
            if (!ReflectionBinderExtension.SetterActionCache<T>.TryGetAction(propertyInfo, out set))
            {
                MethodInfo methodInfo = propertyInfo.GetSetMethod();
                ParameterExpression paramObj = Expression.Parameter(typeof(T), "obj");
                ParameterExpression paramVal = Expression.Parameter(typeof(object), "val");
                UnaryExpression bodyVal = Expression.Convert(paramVal, propertyInfo.PropertyType);
                MethodCallExpression body = Expression.Call(paramObj, methodInfo, new Expression[]
				{
					bodyVal
				});
                set = Expression.Lambda<Action<T, object>>(body, new ParameterExpression[]
				{
					paramObj,
					paramVal
				}).Compile();
                ReflectionBinderExtension.SetterActionCache<T>.AddAction(propertyInfo, set);
            }
            return set;
        }

        internal static void FastSetValue<T>(this PropertyInfo property, T t, object value)
        {
            ReflectionBinderExtension.GetSetFunc<T>(property)(t, value);
        }
    }
}