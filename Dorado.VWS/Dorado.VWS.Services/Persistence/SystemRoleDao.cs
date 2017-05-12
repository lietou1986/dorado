using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dorado.VWS.Model;

namespace Dorado.VWS.Services.Persistence
{
    public class SystemRoleDao : DBbase<SystemRoleEntity>
    {
        /// <summary>
        ///     根据用户名和系统角色删除
        /// </summary>
        public void DeleteByUserRoleId(string username, int roleId)
        {
            const string sql = @"Delete zsync_user_sysytemrole WHERE UserName = @UserName and RoleID=@RoleID";
            SqlParameter[] parameters = {
                                            new SqlParameter("@UserName", username),
                                             new SqlParameter("@RoleID", roleId),
                                        };
            ExecuteNonQuery(CommandType.Text, sql, parameters);
        }

        /// <summary>
        ///     添加一个用户的系统角色
        /// </summary>
        public int Add(UserRoleEntity dto)
        {
            const string sql =
                @"INSERT INTO zsync_user_sysytemrole
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
        ///     获取角色
        /// </summary>
        /// <returns></returns>
        public IList<SystemRoleEntity> GetSysytemRoleByUser(string userName)
        {
            const string sql = @"SELECT * from dbo.zsync_systemrole(nolock) s join dbo.zsync_user_sysytemrole(nolock) us on us.RoleID=s.Id where s.DeleteFlag = @DeleteFlag and us.UserName = @userName";
            SqlParameter[] parameters = {
                                            new SqlParameter("@userName", userName),
                                            new SqlParameter("@DeleteFlag", false)
                                        };
            return GetEntities(CommandType.Text, sql, parameters);
        }
    }
}