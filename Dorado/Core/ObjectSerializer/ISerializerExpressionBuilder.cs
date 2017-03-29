using System.Linq.Expressions;
using System.Reflection;

namespace Dorado.Core.ObjectSerializer
{
    /// <summary>
    /// 字段序列化/反序列化表达式创建器
    /// </summary>
    internal interface ISerializerExpressionBuilder
    {
        /// <summary>
        /// 创建字段的序列化表达式
        /// </summary>
        /// <param name="fInfo"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Expression CreateSerializeExpression(FieldInfo fInfo, ExpressionCreateContext context);

        /// <summary>
        /// 创建字段的反序列化表达式
        /// </summary>
        /// <param name="fInfo"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Expression CreateDeserializeExpression(FieldInfo fInfo, ExpressionCreateContext context);
    }
}