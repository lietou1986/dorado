using Autofac;
using Autofac.Core;
using Dorado.Core.SuperCache;
using Dorado.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dorado.Platform.Infrastructure.DependencyManagement
{
    public static class PropertyInject
    {
        /// <summary>
        /// 组装对象激活代理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="componentType"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<Func<IComponentContext, TypedParameter>, Action<IComponentContext, object>>> BuildInjectors<T>(this Type componentType)
        {
            return componentType.BuildInjectors(typeof(T));
        }

        /// <summary>
        /// 组装对象激活代理
        /// </summary>
        /// <param name="componentType"></param>
        /// <param name="targetTypes"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<Func<IComponentContext, TypedParameter>, Action<IComponentContext, object>>> BuildInjectors(this Type componentType, params Type[] targetTypes)
        {
            foreach (var t in targetTypes)
            {
                var properties = componentType.GetSetPropertys(t);

                foreach (var p in properties)
                {
                    yield return new Tuple<Func<IComponentContext, TypedParameter>, Action<IComponentContext, object>>
                    (
                        (ctx) =>
                        {
                            return new TypedParameter(t, ctx.Resolve(t, new TypedParameter(typeof(Type), componentType)));
                        },
                        (ctx, instance) =>
                        {
                            var o = ctx.Resolve(t, new TypedParameter(typeof(Type), componentType));
                            p.SetValue(instance, o, null);
                        }
                    );
                }
            }
        }

        /// <summary>
        /// 属性注入模块
        /// </summary>
        public class PropertyInjectionModule : Module
        {
            private List<Type> _types = new List<Type>();

            public PropertyInjectionModule(params Type[] types)
            {
                types.ForEach(n => { _types.Add(n); });
            }

            protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
            {
                var injectors = new List<Tuple<Func<IComponentContext, TypedParameter>, Action<IComponentContext, object>>>();

                _types.ForEach(t => { injectors.AddRange(registration.Activator.LimitType.BuildInjectors(t)); });

                if (!injectors.Any())
                    return;

                registration.Preparing += (s, e) =>
                {
                    injectors.ForEach(n => { e.Parameters = e.Parameters.Concat(new[] { n.Item1(e.Context) }); });
                };

                registration.Activated += (s, e) =>
                {
                    injectors.ForEach(n => { n.Item2(e.Context, e.Instance); });
                };
            }
        }
    }
}