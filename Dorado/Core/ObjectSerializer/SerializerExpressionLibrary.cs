using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Dorado.Core.ObjectSerializer
{
    internal static class SerializerExpressionLibrary
    {
        [SerializerExpressionBuilder(typeof(Int32))]
        public static Expression Int32Serilizer(FieldInfo fInfo, ExpressionCreateContext context)
        {
            return null;
        }

        [DeserializerExpressionBuilder(typeof(Int32))]
        public static Expression Int32Deserilizer(FieldInfo fInfo, ExpressionCreateContext context)
        {
            return null;
        }
    }
}