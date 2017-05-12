/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/12 16:04:49
 * 作者：len
 * 联系方式：len@dorado.com 
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dorado.VWS.Model;

#endregion using

namespace Dorado.VWS.Services.Persistence
{
    public class UserRoleDao : DBbase<UserRoleEntity>
    {
        /// <summary>
        ///     根据角色ID删除所有的用户
        /// </summary>
        /// <param name = "roleId"></param>
        public void DeleteByRoleId(int roleId)
        {
            const string sql = @"Update zsync_userrole set DeleteFlag = 1 WHERE RoleID=@RoleID";
            var parameter = new SqlParameter("@RoleID", roleId);
            ExecuteNonQuery(CommandType.Text, sql, parameter);
        }

        /// <summary>
        ///     根据ID删除
        /// </summary>
        /// <param name = "id"></param>
        public void DeleteById(int id)
        {
            const string sql = @"Update zsync_userrole set DeleteFlag = 1 WHERE ID=@ID";
            var parameter = new SqlParameter("@ID", id);
            ExecuteNonQuery(CommandType.Text, sql, parameter);
        }

        /// <summary>
        ///     删除用户角色
        /// </summary>
        /// <param name = "userName">用户名</param>
        /// <param name = "domain">域名</param>
        public void DeleteUserRole(string userName, int domainId)
        {
            const string sql =
                @"Update zsync_userrole set DeleteFlag = 1 WHERE UserName = @UserName and RoleID in(select id from zsync_role where DomainID = @DomainID)";

            SqlParameter[] parameters = {
                                            new SqlParameter("@UserName", userName),
                                            new SqlParameter("@DomainID", domainId)
                                        };
            ExecuteNonQuery(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///      删除用户所有角色和资源
        /// </summary>
        /// <param name = "userName"></param>
        //        public void DeleteByUserName(string userName)
        //        {
        //            const string sql =
        //                @"update zsync_resource_permission set DeleteFlag = 1
        //                           where UserRoleID in (select id from dbo.zsync_userrole where userName=@userName)
        //                           update zsync_userrole set DeleteFlag = 1 where userName=@userName";
        //            var parameter = new SqlParameter("@userName", userName);
        //            ExecuteNonQuery(CommandType.Text, sql, parameter);
        //        }

        /// <summary>
        ///     添加一个用户的角色
        /// </summary>
        public int Add(UserRoleEntity dto)
        {
            const string sql =
                @"INSERT INTO zsync_userrole
        ( UserName, RoleID )
VALUES  ( @UserName,
          @RoleID
          )";
            SqlParameter[] parameters = {
                                            new SqlParameter("@UserName", dto.UserName),
                                            new SqlParameter("@RoleID", dto.roleId)
                                        };
            return ExecuteNonQuery(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     获取用户角色ID列表
        /// </summary>
        /// <param name = "userName">用户账号</param>
        /// <param name = "domainId">域名Id</param>
        /// <returns></returns>
        public List<int> GetUserRoleId(string userName, int domainId)
        {
            const string sql =
                @"select k.id from dbo.zsync_userrole k
                            join dbo.zsync_role p on k.roleid = p.id
                            join zsync_domain q on p.DomainID=q.id
                            where username=@userName and p.DomainID=@DomainID and k.DeleteFlag = 0";

            SqlParameter[] parameters = {
                                            new SqlParameter("@userName", userName),
                                            new SqlParameter("@DomainID", domainId)
                                        };

            DataTable dt = ExecuteDataset(CommandType.Text, sql, parameters).Tables[0];

            var ilist = new List<int>();
            if (dt.Rows.Count > 0)
            {
                ilist.AddRange(from DataRow dr in dt.Rows select Convert.ToInt32(dr["id"]));
            }
            return ilist;
        }

        /// <summary>
        ///     获取系统有权限的用户账号
        /// </summary>
        /// <returns></returns>
        public IList<string> GetVwsUserName()
        {
            const string sql = @"select distinct UserName from zsync_userrole where DeleteFlag = 0";

            var dt = ExecuteDataset(CommandType.Text, sql).Tables[0];
            IList<string> ilist = new List<string>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ilist.Add(dr["UserName"].ToString());
                }
            }
            return ilist;
        }

        /// <summary>
        ///     获取用户权限
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <param name = "userName">用户名</param>
        /// <returns></returns>
        public IList<RoleEntity> GetUserRole(int domainId, string userName)
        {
            const string sql =
                @"SELECT s.DomainName,s.ID DomainID,r.ID,r.RoleName,u.UserName,u.ID UserID FROM zsync_domain s(NOLOCK)
                JOIN zsync_role r(NOLOCK) ON s.ID = r.DomainID
                JOIN zsync_userrole u(NOLOCK) ON u.RoleID=r.ID
                WHERE s.ID = @DomainID AND u.UserName=@UserName and u.DeleteFlag = 0";

            SqlParameter[] parameters = {
                                            new SqlParameter("@UserName", userName),
                                            new SqlParameter("@DomainID", domainId)
                                        };

            var dt = ExecuteDataset(CommandType.Text, sql, parameters).Tables[0];
            IList<RoleEntity> ilist = new List<RoleEntity>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ilist.Add(new RoleEntity
                                  {
                                      Domain = dr["DomainName"].ToString(),
                                      Id = dr["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ID"]),
                                      RoleName = dr["RoleName"].ToString(),
                                      UserName = dr["UserName"].ToString(),
                                      UserId = dr["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(dr["UserID"]),
                                      DomainId = Convert.ToInt32(dr["DomainID"])
                                  });
                }
            }
            return ilist;
        }

        #region heyongdong

        /// <summary>
        ///     获取角色成员账号
        /// </summary>
        /// <returns></returns>
        public IList<string> GetUsersByRoleId(int roleID)
        {
            string sql = @"select UserName from zsync_userrole where DeleteFlag = 0 and RoleId=" + roleID;

            var dt = ExecuteDataset(CommandType.Text, sql).Tables[0];
            IList<string> ilist = new List<string>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ilist.Add(dr["UserName"].ToString());
                }
            }
            return ilist;
        }

        /// <summary>
        ///     根据ID删除
        /// </summary>
        /// <param name = "id"></param>
        public int Delete(int roleID, string userName)
        {
            const string sql = @"Update zsync_userrole set DeleteFlag = 1 WHERE DeleteFlag=0 and  RoleId=@RoleId and UserName=@UserName";
            SqlParameter[] parameters = {
                                            new SqlParameter("@UserName", userName),
                                            new SqlParameter("@RoleId", roleID)
                                        };

            return ExecuteNonQuery(CommandType.Text, sql, parameters);
        }

        #endregion heyongdong
    }
}