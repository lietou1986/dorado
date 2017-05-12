/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 作者：len
 * 联系方式：len@dorado.com 
 * 创建时间： 2011/7/6 9:50:33
 * 版本号：v1.0
 * 本类主要用途描述：文件列表业务类
 *  -------------------------------------------------------------------------*/

#region using

using System.Collections;
using System.Collections.Generic;
using System.Net;
using Dorado.VWS.Model;
using Dorado.VWS.Services.Persistence;
using Dorado.VWS.Utils;

#endregion using

namespace Dorado.VWS.Services
{
    public class FileListProvider
    {
        private readonly ServerProvider _serProvider = new ServerProvider();

        /// <summary>
        /// 获取文件目录集合
        /// </summary>
        /// <param name="sourceid">同步源id</param>
        /// <param name="dirRoot">目录地址</param>
        /// <returns>文件信息集合</returns>
        public List<VwsDirectoryInfo> GetFileList(int sourceid, string dirRoot)
        {
            ServerEntity serverEntity = _serProvider.GetServerById(sourceid);

            var taskSender = new TaskSenderEntity
                                              {
                                                  TaskId = 1,
                                                  TaskCmd = EnumTaskCmd.GETFILELIST,
                                                  TargetRoot = dirRoot,
                                                  DomainType = (int)serverEntity.DomainType,
                                                  OperatePathType = (int)serverEntity.OperatePathType,
                                                  OperatePath = serverEntity.OperatePath,
                                              };
            var client = new SocketClient();

            List<VwsDirectoryInfo> list = new List<VwsDirectoryInfo>();
            TaskResultEntity result = client.SyncSend(IPAddress.Parse(serverEntity.IP), taskSender);

            if (result != null && result.FileList != null)
            {
                list = result.FileList;
            }
            return list;
        }

        /// <summary>
        /// 获取文件目录集合
        /// </summary>
        /// <param name="sourceid">同步源id</param>
        /// <param name="dirRoot">目录地址</param>
        /// <returns>文件信息集合</returns>
        public List<string> GetAllFileName(int sourceid)
        {
            ServerEntity serverEntity = _serProvider.GetServerById(sourceid);

            var taskSender = new TaskSenderEntity
            {
                TaskId = 1,
                TaskCmd = EnumTaskCmd.GETALLFILENAME,
                TargetRoot = serverEntity.Root,
                DomainType = (int)serverEntity.DomainType,
                OperatePathType = (int)serverEntity.OperatePathType,
                OperatePath = serverEntity.OperatePath,
            };
            var client = new SocketClient();

            TaskResultEntity result = client.SyncSend(IPAddress.Parse(serverEntity.IP), taskSender);

            if (result != null && result.AllFileList != null)
            {
                return result.AllFileList;
            }

            return new List<string>();
        }

        /// <summary>
        /// 获取文件目录集合
        /// </summary>
        /// <param name="sourceid">同步源id</param>
        /// <param name="dirRoot">目录地址</param>
        /// <returns>文件信息集合</returns>
        public List<VwsDirectoryInfo> GetFileListNoMd5(int sourceid, string dirRoot)
        {
            ServerEntity serverEntity = _serProvider.GetServerById(sourceid);

            var taskSender = new TaskSenderEntity
            {
                TaskId = 1,
                TaskCmd = EnumTaskCmd.GetFileListNoMd5,
                TargetRoot = dirRoot,
                DomainType = (int)serverEntity.DomainType,
                OperatePathType = (int)serverEntity.OperatePathType,
                OperatePath = serverEntity.OperatePath,
            };
            var client = new SocketClient();

            List<VwsDirectoryInfo> list = new List<VwsDirectoryInfo>();
            TaskResultEntity result = client.SyncSend(IPAddress.Parse(serverEntity.IP), taskSender);

            if (result != null && result.FileList != null)
            {
                list = result.FileList;
            }
            return list;
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="domainid">域名Id</param>
        /// <param name="files">文件列表</param>
        /// <param name="nonExistFiles">不存在的文件列表</param>
        /// <returns>结果</returns>
        public bool FilesExist(int domainid, IList<string> files, out string nonExistFiles)
        {
            ServerEntity serverEntity = _serProvider.GetSourceServerByDomainId(domainid);

            var taskSender = new TaskSenderEntity
                                              {
                                                  TaskId = 1,
                                                  TaskCmd = EnumTaskCmd.CHECKFILELIST,
                                                  TargetRoot = serverEntity.Root,
                                                  AddList = string.Join(",", files),
                                                  DomainType = (int)serverEntity.DomainType,
                                                  OperatePathType = (int)serverEntity.OperatePathType,
                                                  OperatePath = serverEntity.OperatePath,
                                              };

            var client = new SocketClient();

            TaskResultEntity taskResult = client.SyncSend(IPAddress.Parse(serverEntity.IP), taskSender);
            nonExistFiles = taskResult.ErrorMsg;

            return string.IsNullOrEmpty(nonExistFiles);
        }

        public Hashtable GetFileMd5ByDomainId(int domainId)
        {
            var hs = new Hashtable();
            var fileMD5Dao = new FileMD5Dao();
            var fileMD5S = fileMD5Dao.GetByDomainId(domainId);
            foreach (var item in fileMD5S)
            {
                hs.Add(item.FilePath, item.MD5);
            }
            return hs;
        }
    }
}