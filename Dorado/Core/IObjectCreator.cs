using System;
using System.Diagnostics.Contracts;

namespace Dorado.Core
{
    /// <summary>
    /// 对象创建器
    /// </summary>
    public interface IObjectCreator
    {
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="context">上下文参数</param>
        /// <param name="innerCreator">内置的创建器，用于切面编程时拦截内置对象的一些行为</param>
        /// <returns></returns>
        object CreateObject(object context = null, IObjectCreator innerCreator = null);
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public abstract class ObjectCreatorAttributeBase : Attribute
    {
        /// <summary>
        /// 创建对象创建器
        /// </summary>
        /// <returns></returns>
        public abstract IObjectCreator CreateCreator();
    }

    /// <summary>
    /// 用于标注对象的创建器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class ObjectCreatorAttribute : ObjectCreatorAttributeBase
    {
        public ObjectCreatorAttribute(Type creatorType)
        {
            Contract.Requires(creatorType != null);
            CreatorType = creatorType;
        }

        public override IObjectCreator CreateCreator()
        {
            IObjectCreator creator = Activator.CreateInstance(CreatorType) as IObjectCreator;
            if (creator == null)
                throw new InvalidOperationException(string.Format("创建器{0}未实现{1}接口", CreatorType, typeof(IObjectCreator)));

            return creator;
        }

        /// <summary>
        /// 对象创建器的类型
        /// </summary>
        public Type CreatorType { get; private set; }
    }

    /// <summary>
    /// 为接口指定一个默认的实现
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DefaultImplAttribute : ObjectCreatorAttributeBase
    {
        public DefaultImplAttribute(Type implType)
        {
            Contract.Requires(implType != null);

            ImplType = implType;
        }

        /// <summary>
        /// 接口实现的类型
        /// </summary>
        public Type ImplType { get; private set; }

        public override IObjectCreator CreateCreator()
        {
            return (IObjectCreator)Activator.CreateInstance(typeof(ObjectFactory.DefaultObjectCreator<>).MakeGenericType(ImplType));
        }
    }
}