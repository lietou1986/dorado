/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/12 15:51:21
 * 作者：len
 * 联系方式：len@dorado.com 
 * 本类主要用途描述：
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
    public class PermissionDao : DBbase<PermissionEntity>
    {
        /// <summary>
        ///     获取某个角色的权限列表
        /// </summary>
        /// <param name = "roleID">角色ID</param>
        /// <returns></returns>
        public IList<PermissionEntity> GetPermissionByRoleId(int roleID)
        {
            const string sql = @"SELECT * FROM zsync_permission WHERE RoleID=@RoleID and DeleteFlag = 0";
            var parameter = new SqlParameter("@RoleID", roleID);
            return GetEntities(CommandType.Text, sql, parameter);
        }

        /// <summary>
        ///     添加角色路径
        /// </summary>
        /// <param name = "roleID">角色Id</param>
        /// <param name = "permissionEntity">权限实体</param>
        /// <returns>记录Id</returns>
        public int Insert(int roleID, PermissionEntity permissionEntity)
        {
            const string sql = @"INSERT INTO zsync_permission(RoleID, Path, Type)VALUES(@RoleId, @Path, @Type)";

            SqlParameter[] parameters = {
                                            new SqlParameter("@RoleID", roleID),
                                            new SqlParameter("@Path", permissionEntity.Path),
                                            new SqlParameter("@Type", permissionEntity.Type)
                                        };
            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        }

        /// <summary>
        ///     删除某个角色下的所有资源
        /// </summary>
        /// <param name = "roleID">角色ID</param>
        public void Delete(int roleID)
        {
            const string sql = @"Update zsync_permission set DeleteFlag = 1 WHERE RoleID=@RoleID";
            var parameter = new SqlParameter("@RoleID", roleID);
            ExecuteNonQuery(CommandType.Text, sql, parameter);
        }

        /// <summary>
        ///     获取用户某个域名下的文件列表
        /// </summary>
        /// <param name = "userName">用户名</param>
        /// <param name = "domainId">域名ID</param>
        /// <returns></returns>
        public IList<PermissionEntity> GetPermissionByUserAndDomain(string userName, int domainId)
        {
            const string sql =
                @"
                SELECT p.* FROM zsync_userrole u(nolock)
                JOIN zsync_role r(nolock) ON u.RoleID=r.ID
                JOIN zsync_permission p(nolock) ON p.RoleID=r.ID
                WHERE u.UserName=@UserName AND r.DomainID=@DomainID AND p.DeleteFlag = 0 AND u.DeleteFlag = 0";
            SqlParameter[] parameters = {
                                            new SqlParameter("@UserName", userName),
                                            new SqlParameter("@DomainID", domainId)
                                        };
            return GetEntities(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取用户某个域名下的文件列表
        /// </summary>
        /// <param name = "userName">用户名</param>
        /// <param name = "domainName">域名</param>
        /// <returns></returns>
        public IList<PermissionEntity> GetPermissionByUserAndDomain(string userName, string domainName)
        {
            const string sql =
                @"
                SELECT p.* FROM zsync_userrole u(nolock)
                JOIN zsync_role r(nolock) ON u.RoleID=r.ID
                JOIN zsync_permission p(nolock) ON p.RoleID=r.ID
                JOIN zsync_domain d(nolock) on d.Id = r.DomainID
                WHERE u.UserName=@UserName AND d.DomainName=@DomainName
                    and p.DeleteFlag = 0 and r.DeleteFlag=0 and u.DeleteFlag=0";
            SqlParameter[] parameters = {
                                            new SqlParameter("@UserName", userName),
                                            new SqlParameter("@DomainName", domainName)
                                        };
            return GetEntities(CommandType.Text, sql, parameters);
        }
    }
}