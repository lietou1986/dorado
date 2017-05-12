/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/12 16:02:23
 * 作者：len
 * 联系方式：len@dorado.com 
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

#region using

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Dorado.VWS.Model;

#endregion using

namespace Dorado.VWS.Services.Persistence
{
    /// <summary>
    ///     用户详细资源权限数据操作
    /// </summary>
    public class UserResourceDao : DBbase<UserResoureEntity>
    {
        /// <summary>
        ///     获取用户拥有的资源权限
        /// </summary>
        /// <param name = "userRoleIdList">用户权限ID列表</param>
        /// <returns></returns>
        public IList<UserResoureEntity> GetUserResourcePermission(List<int> userRoleIdList)
        {
            const string sql =
                @"SELECT p.ResourceID,p.UserRoleID ,q.ResourceValue,q.Description
                           FROM zsync_resource_permission p join zsync_resource q on p.resourceID = q.id
                           WHERE UserRoleID in ({0}) and q.DeleteFlag = 0";
            var paraUserRoleId = new StringBuilder();

            IList<UserResoureEntity> ilist = new List<UserResoureEntity>();

            if (userRoleIdList.Count > 0)
            {
                foreach (var userRoleId in userRoleIdList)
                {
                    paraUserRoleId.Append(string.Format("{0},", userRoleId));
                }
                paraUserRoleId.Remove(paraUserRoleId.Length - 1, 1);
                ilist = GetEntities(CommandType.Text, string.Format(sql, paraUserRoleId));
            }
            return ilist;
        }

        /// <summary>
        ///     修改用户资源权限
        /// </summary>
        /// <param name = "userRoleId">用户权限ID</param>
        /// <param name = "resourceId">资源ID</param>
        /// <returns></returns>
        public bool InsertUserResource(int userRoleId, int resourceId)
        {
            const string sql =
                @"insert into zsync_resource_permission(ResourceID,UserRoleID) values (@resourceID,@userRoleID)";
            SqlParameter[] parameters = {
                                            new SqlParameter("@resourceID", resourceId),
                                            new SqlParameter("@userRoleID", userRoleId)
                                        };

            var result = ExecuteNonQuery(CommandType.Text, sql, parameters);
            return result == 1;
        }

        /// <summary>
        ///     删除用户资源权限
        /// </summary>
        /// <param name = "userRoleIdList">用户权限ID列表</param>
        /// <returns></returns>
        public bool DeleteUserResource(List<int> userRoleIdList)
        {
            const string sql = @"update zsync_resource_permission set DeleteFlag = 1 where UserRoleID in ({0}) ";

            var paraUserRoleId = new StringBuilder();

            if (userRoleIdList.Count > 0)
            {
                foreach (int userRoleId in userRoleIdList)
                {
                    paraUserRoleId.Append(string.Format("{0},", userRoleId.ToString()));
                }
                paraUserRoleId.Remove(paraUserRoleId.Length - 1, 1);

                var result = ExecuteNonQuery(CommandType.Text,
                                             string.Format(sql, paraUserRoleId));
                return result == 1;
            }
            return true;
        }
    }
}