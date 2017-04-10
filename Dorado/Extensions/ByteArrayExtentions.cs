using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Dorado.Extensions
{
    /// <summary>
    /// <see cref="T:byte[]" />的扩展方法。
    /// </summary>
    public static class ByteArrayExtentions
    {
        /// <summary>
        /// 压缩指定的数据。
        /// </summary>
        /// <param name="input">指定的数据。</param>
        /// <exception cref="T:System.ArgumentNullException">input参数为null时引发。</exception>
        /// <returns>压缩过后的字节数组。</returns>
        public static byte[] Compress(this byte[] input)
        {
            byte[] array;
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            if (input.Length == 0)
            {
                return input;
            }
            using (MemoryStream memoryStream = new MemoryStream())
            {
                DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, true);
                deflateStream.Write(input, 0, input.Length);
                deflateStream.Close();
                memoryStream.Position = 0;
                array = memoryStream.ToArray();
            }
            return array;
        }

        /// <summary>
        /// 解压指定的字节数组。
        /// </summary>
        /// <param name="input">需要解压缩的数据。</param>
        /// <exception cref="T:System.ArgumentNullException">input参数为null时引发。</exception>
        /// <returns>压缩前的数据。</returns>
        public static byte[] Decompress(this byte[] input)
        {
            byte[] array;
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            if (input.Length == 0)
            {
                return input;
            }
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (DeflateStream deflateStream = new DeflateStream(new MemoryStream(input), CompressionMode.Decompress, true))
                {
                    deflateStream.CopyTo(memoryStream);
                    array = memoryStream.ToArray();
                }
            }
            return array;
        }

        /// <summary>
        /// 将指定的字节数组解压为字符串。
        /// </summary>
        /// <param name="input">需要解压的字节数组。</param>
        /// <returns>原始的字符串。</returns>
        public static string DecompressToString(this byte[] input)
        {
            return input.DecompressToString(Encoding.Unicode);
        }

        /// <summary>
        /// 将指定的字节数组解压为字符串。
        /// </summary>
        /// <param name="input">需要解压的字节数组。</param>
        /// <param name="encoding">原始字符串的编码。</param>
        /// <returns>原始的字符串。</returns>
        public static string DecompressToString(this byte[] input, Encoding encoding)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            byte[] numArray = input.Decompress();
            return (encoding ?? Encoding.Unicode).GetString(numArray);
        }

        /// <summary>
        /// 将指定的字介数组，转化成可以阅读的字符串格式。
        /// </summary>
        /// <param name="input">需要转换的字节数组。</param>
        /// <param name="lowercase">结果是否小写，默认为true。</param>
        /// <returns>转化后的字符串格式。</returns>
        public static string ToString(this byte[] input, bool lowercase = true)
        {
            if (input == null || input.Length == 0)
            {
                return string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder(input.Length * 2);
            for (int i = 0; i < input.Length; i++)
            {
                stringBuilder.AppendFormat((lowercase ? "{0:x2}" : "{0:X2}"), input[i]);
            }
            return stringBuilder.ToString();
        }
    }
}