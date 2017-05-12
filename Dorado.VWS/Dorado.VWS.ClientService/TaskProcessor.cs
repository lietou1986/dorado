/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/4 10:10:11
 * 作者：len
 * 联系方式：len@dorado.com
 * 本类主要用途描述：任务处理类
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.VWS.Utils;

#endregion using

namespace Dorado.VWS.ClientHost
{
    /// <summary>
    ///     任务处理类
    /// </summary>
    internal class TaskProcessor
    {
        private readonly Dictionary<string, List<FileCompress>> _dictFileCompress =
            new Dictionary<string, List<FileCompress>>();

        private readonly FileHelper _fileHelper = new FileHelper();
        private readonly IISService _iisService = new IISService();

        private readonly SocketFileServer _socketFileServer = SocketFileServer.GetInstance();
        private readonly SocketServer _socketServer = SocketServer.GetInstance();
        private readonly string _updateFilePath = AssemblyHelper.GetFileLocation(ClientConst.UpdateName);
        private readonly WinServiceHelper _winSvcHelper = new WinServiceHelper();

        internal TaskProcessor()
        {
            _socketServer.ReceiveTask += ProcessTask;
        }

        /// <summary>
        ///     开始处理
        /// </summary>
        /// <returns></returns>
        internal bool Start()
        {
            return _socketFileServer.Start() && _socketServer.Start();
        }

        /// <summary>
        ///     关闭处理
        /// </summary>
        internal void Close()
        {
            _socketServer.Close();
            _socketFileServer.Close();
        }

        /// <summary>
        ///     收到任务后的处理入口
        /// </summary>
        /// <param name = "task">任务</param>
        /// <returns>处理结果</returns>
        private TaskResultEntity ProcessTask(TaskSenderEntity task)
        {
            string resultIP = ClientConst.LocalIP.Split('|')[0];
            try
            {
                resultIP = ClientConst.LocalIP.Split('|').SingleOrDefault(p => p == task.TargetIP);
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS", ex.Message);
            }

            var taskResult = new TaskResultEntity
                                              {
                                                  IP = resultIP,
                                                  TaskId = task.TaskId,
                                                  TaskCmd = task.TaskCmd,
                                                  Success = false,
                                                  HostName = ClientConst.HostName,
                                                  UserName = task.UserName,
                                              };
            try
            {
                taskResult.DomainType = task.DomainType;
                taskResult.OperatePathType = task.OperatePathType;
                if (task.OperatePath != null)
                {
                    taskResult.OperatePath = task.OperatePath;
                }
            }
            catch (Exception)
            {
                taskResult.DomainType = 0;
                taskResult.OperatePathType = 0;
                taskResult.OperatePath = "";
            }
            try
            {
                switch (task.TaskCmd)
                {
                    case EnumTaskCmd.HELLO:
                        //获取客户端基本信息
                        taskResult.Success = ClientConst.LocalIP.Contains(task.TargetIP);
                        taskResult.IISStatus = _iisService.IISAppPoolStatus(task.IISSiteName);
                        taskResult.RelateSvcStatus = CommonHelper.ServiceStatus(task.WinServiceName);
                        taskResult.ClientVersion = ClientConst.ClientVersion;

                        string htmlFileKey = task.SourceRoot + EnumCompresssType.HtmlCompress.ToString();
                        string jsCssFileKey = task.SourceRoot + EnumCompresssType.JsCssCompress.ToString();
                        taskResult.EnableHtmlCompress = _dictFileCompress.ContainsKey(htmlFileKey);
                        taskResult.EnableJSCssCompress = _dictFileCompress.ContainsKey(jsCssFileKey);
                        break;

                    case EnumTaskCmd.GETFILELIST:
                        //获取文件目录树
                        taskResult.FileList = _fileHelper.GetFileList(task.TargetRoot);
                        taskResult.Success = true;
                        break;

                    case EnumTaskCmd.GetFileListNoMd5:
                        //获取文件目录树（不计算MD5)
                        taskResult.FileList = _fileHelper.GetFileListNoMd5(task.TargetRoot);
                        taskResult.Success = true;
                        break;

                    case EnumTaskCmd.GETALLFILENAME:
                        //获取文件名
                        taskResult.AllFileList = _fileHelper.GetAllFileName(task.TargetRoot);
                        taskResult.Success = true;
                        break;

                    case EnumTaskCmd.UPDATECLIENT:
                        //更新客户端程序
                        taskResult.Success = UpdateClient();
                        taskResult.IISStatus = _iisService.IISSiteStatus(task.IISSiteName);
                        taskResult.ClientVersion = ClientConst.ClientVersion;
                        break;

                    case EnumTaskCmd.IISSTOP:
                        //停止IIS站点服务
                        string stopMessage;
                        taskResult.Success = _iisService.AppPoolInvoke(EnumIISOperate.Stop, task.IISSiteName,
                                                                    out stopMessage);
                        taskResult.ErrorMsg = stopMessage;
                        break;

                    case EnumTaskCmd.IISSTART:
                        //启动IIS站点服务
                        string startMessage;
                        taskResult.Success = _iisService.AppPoolInvoke(EnumIISOperate.Start, task.IISSiteName,
                                                                    out startMessage);
                        taskResult.ErrorMsg = startMessage;
                        break;

                    case EnumTaskCmd.IISRESTART:
                        //重启IIS站点服务
                        string restartMessage = string.Empty;
                        //回收应用程序池
                        taskResult.Success = _iisService.AppPoolInvoke(EnumIISOperate.ReStart, task.IISSiteName, out restartMessage);
                        LoggerWrapper.Logger.Info("VWS.ClientHost", "重启应用程序池：" + restartMessage);
                        //重启站点
                        taskResult.Success = _iisService.SiteInvoke(EnumIISOperate.ReStart, task.IISSiteName, out restartMessage);
                        LoggerWrapper.Logger.Info("VWS.ClientHost", "重启站点：" + restartMessage);
                        taskResult.ErrorMsg = restartMessage;
                        break;

                    case EnumTaskCmd.WINSERVICESTOP:
                        if (Security.CheckWindowsService(task.WinServiceName))
                        {
                            //停止Windows服务
                            if (CommonHelper.ServiceStatus(task.WinServiceName) == 1)
                                taskResult.Success = WinServiceStop(task.WinServiceName);
                            taskResult.RelateSvcStatus = 2;
                        }
                        else
                        {
                            taskResult.Success = false;
                            taskResult.ErrorMsg = "无权操作服务[" + task.WinServiceName + "]";
                        }
                        break;

                    case EnumTaskCmd.WINSERVICESTART:
                        if (Security.CheckWindowsService(task.WinServiceName))
                        {
                            //启动Windows服务
                            if (CommonHelper.ServiceStatus(task.WinServiceName) == 2)
                                taskResult.Success = WinServiceStart(task.WinServiceName);
                            taskResult.RelateSvcStatus = 1;
                        }
                        else
                        {
                            taskResult.Success = false;
                            taskResult.ErrorMsg = "无权操作服务[" + task.WinServiceName + "]";
                        }
                        break;

                    case EnumTaskCmd.WINSERVICERESTART:
                        if (Security.CheckWindowsService(task.WinServiceName))
                        {
                            //重启Windows服务
                            if (CommonHelper.ServiceStatus(task.WinServiceName) == 1)
                                taskResult.Success = WinServiceReStart(task.WinServiceName);
                            taskResult.RelateSvcStatus = 1;
                        }
                        else
                        {
                            taskResult.Success = false;
                            taskResult.ErrorMsg = "无权操作服务[" + task.WinServiceName + "]";
                        }
                        break;

                    case EnumTaskCmd.SYNCFILES:
                        //同步文件
                        taskResult.Success = SyncFiles(task);
                        break;

                    case EnumTaskCmd.ROLLBACKFILES:
                        //恢复文件
                        taskResult.Success = RollbackFiles(task);
                        break;

                    case EnumTaskCmd.COMMITFILES:
                        //确认完成
                        taskResult.Success = CommitFiles(task);
                        break;

                    case EnumTaskCmd.BACKUPFILES:
                        //备份任务文件
                        taskResult.Success = BackupFiles(task);
                        taskResult.FileList = _fileHelper.GetFileList(task);
                        break;

                    case EnumTaskCmd.COMPRESSFILES:
                        //压缩文件
                        taskResult.Success = CompressFiles(task);
                        break;

                    case EnumTaskCmd.REVERTFILES:
                        //回滚任务
                        taskResult.Success = RevertFiles(task);
                        break;

                    case EnumTaskCmd.CHECKFILELIST:
                        //检查文件列表是否存在
                        string erroMsg;
                        taskResult.Success = CheckFilesExist(task, out erroMsg);
                        taskResult.ErrorMsg = erroMsg;
                        break;
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS.ClientHost", ex.ToString());
                taskResult.ErrorMsg = ex.ToString();
            }

            return taskResult;
        }

        /// <summary>
        ///     检查文件列表是否存在
        /// </summary>
        /// <param name = "task">任务</param>
        /// <param name = "nonExist">不存在的文件列表</param>
        /// <returns>结果</returns>
        private bool CheckFilesExist(TaskSenderEntity task, out string nonExist)
        {
            string[] files = task.AddList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            IList<string> nonList = files.Where(file => !File.Exists(task.TargetRoot + file)).ToList();

            nonExist = string.Join(ClientConst.FileSeparator, nonList.ToArray());
            return string.IsNullOrEmpty(nonExist);
        }

        /// <summary>
        ///     停止Windows服务
        /// </summary>
        /// <param name = "svcNames">服务名称集合</param>
        /// <returns></returns>
        private bool WinServiceStop(string svcNames)
        {
            if (!string.IsNullOrEmpty(svcNames))
            {
                IList<string> svcNameList = CommonHelper.ConvertByComma(svcNames);

                return svcNameList.All(svc => _winSvcHelper.StopService(svc));
            }
            return true;
        }

        /// <summary>
        ///     开启Windows服务
        /// </summary>
        /// <param name = "svcNames">服务名称集合</param>
        /// <returns></returns>
        private bool WinServiceStart(string svcNames)
        {
            if (!string.IsNullOrEmpty(svcNames))
            {
                IList<string> svcNameList = CommonHelper.ConvertByComma(svcNames);

                return svcNameList.All(svc => _winSvcHelper.StartService(svc));
            }
            return true;
        }

        /// <summary>
        ///     重启Windows服务
        /// </summary>
        /// <param name = "svcNames">服务名称集合</param>
        /// <returns></returns>
        private bool WinServiceReStart(string svcNames)
        {
            if (!string.IsNullOrEmpty(svcNames))
            {
                IList<string> svcNameList = CommonHelper.ConvertByComma(svcNames);

                return svcNameList.All(svc => _winSvcHelper.ReStartService(svc));
            }
            return true;
        }

        /// <summary>
        ///     更新客户端
        /// </summary>
        /// <returns></returns>
        private bool UpdateClient()
        {
            //update.bat不存在时，需要从assembly中读取并放置在当前程序目录下
            if (!File.Exists(_updateFilePath))
            {
                bool success = AssemblyHelper.GetFileFromAssembly(ClientConst.UpdateName, _updateFilePath);
                if (!success)
                {
                    //文件未加载成功，则返回false
                    return false;
                }
            }

            new Thread(
                () =>
                {
                    //执行update.bat
                    LoggerWrapper.Logger.Error("VWS.ClientHost", "start run cmd");
                    CommandHelper.RunShellCommand(_updateFilePath);
                    Thread.Sleep(5000);
                    LoggerWrapper.Logger.Error("VWS.ClientHost", "stop run cmd");
                }
                ).Start();

            return true;
        }

        /// <summary>
        ///     同步文件
        /// </summary>
        /// <param name = "task">同步任务实体</param>
        /// <returns></returns>
        private bool SyncFiles(TaskSenderEntity task)
        {
            IList<string> addfileList = CommonHelper.ConvertByComma(task.AddList);
            IList<string> delfileList = CommonHelper.ConvertByComma(task.DelList);

            // 临时备份当前需要同步的文件（SyncTaskId做为文件夹名）
            if (_fileHelper.TempBackupFiles(task.SyncTaskId, task.TargetRoot, addfileList, delfileList))
            {
                var fileLoader = new FileLoader();
                // 下载文件
                string errorMsg;
                bool downloadSuccess = fileLoader.Download(task.SyncTaskId, task.TargetRoot, addfileList,
                                                           task.SourceRoot, task.SourceIP, out errorMsg);

                // copy文件到目标目录
                if (downloadSuccess)
                {
                    return _fileHelper.CopyToTarget(task.SyncTaskId, task.TargetRoot, addfileList, delfileList);
                }
                throw new ApplicationException(errorMsg);
            }
            return false;
        }

        /// <summary>
        ///     同步任务失败文件回滚
        ///     备注：用于同步失败进行的文件回滚
        /// </summary>
        /// <param name = "task">同步任务实体</param>
        /// <returns></returns>
        private bool RollbackFiles(TaskSenderEntity task)
        {
            // 根据SyncTaskId，从临时备份中获取文件，并覆盖于目标目录中
            _fileHelper.RollbackTempTask(task.SyncTaskId, task.TargetRoot);
            // 根据SyncTaskid，清理临时备份
            _fileHelper.ClearTempTaskFiles(task.SyncTaskId, task.TargetRoot);

            return true;
        }

        /// <summary>
        ///     确认完成任务
        /// </summary>
        /// <param name = "task">同步任务实体</param>
        /// <returns></returns>
        private bool CommitFiles(TaskSenderEntity task)
        {
            // 根据Taskid，清理临时备份
            _fileHelper.ClearTempTaskFiles(task.SyncTaskId, task.TargetRoot);
            return true;
        }

        /// <summary>
        ///     服务器端备份文件
        /// </summary>
        /// <param name = "task">同步任务实体</param>
        /// <returns></returns>
        private bool BackupFiles(TaskSenderEntity task)
        {
            IList<string> addfileList = CommonHelper.ConvertByComma(task.AddList);
            IList<string> delfileList = CommonHelper.ConvertByComma(task.DelList);

            // 备份任务文件
            _fileHelper.BackupFiles(task.SyncTaskId, task.TargetRoot, task.BackupRoot, addfileList, delfileList);
            return true;
        }

        /// <summary>
        ///     压缩文件
        /// </summary>
        /// <param name = "task">同步任务实体</param>
        /// <returns></returns>
        private bool CompressFiles(TaskSenderEntity task)
        {
            //判断是否需要压缩，并启用
            if (!string.IsNullOrEmpty(task.SourceRoot))
            {
                string htmlFileKey = task.SourceRoot + EnumCompresssType.HtmlCompress.ToString();
                string jsCssFileKey = task.SourceRoot + EnumCompresssType.JsCssCompress.ToString();
                try
                {
                    switch (task.CompressType)
                    {
                        //压缩html类型文件
                        case EnumCompresssType.HtmlCompress:
                            {
                                if (!_dictFileCompress.ContainsKey(htmlFileKey))
                                {
                                    _dictFileCompress.Add(htmlFileKey, IntallHtmlCompress(task.SourceRoot));
                                }
                                break;
                            }
                        //压缩Js、Css类型文件
                        case EnumCompresssType.JsCssCompress:
                            {
                                if (!_dictFileCompress.ContainsKey(jsCssFileKey))
                                {
                                    _dictFileCompress.Add(jsCssFileKey, IntallJsCssCompress(task.SourceRoot));
                                }
                                break;
                            }
                        ////压缩html、Js、Css类型文件
                        case EnumCompresssType.HtmlJsCssCompress:
                            {
                                if (!_dictFileCompress.ContainsKey(htmlFileKey))
                                {
                                    _dictFileCompress.Add(htmlFileKey, IntallHtmlCompress(task.SourceRoot));
                                }
                                if (!_dictFileCompress.ContainsKey(jsCssFileKey))
                                {
                                    _dictFileCompress.Add(jsCssFileKey, IntallJsCssCompress(task.SourceRoot));
                                }

                                break;
                            }
                        //不压缩文件
                        default:
                            {
                                if (_dictFileCompress.ContainsKey(htmlFileKey))
                                {
                                    foreach (FileCompress fc in _dictFileCompress[htmlFileKey])
                                    {
                                        fc.Stop();
                                    }
                                    _dictFileCompress.Remove(htmlFileKey);
                                }
                                if (_dictFileCompress.ContainsKey(jsCssFileKey))
                                {
                                    foreach (FileCompress fc in _dictFileCompress[jsCssFileKey])
                                    {
                                        fc.Stop();
                                    }
                                    _dictFileCompress.Remove(jsCssFileKey);
                                }
                                break;
                            }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    LoggerWrapper.Logger.Error("VWS.ClientHost", ex.ToString());
                    //_logger.Error(ex);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        ///     构建Html文件压缩
        /// </summary>
        /// <param name = "sourceRoot">源服务器根目录</param>
        /// <returns></returns>
        private List<FileCompress> IntallHtmlCompress(string sourceRoot)
        {
            var list = new List<FileCompress>();

            foreach (string filter in ClientConst.HtmlPostfixs)
            {
                var fc = new FileCompress(sourceRoot, filter);
                fc.Start();
                list.Add(fc);
            }
            return list;
        }

        /// <summary>
        ///     构建JsCss文件压缩
        /// </summary>
        /// <param name = "sourceRoot">源服务器根目录</param>
        /// <returns></returns>
        private List<FileCompress> IntallJsCssCompress(string sourceRoot)
        {
            var list = new List<FileCompress>();

            foreach (string filter in ClientConst.JsCssPostfixs)
            {
                var fc = new FileCompress(sourceRoot, filter);
                fc.Start();
                list.Add(fc);
            }
            return list;
        }

        /// <summary>
        ///     回滚任务
        ///     备注：用于页面点击回滚操作
        /// </summary>
        /// <param name = "task">同步任务实体</param>
        /// <returns></returns>
        /// [{"TaskId":95,"SyncTaskId":0,"TaskName":"revertfiles","WinServiceName":"","SourceIP":"","SourceRoot":"","TargetIP":"10.3.131.53","TargetRoot":"D:\\DownVancl\\","BackupRoot":"","AddList":"2011_08_10\\79\\Default.aspx","DelList":"","TaskStatus":1}]
        private bool RevertFiles(TaskSenderEntity task)
        {
            if (!string.IsNullOrEmpty(task.AddList))
            {
                IList<string> backUpAddfileList = CommonHelper.ConvertByComma(task.AddList);

                string regexAddList = ClientConst.TwoPrefixRegex.Replace(task.AddList, string.Empty);

                IList<string> addfileList = CommonHelper.ConvertByComma(regexAddList);

                string tempBackPath = task.TargetRoot.Remove(task.TargetRoot.Length - 1) + ".vws\\" + task.TaskId +
                                      "bk\\";

                // 临时备份文件
                //因为是回滚Demo文件，所以task.SyncTaskId（同步任务Id）传过来的值 == 0，创建临时文件的时候传入了task.TaskId作为参数
                if (_fileHelper.TempBackupFiles(task.TaskId, task.TargetRoot, addfileList, null))
                {
                    //复制文件
                    if (_fileHelper.CopyBackUpForDemo(task.TargetRoot, backUpAddfileList))
                    {
                        //删除临时备份文件夹
                        Directory.Delete(tempBackPath, true);
                        return true;
                    }

                    //如果上面复制文件失败，将临时备份文件拷贝到目标目录
                    _fileHelper.CopyTempForDemo(tempBackPath, task.TargetRoot, addfileList);

                    //删除临时备份文件夹
                    Directory.Delete(tempBackPath, true);
                }
                return false;
            }
            return true;
        }
    }
}