/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/12 15:58:03
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
    public class RoleDao : DBbase<RoleEntity>
    {
        /// <summary>
        ///     添加一个角色
        /// </summary>
        /// <param name = "dto">服务器实体类</param>
        /// <returns></returns>
        public int Add(RoleEntity dto)
        {
            const string sql =
                @"INSERT INTO zsync_role
        ( RoleName, DomainID )
VALUES  ( @RoleName,
          @DomainID
          ) SELECT @@IDENTITY";
            SqlParameter[] parameters = {
                                            new SqlParameter("@RoleName", dto.RoleName),
                                            new SqlParameter("@DomainID", dto.DomainId)
                                        };
            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        }

        /// <summary>
        ///     获取一个角色
        /// </summary>
        /// <param name = "roleId">角色ID</param>
        /// <returns></returns>
        public RoleEntity GetRoleById(int roleId)
        {
            const string sql = @"SELECT s.Environment,s.DomainName,s.ID DomainID,r.ID,r.RoleName FROM zsync_domain s(NOLOCK)
                  LEFT JOIN zsync_role r(NOLOCK) ON s.ID = r.DomainID WHERE r.ID=@ID and r.DeleteFlag = 0";
            var parameter = new SqlParameter("@ID", roleId);
            return GetEntity(CommandType.Text, sql, parameter);
        }

        /// <summary>
        ///     更具域名ID获取角色
        /// </summary>
        /// <param name = "DomainID">域名ID</param>
        /// <returns></returns>
        public IList<RoleEntity> GetRoleByDomainId(int DomainID)
        {
            const string sql = @"SELECT s.Environment,s.DomainName,s.ID DomainID,r.ID,r.RoleName FROM zsync_domain s(NOLOCK)
                  LEFT JOIN zsync_role r(NOLOCK) ON s.ID = r.DomainID WHERE r.DomainID=@DomainID and r.DeleteFlag = 0";
            var parameter = new SqlParameter("@DomainID", DomainID);
            return GetEntities(CommandType.Text, sql, parameter);
        }

        /// <summary>
        ///     判断一个域名下是否存在同名的角色名称
        /// </summary>
        /// <param name = "dto"></param>
        /// <returns></returns>
        public bool Exist(RoleEntity dto)
        {
            const string sql =
                @"SELECT COUNT(*) FROM zsync_role(NOLOCK) WHERE RoleName=@RoleName AND DomainID=@DomainID and DeleteFlag = 0";
            SqlParameter[] parameters = {
                                            new SqlParameter("@RoleName", dto.RoleName),
                                            new SqlParameter("@DomainID", dto.DomainId)
                                        };
            int result = Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
            return result >= 1;
        }

        /// <summary>
        ///     判断一个域名下是否存在同名的角色名称(使用于修改时)
        /// </summary>
        /// <param name = "dto"></param>
        /// <returns></returns>
        public bool EditExist(RoleEntity dto)
        {
            const string sql =
                @"SELECT COUNT(*) FROM zsync_role(NOLOCK) WHERE RoleName=@RoleName AND DomainID=@DomainID and ID<>@ID and DeleteFlag = 0";
            SqlParameter[] parameters = {
                                            new SqlParameter("@RoleName", dto.RoleName),
                                            new SqlParameter("@DomainID", dto.DomainId),
                                            new SqlParameter("@ID", dto.Id)
                                        };
            int result = Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
            return result >= 1;
        }

        public void Delete(int roleId)
        {
            const string sql = @"update zsync_role set DeleteFlag = 1 WHERE ID=@ID";
            var parameter = new SqlParameter("@ID", roleId);
            ExecuteNonQuery(CommandType.Text, sql, parameter);
        }

        public void Edit(RoleEntity roleEntity)
        {
            const string sql = @"UPDATE zsync_role SET RoleName=@RoleName WHERE ID=@ID";
            SqlParameter[] parameters = {
                                            new SqlParameter("@RoleName", roleEntity.RoleName),
                                            new SqlParameter("@ID", roleEntity.Id)
                                        };
            ExecuteNonQuery(CommandType.Text, sql, parameters);
        }

        public IList<RoleEntity> GetAllRole(int domainId)
        {
            var sql =
                @"SELECT s.Environment,s.DomainName,s.ID DomainID,r.ID,r.RoleName FROM zsync_domain s(NOLOCK)
                  LEFT JOIN zsync_role r(NOLOCK) ON s.ID = r.DomainID
                  WHERE r.DeleteFlag = 0 ";
            if (domainId >= 1)
            {
                sql += " AND s.id='{0}' ";
                sql = string.Format(sql, domainId);
            }
            sql += "order by s.Environment,s.DomainName,r.RoleName";
            return GetEntities(CommandType.Text, sql);
        }
    }
}