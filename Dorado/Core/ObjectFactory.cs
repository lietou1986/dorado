using Dorado.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;

namespace Dorado.Core
{
    /// <summary>
    /// 对象创建工厂
    /// </summary>
    public static class ObjectFactory
    {
        static ObjectFactory()
        {
            _ConfigurationItemDict = (Dictionary<Type, ObjectFactoryConfigurationItem>)ConfigurationManager.GetSection("ObjectFactory")
                ?? new Dictionary<Type, ObjectFactoryConfigurationItem>();
        }

        private static readonly Dictionary<Type, CreatorItem> _Creators = new Dictionary<Type, CreatorItem>();
        private static readonly Func<Type, CreatorItem> _ObjectCreator = _GetCreator;
        private static readonly Dictionary<Type, ObjectFactoryConfigurationItem> _ConfigurationItemDict;

        #region Class CreatorItem ...

        private class CreatorItem
        {
            public IObjectCreator DefaultCreator;

            public IObjectCreator InnerCreator;
        }

        #endregion Class CreatorItem ...

        private static CreatorItem _GetCreator(Type objectType)
        {
            IObjectCreator creator = _GetCreatorFromConfig(objectType);
            IObjectCreator innerCreator = _GetCreatorFromAttribute(objectType) ?? _GetDefaultCreator(objectType);

            if (creator != null)
            {
                return new CreatorItem { DefaultCreator = creator, InnerCreator = innerCreator };
            }
            else
            {
                return new CreatorItem { DefaultCreator = innerCreator };
            }
        }

        // 从Config文件的配置中寻找创建器
        private static IObjectCreator _GetCreatorFromConfig(Type objectType)
        {
            ObjectFactoryConfigurationItem item;
            if (!_ConfigurationItemDict.TryGetValue(objectType, out item))
                return null;

            IObjectCreator creator = Activator.CreateInstance(item.CreatorType) as IObjectCreator;
            if (creator == null)
                throw new InvalidOperationException(string.Format("创建器{0}未实现{1}接口", objectType, item.CreatorType, typeof(IObjectCreator)));

            return creator;
        }

        // 从Attribute标记中寻找创建器
        private static IObjectCreator _GetCreatorFromAttribute(Type objectType)
        {
            ObjectCreatorAttributeBase[] attrs = (ObjectCreatorAttributeBase[])objectType.GetCustomAttributes(typeof(ObjectCreatorAttributeBase), true);

            if (attrs.Length == 0)
                return null;

            try
            {
                return attrs[0].CreateCreator();
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("创建类型{0}创建器时出现错误", objectType), ex);
            }
        }

        // 默认的创建器
        private static IObjectCreator _GetDefaultCreator(Type objectType)
        {
            if (!objectType.CanCreateDirect())
                return null;

            return (IObjectCreator)Activator.CreateInstance(typeof(DefaultObjectCreator<>).MakeGenericType(objectType));
        }

        /// <summary>
        /// 根据指定的类型创建对象
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="context">上下文参数</param>
        /// <returns></returns>
        public static object CreateObject(Type objectType, object context = null)
        {
            Contract.Requires(objectType != null);

            CreatorItem item = _Creators.GetOrSet(objectType, _ObjectCreator);
            if (item.DefaultCreator == null)
                throw new InvalidOperationException("未找到类型" + objectType + "的创建器");

            return item.DefaultCreator.CreateObject(context, item.InnerCreator);
        }

        /// <summary>
        /// 根据指定的类型创建对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="context">上下文参数</param>
        /// <returns></returns>
        public static T CreateObject<T>(object context = null)
            where T : class
        {
            const string CONNOT_CREATE_OBJECT_ERROR_FORMAT = "未能创建类型{0}的实例";

            try
            {
                object obj = CreateObject(typeof(T));
                if (obj is T)
                    return (T)obj;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format(CONNOT_CREATE_OBJECT_ERROR_FORMAT, typeof(T)), ex);
            }

            throw new InvalidOperationException(string.Format(CONNOT_CREATE_OBJECT_ERROR_FORMAT, typeof(T)));
        }

        /// <summary>
        /// 获取指定类型的创建器
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns></returns>
        public static IObjectCreator GetCreator(Type objectType)
        {
            Contract.Requires(objectType != null);

            return _Creators.GetOrSet(objectType, _ObjectCreator).DefaultCreator;
        }

        /// <summary>
        /// 指定的类型是否具有创建器
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public static bool HasCreator(Type objectType)
        {
            return GetCreator(objectType) != null;
        }

        #region Class DefaultObjectCreator ...

        internal class DefaultObjectCreator<T> : IObjectCreator
            where T : new()
        {
            #region IObjectCreator Members

            public object CreateObject(object context, IObjectCreator innerCreator)
            {
                return new T();
            }

            #endregion IObjectCreator Members
        }

        #endregion Class DefaultObjectCreator ...
    }
}