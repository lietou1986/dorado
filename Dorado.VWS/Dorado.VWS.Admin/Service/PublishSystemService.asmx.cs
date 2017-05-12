#region using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;
using Dorado.Configuration;
using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.VWS.Model;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;
using Dorado.VWS.Utils;

#endregion using

namespace Dorado.VWS.Admin.Service
{
    /// <summary>
    ///     PublishSystemService 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    [SoapDocumentService(RoutingStyle = SoapServiceRoutingStyle.RequestElement)]
    public class PublishSystemService : WebService
    {
        //private readonly ILog _logger = LogManager.GetLogger(typeof(VwsService));
        private readonly ServerProvider _serProvider = new ServerProvider();

        private readonly DomainProvider _domainProvider = new DomainProvider();

        private readonly TestConnectProvider _testProvider = new TestConnectProvider();
        private readonly int cachetime = 20;

        public PublishSystemService()
        {
            try
            {
                cachetime = int.Parse(AppSettingProvider.Get("cachetime"));
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS.Site.PublishSystemService", ex.ToString());
            }
        }

        /// <summary>
        /// 转换单个文件路径格式
        /// </summary>
        /// <param name="path">原路径</param>
        /// <returns>规范路径</returns>
        private string TransFilePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            string p = path.TrimStart(new char[] { ' ', '\\', '/' }).Replace('/', '\\');

            //if (p != path)
            //{
            //    _logger.InfoFormat("WebService调用：转换路径格式[{0}] 为 [{1}]", path, p);
            //}

            return p;
        }

        /// <summary>
        /// 转换多个文件路径格式
        /// </summary>
        /// <param name="files">原路径</param>
        /// <returns>规范路径</returns>
        private string[] TransFilesPath(string[] files)
        {
            List<string> list = new List<string>();

            if (files != null)
            {
                foreach (string file in files)
                {
                    list.Add(TransFilePath(file));
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// 获取域名管理员列表
        /// </summary>
        /// <param name="domain">域名</param>
        /// <returns></returns>
        private string GetManageUsers(string domain)
        {
            string users = string.Empty;

            List<string> manageUserList = DomainPermissionProvider.GetManageUserListByDomainName(domain);
            if (manageUserList != null && manageUserList.Count > 0)
            {
                users = string.Join("@dorado.com,", manageUserList.ToArray()) + "@dorado.com";
            }
            else
            {
                users = "系统管理员";
            }

            return users;
        }

        /// <summary>
        /// 验证文件名首位是否合法（不可以数字开头）
        /// </summary>
        /// <param name="files">文件列表</param>
        /// <param name="errorfileName">错误文件名</param>
        /// <returns></returns>
        private bool CheckFileNameForBussiness(string[] files, out string errorfileName)
        {
            errorfileName = string.Empty;
            if (files == null || files.Length < 1)
            {
                return true;
            }

            foreach (string file in files)
            {
                try
                {
                    int firstChar;
                    if (int.TryParse(file[0].ToString(), out firstChar))
                    {
                        errorfileName = file;
                        return false;
                    }
                }
                catch
                {
                }
            }

            return true;
        }

        /// <summary>
        ///     查看用户是否有文件访问权限
        /// </summary>
        /// <param name="userName">用户Id</param>
        /// <param name = "domain">域名</param>
        /// <param name = "files">判定的文件列表（相对路径(例如：abc\test.txt)）</param>
        /// <param name="resultMessage">返回信息（错误或正常信息）</param>
        /// <returns>逻辑值(true:成功 false:失败)</returns>
        [WebMethod]
        public bool HasPermission(string userName, string domain, string[] files, out string resultMessage)
        {
            //_logger.InfoFormat("WebService调用：检查用户权限：用户[{0}] 域名[{1}] 文件列表[{2}]", userName, domain, files != null ? string.Join(",", files) : string.Empty);
            LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用：检查用户权限：用户[{0}] 域名[{1}] 文件列表[{2}]", userName, domain, files != null ? string.Join(",", files) : string.Empty));
            try
            {
                var permissionProvider = new PermissionProvider();
                List<string> permissionFileList = permissionProvider.GetPermissionByUserAndDomain(userName, domain).Select(c => c.Path).ToList();

                if (permissionFileList.Count > 0)
                {
                    //拥有所有文件权限
                    if (permissionFileList.Contains(string.Empty))
                    {
                        resultMessage = string.Format("用户[{0}]拥有 域名[{1}] 所有文件权限", userName, domain);
                        //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, true);
                        LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, true));
                        return true;
                    }

                    //转换路径格式
                    files = TransFilesPath(files);

                    //记录没有权限的文件
                    var noPermissionString = new StringBuilder();

                    foreach (string file in files)
                    {
                        bool hasPermission = permissionFileList.Any(permissionFile => file.IndexOf(permissionFile, StringComparison.OrdinalIgnoreCase) == 0);
                        //判断用户文件权限
                        if (!hasPermission)
                        {
                            noPermissionString.Append(string.Format("{0};", file));
                        }
                    }
                    if (noPermissionString.Length > 0)
                    {
                        resultMessage = string.Format("无权限的文件列表：{0}, ", noPermissionString);
                        resultMessage += string.Format("请联系[{0}]开通权限", GetManageUsers(domain));

                        //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false);
                        LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false));
                        return false;
                    }
                    resultMessage = "拥有所有文件权限";
                    //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, true);
                    LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, true));
                    return true;
                }
                else
                {
                    resultMessage = string.Format("所有文件都没有权限,请联系[{0}]开通权限", GetManageUsers(domain));
                    //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false);
                    LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false));
                    return false;
                }
            }
            catch (Exception ex)
            {
                //_logger.Error("WebService调用：" + ex.ToString());
                LoggerWrapper.Logger.Error("VWS.Site.PublishSystemService", string.Format("WebService调用：" + ex.ToString()));
                resultMessage = "程序错误：" + ex.ToString();
                //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false);
                LoggerWrapper.Logger.Error("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false));
                return false;
            }
        }

        /// <summary>
        ///     添加同步任务
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name = "domain">域名</param>
        /// <param name = "addFiles">要添加或更改的文件列表（相对路径(例如：abc\test.txt)）</param>
        /// <param name = "delFiles">要删除的文件列表（相对路径(例如：abc\test.txt)）</param>
        /// <param name = "log">日志</param>
        /// <param name="checkPms">是否check用户权限</param>
        /// <param name="resultMessage">返回信息（错误或正常信息）</param>
        /// <returns>返回同步任务Id（返回小于1时，为出错）</returns>
        [WebMethod]
        public int AddSyncTask(string userName, string domain, string[] addFiles, string[] delFiles, string log, bool checkPms,
                               out string resultMessage)
        {
            return AddSyncTaskForBussiness(userName, domain, addFiles, delFiles, log, checkPms, 0, "WebService", out resultMessage);
        }

        /// <summary>
        ///     添加同步任务
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name = "domain">域名</param>
        /// <param name = "addFiles">要添加或更改的文件列表（相对路径(例如：abc\test.txt)）</param>
        /// <param name = "delFiles">要删除的文件列表（相对路径(例如：abc\test.txt)）</param>
        /// <param name = "log">日志</param>
        /// <param name="checkPms">是否check用户权限</param>
        /// <param name="fromDomain">外部调用域名（用来区分由哪里调用的同步系统， 如Admin）</param>
        /// <param name="resultMessage">返回信息（错误或正常信息）</param>
        /// <returns>返回同步任务Id（返回小于1时，为出错）</returns>
        [WebMethod]
        public int AddSyncTaskNew(string userName, string domain, string[] addFiles, string[] delFiles, string log, bool checkPms,
                               string fromDomain, out string resultMessage)
        {
            return AddSyncTaskForBussiness(userName, domain, addFiles, delFiles, log, checkPms, 0, fromDomain, out resultMessage);
        }

        /// <summary>
        ///     添加同步任务
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name = "domain">域名</param>
        /// <param name = "addFiles">要添加或更改的文件列表（相对路径(例如：abc\test.txt)）</param>
        /// <param name = "delFiles">要删除的文件列表（相对路径(例如：abc\test.txt)）</param>
        /// <param name = "log">日志</param>
        /// <param name="checkPms">是否check用户权限</param>
        /// <param name="bussiness">0：不检查 1：文件路径首字母为数字时报错</param>
        /// <param name="fromDomain">外部调用域名（用来区分由哪里调用的同步系统， 如Admin）</param>
        /// <param name="resultMessage">返回信息（错误或正常信息）</param>
        /// <returns>返回同步任务Id（返回小于1时，为出错）</returns>
        [WebMethod]
        public int AddSyncTaskForBussiness(string userName, string domain, string[] addFiles, string[] delFiles, string log, bool checkPms,
                               int bussiness, string fromDomain, out string resultMessage)
        {
            log = fromDomain + ":" + log;

            //_logger.InfoFormat("WebService调用：添加同步任务：用户[{0}] 域名[{1}] 日志[{2}] 文件列表[{3}] PMS验证[{4}]", userName, domain, log,
            //    addFiles != null ? string.Join(",", addFiles) : string.Empty, checkPms);
            LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用：添加同步任务：用户[{0}] 域名[{1}] 日志[{2}] 文件列表[{3}] PMS验证[{4}]", userName, domain, log,
                addFiles != null ? string.Join(",", addFiles) : string.Empty, checkPms));
            try
            {
                #region 判断参数正确性

                if (string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(log) || string.IsNullOrEmpty(userName) ||
                    ((addFiles == null && delFiles == null)) || (addFiles.Length == 0 && delFiles.Length == 0))
                {
                    resultMessage = "参数错误";
                    //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, -1);
                    LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, -1));
                    return -1;
                }

                #endregion 判断参数正确性

                #region 验证文件首字符

                if (bussiness == 1)
                {
                    string errorfileName = string.Empty;
                    if (!(CheckFileNameForBussiness(addFiles, out errorfileName) && CheckFileNameForBussiness(delFiles, out errorfileName)))
                    {
                        resultMessage = "文件名[" + errorfileName + "]首位不能是数字";
                        //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, -1);
                        LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, -1));
                        return -1;
                    }
                }

                #endregion 验证文件首字符

                #region 转换文件路径格式

                //转换路径格式
                addFiles = TransFilesPath(addFiles);
                delFiles = TransFilesPath(delFiles);

                #endregion 转换文件路径格式

                #region 判断用户是否有权限

                if (checkPms)
                {
                    if ((addFiles.Length > 0 && !HasPermission(userName, domain, addFiles, out resultMessage)) ||
                                (delFiles.Length > 0 && !HasPermission(userName, domain, delFiles, out resultMessage)))
                    {
                        //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", "PMS验证未通过" + resultMessage, -1);
                        LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", "PMS验证未通过" + resultMessage, -1));
                        return -1;
                    }
                }
                else
                {
                    //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将直接到下一步", "不进行PMS用户权限验证");
                    LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将直接到下一步", "不进行PMS用户权限验证"));
                }

                #endregion 判断用户是否有权限

                #region 测试连接

                ServerEntity sourceServer = _serProvider.GetSourceServerByDomain(domain);
                if (sourceServer == null)
                {
                    resultMessage = "域名错误";
                    //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, -1);
                    LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, -1));
                    return -1;
                }

                if (!_testProvider.TestConnect(sourceServer.ServerId))
                {
                    resultMessage = string.Format("源服务器[{0}]连接故障", sourceServer.IP);
                    //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, -1);
                    LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, -1));
                    return -1;
                }

                IList<ServerEntity> targets = _serProvider.GetTargetServerByDomain(domain);
                if (targets == null)
                {
                    resultMessage = "无宿主";
                    //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, -1);
                    LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, -1));
                    return -1;
                }

                foreach (var target in targets)
                {
                    if (_testProvider.TestConnect(target.ServerId)) continue;

                    resultMessage = string.Format("宿主服务器[{0}]连接故障", target.IP);
                    //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, -1);
                    LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, -1));
                    return -1;
                }

                #endregion 测试连接

                #region 同步

                var stp = new SyncTaskProcessor();
                int taskid = stp.AddSyncTask(sourceServer.DomainId, userName, log, addFiles, delFiles);

                // 同步目标
                if (taskid > 0)
                {
                    stp.SyncTaskTargets(taskid, targets.Select(t => t.ServerId).ToArray(), userName);
                    resultMessage = "任务建立成功";
                    //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, taskid);
                    LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, taskid));
                    return taskid;
                }

                resultMessage = "任务建立失败";
                //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, -1);
                LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, -1));
                return -1;

                #endregion 同步
            }
            catch (Exception ex)
            {
                //_logger.Error("WebService调用：" + ex.ToString());
                LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用：" + ex.ToString()));
                resultMessage = "程序错误：" + ex.ToString();
                //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, -1);
                LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, -1));
                return -1;
            }
        }

        /// <summary>
        /// 无版本同步接口
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="domain">域名</param>
        /// <param name = "addFiles">要添加或更改的文件列表（相对路径(例如：abc\test.txt)）</param>
        /// <param name = "delFiles">要删除的文件列表（相对路径(例如：abc\test.txt)）</param>
        /// <param name = "log">日志</param>
        /// <param name="checkPms">是否check用户权限</param>
        /// <param name="bussiness">0：不检查 1：文件路径首字母为数字时报错</param>
        /// <param name="resultMessage">返回信息（错误或正常信息）</param>
        /// <returns>逻辑值(true:成功 false:失败)</returns>
        [WebMethod]
        public bool SyncUnverTask(string userName, string domain, string[] addFiles, string[] delFiles, string log, bool checkPms, int bussiness, out string resultMessage)
        {
            return SyncUnverTaskNew(userName, domain, addFiles, delFiles, log, checkPms, bussiness, "WebService", out resultMessage);
        }

        /// <summary>
        /// 无版本同步接口
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="domain">域名</param>
        /// <param name = "addFiles">要添加或更改的文件列表（相对路径(例如：abc\test.txt)）</param>
        /// <param name = "delFiles">要删除的文件列表（相对路径(例如：abc\test.txt)）</param>
        /// <param name = "log">日志</param>
        /// <param name="checkPms">是否check用户权限</param>
        /// <param name="bussiness">0：不检查 1：文件路径首字母为数字时报错</param>
        /// <param name="fromDomain">外部调用域名（用来区分由哪里调用的同步系统， 如Admin）</param>
        /// <param name="resultMessage">返回信息（错误或正常信息）</param>
        /// <returns>逻辑值(true:成功 false:失败)</returns>
        [WebMethod]
        public bool SyncUnverTaskNew(string userName, string domain, string[] addFiles, string[] delFiles, string log, bool checkPms, int bussiness, string fromDomain, out string resultMessage)
        {
            log = fromDomain + ":" + log;

            //_logger.InfoFormat("WebService调用：同步无版本：用户[{0}] 域名[{1}] 文件列表[{2}] PMS验证[{3}]", userName, domain, addFiles != null ? string.Join(",", addFiles) : string.Empty, checkPms);
            LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用：同步无版本：用户[{0}] 域名[{1}] 文件列表[{2}] PMS验证[{3}]", userName, domain, addFiles != null ? string.Join(",", addFiles) : string.Empty, checkPms));
            try
            {
                if (string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(log) ||
                        (addFiles == null && delFiles == null) ||
                        (!((addFiles != null && addFiles.Length > 0) || (delFiles != null && delFiles.Length > 0))))
                {
                    resultMessage = "参数错误";
                    //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false);
                    LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false));
                    return false;
                }

                #region 转换文件路径格式

                //转换路径格式
                addFiles = TransFilesPath(addFiles);
                delFiles = TransFilesPath(delFiles);

                #endregion 转换文件路径格式

                if (addFiles != null && bussiness == 1)
                {
                    foreach (string file in addFiles)
                    {
                        try
                        {
                            int firstChar;
                            if (int.TryParse(file[0].ToString(), out firstChar))
                            {
                                resultMessage = "文件名[" + file + "]首位不能是数字";
                                //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false);
                                LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false));
                                return false;
                            }
                        }
                        catch (Exception ex)
                        {
                            LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", ex.ToString());
                        }
                    }
                }

                #region 简单同步

                SyncTaskProcessor syncTaskProcessor = new SyncTaskProcessor();
                return syncTaskProcessor.SyncUnverTask(userName, domain, addFiles, delFiles, out resultMessage);

                #endregion 简单同步
            }
            catch (Exception ex)
            {
                //_logger.Error("WebService调用：" + ex.ToString());
                LoggerWrapper.Logger.Error("VWS.Site.PublishSystemService", string.Format("WebService调用：" + ex.ToString()));
                resultMessage = "程序错误：" + ex.ToString();
                //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false);
                LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false));
                return false;
            }
        }

        /// <summary>
        ///     获取同步任务状态
        /// </summary>
        /// <param name = "taskid">同步任务Id</param>
        /// <returns>返回结果对象</returns>
        [WebMethod]
        public PublishSystemServiceResult GetTaskStatus(int taskid)
        {
            //_logger.InfoFormat("WebService调用：获取同步任务状态：任务ID[{0}]", taskid);
            LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用：获取同步任务状态：任务ID[{0}]", taskid));
            DateTime end = DateTime.Now.AddMinutes(5);

            try
            {
                var syncProvider = new SyncProvider();
                while (DateTime.Now < end)
                {
                    SynctaskEntity taskEntity = syncProvider.GetSyncTaskById(taskid);

                    if (taskEntity.SyncStatus == EnumSyncStatus.Running)
                    {
                        //_logger.InfoFormat("WebService调用：返回同步任务状态：任务ID[{0}]当前运行中，将等待100毫秒再取", taskid);
                        LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用：返回同步任务状态：任务ID[{0}]当前运行中，将等待100毫秒再取", taskid));
                        System.Threading.Thread.Sleep(100);
                    }
                    else
                    {
                        PublishSystemServiceResult result = new PublishSystemServiceResult { Status = taskEntity.SyncStatus, Message = taskEntity.Description };
                        if (result.Status != EnumSyncStatus.Succeed)
                        {
                            result.Message += string.Format(" 任务状态为[{0}]", result.Status.ToString());
                        }

                        //_logger.InfoFormat("WebService调用：返回同步任务状态：任务ID[{0}], 状态[{1}], 信息[{2}]将返回", taskid, result.Status.ToString(), result.Message);
                        LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用：返回同步任务状态：任务ID[{0}], 状态[{1}], 信息[{2}]将返回", taskid, result.Status.ToString(), result.Message));
                        return result;
                    }
                }

                //_logger.Error("WebService调用：获取任务状态超时");
                LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", "WebService调用：获取任务状态超时");
                return new PublishSystemServiceResult { Status = EnumSyncStatus.Failed, Message = "获取任务状态超时" };
            }
            catch (Exception ex)
            {
                //_logger.Error("WebService调用：获取任务状态出现异常：" + ex.ToString());
                LoggerWrapper.Logger.Error("VWS.Site.PublishSystemService", string.Format("WebService调用：获取任务状态出现异常：" + ex.ToString()));
                return new PublishSystemServiceResult { Status = EnumSyncStatus.Failed, Message = ex.ToString() };
            }
        }

        /// <summary>
        ///     发送文件到demo服务器上
        /// </summary>
        /// <param name = "domainName">域名</param>
        /// <param name = "relativePath">相对路径(例如：abc\test.txt)</param>
        /// <param name = "fileBytes">文件字节数组</param>
        /// <param name = "resultMessage">返回信息（错误或正常信息）</param>
        /// <returns>逻辑值(true:成功 false:失败)</returns>
        [WebMethod]
        public bool SendFileToDemo(string domainName, string relativePath, byte[] fileBytes, out string resultMessage)
        {
            return SendFileToDemoNew(domainName, relativePath, fileBytes, "WebService", out resultMessage);
        }

        /// <summary>
        ///     发送文件到demo服务器上
        /// </summary>
        /// <param name = "domainName">域名</param>
        /// <param name = "relativePath">相对路径(例如：abc\test.txt)</param>
        /// <param name = "fileBytes">文件字节数组</param>
        /// <param name="fromDomain">外部调用域名（用来区分由哪里调用的同步系统， 如Admin）</param>
        /// <param name = "resultMessage">返回信息（错误或正常信息）</param>
        /// <returns>逻辑值(true:成功 false:失败)</returns>
        [WebMethod]
        public bool SendFileToDemoNew(string domainName, string relativePath, byte[] fileBytes, string fromDomain, out string resultMessage)
        {
            //_logger.InfoFormat("WebService调用：{0} 发送文件：域名[{1}] 文件[{2}]", fromDomain, domainName, relativePath);
            LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用：{0} 发送文件：域名[{1}] 文件[{2}]", fromDomain, domainName, relativePath));
            try
            {
                resultMessage = string.Empty;
                if (string.IsNullOrEmpty(domainName) || fileBytes == null)
                {
                    resultMessage = "参数错误";
                    //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false);
                    LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false));
                    return false;
                }

                #region 转换文件路径格式

                //转换路径格式
                relativePath = TransFilePath(relativePath);

                #endregion 转换文件路径格式

                if (string.IsNullOrEmpty(relativePath) || relativePath.StartsWith("\\") || relativePath.StartsWith("/") ||
                    relativePath.StartsWith("#") || relativePath.Contains(":"))
                {
                    resultMessage = "relativePath 参数错误，格式应为abc\\test.txt";
                    //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false);
                    LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false));
                    return false;
                }

                ServerEntity sourceServer = _serProvider.GetSourceServerByDomain(domainName);
                if (sourceServer == null)
                {
                    resultMessage = "此域名没有配置";
                    //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false);
                    LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false));
                    return false;
                }

                if (!_testProvider.TestConnect(sourceServer.ServerId))
                {
                    resultMessage = "Demo服务器连接失败";
                    //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false);
                    LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false));
                    return false;
                }

                var client = new SocketFileClient();
                if (!client.SendFile(IPAddress.Parse(sourceServer.IP), sourceServer.Root + relativePath, fileBytes))
                {
                    resultMessage = "文件发送失败";
                    //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false);
                    LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false));
                    return false;
                }

                // _logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, true);
                LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, true));
                return true;
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS.Site.PublishSystemService", ex.ToString());
                resultMessage = "程序错误：" + ex.ToString();
                //_logger.InfoFormat("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false);
                LoggerWrapper.Logger.Info("VWS.Site.PublishSystemService", string.Format("WebService调用逻辑判断：[{0}]，将返回[{1}]", resultMessage, false));
                return false;
            }
        }

        #region 安全机制扩展

        /// <summary>
        /// 获取主机列表
        /// </summary>
        /// <returns></returns>
        [WebMethod(Description = "获取允许连接主机列表")]
        public List<string> GetServerList()
        {
            string clientIP = Context.Request.UserHostAddress;

            List<string> ipList = (List<string>)WebCache.Get("allowIPs_" + clientIP);
            if (ipList == null)
            {
                ipList = _serProvider.GetAllowIp(clientIP);
                string alwaysAllowIP = AppSettingProvider.Get("AllowIP");
                if (!string.IsNullOrWhiteSpace(alwaysAllowIP))
                {
                    var ips = alwaysAllowIP.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string ip in ips)
                    {
                        if (!ipList.Contains(ip))
                        {
                            ipList.Add(ip);
                        }
                    }
                }
                WebCache.Add("allowIPs_" + clientIP, ipList, cachetime);
            }

            return ipList;
        }

        /// <summary>
        /// 获取允许操作的服务列表
        /// </summary>
        /// <returns></returns>
        [WebMethod(Description = "允许操作的服务列表")]
        public List<string> GetServiceList()
        {
            string clientIP = Context.Request.UserHostAddress;
            List<string> servicelList = (List<string>)WebCache.Get("allowservice_" + clientIP);
            if (servicelList == null)
            {
                var domainList = _domainProvider.GetDomains(clientIP);

                var servicelist = from DomainEntity domain in domainList
                                  where !string.IsNullOrWhiteSpace(domain.WinServiceName)
                                  select domain.WinServiceName;
                servicelList = servicelist.ToList();

                string alwaysAllowService = AppSettingProvider.Get("AllowService");
                if (!string.IsNullOrWhiteSpace(alwaysAllowService))
                {
                    var services = alwaysAllowService.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string s in services)
                    {
                        if (!servicelList.Contains(s))
                        {
                            servicelList.Add(s);
                        }
                    }
                }
                WebCache.Add("allowservice_" + clientIP, servicelList, cachetime);
            }
            return servicelList;
        }

        /// <summary>
        /// 获取不允许操作的服务列表
        /// </summary>
        /// <returns></returns>
        [WebMethod(Description = "拒绝操作的服务列表")]
        public List<string> GetRefuseServiceList()
        {
            string file = AppDomain.CurrentDomain.BaseDirectory + AppSettingProvider.Get("RefuseService");

            List<string> refuseServicelList = (List<string>)WebCache.Get("refuseservicellist");
            if (refuseServicelList == null)
            {
                refuseServicelList = new List<string>() { };

                XmlDocument xml = new XmlDocument();
                try
                {
                    xml.Load(file);

                    var list = xml.GetElementsByTagName("service");
                    foreach (XmlNode node in list)
                    {
                        refuseServicelList.Add(node.InnerText);
                    }
                }
                catch (Exception ex)
                {
                    LoggerWrapper.Logger.Error("VWS.Site.PublishSystemService", ex.ToString());
                }
                WebCache.Add("refuseservicellist", refuseServicelList, cachetime);
            }
            return refuseServicelList;
        }

        #endregion 安全机制扩展
    }

    /// <summary>
    /// 返回类型定义（GetTaskStatus方法会返回这个类型）
    /// </summary>
    [Serializable]
    public struct PublishSystemServiceResult
    {
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Message;

        /// <summary>
        /// 同步状态枚举(状态信息（1：同步中 2：同步挂起 3：同步成功 4：同步失败 5：同步被回滚 6：同步回滚失败）)
        /// </summary>
        public EnumSyncStatus Status;
    }
}