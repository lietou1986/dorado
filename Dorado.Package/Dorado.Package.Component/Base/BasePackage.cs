using Dorado.Core;
using Dorado.Core.Logger;
using Dorado.Package.ServiceInterface.Model;
using Ionic.Zip;
using System;
using System.IO;
using System.Linq;

namespace Dorado.Package.Component.Base
{
    public abstract class BasePackage
    {
        protected PackageStruct PackageStruct { get; set; }

        protected PackageNotice PackageNotice { get; set; }

        public string ZipFileFullPath { get; private set; }

        protected BasePackage(PackageStruct packageStruct)
        {
            this.PackageStruct = packageStruct;
        }

        protected BasePackage(PackageStruct packageStruct, PackageNotice packageNotice)
        {
            this.PackageStruct = packageStruct;
            this.PackageNotice = packageNotice;
        }

        /// <summary>
        /// 初使化包数据
        /// </summary>
        private void InitializationZipFile()
        {
            //create Zip
            try
            {
                string zipFilePath = String.Format(@"{0}\{1}", PackageSettings.Instance.PackagePath, DateTime.Now.ToString("yyyy-MM-dd"));
                string zipFileName = String.Format("{0}-{1}-{2}.zip", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"), PackageStruct.PackName, System.Guid.NewGuid());
                string zipFileFullPath = Path.Combine(zipFilePath, zipFileName);
                if (!Directory.Exists(zipFilePath))
                    Directory.CreateDirectory(zipFilePath);
                this.ZipFileFullPath = zipFileFullPath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 打包
        /// </summary>
        public void Package()
        {
            try
            {
                BeforePackage();
                Packaging();
                AfterPackage();
                if (PackageNotice != null)
                    PackageNoticing();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 打包中
        /// </summary>
        private void Packaging()
        {
            VerifyPackageStruct();
            InitializationZipFile();
            using (ZipFile zip = new ZipFile(ZipFileFullPath, System.Text.Encoding.Default))
            {
                try
                {
                    PackageStruct.ListPackageCatalog.ForEach(listPackageCatalog =>
                    {
                        string category = listPackageCatalog.CatalogLevelStringName;
                        if (listPackageCatalog.ListPackageFileInfo.Count > 0)
                        {
                            listPackageCatalog.ListPackageFileInfo.ForEach(
                                listPackageFileInfo =>
                                {
                                    if (!string.IsNullOrEmpty(listPackageFileInfo.Path))
                                    {
                                        if (!string.IsNullOrEmpty(category))
                                            zip.AddFile(listPackageFileInfo.Path, category);
                                        else
                                            zip.AddFile(listPackageFileInfo.Path);
                                    }
                                });
                        }
                    });
                    zip.Save();
                    zip.Dispose();
                }
                catch (Exception ex)
                {
                    PackageException pkEx = new PackageException("Package error," + ex.Message);
                    throw pkEx;
                }
            }
        }

        private void VerifyPackageStruct()
        {
            if (PackageStruct.ListPackageCatalog.Count == 0)
                throw new PackageStructParamException("ListPackageCatalog is null");

            if (!PackageStruct.ListPackageCatalog.Any(c => c.ListPackageFileInfo.Count > 0))
                throw new PackageStructParamException("ListPackageFileInfo is null");
        }

        /// <summary>
        /// 打包之后
        /// </summary>
        protected abstract void AfterPackage();

        /// <summary>
        /// 打包之前准备数据
        /// </summary>
        protected abstract void BeforePackage();

        /// <summary>
        /// 可以重写此方法，用你自已的通知方式
        /// </summary>
        protected virtual void PackageNoticing()
        {
            try
            {
                //发送通知告诉打包完成
            }
            catch (System.Exception ex)
            {
                PackageNoticeException packageEx = new PackageNoticeException("Send private message error," + ex.Message, ex.InnerException);
                LoggerWrapper.Logger.Error("Package", packageEx);
            }
        }
    }
}