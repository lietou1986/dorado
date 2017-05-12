/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com
 * 创建时间： 2011/7/5 11:36:09
 * 版本号：v1.0
 * 本类主要用途描述：域名实体
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.Data;
using Dorado.VWS.Model.Enum;

#endregion using

namespace Dorado.VWS.Model
{
    /// <summary>
    ///     域名实体
    /// </summary>
    [Serializable]
    public class DomainEntity : EntityBase<DomainEntity>, IConvertToEntity<DomainEntity>
    {
        /// <summary>
        ///     域名Id
        /// </summary>
        public int DomainId { get; set; }

        /// <summary>
        ///     域名名称
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        ///     域名名称
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        ///     IdcId
        /// </summary>
        public int IdcId { get; set; }

        /// <summary>
        ///     Idc名称
        /// </summary>
        public string IdcName { get; set; }

        /// <summary>
        ///     部署windows服务名称
        /// </summary>
        public string WinServiceName { get; set; }

        /// <summary>
        ///     IIS配置的网站名称
        /// </summary>
        public string IISSiteName { get; set; }

        /// <summary>
        ///     缓冲url地址
        /// </summary>
        public string CacheUrl { get; set; }

        /// <summary>
        ///     html文件是否压缩（true代表压缩，false代表不压缩）
        /// </summary>
        public bool HtmlCompress { get; set; }

        /// <summary>
        ///     jsCss文件是否压缩（true代表压缩，false代表不压缩）
        /// </summary>
        public bool JsCssCompress { get; set; }

        /// <summary>
        ///     域名下服务器列表
        /// </summary>
        public IList<ServerEntity> ServerList { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     最后一次修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        ///     同步类型（1.普通同步【有版本】，2.简单同步【无版本】）
        /// </summary>
        public int SyncType { get; set; }

        /// <summary>
        ///     域名是否可用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        ///     域名服务器类型
        /// </summary>
        public EnumDomainType DomainType { get; set; }

        /// <summary>
        ///     服务类型
        /// </summary>
        public EnumOperatePathType OperatePathType { get; set; }

        /// <summary>
        /// 操作路径
        /// </summary>
        public string OperatePath { get; set; }

        #region IConvertToEntity<DomainEntity> Members

        /// <summary>
        ///     将指定的DataRow转换成指定实体
        /// </summary>
        /// <param name = "row">行数据</param>
        /// <returns>实体信息</returns>
        public DomainEntity ConvertToEntity(DataRow row)
        {
            if (row == null)
            {
                return null;
            }
            var domainEntity = new DomainEntity
                                   {
                                       DomainId = Convert.ToInt32(row["Id"]),
                                       Environment = row["Environment"].ToString(),
                                       DomainName = row["DomainName"].ToString(),
                                       IdcId = Convert.ToInt32(row["IdcId"]),
                                       IdcName = row["IdcName"].ToString(),
                                       WinServiceName = row["WinServiceName"].ToString(),
                                       IISSiteName = row["IISSiteName"].ToString(),
                                       CacheUrl = row["CacheUrl"].ToString(),
                                       HtmlCompress = Convert.ToBoolean(row["HtmlCompress"]),
                                       JsCssCompress = Convert.ToBoolean(row["JsCssCompress"]),
                                       CreateTime = Convert.ToDateTime(row["CreateTime"]),
                                       SyncType = Convert.ToInt32(row["SyncType"]),
                                       Enable = Convert.ToBoolean(row["Enable"]),
                                   };
            try
            {
                if (row["DomainType"] != null)
                {
                    domainEntity.DomainType = (EnumDomainType)ConvertToInt(row["DomainType"].ToString());
                }
                if (row["OperatePathType"] != null)
                {
                    domainEntity.OperatePathType = (EnumOperatePathType)ConvertToInt(row["OperatePathType"].ToString());
                }
                if (row["OperatePath"] != null)
                {
                    domainEntity.OperatePath = row["OperatePath"].ToString();
                }
            }
            catch (Exception)
            {
                //Logger.Log("TaskEntity", LogLevel.Info, "HeartBeat Start!");
                domainEntity.DomainType = 0;
                domainEntity.OperatePathType = 0;
                domainEntity.OperatePath = "";
                return domainEntity;
            }
            return domainEntity;
        }

        #endregion IConvertToEntity<DomainEntity> Members
    }
}