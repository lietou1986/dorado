using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Dorado.Core.ObjectSerializer
{
    /// <summary>
    /// 对象序列化/反序列化器
    /// </summary>
    internal class BinaryObjectSerializer : IObjectSerializer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        public BinaryObjectSerializer(Type type)
        {
            _Type = type;
        }

        private readonly Type _Type;
        private int _VerifyCode;
        private Action<object, Stream, int> _Serilizer;
        private Func<Stream, int, object> _Deserilizer;

        private void _EnsureInitialize()
        {
            if (_Serilizer != null)
                return;

            lock (this)
            {
                if (_Serilizer == null)
                {
                    _Initialize();
                }
            }
        }

        private void _Initialize()
        {
            int verifyCode = 0;
            List<Expression> serExps = new List<Expression>(), deserExps = new List<Expression>();

            // 参数的定义
            ParameterExpression param_stream = Expression.Parameter(typeof(Stream), "stream"),  // Stream stream
                param_obj = Expression.Parameter(typeof(object), "obj"),  // object obj
                param_verifyCode = Expression.Parameter(typeof(int), "verifyCode"),  // int verifyCode
                val_buffer = Expression.Variable(typeof(byte*), "buffer"),  // buffer
                val_bufferIndex = Expression.Variable(typeof(int), "bufferIndex"), // int bufferIndex
                val_bufferLength = Expression.Variable(typeof(int), "bufferLength");  // int bufferLength
            var ctx = new ExpressionCreateContext()
            {
                Param_obj = param_obj,
                Param_stream = param_stream,
                Param_verifyCode = param_verifyCode,
                Val_Buffer = val_buffer,
                Val_BufferIndex = val_bufferIndex,
                Val_BufferLength = val_bufferLength,
            };

            int order = 0;
            foreach (FieldInfo fInfo in _Type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Cast<FieldInfo>().OrderBy(f => f.Name))
            {
                verifyCode ^= fInfo.Name.GetHashCode() ^ fInfo.FieldType.FullName.GetHashCode() ^ order++;

                serExps.Add(SerializerExpressionManager.CreateSerializeExpression(fInfo, ctx));
                deserExps.Add(SerializerExpressionManager.CreateDeserializeExpression(fInfo, ctx));
            }

            _VerifyCode = _Type.FullName.GetHashCode() ^ verifyCode;

            _Serilizer = Expression.Lambda<Action<object, Stream, int>>(Expression.Block(serExps)).Compile();
            _Deserilizer = Expression.Lambda<Func<Stream, int, object>>(Expression.Block(deserExps)).Compile();
        }

        #region IObjectSerializer Members

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="inputStream"></param>
        public void Serialize(object obj, Stream inputStream)
        {
            if (obj.GetType() != _Type)
                throw new ArgumentException("指定的对象obj不是类型" + _Type);

            _EnsureInitialize();
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="outputStream"></param>
        /// <returns></returns>
        public object Deserialize(Stream outputStream)
        {
            _EnsureInitialize();
            return null;
        }

        #endregion IObjectSerializer Members
    }
}