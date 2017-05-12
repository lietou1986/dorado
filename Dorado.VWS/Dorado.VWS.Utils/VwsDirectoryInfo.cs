/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/4 10:10:11
 * 作者：len
 * 联系方式：len@dorado.com
 * 本类主要用途描述：文件实体类
 *  -------------------------------------------------------------------------*/

#region using

using System;

#endregion using

namespace Dorado.VWS.Utils
{
    /// <summary>
    ///     文件实体类
    /// </summary>
    [Serializable]
    public class VwsDirectoryInfo
    {
        /// <summary>
        ///     文件名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     文件全路径
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        ///     大小
        /// </summary>
        public long Length { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        ///     是否为文件夹
        /// </summary>
        public bool IsFolder { get; set; }

        /// <summary>
        ///     更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        public string MD5 { get; set; }

        /// <summary>
        ///     重写ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FullName;
        }
    }
}