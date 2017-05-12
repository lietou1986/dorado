/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com 
 * 创建时间： 2011/7/5 14:58:49
 * 版本号：v1.0
 * 本类主要用途描述：服务器数据访问操作类
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Dorado.VWS.Model;

#endregion using

namespace Dorado.VWS.Services.Persistence
{
    /// <summary>
    ///     服务器数据访问操作类
    /// </summary>
    public class ServerDao : DBbase<ServerEntity>
    {
        /// <summary>
        ///     获取服务器信息
        /// </summary>
        /// <param name = "serverId">服务器Id</param>
        /// <returns>服务器实体</returns>
        public ServerEntity GetServerById(int serverId)
        {
            const string sql =
                @"SELECT  vwsserver.* ,
                            vwsdomain.Environment ,
                            vwsdomain.DomainName ,
                            vwsdomain.IdcId ,
                            vwsdomain.DomainType,
                            vwsdomain.OperatePathType,
                            vwsdomain.OperatePath,
                            vwsidc.IdcName
                    FROM    zsync_server vwsserver ( NOLOCK )
                            RIGHT JOIN zsync_domain vwsdomain ( NOLOCK ) ON vwsserver.DomainId = vwsdomain.Id
                            RIGHT JOIN zsync_idc vwsidc ( NOLOCK ) ON vwsdomain.IdcId = vwsidc.Id
                    WHERE   vwsserver.Id = @serverId";// and vwsserver.DeleteFlag = 0

            SqlParameter[] parameters = {
                                            new SqlParameter("@serverId", serverId)
                                        };

            return GetEntity(CommandType.Text, sql, parameters);
        }

        public IList<ServerEntity> GetServersByIP(string ip)
        {
            const string sql =
                @"SELECT  vwsserver.* ,
                            vwsdomain.Environment ,
                            vwsdomain.DomainName ,
                            vwsdomain.IdcId ,
                            vwsdomain.DomainType,
                            vwsdomain.OperatePathType,
                            vwsdomain.OperatePath,
                            vwsidc.IdcName
                    FROM    zsync_server vwsserver ( NOLOCK )
                            RIGHT JOIN zsync_domain vwsdomain ( NOLOCK ) ON vwsserver.DomainId = vwsdomain.Id
                            RIGHT JOIN zsync_idc vwsidc ( NOLOCK ) ON vwsdomain.IdcId = vwsidc.Id
                           WHERE vwsserver.IP = @IP and vwsserver.DeleteFlag = 0";

            SqlParameter[] parameters = {
                                            new SqlParameter("@IP", ip)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取服务器列表
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <returns>服务器实体列表</returns>
        public IList<ServerEntity> GetServersByDomainId(int domainId)
        {
            return GetServersByDomainId("", domainId);
        }

        /// <summary>
        ///     获取服务器列表
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <returns>服务器实体列表</returns>
        public IList<ServerEntity> GetServersByDomainId(string userName, int domainId)
        {
            string sql =
                @"SELECT  idc.Id IdcId ,
                            idc.IdcName ,
                            domain.Id DomainId ,
                            domain.Environment ,
                            domain.DomainName ,
                            domain.DomainType,
                            domain.OperatePathType,
                            domain.OperatePath,
                            server.*
                            FROM (SELECT * FROM zsync_idc WHERE DeleteFlag=0) idc
                            RIGHT JOIN (SELECT * FROM zsync_domain WHERE ID = @DomainId and DeleteFlag=0) domain
                            ON idc.Id=domain.IdcId
                            LEFT JOIN (SELECT * FROM zsync_server WHERE DeleteFlag=0) server
                            ON domain.Id=server.DomainId";
            if (!string.IsNullOrEmpty(userName))
            {
                sql += @" inner join(select * from  [zsync_domain_permission] where DomainId = @DomainId and UserName=@UserName and PermissionType=1 and DeleteFlag=0) pms
                            on pms.DomainID=server.DomainId";
            }
            sql += " ORDER BY Environment, type desc,server.IsAdvanced desc,server.HostName";

            SqlParameter[] parameters;
            parameters = new SqlParameter[] {
                                            new SqlParameter("@DomainId", domainId)
                                        };
            if (!string.IsNullOrEmpty(userName))
            {
                parameters = new SqlParameter[] {
                                            new SqlParameter("@DomainId", domainId),
                 new SqlParameter("@UserName", userName)
                                        };
            }
            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取服务器列表
        /// </summary>
        /// <param name = "domain">域名</param>
        /// <returns>服务器实体列表</returns>
        public IList<ServerEntity> GetServersByDomain(string domain)
        {
            const string sql =
                @"SELECT  idc.Id IdcId ,
                            idc.IdcName ,
                            domain.Id DomainId ,
                            domain.Environment ,
                            domain.DomainName ,
                            domain.DomainType,
                            domain.OperatePathType,
                            domain.OperatePath,
                            server.*
                            FROM (SELECT * FROM zsync_idc WHERE DeleteFlag=0) idc
                            RIGHT JOIN (SELECT * FROM zsync_domain WHERE DomainName = @Domain and DeleteFlag=0) domain
                            ON idc.Id=domain.IdcId
                            LEFT JOIN (SELECT * FROM zsync_server WHERE DeleteFlag=0) server
                            ON domain.Id=server.DomainId
                            ORDER BY Environment, type desc,server.IsAdvanced desc,server.HostName";

            SqlParameter[] parameters = {
                                            new SqlParameter("@Domain", domain)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取服务器列表
        /// </summary>
        /// <param name = "idcId">IdcId</param>
        /// <returns>服务器实体列表</returns>
        public IList<ServerEntity> GetServersByIdc(int idcId)
        {
            const string sql =
                @"SELECT  idc.Id IdcId ,
                            idc.IdcName ,
                            domain.Id DomainId ,
                            domain.Environment ,
                            domain.DomainName ,
                            domain.DomainType,
                            domain.OperatePathType,
                            domain.OperatePath,
                            server.*
                            FROM (SELECT * FROM zsync_idc WHERE DeleteFlag=0) idc
                            LEFT JOIN (SELECT * FROM zsync_domain WHERE DeleteFlag=0) domain
                            ON idc.Id=domain.IdcId
                            LEFT JOIN (SELECT * FROM zsync_server WHERE DeleteFlag=0) server
                            ON domain.Id=server.DomainId
                            WHERE idc.Id = @IdcId
                            ORDER BY domain.Environment, domain.DomainName,type desc,server.IsAdvanced desc,server.HostName";

            SqlParameter[] parameters = {
                                            new SqlParameter("@IdcId", idcId)
                                        };
            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取源服务器
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <returns>服务器实体列表</returns>
        public ServerEntity GetSourceServersByDomainId(int domainId)
        {
            const string sql =
                @"SELECT  vwsserver.* ,
                            vwsdomain.Environment ,
                            vwsdomain.DomainName ,
                            vwsdomain.IdcId ,
                            vwsdomain.DomainType,
                            vwsdomain.OperatePathType,
                            vwsdomain.OperatePath,
                            vwsidc.IdcName FROM zsync_server vwsserver(NOLOCK)
                           right join zsync_domain vwsdomain(NOLOCK) on vwsserver.DomainId = vwsdomain.Id
                           right join zsync_idc vwsidc(NOLOCK) on vwsdomain.IdcId = vwsidc.Id WHERE vwsserver.DeleteFlag = 0
                           and vwsserver.DomainId = @DomainId and Type=3";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId)
                                        };

            return GetEntity(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取源服务器
        /// </summary>
        /// <param name = "ip"></param>
        /// <param name = "root"></param>
        /// <returns>服务器实体列表</returns>
        public ServerEntity GetSourceServersByIpAndRoot(string ip, string root)
        {
            const string sql =
                @"SELECT  vwsserver.* ,
                            vwsdomain.Environment ,
                            vwsdomain.DomainName ,
                            vwsdomain.IdcId ,
                            vwsdomain.DomainType,
                            vwsdomain.OperatePathType,
                            vwsdomain.OperatePath,
                            vwsidc.IdcName FROM zsync_server vwsserver(NOLOCK)
                           right join zsync_domain vwsdomain(NOLOCK) on vwsserver.DomainId = vwsdomain.Id
                           right join zsync_idc vwsidc(NOLOCK) on vwsdomain.IdcId = vwsidc.Id WHERE vwsserver.DeleteFlag = 0
                           and vwsserver.IP = @IP and vwsserver.Root = @Root and Type=3";

            SqlParameter[] parameters = {
                                            new SqlParameter("@IP", ip),
                                            new SqlParameter("@Root", root)
                                        };

            return GetEntity(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取目标服务器
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <returns>服务器实体列表</returns>
        public IList<ServerEntity> GetTargetServersByDomainId(int domainId)
        {
            const string sql =
                @"SELECT  vwsserver.* ,
                            vwsdomain.Environment ,
                            vwsdomain.DomainName ,
                            vwsdomain.IdcId ,
                            vwsdomain.DomainType,
                            vwsdomain.OperatePathType,
                            vwsdomain.OperatePath,
                            vwsidc.IdcName FROM zsync_server vwsserver(NOLOCK)
                           right join zsync_domain vwsdomain(NOLOCK) on vwsserver.DomainId = vwsdomain.Id
                           right join zsync_idc vwsidc(NOLOCK) on vwsdomain.IdcId = vwsidc.Id WHERE vwsserver.DeleteFlag = 0
                           and vwsserver.DomainId = @DomainId and Type=1  order by vwsserver.IsAdvanced desc,vwsserver.HostName";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取目标服务器
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <param name = "ip"></param>
        /// <returns>服务器实体</returns>
        public ServerEntity GetTargetServersByDomainIdAndIP(int domainId, string ip)
        {
            const string sql =
                @"SELECT  vwsserver.* ,
                            vwsdomain.Environment ,
                            vwsdomain.DomainName ,
                            vwsdomain.IdcId ,
                            vwsdomain.DomainType,
                            vwsdomain.OperatePathType,
                            vwsdomain.OperatePath,
                            vwsidc.IdcName FROM zsync_server vwsserver(NOLOCK)
                           right join zsync_domain vwsdomain(NOLOCK) on vwsserver.DomainId = vwsdomain.Id
                           right join zsync_idc vwsidc(NOLOCK) on vwsdomain.IdcId = vwsidc.Id WHERE vwsserver.DeleteFlag = 0
                           and vwsserver.DomainId = @DomainId and vwsserver.IP=@IP and Type=1";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainId),
                                            new SqlParameter("@IP", ip)
                                        };

            return GetEntity(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取所有服务器列表
        /// </summary>
        /// <returns>服务器实体列表</returns>
        public IList<ServerEntity> GetAllServers()
        {
            const string sql =
                @"SELECT  idc.Id IdcId ,
                            idc.IdcName ,
                            domain.Id DomainId ,
                            domain.Environment ,
                            domain.DomainName ,
                            domain.DomainType,
                            domain.OperatePathType,
                            domain.OperatePath,
                            server.*
                            FROM (SELECT * FROM zsync_idc WHERE DeleteFlag=0) idc
                            RIGHT JOIN (SELECT * FROM zsync_domain WHERE DeleteFlag=0) domain
                            ON idc.Id=domain.IdcId
                            LEFT JOIN (SELECT * FROM zsync_server WHERE DeleteFlag=0) server
                            ON domain.Id=server.DomainId
                            ORDER BY domain.Environment, domain.DomainName,type desc,server.IsAdvanced desc,server.HostName";

            return GetEntities(CommandType.Text, sql);
        }

        /// <summary>
        ///     获取所有服务器列表
        /// </summary>
        /// <param name = "beginIndex">开始记录数</param>
        /// <param name = "endIndex">结尾记录数</param>
        /// <returns>服务器实体列表</returns>
        public IList<ServerEntity> GetAllServers(int beginIndex, int endIndex)
        {
            const string sql =
                @"SELECT * FROM
                           (SELECT idc.Id IdcId, idc.IdcName, domain.Id DomainId, domain.Environment, domain.DomainName,domain.DomainType,
                            domain.OperatePathType,
                            domain.OperatePath, server.Id,
                            server.Type, server.IP, server.Root, server.IISStatus, server.CreateTime,
                            server.Creator, server.UpdateTime, server.Updator, server.ClientVersion,server.HostName,server.LastHeartBeatDate,server.DeleteFlag,server.IsAdvanced,
                            ROW_NUMBER() OVER (order by DomainName,type desc,HostName)as RowNumber
                            FROM (SELECT * FROM zsync_idc WHERE DeleteFlag=0) idc
                            RIGHT JOIN (SELECT * FROM zsync_domain WHERE DeleteFlag=0) domain
                            ON idc.Id=domain.IdcId
                            LEFT JOIN (SELECT * FROM zsync_server WHERE DeleteFlag=0) server
                            ON domain.Id=server.DomainId) tab
                            WHERE RowNumber between @BeginIndex AND @EndIndex
                            ORDER BY Environment, DomainName,type desc,IsAdvanced desc,HostName";

            SqlParameter[] parameters = {
                                            new SqlParameter("@BeginIndex", beginIndex),
                                            new SqlParameter("@EndIndex", endIndex)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取所有服务器列表
        /// </summary>
        /// <param name="username"></param>
        /// <param name = "beginIndex">开始记录数</param>
        /// <param name = "endIndex">结尾记录数</param>
        /// <returns>服务器实体列表</returns>
        public IList<ServerEntity> GetAllServers(string userName, int beginIndex, int endIndex)
        {
            const string sql =
                @"SELECT * FROM
                           (SELECT idc.Id IdcId, idc.IdcName, domain.Id DomainId, domain.Environment, domain.DomainName,domain.DomainType,
                            domain.OperatePathType,
                            domain.OperatePath, server.Id,
                            server.Type, server.IP, server.Root, server.IISStatus, server.CreateTime,
                            server.Creator, server.UpdateTime, server.Updator, server.ClientVersion,server.HostName,server.LastHeartBeatDate,server.DeleteFlag,server.IsAdvanced,
                            ROW_NUMBER() OVER (order by DomainName,type desc,HostName)as RowNumber
                            FROM (SELECT * FROM zsync_idc WHERE DeleteFlag=0) idc
                            RIGHT JOIN (SELECT * FROM zsync_domain WHERE DeleteFlag=0) domain
                            ON idc.Id=domain.IdcId
                            right join(select * from  [zsync_domain_permission] where UserName=@UserName and PermissionType=1 and DeleteFlag=0) pms
                            on pms.DomainID=domain.id
                            LEFT JOIN (SELECT * FROM zsync_server WHERE DeleteFlag=0) server
                            ON domain.Id=server.DomainId
                            ) tab
                            WHERE RowNumber between @BeginIndex AND @EndIndex
                            ORDER BY Environment, DomainName,type desc,IsAdvanced desc,HostName";

            SqlParameter[] parameters = {
                                             new SqlParameter("@UserName", userName),
                                            new SqlParameter("@BeginIndex", beginIndex),
                                            new SqlParameter("@EndIndex", endIndex)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取所有服务器数量
        /// </summary>
        /// <returns>服务器实体列表</returns>
        public int GetAllServersCount()
        {
            return GetAllServersCount("");
        }

        /// <summary>
        /// 获用户服务器数量
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public int GetAllServersCount(string userName)
        {
            string sql =
              @" SELECT count(*)
                            FROM (SELECT * FROM zsync_idc WHERE DeleteFlag=0) idc
                            RIGHT JOIN (SELECT * FROM zsync_domain WHERE DeleteFlag=0) domain
                            ON idc.Id=domain.IdcId";
            if (!string.IsNullOrWhiteSpace(userName))
            {
                sql += @" right join(select * from  [zsync_domain_permission] where UserName='" + userName + "' and DeleteFlag=0) pms on pms.DomainID=domain.Id and PermissionType=1 ";
            }
            sql += @" LEFT JOIN (SELECT * FROM zsync_server WHERE DeleteFlag=0) server
                            ON domain.Id=server.DomainId";

            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql));
        }

        #region 雷斌添加

        /// <summary>
        /// 获取服务器数量
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="type">管理类型</param>
        /// <param name="domainID">域名ID</param>
        /// <returns></returns>
        public int GetManageServerCount(string userName, Model.Enum.EnumManageType type, int? domainID)
        {
            string sql = @"SELECT COUNT(*)
                            FROM zsync_domain_permission T
                            WHERE T.UserName=@UserName
                            AND T.PermissionType=@PermissionType
                            AND T.DeleteFlag=0";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@UserName", userName));
            parameters.Add(new SqlParameter("@PermissionType", (byte)type));
            if (domainID.HasValue && domainID.Value > 0)
            {
                sql += " AND T.DomainID=@DomainID";
                parameters.Add(new SqlParameter("@DomainID", domainID.Value));
            }

            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters.ToArray()));
        }

        #endregion 雷斌添加

        public IList<ServerEntity> GetTargetServersByDomain(string domain)
        {
            const string sql =
                @"SELECT  vwsserver.* ,
                            vwsdomain.Environment ,
                            vwsdomain.DomainName ,
                            vwsdomain.IdcId ,
                            vwsdomain.DomainType,
                            vwsdomain.OperatePathType,
                            vwsdomain.OperatePath,
                            vwsidc.IdcName FROM zsync_server vwsserver(NOLOCK)
                           right join zsync_domain vwsdomain(NOLOCK) on vwsserver.DomainId = vwsdomain.Id
                           right join zsync_idc vwsidc(NOLOCK) on vwsdomain.IdcId = vwsidc.Id WHERE vwsserver.DeleteFlag = 0 and vwsdomain.DomainName = @DomainName and Type=1  order by vwsserver.HostName";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainName", domain)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        public ServerEntity GetSourceServersByDomain(string domain)
        {
            const string sql =
                @"SELECT  vwsserver.* ,
                            vwsdomain.Environment ,
                            vwsdomain.DomainName ,
                            vwsdomain.IdcId ,
                            vwsdomain.DomainType,
                            vwsdomain.OperatePathType,
                            vwsdomain.OperatePath,
                            vwsidc.IdcName FROM zsync_server vwsserver(NOLOCK)
                           right join zsync_domain vwsdomain(NOLOCK) on vwsserver.DomainId = vwsdomain.Id
                           right join zsync_idc vwsidc(NOLOCK) on vwsdomain.IdcId = vwsidc.Id WHERE vwsserver.DeleteFlag = 0 and vwsdomain.DomainName = @DomainName and Type=3";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainName", domain)
                                        };

            return GetEntity(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     插入服务器数据
        /// </summary>
        /// <param name = "serverEntity">服务器实体</param>
        /// <returns>记录Id</returns>
        public int Insert(ServerEntity serverEntity)
        {
            const string sql =
                @"INSERT INTO zsync_server ([Type],[DomainId],[IP],[Root],[IISStatus],[Creator],[IsAdvanced])
                           VALUES(@Type,@DomainId,@IP,@Root,@IISStatus,@Creator,@IsAdvanced)
                           select SCOPE_IDENTITY()";

            SqlParameter[] parameters = {
                                            new SqlParameter("@Type", serverEntity.ServerType),
                                            new SqlParameter("@DomainId", serverEntity.DomainId),
                                            new SqlParameter("@IP", serverEntity.IP),
                                            new SqlParameter("@Root", serverEntity.Root),
                                            new SqlParameter("@IISStatus", serverEntity.IISStatus),
                                            new SqlParameter("@Creator", serverEntity.Creator),
                                            new SqlParameter("@IsAdvanced", serverEntity.IsAdvanced)
                                        };

            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        }

        /// <summary>
        ///     修改服务器数据
        /// </summary>
        /// <param name = "serverEntity">服务器实体</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool Update(ServerEntity serverEntity)
        {
            const string sql =
                @"UPDATE zsync_server SET DomainId = ISNULL(@DomainId,DomainId),  IP = ISNULL(@IP,IP), Root = ISNULL(@Root,Root),
                          IISStatus = ISNULL(@IISStatus,IISStatus),Updator=ISNULL(@Updator,Updator),
                          UpdateTime=GETDATE(), ClientVersion=@ClientVersion, HostName=@HostName, IsAdvanced=@IsAdvanced where Id = @ServerId";

            SqlParameter[] parameters = {
                                            new SqlParameter("@ServerId", serverEntity.ServerId),
                                            new SqlParameter("@DomainId", serverEntity.DomainId),
                                            new SqlParameter("@IP", serverEntity.IP),
                                            new SqlParameter("@Root", serverEntity.Root),
                                            new SqlParameter("@IISStatus", serverEntity.IISStatus),
                                            new SqlParameter("@Updator", serverEntity.Updator),
                                            new SqlParameter("@ClientVersion", serverEntity.ClientVersion),
                                            new SqlParameter("@HostName", serverEntity.HostName),
                                            new SqlParameter("@IsAdvanced", serverEntity.IsAdvanced)
                                        };

            var result = ExecuteNonQuery(CommandType.Text, sql, parameters);
            return result == 1;
        }

        /// <summary>
        ///     修改服务器数据
        /// </summary>
        /// <param name = "serverEntity">服务器实体</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool UpdateLastHeartBeatDate(string IP, DateTime? LastHeartBeatDate)
        {
            const string sql =
                @"UPDATE zsync_server SET LastHeartBeatDate=@LastHeartBeatDate where IP = @IP";

            SqlParameter[] parameters = {
                                            new SqlParameter("@IP", IP),
                                            new SqlParameter("@LastHeartBeatDate", LastHeartBeatDate)
                                        };

            var result = ExecuteNonQuery(CommandType.Text, sql, parameters);
            return result == 1;
        }

        /// <summary>
        ///     删除服务器数据
        /// </summary>
        /// <param name = "serverId">服务器Id</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool Delete(int serverId)
        {
            const string sql = @"UPDATE zsync_server SET DeleteFlag = 1 where Id = @serverId";

            SqlParameter[] parameters = {
                                            new SqlParameter("@serverId", serverId)
                                        };

            int result = ExecuteNonQuery(CommandType.Text, sql, parameters);
            return result >= 1;
        }

        /// <summary>
        ///     删除服务器数据
        /// </summary>
        /// <param name = "domainIdList">域名Id列表</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool Delete(IList<int> domainIdList)
        {
            const string sql = @"UPDATE zsync_server SET DeleteFlag = 1 where DomainId in ({0})";

            var domainStrBuilder = new StringBuilder();

            if (domainIdList.Count > 0)
            {
                foreach (var domainId in domainIdList)
                {
                    domainStrBuilder.Append(string.Format("{0},", domainId));
                }
                domainStrBuilder.Remove(domainStrBuilder.Length - 1, 1);

                int result = ExecuteNonQuery(CommandType.Text, string.Format(sql, domainStrBuilder));

                return result >= 1;
            }
            return true;
        }

        /// <summary>
        ///     一个域名只能有一个同步中继
        /// </summary>
        /// <param name = "serverEntity">服务器实体</param>
        /// <returns>已经存在返回true，反之返回false</returns>
        public bool ExistsRelay(ServerEntity serverEntity)
        {
            var sql =
                @"SELECT count(Id) FROM zsync_server(NOLOCK) WHERE DomainId=@DomainId and Type=2 and DeleteFlag = 0 ";

            var parameters = new List<SqlParameter> { new SqlParameter("@DomainId", serverEntity.DomainId) };

            if (serverEntity.ServerId >= 1)
            {
                sql += "and ID <> @ServerId";
                parameters.Add(new SqlParameter("@ServerId", serverEntity.ServerId));
            }

            int result = Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters.ToArray()));
            return result >= 1;
        }

        /// <summary>
        ///     一个域名只能有一个同步源
        /// </summary>
        /// <param name = "serverEntity">服务器实体</param>
        /// <returns>已经存在返回true，反之返回false</returns>
        public bool ExistsSource(ServerEntity serverEntity)
        {
            var sql =
                @"SELECT count(Id) FROM zsync_server(NOLOCK) WHERE DomainId=@DomainId and Type=3 and DeleteFlag = 0 ";

            var parameters = new List<SqlParameter> { new SqlParameter("@DomainId", serverEntity.DomainId) };

            if (serverEntity.ServerId >= 1)
            {
                sql += "and ID <> @ServerId";
                parameters.Add(new SqlParameter("@ServerId", serverEntity.ServerId));
            }
            int result = Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters.ToArray()));
            return result >= 1;
        }

        /// <summary>
        ///     同一域名下不能出现多台宿主一个IP
        /// </summary>
        /// <param name = "serverEntity">服务器实体</param>
        /// <returns>已经存在返回true，反之返回false</returns>
        public bool ExistsHostCommonIp(ServerEntity serverEntity)
        {
            var sql =
                @"SELECT count(Id) FROM zsync_server(NOLOCK) WHERE DomainId=@DomainId and IP=@ip and Type=1 and DeleteFlag = 0 ";

            var parameters = new List<SqlParameter>
                                 {
                                     new SqlParameter("@DomainId", serverEntity.DomainId),
                                     new SqlParameter("@ip", serverEntity.IP)
                                 };

            if (serverEntity.ServerId >= 1)
            {
                sql += "and ID <> @ServerId";
                parameters.Add(new SqlParameter("@ServerId", serverEntity.ServerId));
            }
            var result = Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters.ToArray()));

            return result >= 1;
        }

        #region heyongdong add

        public List<string> GetAllowIp(string ip)
        {
            var sql = @"
                        select distinct ip from zsync_server nolock where Type=1 and DomainId in(
                        select DomainId from zsync_server nolock where IP=@IP and Type=3)
                        union
                        select distinct ip from zsync_server nolock where Type=3 and DomainId in(
                        select DomainId from zsync_server nolock where IP=@IP and Type=1)";

            var parameters = new List<SqlParameter> { new SqlParameter("@IP", ip) };

            DataSet ds = ExecuteDataset(CommandType.Text, sql, parameters.ToArray());
            if (ds.Tables != null && ds.Tables[0].Rows.Count > 0)
            {
                var ipList = new List<string>();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    ipList.Add(row["ip"].ToString());
                }
                return ipList;
            }
            else
            {
                return null;
            }
        }

        #endregion heyongdong add

        #region add by mhz

        public IList<ServerEntity> GetAllServersForServerList(int beginIndex, int endIndex)
        {
            const string sql =
                             @"SELECT * FROM
                           (SELECT idc.Id IdcId, idc.IdcName, domain.Id DomainId, domain.Environment, domain.DomainName, domain.DomainType,
                            domain.OperatePathType,
                            domain.OperatePath,server.Id,
                            server.Type, server.IP, server.Root, server.IISStatus, server.CreateTime,
                            server.Creator, server.UpdateTime, server.Updator, server.ClientVersion,server.HostName,server.LastHeartBeatDate,server.DeleteFlag,server.IsAdvanced,
                            ROW_NUMBER() OVER (order by DomainName,type desc,HostName)as RowNumber
                            FROM (SELECT * FROM zsync_idc WHERE DeleteFlag=0) idc
                            RIGHT JOIN (SELECT * FROM zsync_domain WHERE DeleteFlag=0) domain
                            ON idc.Id=domain.IdcId
                            LEFT JOIN (SELECT * FROM zsync_server) server
                            ON domain.Id=server.DomainId) tab
                            WHERE RowNumber between @BeginIndex AND @EndIndex
                            ORDER BY Environment, DomainName,type desc,IsAdvanced desc,HostName";

            SqlParameter[] parameters = {
                                            new SqlParameter("@BeginIndex", beginIndex),
                                            new SqlParameter("@EndIndex", endIndex)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        public IList<ServerEntity> GetServersByDomainIdForServerList(int domainId)
        {
            return GetServersByDomainIdForServerList("", domainId);
        }

        public IList<ServerEntity> GetAllServersForServerList(string userName, int beginIndex, int endIndex)
        {
            const string sql =
                            @"SELECT * FROM
                           (SELECT idc.Id IdcId, idc.IdcName, domain.Id DomainId, domain.Environment, domain.DomainName, domain.DomainType,
                            domain.OperatePathType,
                            domain.OperatePath,server.Id,
                            server.Type, server.IP, server.Root, server.IISStatus, server.CreateTime,
                            server.Creator, server.UpdateTime, server.Updator, server.ClientVersion,server.HostName,server.LastHeartBeatDate,server.DeleteFlag,server.IsAdvanced,
                            ROW_NUMBER() OVER (order by DomainName,type desc,HostName)as RowNumber
                            FROM (SELECT * FROM zsync_idc WHERE DeleteFlag=0) idc
                            RIGHT JOIN (SELECT * FROM zsync_domain WHERE DeleteFlag=0) domain
                            ON idc.Id=domain.IdcId
                            right join(select * from  [zsync_domain_permission] where UserName=@UserName and PermissionType=1 and DeleteFlag=0) pms
                            on pms.DomainID=domain.id
                            LEFT JOIN (SELECT * FROM zsync_server) server
                            ON domain.Id=server.DomainId
                            ) tab
                            WHERE RowNumber between @BeginIndex AND @EndIndex
                            ORDER BY Environment, DomainName,type desc,IsAdvanced desc,HostName";

            SqlParameter[] parameters = {
                                             new SqlParameter("@UserName", userName),
                                            new SqlParameter("@BeginIndex", beginIndex),
                                            new SqlParameter("@EndIndex", endIndex)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        public IList<ServerEntity> GetServersByDomainIdForServerList(string userName, int domainId)
        {
            string sql =
                            @"SELECT idc.Id IdcId, idc.IdcName, domain.Id DomainId, domain.Environment, domain.DomainName, domain.DomainType,
                            domain.OperatePathType,
                            domain.OperatePath,server.*
                            FROM (SELECT * FROM zsync_idc WHERE DeleteFlag=0) idc
                            RIGHT JOIN (SELECT * FROM zsync_domain WHERE ID = @DomainId and DeleteFlag=0) domain
                            ON idc.Id=domain.IdcId
                            LEFT JOIN (SELECT * FROM zsync_server) server
                            ON domain.Id=server.DomainId";
            if (!string.IsNullOrEmpty(userName))
            {
                sql += @" inner join(select * from  [zsync_domain_permission] where DomainId = @DomainId and UserName=@UserName and PermissionType=1 and DeleteFlag=0) pms
                            on pms.DomainID=server.DomainId";
            }
            sql += " ORDER BY Environment, type desc,server.IsAdvanced desc,server.HostName";

            SqlParameter[] parameters;
            parameters = new SqlParameter[] {
                                            new SqlParameter("@DomainId", domainId)
                                        };
            if (!string.IsNullOrEmpty(userName))
            {
                parameters = new SqlParameter[] {
                                            new SqlParameter("@DomainId", domainId),
                 new SqlParameter("@UserName", userName)
                                        };
            }
            return GetEntities(CommandType.Text, sql, parameters);
        }

        public ServerEntity GetServerByIdForServerList(int serverId)
        {
            const string sql =
                @"SELECT vwsserver.*, vwsdomain.Environment,vwsdomain.DomainName,vwsdomain.IdcId,vwsdomain.DomainType,
                            vwsdomain.OperatePathType,
                            vwsdomain.OperatePath,vwsidc.IdcName FROM zsync_server vwsserver(NOLOCK)
                           right join zsync_domain vwsdomain(NOLOCK) on vwsserver.DomainId = vwsdomain.Id
                           right join zsync_idc vwsidc(NOLOCK) on vwsdomain.IdcId = vwsidc.Id
                           WHERE vwsserver.Id = @serverId";

            SqlParameter[] parameters = {
                                            new SqlParameter("@serverId", serverId)
                                        };

            return GetEntity(CommandType.Text, sql, parameters);
        }

        public IList<ServerEntity> GetServersByIdcForServerList(int idcId)
        {
            const string sql =
                            @"SELECT idc.Id IdcId, idc.IdcName, domain.Id DomainId, domain.Environment, domain.DomainName, domain.DomainType,
                            domain.OperatePathType,
                            domain.OperatePath,server.*
                            FROM (SELECT * FROM zsync_idc WHERE DeleteFlag=0) idc
                            LEFT JOIN (SELECT * FROM zsync_domain WHERE DeleteFlag=0) domain
                            ON idc.Id=domain.IdcId
                            LEFT JOIN (SELECT * FROM zsync_server) server
                            ON domain.Id=server.DomainId
                            WHERE idc.Id = @IdcId
                            ORDER BY domain.Environment, domain.DomainName,type desc,server.IsAdvanced desc,server.HostName";

            SqlParameter[] parameters = {
                                            new SqlParameter("@IdcId", idcId)
                                        };
            return GetEntities(CommandType.Text, sql, parameters);
        }

        public IList<ServerEntity> GetAllServersForServerList()
        {
            const string sql =
                            @"SELECT idc.Id IdcId, idc.IdcName, domain.Id DomainId, domain.Environment, domain.DomainName,domain.DomainType,
                            domain.OperatePathType,
                            domain.OperatePath, server.*
                            FROM (SELECT * FROM zsync_idc WHERE DeleteFlag=0) idc
                            RIGHT JOIN (SELECT * FROM zsync_domain WHERE DeleteFlag=0) domain
                            ON idc.Id=domain.IdcId
                            LEFT JOIN (SELECT * FROM zsync_server) server
                            ON domain.Id=server.DomainId
                            ORDER BY domain.Environment, domain.DomainName,type desc,server.IsAdvanced desc,server.HostName";

            return GetEntities(CommandType.Text, sql);
        }

        public int GetAllServersCountForServerList(string userName)
        {
            string sql =
             @" SELECT count(*)
                            FROM (SELECT * FROM zsync_idc WHERE DeleteFlag=0) idc
                            RIGHT JOIN (SELECT * FROM zsync_domain WHERE DeleteFlag=0) domain
                            ON idc.Id=domain.IdcId";
            if (!string.IsNullOrWhiteSpace(userName))
            {
                sql += @" right join(select * from  [zsync_domain_permission] where UserName='" + userName + "' and DeleteFlag=0) pms on pms.DomainID=domain.Id and PermissionType=1 ";
            }
            sql += @" LEFT JOIN (SELECT * FROM zsync_server) server
                            ON domain.Id=server.DomainId";

            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql));
        }

        public int GetManageServerCountForServerList(string userName, Model.Enum.EnumManageType type, int? domainID)
        {
            string sql = @"SELECT COUNT(*)
                            FROM zsync_domain_permission T
                            WHERE T.UserName=@UserName
                            AND T.PermissionType=@PermissionType
                            AND T.DeleteFlag=0";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@UserName", userName));
            parameters.Add(new SqlParameter("@PermissionType", (byte)type));
            if (domainID.HasValue && domainID.Value > 0)
            {
                sql += " AND T.DomainID=@DomainID";
                parameters.Add(new SqlParameter("@DomainID", domainID.Value));
            }

            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters.ToArray()));
        }

        public bool UnDelete(int serverId)
        {
            const string sql = @"UPDATE zsync_server SET DeleteFlag = 0 where Id = @serverId";

            SqlParameter[] parameters = {
                                            new SqlParameter("@serverId", serverId)
                                        };

            int result = ExecuteNonQuery(CommandType.Text, sql, parameters);
            return result >= 1;
        }

        #endregion add by mhz
    }
}