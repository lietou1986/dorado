/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 时间： 2011/11/24 11:04:36
 * 作者：
 * 版本            时间                  作者                 描述
 * v 1.0    2011/11/24 11:04:36               创建
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
    public class DomainPermissionDao : DBbase<DomainPermissionEntity>
    {
        /// <summary>
        /// 获取某用户的所有域名
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public IList<DomainPermissionEntity> GetDomainsByUser(string username)
        {
            const string sql = @"SELECT * FROM zsync_domain_permission WHERE UserName=@UserName and DeleteFlag = 0";
            var parameter = new SqlParameter("@UserName", username);
            return GetEntities(CommandType.Text, sql, parameter);
        }

        /// <summary>
        /// 获取某域名和权限的所有用户
        /// </summary>
        /// <param name="domianID"></param>
        /// <param name="permissionType"></param>
        /// <returns></returns>
        public IList<DomainPermissionEntity> GetUsersByDomainAndPermissionType(int domainID, int permissionType)
        {
            const string sql = @"SELECT * FROM zsync_domain_permission WHERE DomainID=@DomainID and PermissionType=@PermissionType and DeleteFlag = 0";
            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainID", domainID),
                                            new SqlParameter("@PermissionType", permissionType)
                                        };
            return GetEntities(CommandType.Text, sql, parameters);
        }

        public IList<DomainPermissionEntity> GetPermission(int domainID, string userName)
        {
            const string sql = @"SELECT * FROM zsync_domain_permission WHERE DomainID=@DomainID and UserName=@UserName and DeleteFlag = 0";
            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainID", domainID),
                                            new SqlParameter("@UserName", userName)
                                        };
            return GetEntities(CommandType.Text, sql, parameters);
        }

        public DomainPermissionEntity GetPermission(int domainID, string userName, int permissionType)
        {
            const string sql = @"SELECT * FROM zsync_domain_permission WHERE DomainID=@DomainID and UserName=@UserName and PermissionType=@PermissionType and DeleteFlag = 0";
            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainID", domainID),
                                            new SqlParameter("@UserName", userName),
                                            new SqlParameter("@PermissionType", permissionType)
                                        };
            return GetEntity(CommandType.Text, sql, parameters);
        }

        /// <summary>
        /// 添加权限
        /// </summary>
        /// <param name="domainPermissionEntity"></param>
        /// <returns></returns>
        public int Add(DomainPermissionEntity domainPermissionEntity)
        {
            const string sql = @"
                if EXISTS(SELECT *  FROM [zsync_domain_permission] where domainid=@DomainID and username=@UserName and [PermissionType]=@PermissionType)
                begin
                update [zsync_domain_permission] set deleteflag=0,updateusername=@UpdateUserName,updatetime=@UpdateTime where domainid=@DomainID and username=@UserName and [PermissionType]=@PermissionType

                end
                else
                begin
                insert into [zsync_domain_permission]
                ([DomainID],[UserName],[PermissionType],[DeleteFlag],[AddTime],[AddUserName])
                 values(@DomainID,@UserName,@PermissionType,0,@AddTime,@AddUserName)
                end
                ";

            SqlParameter[] parameters = {
                                            new SqlParameter("@DomainID", domainPermissionEntity.DomainID),
                                            new SqlParameter("@UserName", domainPermissionEntity.UserName),
                                            new SqlParameter("@PermissionType", domainPermissionEntity.PermissionType),
                                            new SqlParameter("@AddTime", domainPermissionEntity.AddTime),
                                            new SqlParameter("@AddUserName", domainPermissionEntity.AddUserName),
                                            new SqlParameter("@UpdateTime", domainPermissionEntity.UpdateTime),
                                            new SqlParameter("@UpdateUserName", domainPermissionEntity.UpdateUserName),
                                        };
            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        }

        #region 快速删除权限

        //public int DeleteByDomainID(int domainID)
        //{
        //    const string sql = @"Update zsync_domain_permission set  DeleteFlag = 1 WHERE DomianID=@DomianID  ";
        //    SqlParameter[] parameters = {
        //                                    new SqlParameter("@DomianID", domainID)

        //                                };
        //    return  Convert.ToInt32(ExecuteScalar (CommandType.Text, sql, parameters));
        //}

        //public int DeleteByUserName(string userName )
        //{
        //    const string sql = @"Update zsync_domain_permission set  DeleteFlag = 1 WHERE UserName=@UserName  ";
        //    SqlParameter[] parameters = {
        //                                    new SqlParameter("@UserName", userName)
        //                                 };
        //    return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        //}

        //public int DeleteByPermissionType(int permissionType)
        //{
        //    const string sql = @"Update zsync_domain_permission set  DeleteFlag = 1 WHERE  PermissionType=@PermissionType  ";
        //    SqlParameter[] parameters = {
        //                                    new SqlParameter("@PermissionType", permissionType)
        //                                 };
        //    return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        //}

        #endregion 快速删除权限

        /// <summary>
        /// 删除授权
        /// </summary>
        /// <param name="domainID"></param>
        /// <param name="userName"></param>
        /// <param name="permissionType"></param>
        /// <returns></returns>
        //[Obsolete("Please use void Delete(DomainPermissionEntity domainPermissionEntity)")]
        //  public int Delete(int domainID,string userName,int permissionType)
        //  {
        //      const string sql = @"Update zsync_domain_permission set  DeleteFlag = 1 WHERE  DomainID=@DomainID  and UserName=@UserName and PermissionType=@PermissionType  ";
        //      SqlParameter[] parameters = {
        //                                      new SqlParameter("@PermissionType", permissionType),
        //                                      new SqlParameter("@UserName", userName),
        //                                       new SqlParameter("@DomainID", domainID)
        //                                   };
        //      return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        //  }

        public int Delete(DomainPermissionEntity domainPermissionEntity)
        {
            const string sql = @"Update zsync_domain_permission
                                            set  DeleteFlag = 1 ,UpdateTime=@UpdateTime,UpdateUserName=@UpdateUserName
                                            WHERE  DomainID=@DomainID  and UserName=@UserName and PermissionType=@PermissionType  ";
            SqlParameter[] parameters = {
                                            new SqlParameter("@PermissionType", domainPermissionEntity.PermissionType),
                                            new SqlParameter("@UserName", domainPermissionEntity.UserName),
                                             new SqlParameter("@DomainID", domainPermissionEntity.DomainID),
                                             new SqlParameter("@UpdateUserName", domainPermissionEntity.UpdateUserName),
                                              new SqlParameter("@UpdateTime", domainPermissionEntity.UpdateTime)
                                         };
            return Convert.ToInt32(ExecuteScalar(CommandType.Text, sql, parameters));
        }
    }
}