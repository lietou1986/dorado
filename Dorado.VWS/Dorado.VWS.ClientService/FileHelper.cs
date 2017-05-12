/*-------------------------------------------------------------------------
 * 版权所有：Dorado
 * 版本：v1.0
 * 时间： 2011/8/1 15:25:19
 * 作者：len
 * 联系方式：len@dorado.com
 * 本类主要用途描述：文件操作类
 *  -------------------------------------------------------------------------*/

#region using

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.VWS.Utils;

#endregion using

namespace Dorado.VWS.ClientHost
{
    /// <summary>
    ///     文件操作类
    /// </summary>
    internal class FileHelper
    {
        /// <summary>
        ///     临时备份文件
        /// </summary>
        /// <param name = "syncTaskid">同步任务Id</param>
        /// <param name = "originPath">需要备份文件所在文件夹的绝对路径</param>
        /// <param name = "adds">添加的文件（相对路径）</param>
        /// <param name = "dels">删除的文件（相对路径）</param>
        /// <returns></returns>
        internal bool TempBackupFiles(int syncTaskid, string originPath, IList<string> adds, IList<string> dels)
        {
            try
            {
                if (adds == null) adds = new List<string>();
                if (dels == null) dels = new List<string>();

                string backupDir = originPath.Remove(originPath.Length - 1) + ".vws\\" + syncTaskid + "bk\\";

                // 备份目录不存在时，需要先创建
                Directory.CreateDirectory(backupDir);

                //记录本次task到文件vwstask.txt
                string taskFilePath = backupDir + ClientConst.VwsTaskFile;
                using (var fs = new FileStream(taskFilePath, FileMode.Create))
                {
                    using (var sw = new StreamWriter(fs, Encoding.Default))
                    {
                        //add
                        foreach (string item in adds.Where(item => !string.IsNullOrEmpty(item)))
                        {
                            sw.WriteLine("A {0}", item);
                        }

                        //del
                        foreach (string item in dels.Where(item => !string.IsNullOrEmpty(item)))
                        {
                            sw.WriteLine("D {0}", item);
                        }
                        sw.Flush();
                    }
                }
                //备份文件
                var backupFiles = new List<string>();
                foreach (var item in adds.Union(dels))
                {
                    string orgin = originPath + item;
                    string target = backupDir + item;

                    //判断源文件是否存在
                    if (File.Exists(orgin))
                    {
                        //创建备份目录
                        Directory.CreateDirectory(Path.GetDirectoryName(target));
                        //复制文件
                        File.Copy(orgin, target, true);
                        backupFiles.Add(item);
                    }
                }

                //记录本次backup到文件vwsbk.txt
                string backupFilePath = backupDir + ClientConst.VwsBackupFile;
                using (var fs = new FileStream(backupFilePath, FileMode.Create))
                {
                    using (var sw = new StreamWriter(fs, Encoding.Default))
                    {
                        foreach (string item in backupFiles.Where(item => !string.IsNullOrEmpty(item)))
                        {
                            sw.WriteLine(item);
                        }
                        sw.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS.ClientHost", ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        ///     宿主服务器复制文件
        /// </summary>
        /// <param name = "syncTaskid">同步任务Id</param>
        /// <param name = "targetPath">保存文件的文件夹的绝对路径</param>
        /// <param name = "adds">需要添加的文件</param>
        /// <param name = "dels">需要删除的文件</param>
        /// <returns></returns>
        internal bool CopyToTarget(int syncTaskid, string targetPath, IList<string> adds, IList<string> dels)
        {
            if (adds == null) adds = new List<string>();
            if (dels == null) dels = new List<string>();

            string downloadDir = targetPath.Remove(targetPath.Length - 1) + ".vws\\" + syncTaskid + "\\";
            foreach (var item in adds)
            {
                string downloadFile = downloadDir + item;
                string targetFile = targetPath + item;

                if (File.Exists(downloadFile))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
                }

                if (File.Exists(targetFile))
                {
                    FileAttributes attributes = File.GetAttributes(targetFile);

                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        // Make the file not RW
                        attributes = RemoveAttribute(attributes, FileAttributes.ReadOnly);
                        File.SetAttributes(targetFile, attributes);
                    }
                }
                File.Copy(downloadFile, targetFile, true);
            }

            foreach (string targetFile in dels.Select(item => targetPath + item).Where(File.Exists))
            {
                File.Delete(targetFile);
            }

            return true;
        }

        internal FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }

        /// <summary>
        ///     Demo服务器复制备份文件
        /// </summary>
        /// <param name = "sourcePath">保存文件的文件夹的绝对路径</param>
        /// <param name = "backUpAdds">需要复制的备份文件路径</param>
        /// <returns></returns>
        internal bool CopyBackUpForDemo(string sourcePath, IList<string> backUpAdds)
        {
            if (backUpAdds == null) backUpAdds = new List<string>();

            foreach (var item in backUpAdds)
            {
                string backUpFile = sourcePath.Remove(sourcePath.Length - 1) + ".vws\\" + item;
                string targetFile = sourcePath + ClientConst.TwoPrefixRegex.Replace(item, string.Empty);

                if (File.Exists(backUpFile))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
                }
                File.Copy(backUpFile, targetFile, true);
            }
            return true;
        }

        /// <summary>
        ///     Demo服务器复制临时备份文件
        /// </summary>
        /// <param name="tempBackUpPath"></param>
        /// <param name = "sourcePath">保存文件的文件夹的绝对路径</param>
        /// <param name = "adds">需要复制的文件路径</param>
        /// <returns></returns>
        internal bool CopyTempForDemo(string tempBackUpPath, string sourcePath, IList<string> adds)
        {
            if (adds == null) adds = new List<string>();

            foreach (var item in adds)
            {
                string tempBackUpFile = tempBackUpPath + item;
                string targetFile = sourcePath + item;

                if (File.Exists(tempBackUpFile))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
                }
                File.Copy(tempBackUpFile, targetFile, true);
            }
            return true;
        }

        /// <summary>
        ///     清除任务生成的临时文件
        /// </summary>
        /// <param name = "syncTaskid">同步任务Id</param>
        /// <param name = "originPath">源文件夹绝对路径</param>
        /// <returns></returns>
        internal bool ClearTempTaskFiles(int syncTaskid, string originPath)
        {
            string backupDir = originPath.Remove(originPath.Length - 1) + ".vws\\" + syncTaskid + "bk\\";
            string downloadDir = originPath.Remove(originPath.Length - 1) + ".vws\\" + syncTaskid + "\\";

            if (Directory.Exists(backupDir))
            {
                Directory.Delete(backupDir, true);
            }
            if (Directory.Exists(downloadDir))
            {
                Directory.Delete(downloadDir, true);
            }
            return true;
        }

        /// <summary>
        ///     回滚任务
        /// </summary>
        /// <param name = "syncTaskid">回滚任务Id</param>
        /// <param name = "originPath">源文件夹绝对路径</param>
        /// <returns></returns>
        internal bool RollbackTempTask(int syncTaskid, string originPath)
        {
            string backupDir = originPath.Remove(originPath.Length - 1) + ".vws\\" + syncTaskid + "bk\\";
            string vwsTaskFilePath = backupDir + ClientConst.VwsTaskFile;
            string vwsTaskBkFilePath = backupDir + ClientConst.VwsBackupFile;

            var taskAddFiles = new List<string>();
            var taskBkFiles = new List<string>();
            // 读取vwstask.txt中的A项-->taskAddFiles
            using (var sr = new StreamReader(vwsTaskFilePath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;

                    char status = line[0];
                    string file = line.Substring(2);
                    switch (status)
                    {
                        case 'A':
                            taskAddFiles.Add(file);
                            break;
                    }
                }
            }
            // 读取vwstaskbk.txt中的所有项-->taskBkFiles
            using (var sr = new StreamReader(vwsTaskBkFilePath, Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        taskBkFiles.Add(line);
                    }
                }
            }
            // 取存在于a不存在于b的项-->delTemp
            var delTemp = taskAddFiles.Where(item => !taskBkFiles.Contains(item)).ToList();

            // 把taskBkFiles覆盖到目标中
            foreach (var item in taskBkFiles)
            {
                string origin = backupDir + item;
                string target = originPath + item;

                Directory.CreateDirectory(Path.GetDirectoryName(target));
                File.Copy(origin, target, true);
            }
            // 在目标中删除delTemp
            foreach (string target in delTemp.Select(item => originPath + item).Where(File.Exists))
            {
                File.Delete(target);
            }
            return true;
        }

        /// <summary>
        ///     备份文件（服务器端使用）
        /// </summary>
        /// <param name = "syncTaskid">同步任务Id</param>
        /// <param name = "originPath">源文件夹绝对路径</param>
        /// <param name = "backupdir">备份文件夹绝对路径</param>
        /// <param name = "adds">添加的文件（相对路径）</param>
        /// <param name = "dels">删除的文件（相对路径）</param>
        /// <returns></returns>
        internal bool BackupFiles(int syncTaskid, string originPath, string backupdir, IList<string> adds,
                                  IList<string> dels)
        {
            if (adds == null) adds = new List<string>();
            if (dels == null) dels = new List<string>();

            string backupDir = originPath.Remove(originPath.Length - 1) + ".vws\\" + backupdir + "\\" + syncTaskid +
                               "\\";

            // 备份目录不存在时，需要先创建
            Directory.CreateDirectory(backupDir);

            //记录本次task到文件vwstask.txt
            string taskFilePath = backupDir + ClientConst.VwsTaskFile;
            using (var fs = new FileStream(taskFilePath, FileMode.Create))
            {
                using (var sw = new StreamWriter(fs, Encoding.Default))
                {
                    //add
                    foreach (string item in adds.Where(item => !string.IsNullOrEmpty(item)))
                    {
                        sw.WriteLine("A {0}", item);
                    }

                    //del
                    foreach (string item in dels.Where(item => !string.IsNullOrEmpty(item)))
                    {
                        sw.WriteLine("D {0}", item);
                    }
                    sw.Flush();
                }
            }

            //备份文件
            var backupFiles = new List<string>();
            foreach (var item in adds.Union(dels))
            {
                string orgin = originPath + item;
                string target = backupDir + item;

                //判断源文件是否存在
                if (!File.Exists(orgin)) continue;

                //创建备份目录
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                //复制文件
                File.Copy(orgin, target, true);
                backupFiles.Add(item);
            }

            //任务中的删除操作
            foreach (string orgin in dels.Select(item => originPath + item).Where(File.Exists))
            {
                File.Delete(orgin);
            }
            return true;
        }

        /// <summary>
        ///     获取文件列表
        /// </summary>
        /// <param name = "folderPath">文件夹地址</param>
        /// <returns></returns>
        internal List<string> GetAllFileName(string folderPath)
        {
            List<string> list = new List<string>();

            GetFileName(folderPath, ref list);

            return list;
        }

        /// <summary>
        /// 递归获取文件名
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="list"></param>
        private void GetFileName(string folderPath, ref List<string> list)
        {
            //判断folder是否存在，不存在时返回False
            if (!Directory.Exists(folderPath))
            {
                list.Add("end ====== " + folderPath);
                return;
            }

            DirectoryInfo dir = new DirectoryInfo(folderPath);
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                list.Add(file.FullName);
            }

            // 文件夹列表
            DirectoryInfo[] folders = dir.GetDirectories();
            foreach (DirectoryInfo folder in folders)
            {
                GetFileName(folder.FullName, ref list);
            }
        }

        /// <summary>
        ///     获取文件列表
        /// </summary>
        /// <param name = "folderPath">文件夹地址</param>
        /// <returns></returns>
        internal List<VwsDirectoryInfo> GetFileListNoMd5(string folderPath)
        {
            var fileList = new List<VwsDirectoryInfo>();
            //判断folder是否存在，不存在时返回False
            if (!Directory.Exists(folderPath))
            {
                return fileList;
            }

            var dir = new DirectoryInfo(folderPath);
            // 文件列表，最后写入时间倒序
            FileInfo[] files = dir.GetFiles();
            foreach (var file in files)
            {
                file.Refresh();
                var info = new VwsDirectoryInfo()
                {
                    Name = file.Name,
                    FullName = file.FullName,
                    Length = file.Length,
                    CreationTime = file.CreationTime,
                    UpdateTime = file.LastWriteTime,
                    IsFolder = false,
                    MD5 = string.Empty
                };
                fileList.Add(info);
            }

            // 文件夹列表
            DirectoryInfo[] folders = dir.GetDirectories();
            foreach (var folder in folders)
            {
                folder.Refresh();
                var info = new VwsDirectoryInfo()
                {
                    Name = folder.Name,
                    FullName = folder.FullName + "\\",
                    CreationTime = folder.CreationTime,
                    UpdateTime = folder.LastWriteTime,
                    IsFolder = true
                };
                fileList.Add(info);
            }
            return fileList;
        }

        /// <summary>
        ///     获取文件列表
        /// </summary>
        /// <param name = "folderPath">文件夹地址</param>
        /// <returns></returns>
        internal List<VwsDirectoryInfo> GetFileList(string folderPath)
        {
            var fileList = new List<VwsDirectoryInfo>();
            //判断folder是否存在，不存在时返回False
            if (!Directory.Exists(folderPath))
            {
                return fileList;
            }

            var dir = new DirectoryInfo(folderPath);
            // 文件列表，最后写入时间倒序
            foreach (var file in dir.GetFiles().OrderByDescending(item => item.LastWriteTime).Where(file => (file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden))
            {
                file.Refresh();
                var info = new VwsDirectoryInfo()
                {
                    Name = file.Name,
                    FullName = file.FullName,
                    Length = file.Length,
                    CreationTime = file.CreationTime,
                    UpdateTime = file.LastWriteTime,
                    IsFolder = false,
                    MD5 = CommonHelper.MD5File(file.FullName)
                };
                fileList.Add(info);
            }

            // 文件夹列表
            foreach (var folder in dir.GetDirectories().Where(folder => (folder.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden))
            {
                folder.Refresh();
                var info = new VwsDirectoryInfo()
                {
                    Name = folder.Name,
                    FullName = folder.FullName + "\\",
                    CreationTime = folder.CreationTime,
                    UpdateTime = folder.LastWriteTime,
                    IsFolder = true
                };
                fileList.Add(info);
            }
            return fileList;
        }

        internal List<VwsDirectoryInfo> GetFileList(TaskSenderEntity task)
        {
            var fileList = new List<VwsDirectoryInfo>();
            foreach (var filePath in task.AddList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var fileFullPath = task.TargetRoot + filePath;
                if (!File.Exists(fileFullPath)) continue;

                var fi = new FileInfo(fileFullPath);
                var info = new VwsDirectoryInfo
                               {
                                   Name = fi.Name,
                                   FullName = fi.FullName,
                                   Length = fi.Length,
                                   CreationTime = fi.CreationTime,
                                   UpdateTime = fi.LastWriteTime,
                                   IsFolder = false,
                                   MD5 = CommonHelper.MD5File(fi.FullName)
                               };
                fileList.Add(info);
            }
            return fileList;
        }
    }
}