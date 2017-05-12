using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Dorado.Configuration;
using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.VWS.Model.Enum;
using Dorado.VWS.Services;
using ICSharpCode.SharpZipLib.Zip;

namespace Dorado.VWS.Admin.Handler
{
    /// <summary>
    /// Summary description for UploadFile
    /// </summary>
    public class UploadFile : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var files = context.Request.Files;

                string packageLocalPath = AppSettingProvider.Get("UploadPackagePath");
                if (!Directory.Exists(packageLocalPath))
                {
                    //DirectorySecurity security = new DirectorySecurity();
                    Directory.CreateDirectory(packageLocalPath);
                }
                context.Response.ContentType = "text/plain";
                if (files != null && files.Count > 0)
                {
                    HttpPostedFile file = files[0];
                    string localFullFileName = packageLocalPath + file.FileName;

                    if (File.Exists(localFullFileName))
                        File.Delete(localFullFileName);

                    file.SaveAs(localFullFileName);

                    if (UnzipAndOverWritePackage(localFullFileName, int.Parse(context.Request.QueryString["domainid"].ToString().Trim())))
                    {
                        context.Response.Write("Hello World");
                    }
                    else
                    {
                        context.Response.Write("Failed");
                    }
                }
                else
                {
                    context.Response.Write("Failed");
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS.UploadZipFile", ex.ToString());
                context.Response.ContentType = "text/plain";
                context.Response.Write("Failed");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private bool UnzipAndOverWritePackage(string packageName, int domainId)
        {
            try
            {
                ServerProvider serverProvider = new ServerProvider();
                var serverList = serverProvider.GetServersByDomainId(domainId);
                var path = (from server in serverList where server.ServerType == EnumServerType.Source select server.Root).First();
                UnZip(packageName, path);
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("VWS.UploadZipFile", ex.ToString() + packageName + " domainId:" + domainId);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 功能：解压zip格式的文件。
        /// </summary>
        /// <param name="zipFilePath">压缩文件路径</param>
        /// <param name="unZipDir">解压文件存放路径,为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>
        /// <param name="err">出错信息</param>
        /// <returns>解压是否成功</returns>
        private void UnZip(string zipFilePath, string unZipDir)
        {
            if (zipFilePath == string.Empty)
            {
                throw new Exception("压缩文件不能为空！");
            }
            if (!File.Exists(zipFilePath))
            {
                throw new System.IO.FileNotFoundException("压缩文件不存在！");
            }
            //解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹
            if (unZipDir == string.Empty)
                unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
            if (!unZipDir.EndsWith("\\"))
                unZipDir += "\\";
            if (!Directory.Exists(unZipDir))
                Directory.CreateDirectory(unZipDir);
            Encoding code = Encoding.GetEncoding("GB2312");
            ZipConstants.DefaultCodePage = code.CodePage;
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(unZipDir + directoryName);
                    }
                    if (!directoryName.EndsWith("\\"))
                        directoryName += "\\";
                    if (fileName != String.Empty)
                    {
                        if (File.Exists(unZipDir + theEntry.Name))
                        {
                            FileAttributes attributes = File.GetAttributes(unZipDir + theEntry.Name);

                            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                            {
                                // Make the file not RW
                                attributes = RemoveAttribute(attributes, FileAttributes.ReadOnly);
                                File.SetAttributes(unZipDir + theEntry.Name, attributes);
                            }
                        }
                        using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))
                        {
                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }
    }
}