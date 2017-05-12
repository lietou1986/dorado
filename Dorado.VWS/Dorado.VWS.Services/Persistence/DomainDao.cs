/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com 
 * 创建时间： 2011/7/5 15:30:39
 * 版本号：v1.0
 * 本类主要用途描述：域名数据访问操作类
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dorado.VWS.Model;

#endregion using

namespace Dorado.VWS.Services.Persistence
{
    /// <summary>
    ///     域名数据访问操作类
    /// </summary>
    public class DomainDao : DBbase<DomainEntity>
    {
        /// <summary>
        ///     获取域名信息
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <returns>域名实体</returns>
        public DomainEntity GetDomainById(int domainId)
        {
            const string sql =
                @"SELECT vwsDomain.*,vwsIdc.idcName FROM zsync_domain vwsDomain(NOLOCK) join zsync_idc vwsIdc(NOLOCK)
                           on vwsDomain.IdcId = vwsIdc.Id WHERE vwsDomain.Id = @domainId and vwsDomain.DeleteFlag = 0";

            SqlParameter[] parameters = {
                                            new SqlParameter("@domainId", domainId)
                                        };
            return GetEntity(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取域名信息
        /// </summary>
        /// <param name = "domainName">域名</param>
        /// <returns>域名实体</returns>
        public DomainEntity GetDomainByName(string domainName)
        {
            const string sql =
                @"SELECT vwsDomain.*,vwsIdc.idcName FROM zsync_domain vwsDomain(NOLOCK) join zsync_idc vwsIdc(NOLOCK)
                           on vwsDomain.IdcId = vwsIdc.Id WHERE vwsDomain.DomainName = @DomainName and vwsDomain.DeleteFlag = 0";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainName", domainName)
                                        };
            return GetEntity(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取域名列表
        /// </summary>
        /// <param name = "idcId">idcId</param>
        /// <returns>域名实体列表</returns>
        public IList<DomainEntity> GetDomainsByIdcId(int idcId)
        {
            const string sql =
                @"SELECT vwsDomain.*,vwsIdc.idcName FROM zsync_domain vwsDomain(NOLOCK) join zsync_idc vwsIdc(NOLOCK)
                           on vwsDomain.IdcId = vwsIdc.Id WHERE vwsIdc.Id = @idcId and vwsDomain.DeleteFlag = 0
                           order by vwsDomain.Environment,vwsDomain.DomainName ";

            SqlParameter[] parameters = {
                                            new SqlParameter("@idcId", idcId)
                                        };
            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取域名列表
        /// </summary>
        /// <param name = "idcId">idcId</param>
        /// <returns>域名实体列表</returns>
        public IList<DomainEntity> GetDomainsByIdcId(string environment, int idcId)
        {
            const string sql =
                @"SELECT vwsDomain.*,vwsIdc.idcName FROM zsync_domain vwsDomain(NOLOCK) join zsync_idc vwsIdc(NOLOCK)
                           on vwsDomain.IdcId = vwsIdc.Id WHERE vwsIdc.Id = @idcId and vwsDomain.Environment=@Environment and vwsDomain.DeleteFlag = 0
                           order by vwsDomain.Environment,vwsDomain.DomainName ";

            SqlParameter[] parameters = {
                                            new SqlParameter("@Environment", environment),
                                            new SqlParameter("@idcId", idcId)
                                        };
            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取用户拥有权限的启用状态域名列表
        /// </summary>
        /// <param name = "userName">用户名</param>
        /// <param name = "syncType">同步类型（1.普通同步【有版本】；2.简单同步【无版本】）</param>
        /// <returns>域名实体列表</returns>
        public IList<DomainEntity> GetDomainsByUser(string userName, int? syncType)
        {
            const string sql =
                @"select distinct domain.*,vwsIdc.idcName from zsync_userrole userrole(NOLOCK) join zsync_role role(NOLOCK)
                            on userrole.RoleId = role.Id join zsync_domain domain(NOLOCK)
                            on role.DomainId = domain.Id join zsync_idc vwsIdc(NOLOCK)
                            on domain.IdcId = vwsIdc.Id
                            where userrole.UserName = @UserName and domain.SyncType = ISNULL(@SyncType,SyncType) and domain.Enable = 1 and domain.DeleteFlag = 0 and userrole.DeleteFlag = 0
                            order by domain.Environment,domain.DomainName ";

            SqlParameter[] parameters = {
                                            new SqlParameter("@UserName", userName),
                                            new SqlParameter("@SyncType", syncType)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取用户拥有权限的启用状态域名列表
        /// </summary>
        /// <param name = "userName">用户名</param>
        /// <param name = "syncType">同步类型（1.普通同步【有版本】；2.简单同步【无版本】）</param>
        /// <returns>域名实体列表</returns>
        public IList<DomainEntity> GetDomainsByUser(string userName, int? syncType, string environment)
        {
            const string sql =
                @"select distinct domain.*,vwsIdc.idcName from zsync_userrole userrole(NOLOCK) join zsync_role role(NOLOCK)
                            on userrole.RoleId = role.Id join zsync_domain domain(NOLOCK)
                            on role.DomainId = domain.Id join zsync_idc vwsIdc(NOLOCK)
                            on domain.IdcId = vwsIdc.Id
                            where userrole.UserName = @UserName and domain.SyncType = ISNULL(@SyncType,SyncType) and domain.Environment= @Environment and domain.Enable = 1 and domain.DeleteFlag = 0 and userrole.DeleteFlag = 0
                            order by domain.Environment,domain.DomainName ";

            SqlParameter[] parameters = {
                                            new SqlParameter("@UserName", userName),
                                            new SqlParameter("@Environment", environment),
                                            new SqlParameter("@SyncType", syncType)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取用户拥有权限的启用状态域名列表
        /// </summary>
        /// <param name = "userName">用户名</param>
        /// <param name = "syncType">同步类型（1.普通同步【有版本】；2.简单同步【无版本】）</param>
        /// <returns>域名实体列表</returns>
        public IList<DomainEntity> GetDomainsByUser(string userName, int? syncType, string environment, int idcId)
        {
            const string sql =
                @"select distinct domain.*,vwsIdc.idcName from zsync_userrole userrole(NOLOCK) join zsync_role role(NOLOCK)
                            on userrole.RoleId = role.Id join zsync_domain domain(NOLOCK)
                            on role.DomainId = domain.Id join zsync_idc vwsIdc(NOLOCK)
                            on domain.IdcId = vwsIdc.Id
                            where userrole.UserName = @UserName and vwsIdc.Id=@idcId and domain.SyncType = ISNULL(@SyncType,SyncType) and domain.Environment= @Environment and domain.Enable = 1 and domain.DeleteFlag = 0 and userrole.DeleteFlag = 0
                            order by domain.Environment,domain.DomainName ";

            SqlParameter[] parameters = {
                                            new SqlParameter("@UserName", userName),
                                            new SqlParameter("@idcId", idcId),
                                            new SqlParameter("@Environment", environment),
                                            new SqlParameter("@SyncType", syncType)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取所有域名列表
        /// </summary>
        /// <returns>域名实体列表</returns>
        public IList<DomainEntity> GetAllDomains()
        {
            const string sql =
                @"SELECT vwsDomain.*,vwsIdc.idcName FROM zsync_domain vwsDomain(NOLOCK) join zsync_idc vwsIdc(NOLOCK)
                           on vwsDomain.IdcId = vwsIdc.Id WHERE vwsDomain.DeleteFlag = 0 order by vwsDomain.Environment,vwsDomain.DomainName ";

            return GetEntities(CommandType.Text, sql);
        }

        /// <summary>
        ///     插入域名数据
        /// </summary>
        /// <param name = "domainEntity">域名实体</param>
        /// <returns>记录Id</returns>
        public int Insert(DomainEntity domainEntity)
        {
            const string sql =
                @"INSERT INTO zsync_domain(IdcId,Environment, DomainName,WinServiceName,IISSiteName,CacheUrl,HtmlCompress,JsCssCompress,SyncType,DomainType,OperatePathType,OperatePath) VALUES
                          (@IdcId,@Environment,@DomainName,ISNULL(@WinServiceName,NULL),ISNULL(@IISSiteName,NULL),ISNULL(@CacheUrl,NULL),@HtmlCompress,@JsCssCompress,@SyncType,@DomainType,@OperatePathType,ISNULL(@OperatePath,NULL))
                          select SCOPE_IDENTITY()";

            SqlParameter[] parameters = {
                                            new SqlParameter("@IdcId", domainEntity.IdcId),
                                            new SqlParameter("@Environment", domainEntity.Environment),
                                            new SqlParameter("@DomainName", domainEntity.DomainName),
                                            new SqlParameter("@WinServiceName", domainEntity.WinServiceName),
                                            new SqlParameter("@IISSiteName", domainEntity.IISSiteName),
                                            new SqlParameter("@CacheUrl", domainEntity.CacheUrl),
                                            new SqlParameter("@HtmlCompress", domainEntity.HtmlCompress),
                                            new SqlParameter("@JsCssCompress", domainEntity.JsCssCompress),
                                            new SqlParameter("@SyncType", domainEntity.SyncType),
                                            new SqlParameter("@DomainType", domainEntity.DomainType),
                                            new SqlParameter("@OperatePathType", domainEntity.OperatePathType),
                                            new SqlParameter("@OperatePath", domainEntity.OperatePath),
                                        };

            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        }

        /// <summary>
        ///     修改域名数据
        /// </summary>
        /// <param name = "domainEntity">域名实体</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool Update(DomainEntity domainEntity)
        {
            const string sql =
                @"UPDATE zsync_domain SET IdcId = ISNULL(@IdcId,IdcId),Environment=@Environment, DomainName = ISNULL(@DomainName,DomainName),WinServiceName = ISNULL(@WinServiceName,WinServiceName),
                         IISSiteName = ISNULL(@IISSiteName,IISSiteName), CacheUrl = ISNULL(@CacheUrl,CacheUrl),HtmlCompress = ISNULL(@HtmlCompress,HtmlCompress),
                         JsCssCompress = ISNULL(@JsCssCompress,JsCssCompress),SyncType = @SyncType,UpdateTime=GETDATE(),DomainType=ISNULL(@DomainType,DomainType),OperatePathType=ISNULL(@OperatePathType,OperatePathType),OperatePath=ISNULL(@OperatePath,OperatePath) where Id = @DomainId";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainId", domainEntity.DomainId),
                                            new SqlParameter("@IdcId", domainEntity.IdcId),
                                            new SqlParameter("@Environment", domainEntity.Environment),
                                            new SqlParameter("@DomainName", domainEntity.DomainName),
                                            new SqlParameter("@WinServiceName", domainEntity.WinServiceName),
                                            new SqlParameter("@IISSiteName", domainEntity.IISSiteName),
                                            new SqlParameter("@CacheUrl", domainEntity.CacheUrl),
                                            new SqlParameter("@HtmlCompress", domainEntity.HtmlCompress),
                                            new SqlParameter("@JsCssCompress", domainEntity.JsCssCompress),
                                            new SqlParameter("@SyncType", domainEntity.SyncType),
                                            new SqlParameter("@DomainType", domainEntity.DomainType),
                                            new SqlParameter("@OperatePathType", domainEntity.OperatePathType),
                                            new SqlParameter("@OperatePath", domainEntity.OperatePath),
                                        };

            var result = ExecuteNonQuery(CommandType.Text, sql, parameters);
            return result == 1;
        }

        /// <summary>
        ///     删除域名数据
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool Delete(int domainId)
        {
            const string sql =
                @"UPDATE zsync_domain SET DeleteFlag = 1 where Id = @domainId
                           UPDATE zsync_server SET DeleteFlag = 1 where DomainId = @domainId";

            SqlParameter[] parameters = {
                                            new SqlParameter("@domainId", domainId)
                                        };

            var result = ExecuteNonQuery(CommandType.Text, sql, parameters);
            return result == 1;
        }

        /// <summary>
        ///     删除域名数据
        /// </summary>
        /// <param name = "idcId">idcId</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool DeleteByIdcId(int idcId)
        {
            const string sql = @"UPDATE zsync_domain SET DeleteFlag = 1 where IdcId = @idcId";

            SqlParameter[] parameters = {
                                            new SqlParameter("@idcId", idcId)
                                        };
            var result = ExecuteNonQuery(CommandType.Text, sql, parameters);
            return result == 1;
        }

        /// <summary>
        ///     修改域名状态（启用/停止）
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <param name = "enable">状态</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool UpdateStatus(int domainId, bool enable)
        {
            const string sql = @"UPDATE zsync_domain SET Enable = @Enable where Id = @domainId";

            SqlParameter[] parameters = {
                                            new SqlParameter("@domainId", domainId),
                                            new SqlParameter("@Enable", enable)
                                        };

            int result = ExecuteNonQuery(CommandType.Text, sql, parameters);
            return result == 1;
        }

        /// <summary>
        ///     验证域名是否存在
        /// </summary>
        /// <param name = "domainEntity">域名实体</param>
        /// <returns>已经存在返回true，反之返回false</returns>
        public bool ExistsDomain(DomainEntity domainEntity)
        {
            var sql =
                @"SELECT count(Id) FROM zsync_domain
                          WHERE DomainName = @DomainName  AND Environment= @Environment  and DeleteFlag = 0  ";

            var parameters = new List<SqlParameter> { new SqlParameter("@DomainName", domainEntity.DomainName), new SqlParameter("@Environment", domainEntity.Environment) };

            if (domainEntity.DomainId >= 1)
            {
                sql += "and ID <> @DomainId";
                parameters.Add(new SqlParameter("@DomainId", domainEntity.DomainId));
            }

            var result = Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters.ToArray()));
            return result >= 1;
        }

        #region 安全控制

        /// <summary>
        ///     获取指定ip所有域名列表
        /// </summary>
        /// <returns>域名实体列表</returns>
        public IList<DomainEntity> GetDomains(string ip)
        {
            const string sql =
                @"select  * from zsync_domain(NOLOCK) d
                    join zsync_idc vwsIdc(NOLOCK)
                        on d.IdcId = vwsIdc.Id
                    where d.id in(
                        select  domainid from zsync_server  where IP=@IP
                        ) ";

            SqlParameter[] parameters = {
                                            new SqlParameter("@IP", ip)
                                        };

            return GetEntities(CommandType.Text, sql, parameters);
        }

        #endregion 安全控制
    }
}