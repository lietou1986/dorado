using Dorado.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ExpressionBuilderFunc = System.Func<System.Reflection.FieldInfo, Dorado.Core.ObjectSerializer.ExpressionCreateContext, System.Linq.Expressions.Expression>;

namespace Dorado.Core.ObjectSerializer
{
    internal static class SerializerExpressionManager
    {
        static SerializerExpressionManager()
        {
            _LoadExpressionBuilders();
        }

        /// <summary>
        /// 创建字段序列化表达式
        /// </summary>
        /// <param name="fInfo"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Expression CreateSerializeExpression(FieldInfo fInfo, ExpressionCreateContext context)
        {
            ISerializerExpressionBuilder builder = _Builders.GetOrDefault(fInfo.FieldType);
            if (builder == null)
                throw new NotSupportedException(string.Format("不支持类型{0}的序列化", fInfo.FieldType));

            return builder.CreateSerializeExpression(fInfo, context);
        }

        /// <summary>
        /// 创建字段的反序列化表达式
        /// </summary>
        /// <param name="fInfo"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Expression CreateDeserializeExpression(FieldInfo fInfo, ExpressionCreateContext context)
        {
            ISerializerExpressionBuilder builder = _Builders.GetOrDefault(fInfo.FieldType);
            if (builder == null)
                throw new NotSupportedException(string.Format("不支持类型{0}的反序列化", fInfo.FieldType));

            return builder.CreateDeserializeExpression(fInfo, context);
        }

        private static readonly Dictionary<Type, ISerializerExpressionBuilder> _Builders = new Dictionary<Type, ISerializerExpressionBuilder>();

        private static void _LoadExpressionBuilders()
        {
            var result = from item in
                             (
                                 from mInfo in typeof(SerializerExpressionLibrary).GetMethods(BindingFlags.Static | BindingFlags.Public)
                                 where mInfo.IsDefined(typeof(ExpressionBuilderAttributeBase), true)
                                 let attr = mInfo.GetAttribute<ExpressionBuilderAttributeBase>()
                                 group new { Attr = attr, MInfo = mInfo } by attr.FieldType)
                         let fieldType = item.Key
                         let mInfos = item.Select(v => v.MInfo)
                         let serMInfo = item.FirstOrDefault(v => v.Attr is SerializerExpressionBuilderAttribute).Get(v => v.MInfo)
                         let deserMInfo = item.FirstOrDefault(v => v.Attr is DeserializerExpressionBuilderAttribute).Get(v => v.MInfo)
                         where serMInfo != null || deserMInfo != null
                         select new { FieldType = fieldType, Builder = new SerializerExpressionBuilderAdapter(serMInfo, deserMInfo) };

            _Builders.AddRange(result, v => v.FieldType, v => v.Builder);
        }

        private static TProperty Get<T, TProperty>(this T item, Func<T, TProperty> selector)
            where T : class
            where TProperty : class
        {
            if (item == null)
                return null;

            return selector(item);
        }

        #region SerializerExpressionBuilderAdapter ...

        private class SerializerExpressionBuilderAdapter : ISerializerExpressionBuilder
        {
            public SerializerExpressionBuilderAdapter(MethodInfo serMInfo, MethodInfo deserMInfo)
            {
                if (serMInfo != null)
                    _SerBuilder = (ExpressionBuilderFunc)Delegate.CreateDelegate(typeof(ExpressionBuilderFunc), null, serMInfo);

                if (deserMInfo != null)
                    _DeserBuilder = (ExpressionBuilderFunc)Delegate.CreateDelegate(typeof(ExpressionBuilderFunc), null, deserMInfo);
            }

            private readonly ExpressionBuilderFunc _SerBuilder;
            private readonly ExpressionBuilderFunc _DeserBuilder;

            #region ISerializerExpressionBuilder Members

            public Expression CreateSerializeExpression(FieldInfo fInfo, ExpressionCreateContext context)
            {
                if (_SerBuilder == null)
                    throw new NotSupportedException(string.Format("不支持类型{0}的序列化", fInfo.FieldType));

                return _SerBuilder(fInfo, context);
            }

            public Expression CreateDeserializeExpression(FieldInfo fInfo, ExpressionCreateContext context)
            {
                if (_DeserBuilder == null)
                    throw new NotSupportedException(string.Format("不支持类型{0}的反序列化", fInfo.FieldType));

                return _DeserBuilder(fInfo, context);
            }

            #endregion ISerializerExpressionBuilder Members
        }

        #endregion SerializerExpressionBuilderAdapter ...
    }

    /// <summary>
    /// 对象序列化/反序列化表达式的上下文参数
    /// </summary>
    internal class ExpressionCreateContext
    {
        public ParameterExpression Param_stream { get; set; }

        public ParameterExpression Param_obj { get; set; }

        public ParameterExpression Param_verifyCode { get; set; }

        public ParameterExpression Val_Buffer { get; set; }

        public ParameterExpression Val_BufferIndex { get; set; }

        public ParameterExpression Val_BufferLength { get; set; }
    }
}