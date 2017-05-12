/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com 
 * 创建时间： 2011/7/6 11:06:16
 * 版本号：v1.0
 * 本类主要用途描述：测试连接类
 *  -------------------------------------------------------------------------*/

#region using

using System.Collections.Generic;
using System.Net;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Utils;

#endregion using

namespace Dorado.VWS.Services
{
    public class TestConnectProvider
    {
        private readonly ServerProvider _serProvider = new ServerProvider();
        private readonly SocketClient _socketClient = new SocketClient();
        private readonly DomainProvider _domainProvider = new DomainProvider();

        public bool TestConnect(int serverid)
        {
            ServerEntity server = _serProvider.GetServerById(serverid);

            if (server != null)
            {
                var task = new TaskSenderEntity
                                            {
                                                TaskId = 2,
                                                TaskCmd = EnumTaskCmd.HELLO,
                                                TargetIP = server.IP,
                                                DomainType = (int)server.DomainType,
                                                OperatePathType = (int)server.OperatePathType,
                                                OperatePath = server.OperatePath,
                                            };
                TaskResultEntity taskResult = _socketClient.SyncSend(IPAddress.Parse(server.IP), task);
                if (taskResult == null)
                {
                    return false;
                }

                SetServerInfo(server, taskResult);
                return taskResult.Success;
            }
            return false;
        }

        public bool TestConnectByDomainId(int domainId)
        {
            ServerEntity server = _serProvider.GetSourceServerByDomainId(domainId);
            DomainEntity domain = _domainProvider.GetDomainById(domainId);
            if (server != null)
            {
                var task = new TaskSenderEntity
                {
                    TaskId = 2,
                    TaskCmd = EnumTaskCmd.HELLO,
                    TargetIP = server.IP,
                    DomainType = (int)server.DomainType,
                    OperatePathType = (int)server.OperatePathType,
                    OperatePath = server.OperatePath,
                };
                TaskResultEntity taskResult = _socketClient.SyncSend(IPAddress.Parse(server.IP), task);
                if (taskResult == null)
                {
                    return false;
                }

                SetServerInfo(server, taskResult);
                return taskResult.Success;
            }
            return false;
        }

        public bool TestConnect(string ip)
        {
            IList<ServerEntity> servers = _serProvider.GetServersByIP(ip);
            int domainType = 0;
            int operatePathType = 0;
            string operatePath = string.Empty;
            if (servers != null && servers.Count > 0)
            {
                foreach (ServerEntity server in servers)
                {
                    if ((int)server.DomainType != 0)
                    {
                        domainType = (int)server.DomainType;
                    }
                    if ((int)server.OperatePathType != 0)
                    {
                        operatePathType = (int)server.OperatePathType;
                    }
                    if (!string.IsNullOrEmpty(server.OperatePath))
                    {
                        operatePath = server.OperatePath;
                    }
                }
                var task = new TaskSenderEntity
                                            {
                                                TaskId = 2,
                                                TaskCmd = EnumTaskCmd.HELLO,
                                                TargetIP = ip,
                                                DomainType = domainType,
                                                OperatePathType = operatePathType,
                                                OperatePath = operatePath,
                                            };
                TaskResultEntity taskResult = _socketClient.SyncSend(IPAddress.Parse(ip), task);
                if (taskResult == null)
                {
                    return false;
                }
                foreach (var server in servers)
                {
                    SetServerInfo(server, taskResult);
                }
                return taskResult.Success;
            }

            return false;
        }

        private void SetServerInfo(ServerEntity server, TaskResultEntity result)
        {
            bool modify = false;
            //注释以下代码以取消TestConnect后自动数据库中配置的ip   by heyogndong
            //if (server.IP != result.IP)
            //{
            //    server.IP = result.IP;
            //    modify = true;
            //}

            if (server.HostName != result.HostName)
            {
                server.HostName = result.HostName;
                modify = true;
            }

            if (server.IISStatus != (EnumIISStatus)(result.IISStatus))
            {
                server.IISStatus = (EnumIISStatus)(result.IISStatus);
                modify = true;
            }

            if (server.ClientVersion != result.ClientVersion)
            {
                server.ClientVersion = result.ClientVersion;
                modify = true;
            }

            if (modify)
            {
                _serProvider.UpdateServer(server);
            }
        }

        /// <summary>
        /// 获取服务器服务状态  ad by heyogndong
        /// </summary>
        /// <param name="serverid"></param>
        /// <returns></returns>
        public TaskResultEntity GetServerStatus(int serverid)
        {
            var server = _serProvider.GetServerById(serverid);
            var domian = _serProvider.GetDomainById(server.DomainId);
            if (server != null && domian != null)
            {
                var task = new TaskSenderEntity
                {
                    TaskId = 2,
                    TaskCmd = EnumTaskCmd.HELLO,
                    TargetIP = server.IP,
                    IISSiteName = domian.IISSiteName,
                    WinServiceName = domian.WinServiceName,
                    DomainType = (int)server.DomainType,
                    OperatePathType = (int)server.OperatePathType,
                    OperatePath = server.OperatePath,
                };
                TaskResultEntity taskResult = _socketClient.SyncSend(IPAddress.Parse(server.IP), task);

                //SetServerInfo(server, taskResult);
                return taskResult;
            }
            return null;
        }
    }
}