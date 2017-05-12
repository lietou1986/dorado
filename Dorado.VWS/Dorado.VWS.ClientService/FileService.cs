/*-------------------------------------------------------------------------
 * 版权所有：凡客诚品（北京）科技有限公司
 * 版本：v1.0
 * 时间： 2011/7/29 11:38:47
 * 作者：蔡昌艳（Bruce Tscai）
 * 联系方式：caichangyan@vancl.cn
 * 本类主要用途描述：
 *  -------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;
using Vancl.IC.VWS.ClientService.Model;

namespace Vancl.IC.VWS.ClientService
{
    public class FileService : IFileTransService
    {
        private ILog _logger = LogManager.GetLogger(typeof(FileService));

        #region IFileTransService 成员

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <param name="md5">md5文件验证</param>
        /// <returns>文件byte数组</returns>
        public byte[] Download(string filePath, out string md5)
        {
            byte[] fileContents;
            md5 = string.Empty;

            //file verify
            if (File.Exists(filePath))
            {
                //read file content
                try
                {
                    using (FileStream sourceFileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        fileContents = new byte[sourceFileStream.Length];
                        sourceFileStream.Read(fileContents, 0, (int)sourceFileStream.Length);

                        md5 = Common.MD5File(filePath);

                        return fileContents;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileBytes">文件byte数组</param>
        /// <param name="md5">md5文件验证</param>
        /// <returns>成功返回True;失败返回false</returns>
        public bool Upload(byte[] fileBytes, string md5)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="folderPath">文件夹地址</param>
        /// <returns></returns>
        public VwsDirectoryInfoList GetFileList(string folderPath)
        {
            VwsDirectoryInfoList fileList = new VwsDirectoryInfoList();
            //判断folder是否存在，不存在时返回False
            if (!Directory.Exists(folderPath))
            {
                return fileList;
            }

            DirectoryInfo dir = new DirectoryInfo(folderPath);
            // 文件列表，最后写入时间倒序 
            foreach (var file in dir.GetFiles().OrderByDescending(item => item.LastWriteTime))
            {
                if ((file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    VwsDirectoryInfo info = new VwsDirectoryInfo()
                    {
                        Name = file.Name,
                        FullName = file.FullName,
                        Length = file.Length,
                        CreationTime = file.CreationTime,
                        UpdateTime = file.LastWriteTime,
                        IsFolder = false
                    };
                    fileList.Add(info);
                }
            }

            // 文件夹列表
            foreach (var folder in dir.GetDirectories())
            {
                if ((folder.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    VwsDirectoryInfo info = new VwsDirectoryInfo()
                    {
                        Name = folder.Name,
                        FullName = folder.FullName,
                        CreationTime = folder.CreationTime,
                        UpdateTime = folder.LastWriteTime,
                        IsFolder = true
                    };
                    fileList.Add(info);
                }
            }
            return fileList;
        }

        #endregion
    }
}
