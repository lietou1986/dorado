/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 时间： 2011/10/25 14:43:04
 * 作者：
 * 版本            时间                  作者                 描述
 * v 1.0    2011/10/25 14:43:04               创建
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

using System.Collections.Generic;
using Dorado.VWS.Model;
using Dorado.VWS.Services.Persistence;

namespace Dorado.VWS.Services
{
    public class DomainProvider
    {
        /// <summary>
        ///     定义域名数据访问操作对象
        /// </summary>
        private readonly DomainDao _domainDao = new DomainDao();

        /// <summary>
        ///     获取域名信息
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <returns>域名实体</returns>
        public IList<DomainEntity> GetAllDomains()
        {
            return _domainDao.GetAllDomains();
        }

        /// <summary>
        ///     获取域名信息
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <returns>域名实体</returns>
        public DomainEntity GetDomainById(int domainId)
        {
            return _domainDao.GetDomainById(domainId);
        }

        /// <summary>
        /// 根据域名获取信息
        /// </summary>
        /// <param name="domainName"></param>
        public DomainEntity GetDomainByName(string domainName)
        {
            return _domainDao.GetDomainByName(domainName);
        }

        #region 安全控制

        /// <summary>
        ///     获取指定ip所有域名列表
        /// </summary>
        /// <returns>域名实体列表</returns>
        public IList<DomainEntity> GetDomains(string ip)
        {
            return _domainDao.GetDomains(ip);
        }

        #endregion 安全控制
    }
}