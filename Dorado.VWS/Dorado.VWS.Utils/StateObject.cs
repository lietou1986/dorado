/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/4 10:10:11
 * 作者：len
 * 联系方式：len@dorado.com
 * 本类主要用途描述：Socket上下文转换时的状态类
 *  -------------------------------------------------------------------------*/

#region using

using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

#endregion using

namespace Dorado.VWS.Utils
{
    internal class StateObject
    {
        /// <summary>
        ///     Buffer大小
        /// </summary>
        internal const int BufferSize = 8192;

        /// <summary>
        ///     Buffer数据
        /// </summary>
        internal byte[] Buffer = new byte[BufferSize];

        /// <summary>
        ///     总接收数量
        /// </summary>
        internal List<byte> Bytes = new List<byte>(); //memorystream

        /// <summary>
        ///     剩余长度
        /// </summary>
        internal long RemainLength;

        /// <summary>
        ///     Socket对象
        /// </summary>
        internal Socket WorkSocket;

        #region 文件流使用

        internal FileStream FS;
        internal string FileFullPath;
        internal string FileMD5;

        #endregion 文件流使用
    }
}