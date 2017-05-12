/*-------------------------------------------------------------------------
 * 版权所有：凡客诚品（北京）科技有限公司
 * 版本：v1.0
 * 时间： 2011/7/29 11:23:54
 * 作者：蔡昌艳（Bruce Tscai）
 * 联系方式：caichangyan@vancl.cn
 * 本类主要用途描述：文件实体类
 *  -------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Vancl.IC.VWS.ClientService.Model
{
    /// <summary>
    /// 文件实体类
    /// </summary>
    [DataContract]
    public class VwsDirectoryInfo
    {
        /// <summary>
        /// 文件名
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 文件全路径
        /// </summary>
        [DataMember]
        public string FullName { get; set; }

        /// <summary>
        /// 大小
        /// </summary>
        [DataMember]
        public long Length { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 是否为文件夹
        /// </summary>
        [DataMember]
        public bool IsFolder { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [DataMember]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 重写ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FullName;
        }
    }
}
