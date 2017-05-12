/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com 
 * 创建时间： 2011/7/6 11:06:16
 * 版本号：v1.0
 * 本类主要用途描述：服务器业务类
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services.Persistence;

#endregion using

namespace Dorado.VWS.Services
{
    public class ServerProvider
    {
        /// <summary>
        ///     定义域名数据访问操作对象
        /// </summary>
        private readonly DomainDao _domainDao = new DomainDao();

        /// <summary>
        ///     定义Idc数据访问操作对象
        /// </summary>
        private readonly IdcDao _idcDao = new IdcDao();

        /// <summary>
        ///     定义服务器数据访问操作对象
        /// </summary>
        private readonly ServerDao _serverDao = new ServerDao();

        /// <summary>
        ///     获取服务器信息
        /// </summary>
        /// <param name = "serverId">服务器Id</param>
        /// <returns>服务器实体</returns>
        public ServerEntity GetServerById(int serverId)
        {
            return _serverDao.GetServerById(serverId);
        }

        /// <summary>
        /// 获取服务器信息
        /// </summary>
        /// <param name="ip">服务器IP</param>
        /// <returns>服务器实体</returns>
        public IList<ServerEntity> GetServersByIP(string ip)
        {
            return _serverDao.GetServersByIP(ip);
        }

        /// <summary>
        ///     获取所有服务器列表
        /// </summary>
        /// <returns>服务器实体列表</returns>
        public IList<ServerEntity> GetAllServers()
        {
            return _serverDao.GetAllServers();
        }

        /// <summary>
        ///     获取所有服务器列表
        /// </summary>
        /// <param name = "beginIndex">开始记录数</param>
        /// <param name = "endIndex">结尾记录数</param>
        /// <returns>服务器实体列表</returns>
        public IList<ServerEntity> GetAllServers(int beginIndex, int endIndex)
        {
            return _serverDao.GetAllServers(beginIndex, endIndex);
        }

        /// <summary>
        ///     获取服务器列表
        ///     备注：域名id等于0，返回所有服务器列表
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <returns>服务器实体列表</returns>
        public IList<ServerEntity> GetServersByDomainId(int domainId)
        {
            IList<ServerEntity> serverList = domainId == 0 ? _serverDao.GetAllServers() : _serverDao.GetServersByDomainId(domainId);

            return serverList;
        }

        /// <summary>
        ///     获取服务器列表
        ///     备注：域名id等于0，返回所有服务器列表
        /// </summary>
        /// <param name = "domain">域名</param>
        /// <returns>服务器实体列表</returns>
        public IList<ServerEntity> GetServersByDomain(string domain)
        {
            if (string.IsNullOrEmpty(domain))
            {
                return null;
            }

            IList<ServerEntity> serverList = _serverDao.GetServersByDomain(domain);

            return serverList;
        }

        /// <summary>
        ///     获取服务器列表
        ///     备注：域名id等于0，返回所有服务器列表
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <param name="beginIndex">起始位</param>
        /// <param name="endIndex">结束位</param>
        /// <returns>服务器实体列表</returns>
        public IList<ServerEntity> GetServersByDomainId(int domainId, int beginIndex, int endIndex)
        {
            IList<ServerEntity> serverList = domainId == 0 ? _serverDao.GetAllServers(beginIndex, endIndex) : _serverDao.GetServersByDomainId(domainId);

            return serverList;
        }

        /// <summary>
        ///     获取用户服务器列表
        ///     备注：域名id等于0，返回用户所有服务器列表
        /// </summary>
        /// <param name = "userName">用户名</param>
        /// <param name = "domainId">域名Id</param>
        /// <param name="beginIndex">起始位</param>
        /// <param name="endIndex">结束位</param>
        /// <returns>服务器实体列表</returns>
        public IList<ServerEntity> GetServersByDomainId(string userName, int domainId, int beginIndex, int endIndex)
        {
            IList<ServerEntity> serverList = domainId == 0 ? _serverDao.GetAllServers(userName, beginIndex, endIndex) : _serverDao.GetServersByDomainId(userName, domainId);

            return serverList;
        }

        /// <summary>
        ///     获取所有服务器数量
        /// </summary>
        /// <returns>服务器数量</returns>
        public int GetAllServersCount()
        {
            return GetAllServersCount("");
        }

        /// <summary>
        /// 获取用户所有服务器数量
        /// </summary>
        /// <param name="userName"></param>
        /// <returns>服务器数量</returns>
        public int GetAllServersCount(string userName)
        {
            return _serverDao.GetAllServersCount(userName);
        }

        #region 雷斌添加

        /// <summary>
        /// 获取服务器数量
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="type">管理类型</param>
        /// <param name="domainID">域名ID</param>
        /// <returns></returns>
        public int GetManageServerCount(string userName, Model.Enum.EnumManageType type, int? domainID)
        {
            return _serverDao.GetManageServerCount(userName, type, domainID);
        }

        #endregion 雷斌添加

        /// <summary>
        ///     获取服务器列表
        /// </summary>
        /// <param name = "idcId">IdcId</param>
        /// <returns>服务器实体列表</returns>
        public IList<ServerEntity> GetServersByIdc(int idcId)
        {
            IList<ServerEntity> serverList = idcId == 0 ? _serverDao.GetAllServers() : _serverDao.GetServersByIdc(idcId);

            return serverList;
        }

        /// <summary>
        ///     获取用户拥有权限的启用状态域名列表
        /// </summary>
        /// <param name = "userName">用户名</param>
        /// <param name = "syncType">同步类型（1.普通同步【有版本】；2.简单同步【无版本】）</param>
        /// <returns>域名实体列表</returns>
        public IList<DomainEntity> GetDomainsByUser(string userName, int? syncType)
        {
            return _domainDao.GetDomainsByUser(userName, syncType);
        }

        /// <summary>
        ///     获取用户拥有权限的启用状态域名列表
        /// </summary>
        /// <param name = "userName">用户名</param>
        /// <param name = "syncType">同步类型（1.普通同步【有版本】；2.简单同步【无版本】）</param>
        /// <returns>域名实体列表</returns>
        public IList<DomainEntity> GetDomainsByUser(string userName, int? syncType, string environment)
        {
            return _domainDao.GetDomainsByUser(userName, syncType, environment);
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
        ///     获取域名列表
        ///     备注：Idcid等于0，返回所有域名列表
        /// </summary>
        /// <param name = "idcId">idcId</param>
        /// <returns>域名实体列表</returns>
        public IList<DomainEntity> GetDomainsByIdcId(int idcId)
        {
            IList<DomainEntity> domainList = idcId == 0 ? _domainDao.GetAllDomains() : _domainDao.GetDomainsByIdcId(idcId);

            return domainList;
        }

        /// <summary>
        ///     获取域名列表
        ///     备注：Idcid等于0，返回所有域名列表
        /// </summary>
        /// <param name = "idcId">idcId</param>
        /// <returns>域名实体列表</returns>
        public IList<DomainEntity> GetDomainsByIdcIdAndEnvironment(string environment, int idcId)
        {
            IList<DomainEntity> domainList = idcId == 0 ? _domainDao.GetAllDomains() : _domainDao.GetDomainsByIdcId(environment, idcId);

            return domainList;
        }

        /// <summary>
        ///     置域名为启用状态
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool SetDomainEnable(int domainId)
        {
            return _domainDao.UpdateStatus(domainId, true);
        }

        /// <summary>
        ///     置域名为停用状态
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool SetDomainDisable(int domainId)
        {
            return _domainDao.UpdateStatus(domainId, false);
        }

        /// <summary>
        ///     获取所有idc列表
        /// </summary>
        /// <returns>Idc实体列表</returns>
        public IList<IdcEntity> GetAllIdcs()
        {
            return _idcDao.GetAllIdcs();
        }

        /// <summary>
        ///     插入Idc数据
        /// </summary>
        /// <param name = "idcEntity">Idc实体信息</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool InsertIdc(IdcEntity idcEntity)
        {
            IdcEntity idc = _idcDao.GetIdcByName(idcEntity.IdcName);
            //如果已经存在这个idc名称，不允许添加
            if (idc == null)
            {
                int result = _idcDao.Insert(idcEntity);
                return result > 0;
            }
            return false;
        }

        /// <summary>
        ///     插入域名数据
        /// </summary>
        /// <param name = "domainEntity">域名实体</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool InsertDomain(DomainEntity domainEntity)
        {
            //如果已经存在这个域名，不允许添加
            if (!_domainDao.ExistsDomain(domainEntity))
            {
                int result = _domainDao.Insert(domainEntity);
                if (result > 0)
                {
                    SecurityExt.RemoveAllCache();//清除所有安全相关的缓存
                }
                return result > 0;
            }
            return false;
        }

        /// <summary>
        ///     插入服务器数据
        /// </summary>
        /// <param name = "serverEntity">服务器实体</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool InsertServer(ServerEntity serverEntity)
        {
            bool result = false;
            switch (serverEntity.ServerType)
            {
                case EnumServerType.Host:
                    {
                        if (!_serverDao.ExistsHostCommonIp(serverEntity))
                        {
                            result = _serverDao.Insert(serverEntity) > 0;
                        }
                        break;
                    }
                case EnumServerType.Relay:
                    {
                        if (!_serverDao.ExistsRelay(serverEntity))
                        {
                            result = _serverDao.Insert(serverEntity) > 0;
                        }
                        break;
                    }
                case EnumServerType.Source:
                    {
                        if (!_serverDao.ExistsSource(serverEntity))
                        {
                            result = _serverDao.Insert(serverEntity) > 0;
                        }
                        break;
                    }
            }
            if (result)
            {
                SecurityExt.RemoveAllCache();//清除所有安全相关的缓存
            }
            return result;
        }

        /// <summary>
        ///     修改域名数据
        /// </summary>
        /// <param name = "domainEntity">域名实体</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool UpdateDomain(DomainEntity domainEntity)
        {
            bool result = false;
            //如果已经存在这个域名，不允许修改
            if (!_domainDao.ExistsDomain(domainEntity))
            {
                result = _domainDao.Update(domainEntity);
                if (result)
                {
                    SecurityExt.RemoveAllCache();//清除所有安全相关的缓存
                }
            }
            return result;
        }

        public bool UpdateLastHeartBeatDate(string IP, DateTime? LastHeartBeatDate)
        {
            bool result = false;
            result = _serverDao.UpdateLastHeartBeatDate(IP, LastHeartBeatDate);
            return result;
        }

        /// <summary>
        ///     修改服务器数据
        /// </summary>
        /// <param name = "serverEntity">服务器实体</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool UpdateServer(ServerEntity serverEntity)
        {
            bool result = false;
            switch (serverEntity.ServerType)
            {
                case EnumServerType.Host:
                    {
                        if (!_serverDao.ExistsHostCommonIp(serverEntity))
                        {
                            result = _serverDao.Update(serverEntity);
                        }
                        break;
                    }
                case EnumServerType.Relay:
                    {
                        if (!_serverDao.ExistsRelay(serverEntity))
                        {
                            result = _serverDao.Update(serverEntity);
                        }
                        break;
                    }
                case EnumServerType.Source:
                    {
                        if (!_serverDao.ExistsSource(serverEntity))
                        {
                            result = _serverDao.Update(serverEntity);
                        }
                        break;
                    }
            }
            if (result)
            {
                SecurityExt.RemoveAllCache();//清除所有安全相关的缓存
            }
            return result;
        }

        /// <summary>
        ///     删除域名数据
        /// </summary>
        /// <param name = "domainId">域名Id</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool DeleteDoamin(int domainId)
        {
            SqlTransaction transaction = DBbase.GetNewTransation();

            _domainDao.Transation = transaction;
            try
            {
                _domainDao.Delete(domainId);

                transaction.Commit();

                SecurityExt.RemoveAllCache();//清除所有安全相关的缓存

                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
            finally
            {
                _domainDao.Transation = null;
            }
        }

        /// <summary>
        ///     删除服务器数据
        /// </summary>
        /// <param name = "serverId">服务器Id</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool DeleteServer(int serverId)
        {
            if (_serverDao.Delete(serverId))
            {
                SecurityExt.RemoveAllCache();//清除所有安全相关的缓存
                return true;
            }
            else
            {
                return false;
            };
        }

        /// <summary>
        ///     删除Idc数据
        /// </summary>
        /// <param name = "idcId">idcId</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool DeleteIdc(int idcId)
        {
            SqlTransaction transaction = DBbase.GetNewTransation();

            _idcDao.Transation = transaction;
            try
            {
                _idcDao.Delete(idcId);

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
            finally
            {
                _idcDao.Transation = null;
            }
        }

        /// <summary>
        ///     获取源服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="root"></param>
        /// <returns>服务器实体列表</returns>
        public ServerEntity GetSourceServersByIpAndRoot(string ip, string root)
        {
            return _serverDao.GetSourceServersByIpAndRoot(ip, root);
        }

        public ServerEntity GetSourceServerByDomainId(int domainid)
        {
            return _serverDao.GetSourceServersByDomainId(domainid);
        }

        public ServerEntity GetSourceServerByDomain(string domain)
        {
            return _serverDao.GetSourceServersByDomain(domain);
        }

        public IList<ServerEntity> GetTargetServerByDomainId(int domainid)
        {
            return _serverDao.GetTargetServersByDomainId(domainid);
        }

        public IList<ServerEntity> GetTargetServerByDomain(string domain)
        {
            return _serverDao.GetTargetServersByDomain(domain);
        }

        public ServerEntity GetTargetServerByDomainIdAndIP(int domainid, string ip)
        {
            return _serverDao.GetTargetServersByDomainIdAndIP(domainid, ip);
        }

        #region 业务逻辑判断（付峰）

        public bool ExistsSource(ServerEntity serverEntity)
        {
            return _serverDao.ExistsSource(serverEntity);
        }

        public bool ExistsHostCommonIp(ServerEntity serverEntity)
        {
            return _serverDao.ExistsHostCommonIp(serverEntity);
        }

        public bool ExistsRelay(ServerEntity serverEntity)
        {
            return _serverDao.ExistsRelay(serverEntity);
        }

        #endregion 业务逻辑判断（付峰）

        #region heyongdong add

        public List<string> GetAllowIp(string ip)
        {
            return _serverDao.GetAllowIp(ip);
        }

        #endregion heyongdong add

        public IList<ServerEntity> GetServersByDomainIdForServerList(int domainId, int beginIndex, int endIndex)
        {
            IList<ServerEntity> serverList = domainId == 0 ? _serverDao.GetAllServersForServerList(beginIndex, endIndex) : _serverDao.GetServersByDomainIdForServerList(domainId);

            return serverList;
        }

        public IList<ServerEntity> GetServersByDomainIdForServerList(string userName, int domainId, int beginIndex, int endIndex)
        {
            IList<ServerEntity> serverList = domainId == 0 ? _serverDao.GetAllServersForServerList(userName, beginIndex, endIndex) : _serverDao.GetServersByDomainIdForServerList(userName, domainId);

            return serverList;
        }

        public ServerEntity GetServerByIdForServerList(int serverId)
        {
            return _serverDao.GetServerByIdForServerList(serverId);
        }

        public IList<ServerEntity> GetServersByIdcForServerList(int idcId)
        {
            IList<ServerEntity> serverList = idcId == 0 ? _serverDao.GetAllServersForServerList() : _serverDao.GetServersByIdcForServerList(idcId);

            return serverList;
        }

        public IList<ServerEntity> GetServersByDomainIdForServerList(int domainId)
        {
            IList<ServerEntity> serverList = domainId == 0 ? _serverDao.GetAllServersForServerList() : _serverDao.GetServersByDomainIdForServerList(domainId);

            return serverList;
        }

        public int GetAllServersCountForServerList()
        {
            return GetAllServersCountForServerList("");
        }

        private int GetAllServersCountForServerList(string userName)
        {
            return _serverDao.GetAllServersCountForServerList(userName);
        }

        public int GetManageServerCountForServerList(string userName, EnumManageType type, int? domainID)
        {
            return _serverDao.GetManageServerCountForServerList(userName, type, domainID);
        }

        public bool UnDeleteServer(int serverId)
        {
            if (_serverDao.UnDelete(serverId))
            {
                SecurityExt.RemoveAllCache();//清除所有安全相关的缓存
                return true;
            }
            else
            {
                return false;
            };
        }
    }
}