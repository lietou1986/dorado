/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 时间： 2014/6/16 14:43:04
 * 作者：
 * 版本            时间                  作者                 描述
 * v 1.0   2014/6/16 14:43:04               创建
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/

using System.Collections.Generic;
using Dorado.VWS.Model;
using Dorado.VWS.Services.Persistence;

namespace Dorado.VWS.Services
{
    public class FilesWatcherProvider
    {
        /// <summary>
        ///     定义域名数据访问操作对象
        /// </summary>
        private readonly FilesWatcherDao _fwDao = new FilesWatcherDao();

        /// <summary>
        /// 获取文件变更数据
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public IList<FilesWatcherEntity> GetFilesWatcherById(int id)
        {
            return _fwDao.GetFilesWatcherById(id);
        }

        /// <summary>
        /// 获取文件变更数据
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        public IList<FilesWatcherEntity> GetFilesWatcherByServerId(int serverId)
        {
            return _fwDao.GetFilesWatcherByServerId(serverId);
        }

        /// <summary>
        /// 添加文件变更数据
        /// </summary>
        /// <param name="fwentity"></param>
        /// <returns></returns>
        public int Insert(FilesWatcherEntity fwentity)
        {
            return _fwDao.Insert(fwentity);
        }

        /// <summary>
        /// 删除文件变更数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(IList<int> id)
        {
            return _fwDao.Delete(id);
        }
    }
}