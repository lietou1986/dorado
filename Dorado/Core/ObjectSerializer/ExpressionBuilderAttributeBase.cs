using System;

namespace Dorado.Core.ObjectSerializer
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    internal class ExpressionBuilderAttributeBase : Attribute
    {
        public ExpressionBuilderAttributeBase(Type fieldType)
        {
            FieldType = fieldType;
        }

        public Type FieldType { get; private set; }
    }

    /// <summary>
    /// 用于标注序列化方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    internal class SerializerExpressionBuilderAttribute : ExpressionBuilderAttributeBase
    {
        public SerializerExpressionBuilderAttribute(Type fieldType)
            : base(fieldType)
        {
        }
    }

    /// <summary>
    /// 用于标注反序列化方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    internal class DeserializerExpressionBuilderAttribute : ExpressionBuilderAttributeBase
    {
        public DeserializerExpressionBuilderAttribute(Type fieldType)
            : base(fieldType)
        {
        }
    }
}