/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/12 16:15:21
 * 作者：len
 * 联系方式：len@dorado.com 
 * 本类主要用途描述：用户角色业务类
 *  -------------------------------------------------------------------------*/

#region using

using System.Collections.Generic;
using Dorado.VWS.Model;
using Dorado.VWS.Services.Persistence;

#endregion using

namespace Dorado.VWS.Services
{
    public class UserRoleProvider
    {
        private readonly UserRoleDao _userRoleDao = new UserRoleDao();

        /// <summary>
        ///     添加一个用户的角色
        /// </summary>
        public int Add(UserRoleEntity dto)
        {
            return _userRoleDao.Add(dto);
        }

        /// <summary>
        ///     添加多个用户的角色
        /// </summary>
        public int Add(IList<UserRoleEntity> dtos)
        {
            int i = 0;
            foreach (UserRoleEntity dto in dtos)
            {
                i += _userRoleDao.Add(dto);
            }
            return i;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public int Delete(int roleID, string userName)
        {
            return _userRoleDao.Delete(roleID, userName);
        }

        /// <summary>
        ///     根据角色ID删除所有的用户
        /// </summary>
        /// <param name = "roleId"></param>
        public void DeleteByRoleId(int roleId)
        {
            _userRoleDao.DeleteByRoleId(roleId);
        }

        /// <summary>
        ///     根据ID删除
        /// </summary>
        /// <param name = "id"></param>
        public void DeleteById(int id)
        {
            _userRoleDao.DeleteById(id);
        }

        /// <summary>
        ///     更新用户角色
        /// </summary>
        public void UpdateUserRole(List<UserRoleEntity> userRoleList, int domainId)
        {
            string userName = string.Empty;
            if (userRoleList.Count > 0)
            {
                userName = userRoleList[0].UserName;
            }
            _userRoleDao.DeleteUserRole(userName, domainId);

            foreach (UserRoleEntity userRoleEntity in userRoleList)
            {
                if (userRoleEntity.roleId >= 1)
                {
                    _userRoleDao.Add(userRoleEntity);
                }
            }
        }

        /// <summary>
        ///     删除用户角色
        /// </summary>
        /// <param name = "userName">用户名</param>
        /// <param name="domainId"></param>
        public void DeleteUserRole(string userName, int domainId)
        {
            _userRoleDao.DeleteUserRole(userName, domainId);
        }

        /// <summary>
        ///     获取用户角色ID列表
        /// </summary>
        /// <param name = "userName">用户账号</param>
        /// <param name="domainId"></param>
        /// <returns></returns>
        public List<int> GetUserRoleId(string userName, int domainId)
        {
            return _userRoleDao.GetUserRoleId(userName, domainId);
        }

        /// <summary>
        ///     获取用户权限
        /// </summary>
        /// <param name="domainId"></param>
        /// <param name = "userName">用户名</param>
        /// <returns></returns>
        public IList<RoleEntity> GetUserRole(int domainId, string userName)
        {
            return _userRoleDao.GetUserRole(domainId, userName);
        }

        /// <summary>
        ///     获取系统有权限的用户账号
        /// </summary>
        /// <returns></returns>
        public IList<string> GetVwsUserName()
        {
            return _userRoleDao.GetVwsUserName();
        }

        #region heyongdong添加的方法

        /// <summary>
        ///     获取角色成员账号
        /// </summary>
        /// <returns></returns>
        public IList<string> GetUsersByRoleId(int roleID)
        {
            return _userRoleDao.GetUsersByRoleId(roleID);
        }

        #endregion heyongdong添加的方法
    }
}