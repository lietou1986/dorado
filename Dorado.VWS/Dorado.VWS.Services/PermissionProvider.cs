/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/12 16:11:14
 * 作者：len
 * 联系方式：len@dorado.com 
 * 本类主要用途描述：文件权限业务类
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dorado.VWS.Model;
using Dorado.VWS.Services.Persistence;

#endregion using

namespace Dorado.VWS.Services
{
    public class PermissionProvider
    {
        private readonly PermissionDao _pmsDao = new PermissionDao();

        /// <summary>
        ///     获取某个角色的权限列表
        /// </summary>
        /// <param name = "roleId">角色Id</param>
        /// <returns>结果</returns>
        public IList<PermissionEntity> GetPermissionByRoleId(int roleId)
        {
            return _pmsDao.GetPermissionByRoleId(roleId);
        }

        /// <summary>
        ///     添加权限
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name = "ilist">权限列表</param>
        public void Insert(int roleId, IList<PermissionEntity> ilist)
        {
            SqlTransaction transaction = DBbase.GetNewTransation();

            _pmsDao.Transation = transaction;

            try
            {
                foreach (PermissionEntity permissionEntity in ilist)
                {
                    _pmsDao.Insert(roleId, permissionEntity);
                }
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
            }
            finally
            {
                _pmsDao.Transation = null;
            }
        }

        /// <summary>
        ///     删除某个角色下的所有文件权限
        /// </summary>
        /// <param name = "roleId">角色ID</param>
        public void DeletePermissionByRoleId(int roleId)
        {
            _pmsDao.Delete(roleId);
        }

        /// <summary>
        ///     获取用户某个服务器下的文件列表
        /// </summary>
        /// <param name = "userName">用户ID</param>
        /// <param name = "domainId">域名ID</param>
        /// <returns></returns>
        public IList<PermissionEntity> GetPermissionByUserAndDomain(string userName, int domainId)
        {
            return _pmsDao.GetPermissionByUserAndDomain(userName, domainId);
        }

        /// <summary>
        ///     获取用户某个服务器下的文件列表
        /// </summary>
        /// <param name = "userName">用户ID</param>
        /// <param name = "domainName">域名</param>
        /// <returns></returns>
        public IList<PermissionEntity> GetPermissionByUserAndDomain(string userName, string domainName)
        {
            return _pmsDao.GetPermissionByUserAndDomain(userName, domainName);
        }

        /// <summary>
        ///     获取需要剔除的文件目录和文件
        /// </summary>
        /// <param name = "ilist"></param>
        /// <returns></returns>
        public IEnumerable<PermissionEntity> GetExceptFile(IList<PermissionEntity> ilist)
        {
            //文件夹
            var folderList = ilist.Where(s => s.Type == 0).ToList();
            //文件
            var fileList = ilist.Where(s => s.Type == 1).ToList();

            //保存最大化文件，对于子目录和子文件不插入，只插入父级节点目录和文件
            IList<PermissionEntity> exceptFileList = new List<PermissionEntity>();

            //剔除文件
            foreach (PermissionEntity file in fileList)
            {
                if (folderList.Any(folder => file.Path.IndexOf(folder.Path) == 0))
                {
                    exceptFileList.Add(file);
                }
            }

            //剔除文件夹
            foreach (PermissionEntity childFolder in folderList)
            {
                foreach (PermissionEntity parentFolder in folderList)
                {
                    if (childFolder.Path.Equals(parentFolder.Path))
                    {
                        break;
                    }
                    if (childFolder.Path.IndexOf(parentFolder.Path) != 0) continue;

                    exceptFileList.Add(childFolder);
                    break;
                }
            }
            return exceptFileList;
        }

        /// <summary>
        ///     判断是否有资源权限
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name = "domainId">域名ID</param>
        /// <param name = "resouceValue">资源值</param>
        /// <returns>结果</returns>
        [Obsolete("旧的资源权限判断")]
        public bool HasResoucePermission(string username, int domainId, string resouceValue)
        {
            var userRoleProvider = new UserRoleProvider();
            List<int> userRoleIdList = userRoleProvider.GetUserRoleId(username, domainId);

            var userResourceProvider = new UserResourceProvider();
            if (userRoleIdList.Count > 0)
            {
                IList<UserResoureEntity> listUserResource = userResourceProvider.GetUserResourcePermission(userRoleIdList);

                return listUserResource.Any(userResoureEntity => userResoureEntity.ResourceValue.Equals(resouceValue));
            }
            return false;
        }
    }
}