/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/19 14:41:36
 * 作者：len
 * 联系方式：len@dorado.com 
 * 本类主要用途描述：更新客户端
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
    public class UpdateClientProvider
    {
        private readonly LogProvider _logProvider = new LogProvider();
        private readonly ServerProvider _serProvider = new ServerProvider();
        private readonly SocketClient _socketClient = new SocketClient();

        /// <summary>
        ///     更新客户端
        /// </summary>
        /// <param name = "ip">客户端ip</param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool UpdateClientService(string ip, string userName)
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
                //定义操作日志
                var operateLogEntity = new OperationLogEntity
                                                          {
                                                              UserName = userName,
                                                              OperateType = EnumOperateType.ServerControl,
                                                              DomainName = servers[0].DomainName,
                                                              Log = string.Format("更新客户端服务，服务器IP:{0}", ip)
                                                          };

                var task = new TaskSenderEntity
                                            {
                                                TaskId = 2,
                                                TaskCmd = EnumTaskCmd.UPDATECLIENT,
                                                TargetIP = ip,
                                                DomainType = domainType,
                                                OperatePathType = operatePathType,
                                                OperatePath = operatePath,
                                            };
                TaskResultEntity taskResult = _socketClient.SyncSend(IPAddress.Parse(ip), task);
                if (taskResult == null)
                {
                    operateLogEntity.Result = false;
                    _logProvider.AddOperateLog(operateLogEntity);
                    return false;
                }
                foreach (var server in servers)
                {
                    SetServerInfo(server, taskResult);
                }
                operateLogEntity.Result = taskResult.Success;
                _logProvider.AddOperateLog(operateLogEntity);

                return taskResult.Success;
            }
            return false;
        }

        /// <summary>
        ///     批量更新客户端
        /// </summary>
        /// <param name = "serverList">服务器列表</param>
        /// <param name = "userName">操作人</param>
        public void UpdateClientService(IList<ServerEntity> serverList, string userName)
        {
            //定义操作日志
            var operateLogEntity = new OperationLogEntity
                                                      {
                                                          UserName = userName,
                                                          OperateType = EnumOperateType.ServerControl,
                                                      };
            var iptmp = new List<string>();

            foreach (ServerEntity serverEntity in serverList)
            {
                if (serverEntity.IP != null && !iptmp.Contains(serverEntity.IP))
                {
                    iptmp.Add(serverEntity.IP);
                    var task = new TaskSenderEntity
                                                {
                                                    TaskId = 2,
                                                    TaskCmd = EnumTaskCmd.UPDATECLIENT,
                                                    TargetIP = serverEntity.IP,
                                                    DomainType = (int)serverEntity.DomainId,
                                                    OperatePathType = (int)serverEntity.OperatePathType,
                                                    OperatePath = serverEntity.OperatePath
                                                };
                    TaskResultEntity taskResult = _socketClient.SyncSend(IPAddress.Parse(serverEntity.IP), task);

                    operateLogEntity.DomainName = serverEntity.DomainName;
                    operateLogEntity.Log = string.Format("更新客户端服务，服务器IP:{0}", serverEntity.IP);

                    if (taskResult == null)
                    {
                        operateLogEntity.Result = false;
                        _logProvider.AddOperateLog(operateLogEntity);
                    }
                    else
                    {
                        SetServerInfo(serverEntity, taskResult);

                        operateLogEntity.Result = taskResult.Success;
                        _logProvider.AddOperateLog(operateLogEntity);
                    }
                }
            }
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
    }
}