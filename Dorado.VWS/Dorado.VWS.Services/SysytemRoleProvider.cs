using System.Collections.Generic;
using Dorado.VWS.Model;
using Dorado.VWS.Services.Persistence;

namespace Dorado.VWS.Services
{
    public class SysytemRoleProvider
    {
        private readonly SystemRoleDao _systemRoleDao = new SystemRoleDao();

        /// <summary>
        ///     根据用户名和系统角色删除
        /// </summary>
        /// <param name = "id"></param>
        public void DeleteByUserRoleId(string username, int roleId)
        {
            _systemRoleDao.DeleteByUserRoleId(username, roleId);
        }

        /// <summary>
        ///     添加一个用户的系统角色
        /// </summary>
        public int Add(UserRoleEntity dto)
        {
            return _systemRoleDao.Add(dto);
        }

        /// <summary>
        ///     获取系统角色
        /// </summary>
        /// <returns></returns>
        public IList<SystemRoleEntity> GetSysytemRoleByUser(string userName)
        {
            return _systemRoleDao.GetSysytemRoleByUser(userName);
        }
    }
}