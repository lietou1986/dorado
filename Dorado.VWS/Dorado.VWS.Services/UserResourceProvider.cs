/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/12 16:13:37
 * 作者：len
 * 联系方式：len@dorado.com 
 * 本类主要用途描述：资源权限业务类
 *  -------------------------------------------------------------------------*/

#region using

using System.Collections.Generic;
using Dorado.VWS.Model;
using Dorado.VWS.Services.Persistence;

#endregion using

namespace Dorado.VWS.Services
{
    public class UserResourceProvider
    {
        private readonly ResourceDao _resourceDao = new ResourceDao();
        private readonly UserResourceDao _userResourceDao = new UserResourceDao();

        /// <summary>
        ///     获取用户拥有的资源权限
        /// </summary>
        /// <param name = "userRoleIdList">用户权限ID列表</param>
        /// <returns></returns>
        public IList<UserResoureEntity> GetUserResourcePermission(List<int> userRoleIdList)
        {
            return _userResourceDao.GetUserResourcePermission(userRoleIdList);
        }

        /// <summary>
        ///     修改用户资源权限
        ///     原理：先删除以前数据然后添加
        /// </summary>
        /// <param name = "oldUserRoleIdList">用户老权限ID列表</param>
        /// <param name = "newUserRoleIdList">用户新权限ID列表</param>
        /// <param name = "listResourceId">资源ID列表</param>
        /// <returns></returns>
        public bool UpdateUserResource(List<int> oldUserRoleIdList, List<int> newUserRoleIdList,
                                       List<int> listResourceId)
        {
            _userResourceDao.DeleteUserResource(oldUserRoleIdList);

            foreach (int newUserRoleId in newUserRoleIdList)
            {
                foreach (int resourceId in listResourceId)
                {
                    var flag = _userResourceDao.InsertUserResource(newUserRoleId, resourceId);
                    if (!flag)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        ///     获取资源信息
        /// </summary>
        /// <param name = "resourceId">资源Id</param>
        /// <returns>资源实体</returns>
        public ResourceEntity GetResourceById(int resourceId)
        {
            return _resourceDao.GetResourceById(resourceId);
        }

        /// <summary>
        ///     获取所有可用资源
        /// </summary>
        /// <returns></returns>
        public IList<ResourceEntity> GetResourceList()
        {
            return _resourceDao.GetResourceList();
        }

        /// <summary>
        ///     插入资源数据
        /// </summary>
        /// <param name = "resoureEntity">资源实体信息</param>
        /// <returns>记录Id</returns>
        public bool InsertResource(ResourceEntity resoureEntity)
        {
            //如果已经存在这个资源值，不允许添加
            if (!_resourceDao.ExistsResource(resoureEntity))
            {
                int result = _resourceDao.Insert(resoureEntity);
                return result > 0;
            }
            return false;
        }

        /// <summary>
        ///     修改资源数据
        /// </summary>
        /// <param name = "resoureEntity">资源实体信息</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool UpdateResource(ResourceEntity resoureEntity)
        {
            //如果已经存在这个资源值，不允许修改
            return !_resourceDao.ExistsResource(resoureEntity) && _resourceDao.Update(resoureEntity);
        }

        /// <summary>
        ///     删除资源数据
        /// </summary>
        /// <param name = "resourceId">资源Id</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool DeleteResource(int resourceId)
        {
            return _resourceDao.Delete(resourceId);
        }
    }
}