using Dorado.Core.ObjectSerializer;
using Dorado.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace Dorado.Core
{
    /// <summary>
    /// 对象序列化/反序列化的管理器
    /// </summary>
    public static class ObjectSerializerManager
    {
        static ObjectSerializerManager()
        {
            int count = Enum.GetValues(typeof(SerializerType)).Cast<int>().Max() + 1;
            _Dicts = new Dictionary<Type, IObjectSerializer>[count];
            for (int k = 0; k < _Dicts.Length; k++)
            {
                _Dicts[k] = new Dictionary<Type, IObjectSerializer>();
            }
        }

        private static readonly Dictionary<Type, IObjectSerializer>[] _Dicts;
        private static readonly Func<Type, SerializerType, IObjectSerializer> _SerializerCreator = _CreateSerializer;

        /// <summary>
        /// 创建对象的序列化/反序列化器
        /// </summary>
        /// <param name="type"></param>
        /// <param name="serType"></param>
        /// <returns></returns>
        private static IObjectSerializer _CreateSerializer(Type type, SerializerType serType)
        {
            switch (serType)
            {
                case SerializerType.Binary:
                    return new BinaryObjectSerializer(type);
            }

            throw new NotSupportedException(string.Format("序列化类型{0}不被支持", serType));
        }

        /// <summary>
        /// 获取指定类型的序列化器
        /// </summary>
        /// <param name="type"></param>
        /// <param name="serType"></param>
        /// <returns></returns>
        public static IObjectSerializer GetSerializer(Type type, SerializerType serType)
        {
            Contract.Requires(type != null);

            if (!CanSerialize(type, serType))
                throw new ArgumentException(string.Format("对象{0}不支持序列化", type));

            return _Dicts[(int)serType].GetOrSet(type, serType, _SerializerCreator);
        }

        /// <summary>
        /// 判断指定的类型是否支持序列化/反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="serType"></param>
        /// <returns></returns>
        public static bool CanSerialize(Type type, SerializerType serType)
        {
            Contract.Requires(type != null);

            return type.IsValueType || type.IsClass && !type.IsAbstract;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="serType">序列化类型</param>
        /// <param name="outputStream">输出流</param>
        public static void Serialize(object obj, SerializerType serType, Stream outputStream)
        {
            Contract.Requires(obj != null || outputStream != null);

            GetSerializer(obj.GetType(), serType).Serialize(obj, outputStream);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="serType">序列化类型</param>
        /// <param name="inputStream">输入流</param>
        /// <returns></returns>
        public static object Deserialize(Type type, SerializerType serType, Stream inputStream)
        {
            Contract.Requires(type != null && inputStream != null);

            return GetSerializer(type, serType).Deserialize(inputStream);
        }
    }

    /// <summary>
    /// 序列化类型
    /// </summary>
    public enum SerializerType
    {
        /// <summary>
        /// 二进制
        /// </summary>
        Binary = 0,

        /// <summary>
        /// XML
        /// </summary>
        Xml = 1,

        /// <summary>
        /// Json
        /// </summary>
        Json = 2,
    }
}