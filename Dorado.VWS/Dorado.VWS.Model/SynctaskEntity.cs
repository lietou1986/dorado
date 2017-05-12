/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com
 * 创建时间： 2011/7/6 17:48:40
 * 版本号：v1.0
 * 本类主要用途描述：同步任务实体
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
    ///     同步任务实体
    /// </summary>
    [Serializable]
    public class SynctaskEntity : EntityBase<SynctaskEntity>, IConvertToEntity<SynctaskEntity>
    {
        /// <summary>
        ///     同步任务Id
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        ///     操作类型
        /// </summary>
        public EnumOperateType OperateType { get; set; }

        /// <summary>
        ///     域名Id
        /// </summary>
        public int DomainId { get; set; }

        /// <summary>
        ///     域名名称
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        ///     同步需要增加或修改文件集合
        /// </summary>
        public string AddFiles { get; set; }

        /// <summary>
        ///     同步需要删除文件集合
        /// </summary>
        public string DelFiles { get; set; }

        /// <summary>
        ///     同步操作者用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     操作者Ip
        /// </summary>
        public string OperatorIp { get; set; }

        /// <summary>
        ///     调用类型
        /// </summary>
        public EnumCallType CallType { get; set; }

        /// <summary>
        ///     记录创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     记录修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        ///     日志信息
        /// </summary>
        public string LogInfo { get; set; }

        /// <summary>
        ///     日志信息
        /// </summary>
        public string AddLog { get; set; }

        /// <summary>
        ///     同步状态
        /// </summary>
        public EnumSyncStatus SyncStatus { get; set; }

        /// <summary>
        ///     同步任务描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     同步子任务列表
        /// </summary>
        public IList<SynctaskSubEntity> SyncTaskSubList { get; set; }

        #region 当前Server共有域名信息

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

        #endregion 当前Server共有域名信息

        #region IConvertToEntity<SynctaskEntity> Members

        /// <summary>
        ///     将指定的DataRow转换成指定实体
        /// </summary>
        /// <param name = "row">行数据</param>
        /// <returns>实体信息</returns>
        public SynctaskEntity ConvertToEntity(DataRow row)
        {
            if (row != null)
            {
                var synctaskEntity = new SynctaskEntity
                                         {
                                             TaskId = Convert.ToInt32(row["TaskId"]),
                                             OperateType =
                                                 (EnumOperateType)Convert.ToInt32(row["OperateType"]),
                                             DomainId = Convert.ToInt32(row["DomainId"]),
                                             DomainName = row["DomainName"].ToString(),
                                             UserName = row["UserName"].ToString(),
                                             OperatorIp = row["OperatorIp"].ToString(),
                                             CallType = (EnumCallType)Convert.ToInt32(row["CallType"]),
                                             CreateTime = Convert.ToDateTime(row["CreateTime"]),
                                             SyncStatus = (EnumSyncStatus)Convert.ToInt32(row["SyncStatus"].ToString()),
                                             DomainType = (EnumDomainType)ConvertToInt(row["DomainType"].ToString()),
                                             OperatePathType = (EnumOperatePathType)ConvertToInt(row["OperatePathType"].ToString()),
                                             OperatePath = row["OperatePath"].ToString(),
                                         };

                if (row["AddFiles"] != DBNull.Value)
                {
                    synctaskEntity.AddFiles = row["AddFiles"].ToString();
                }
                if (row["DelFiles"] != DBNull.Value)
                {
                    synctaskEntity.DelFiles = row["DelFiles"].ToString();
                }

                if (row["UpdateTime"] != DBNull.Value)
                {
                    synctaskEntity.UpdateTime = Convert.ToDateTime(row["UpdateTime"]);
                }
                if (row["LogInfo"] != DBNull.Value)
                {
                    synctaskEntity.LogInfo = row["LogInfo"].ToString();
                }
                if (row["Description"] != DBNull.Value)
                {
                    synctaskEntity.Description = row["Description"].ToString();
                }
                return synctaskEntity;
            }
            return null;
        }

        #endregion IConvertToEntity<SynctaskEntity> Members
    }
}