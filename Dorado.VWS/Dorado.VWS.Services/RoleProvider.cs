/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/12 16:07:43
 * 作者：len
 * 联系方式：len@dorado.com 
 * 本类主要用途描述：角色业务类
 *  -------------------------------------------------------------------------*/

#region using

using System.Collections.Generic;
using System.Data.SqlClient;
using Dorado.VWS.Model;
using Dorado.VWS.Services.Persistence;

#endregion using

namespace Dorado.VWS.Services
{
    public class RoleProvider
    {
        private readonly RoleDao _roleDao = new RoleDao();
        private readonly PermissionDao _permissionDao = new PermissionDao();

        /// <summary>
        ///     添加一个角色
        /// </summary>
        /// <param name = "dto">角色实体类</param>
        /// <param name = "ilist">权限列表</param>
        /// <returns></returns>
        public bool Add(RoleEntity dto, IList<PermissionEntity> ilist)
        {
            SqlTransaction transaction = DBbase.GetNewTransation();

            _roleDao.Transation = transaction;
            _permissionDao.Transation = transaction;

            bool flag = _roleDao.Exist(dto);
            try
            {
                if (!flag)
                {
                    int roleid = _roleDao.Add(dto);
                    foreach (PermissionEntity permissionEntity in ilist)
                    {
                        _permissionDao.Insert(roleid, permissionEntity);
                    }
                    transaction.Commit();
                    flag = true;
                }
                else
                {
                    flag = false;
                }
                return flag;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
            finally
            {
                _roleDao.Transation = null;
                _permissionDao.Transation = null;
            }
        }

        /// <summary>
        ///     删除一个角色
        /// </summary>
        /// <param name = "roleID">角色ID</param>
        public void Delete(int roleID)
        {
            var userRoleProvider = new UserRoleProvider();
            _roleDao.Delete(roleID);
            _permissionDao.Delete(roleID);
            userRoleProvider.DeleteByRoleId(roleID);
        }

        public bool Edit(RoleEntity roleEntity, IList<PermissionEntity> ilist)
        {
            bool flag = _roleDao.EditExist(roleEntity);
            if (!flag)
            {
                _roleDao.Edit(roleEntity);
                var Provider = new PermissionProvider();
                Provider.DeletePermissionByRoleId(roleEntity.Id);
                Provider.Insert(roleEntity.Id, ilist);
            }
            return !flag;
        }

        /// <summary>
        ///     获取一个角色
        /// </summary>
        /// <param name = "roleId">角色ID</param>
        /// <returns></returns>
        public RoleEntity GetRoleByID(int roleId)
        {
            return _roleDao.GetRoleById(roleId);
        }

        /// <summary>
        ///     更具域名ID获取角色
        /// </summary>
        /// <param name = "DomainID">域名ID</param>
        /// <returns></returns>
        public IList<RoleEntity> GetRoleByDomainId(int DomainID)
        {
            return _roleDao.GetRoleByDomainId(DomainID);
        }

        /// <summary>
        ///     获取所有的角色列表
        /// </summary>
        /// <param name = "domainId">域名ID</param>
        /// <returns></returns>
        public IList<RoleEntity> GetAllRole(int domainId)
        {
            return _roleDao.GetAllRole(domainId);
        }
    }
}