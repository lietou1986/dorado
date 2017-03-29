using System.IO;

namespace Dorado.Core
{
    /// <summary>
    /// 对象的序列化与反序列化策略
    /// </summary>
    public interface IObjectSerializer
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="inputStream">输入流</param>
        void Serialize(object obj, Stream inputStream);

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="outputStream">输出流</param>
        /// <returns></returns>
        object Deserialize(Stream outputStream);
    }
}