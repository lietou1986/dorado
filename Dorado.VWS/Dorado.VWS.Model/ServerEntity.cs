/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com
 * 创建时间： 2011/7/5 13:50:11
 * 版本号：v1.0
 * 本类主要用途描述：服务器实体
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Data;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Utils;

#endregion using

namespace Dorado.VWS.Model
{
    /// <summary>
    ///     服务器实体
    /// </summary>
    [Serializable]
    public class ServerEntity : EntityBase<ServerEntity>, IConvertToEntity<ServerEntity>
    {
        /// <summary>
        ///     服务器Id
        /// </summary>
        public int ServerId { get; set; }

        /// <summary>
        ///     IdcId
        /// </summary>
        public int IdcId { get; set; }

        /// <summary>
        ///     idc名称
        /// </summary>
        public string IdcName { get; set; }

        /// <summary>
        ///     域名Id
        /// </summary>
        public int DomainId { get; set; }

        /// <summary>
        ///     域名名称
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        ///     服务器类型
        /// </summary>
        public EnumServerType ServerType { get; set; }

        /// <summary>
        ///     服务器类型名称
        /// </summary>
        public string ServerTypeName
        {
            get { return ServerType.GetDescription(); }
        }

        /// <summary>
        ///     服务器IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        ///     根目录
        /// </summary>
        public string Root { get; set; }

        /// <summary>
        ///     服务器IIS状态
        /// </summary>
        public EnumIISStatus IISStatus { get; set; }

        /// <summary>
        ///     记录创建人
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        ///     创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        ///     记录修改人
        /// </summary>
        public string Updator { get; set; }

        /// <summary>
        ///     最后一次修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        ///     客户端版本
        /// </summary>
        public string ClientVersion { get; set; }

        /// <summary>
        ///     机器名
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// 服务器状态
        /// </summary>
        public bool ServerStatus { get; set; }

        /// <summary>
        ///     最后心跳时间
        /// </summary>
        public DateTime? LastHeartBeatDate { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool DeleteFlag { get; set; }

        /// <summary>
        /// 是否是预上线
        /// </summary>
        public bool IsAdvanced { get; set; }

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

        #region IConvertToEntity<ServerEntity> Members

        public ServerEntity ConvertToEntity(DataRow row)
        {
            if (row != null)
            {
                var serverEntity = new ServerEntity();

                if (row["Id"] != DBNull.Value)
                {
                    serverEntity.ServerId = Convert.ToInt32(row["Id"]);
                }
                if (row["IdcId"] != DBNull.Value)
                {
                    serverEntity.IdcId = Convert.ToInt32(row["IdcId"]);
                }
                if (row["IdcName"] != DBNull.Value)
                {
                    serverEntity.IdcName = row["IdcName"].ToString();
                }
                if (row["DomainId"] != DBNull.Value)
                {
                    serverEntity.DomainId = Convert.ToInt32(row["DomainId"]);
                }
                if (row["Environment"] != DBNull.Value)
                {
                    serverEntity.Environment = row["Environment"].ToString();
                }
                if (row["DomainName"] != DBNull.Value)
                {
                    serverEntity.DomainName = row["DomainName"].ToString();
                }
                if (row["Type"] != DBNull.Value)
                {
                    serverEntity.ServerType = (EnumServerType)Convert.ToInt32(row["Type"]);
                }
                if (row["Root"] != DBNull.Value)
                {
                    serverEntity.Root = row["Root"].ToString();
                }
                if (row["IISStatus"] != DBNull.Value)
                {
                    serverEntity.IISStatus = (EnumIISStatus)Convert.ToInt32(row["IISStatus"]);
                }
                if (row["Creator"] != DBNull.Value)
                {
                    serverEntity.Creator = row["Creator"].ToString();
                }
                if (row["CreateTime"] != DBNull.Value)
                {
                    serverEntity.CreateTime = Convert.ToDateTime(row["CreateTime"]);
                }
                if (row["LastHeartBeatDate"] != DBNull.Value)
                {
                    serverEntity.LastHeartBeatDate = Convert.ToDateTime(row["LastHeartBeatDate"]);
                }
                serverEntity.ServerStatus = false;
                if (row["LastHeartBeatDate"] != DBNull.Value)
                {
                    if (Convert.ToDateTime(row["LastHeartBeatDate"]).AddMinutes(15) > DateTime.Now)
                        serverEntity.ServerStatus = true;
                }

                if (row["DeleteFlag"] != DBNull.Value)
                {
                    serverEntity.DeleteFlag = Convert.ToInt32(row["DeleteFlag"]) == 1 ? true : false;
                }
                if (row["IsAdvanced"] != DBNull.Value)
                {
                    serverEntity.IsAdvanced = Convert.ToInt32(row["IsAdvanced"]) == 1 ? true : false;
                }

                if (!string.IsNullOrEmpty(row["IP"].ToString()))
                {
                    serverEntity.IP = row["IP"].ToString();
                }
                if (!string.IsNullOrEmpty(row["Updator"].ToString()))
                {
                    serverEntity.Updator = row["Updator"].ToString();
                }
                if (!string.IsNullOrEmpty(row["UpdateTime"].ToString()))
                {
                    serverEntity.UpdateTime = Convert.ToDateTime(row["UpdateTime"]);
                }

                serverEntity.HostName = row["HostName"].ToString();
                serverEntity.ClientVersion = row["ClientVersion"].ToString();
                try
                {
                    if (row["DomainType"] != null)
                    {
                        serverEntity.DomainType = (EnumDomainType)ConvertToInt(row["DomainType"].ToString());
                    }
                    if (row["OperatePathType"] != null)
                    {
                        serverEntity.OperatePathType = (EnumOperatePathType)ConvertToInt(row["OperatePathType"].ToString());
                    }
                    if (row["OperatePath"] != null)
                    {
                        serverEntity.OperatePath = row["OperatePath"].ToString();
                    }
                }
                catch (Exception)
                {
                    //Logger.Log("TaskEntity", LogLevel.Info, "HeartBeat Start!");
                    serverEntity.DomainType = 0;
                    serverEntity.OperatePathType = 0;
                    serverEntity.OperatePath = "";
                    return serverEntity;
                }
                return serverEntity;
            }
            return null;
        }

        #endregion IConvertToEntity<ServerEntity> Members

        public string Environment { get; set; }
    }
}