/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 时间： 2011/11/24 15:31:11
 * 作者：
 * 版本            时间                  作者                 描述
 * v 1.0    2011/11/24 15:31:11               创建
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

using System.Collections.Generic;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services.Persistence;

namespace Dorado.VWS.Services
{
    public class DomainPermissionProvider
    {
        /// <summary>
        ///     定义域名数据访问操作对象
        /// </summary>
        private readonly DomainPermissionDao _domainPermissionDao = new DomainPermissionDao();

        /// <summary>
        /// 获取域名管理员列表
        /// </summary>
        /// <param name="domainName">域名</param>
        /// <returns></returns>
        public static List<string> GetManageUserListByDomainName(string domainName)
        {
            List<string> list = new List<string>();

            try
            {
                DomainProvider domainProvider = new DomainProvider();
                DomainEntity de = domainProvider.GetDomainByName(domainName);
                if (de == null)
                {
                    return list;
                }
                DomainPermissionDao dpd = new DomainPermissionDao();
                IList<DomainPermissionEntity> dpeList = dpd.GetUsersByDomainAndPermissionType(de.DomainId, (int)Model.Enum.EnumManageType.DailyManage);

                if (dpeList != null)
                {
                    int i = 0;
                    foreach (DomainPermissionEntity dpe in dpeList)
                    {
                        list.Add(dpe.UserName);
                        i++;
                        if (i >= 3)
                        {
                            break;
                        }
                    }
                }
            }
            catch
            {
            }

            return list;
        }

        /// <summary>
        /// 获取已授权用户列表
        /// </summary>
        /// <param name="domianID"></param>
        /// <param name="permissionType"></param>
        /// <returns></returns>
        public IList<DomainPermissionEntity> GetUsersByDomainAndPermissionType(int domianID, int permissionType)
        {
            return _domainPermissionDao.GetUsersByDomainAndPermissionType(domianID, permissionType);
        }

        /// <summary>
        /// 获取用户已授权列表
        /// </summary>
        /// <param name="domianID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public IList<DomainPermissionEntity> GetPermission(int domianID, string userName)
        {
            return _domainPermissionDao.GetPermission(domianID, userName);
        }

        /// <summary>
        /// 获取用户已授权列表
        /// </summary>
        /// <param name="domianID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public DomainPermissionEntity GetPermission(int domianID, string userName, int permissionType)
        {
            return _domainPermissionDao.GetPermission(domianID, userName, permissionType);
        }

        /// <summary>
        /// 判断用户是否有权限
        /// </summary>
        /// <param name="domianID"></param>
        /// <param name="userName"></param>
        /// <param name="permissionType"></param>
        /// <returns></returns>
        public bool HasPermission(int domianID, string userName, int permissionType)
        {
            return (_domainPermissionDao.GetPermission(domianID, userName, permissionType) != null);
        }

        /// <summary>
        /// 判断用户是否有权限
        /// </summary>
        /// <param name="domianID"></param>
        /// <param name="userName"></param>
        /// <param name="permissionType"></param>
        /// <returns></returns>
        public bool HasPermission(int domianID, string userName, EnumManageType manageType)
        {
            return HasPermission(domianID, userName, (int)manageType);
        }

        /// <summary>
        /// 获取某用户的所有域名
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public IList<DomainPermissionEntity> GetDomainsByUser(string username)
        {
            return _domainPermissionDao.GetDomainsByUser(username);
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="domainPermissionEntity"></param>
        /// <returns></returns>
        public int Add(DomainPermissionEntity domainPermissionEntity)
        {
            return _domainPermissionDao.Add(domainPermissionEntity);
        }

        /// <summary>
        /// 删除授权
        /// </summary>
        /// <param name="domainID"></param>
        /// <param name="userName"></param>
        /// <param name="permissionType"></param>
        /// <returns></returns>
        //public int Delete(int domainID, string userName, int permissionType)
        //{
        //    return _domainPermissionDao.Delete(domainID,userName,permissionType);
        //}

        /// <summary>
        /// 删除授权
        /// </summary>
        /// <param name="domainPermissionEntity"></param>
        /// <returns></returns>
        public int Delete(DomainPermissionEntity domainPermissionEntity)
        {
            return _domainPermissionDao.Delete(domainPermissionEntity);
        }
    }
}