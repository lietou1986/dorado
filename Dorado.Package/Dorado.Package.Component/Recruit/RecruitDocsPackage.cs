using System;
using System.IO;
using Dorado.Package.Component.Base;
using Dorado.Package.ServiceInterface.Model;

namespace Dorado.Package.Component.Recruit
{
    public class RecruitDocsPackage : BasePackage
    {
        public RecruitDocsPackage(PackageStruct packageStruct)
            : base(packageStruct)
        {
        }

        public RecruitDocsPackage(PackageStruct packageStruct, PackageNotice packageNotice)
            : base(packageStruct, packageNotice)
        {
        }

        /// <summary>
        /// 业务重写，准备数据生成文件，填充PackageStruct对象中的Path属性
        /// </summary>
        /// <remarks>
        /// 从DFS/网络磁盘/本地磁盘/数据库等生成的文件请统一放在PackageSettings.Instance.PackageTempPath
        /// 打包业务特殊异常，请使用PackBizException包装
        /// 根据需要，可以在此项目引用其它业务DLL
        /// </remarks>
        protected override void BeforePackage()
        {
            //业务自己实现
            //throw new NotImplementedException();

            string tempPath = Path.Combine(PackageSettings.Instance.PackageTempPath,
                                        PackageStruct.PackageType.ToString(), DateTime.Now.ToString("yyyy-MM-dd"));
            if (!Directory.Exists(tempPath))
                Directory.CreateDirectory(tempPath);

            //IApplicantProvider applicantProvider = PlatformServiceFactory<IApplicantProvider>.CreateInstance( );
            // IResumeProvider resumeProvider = PlatformServiceFactory<IResumeProvider>.CreateInstance( );

            string downName = string.Empty;
            byte[] bytes;
            PackageStruct.ListPackageCatalog.ForEach(listPackageCatalog =>
            {
                listPackageCatalog.ListPackageFileInfo.ForEach(
                    listPackageFileInfo =>
                    {
                        if (!string.IsNullOrEmpty(listPackageFileInfo.IdentID))
                        {
                            string identID = listPackageFileInfo.IdentID;
                            string[] temp = identID.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);//取得文件标识
                            int personId = System.Int32.Parse(temp[0]);
                            string docFlag = temp[1];
                            int docType = System.Int32.Parse(temp[2]);

                            switch (docType)
                            {
                                default:
                                    bytes = null;
                                    break;
                            }

                            string filePath = string.Empty;
                            if (docType < 8)
                            {
                                filePath = Path.Combine(tempPath, downName);
                                using (MemoryStream m = new MemoryStream(bytes))

                                //定义实际文件对象，保存上载的文件。
                                using (FileStream f = new FileStream(filePath, FileMode.Create))
                                {
                                    //把内内存里的数据写入物理文件
                                    m.WriteTo(f);
                                    m.Close();
                                    f.Close();
                                }
                            }

                            //*****对listPackageFileInfo.Path进行赋值*****
                            listPackageFileInfo.Path = filePath;
                        }
                    });
            });
        }

        /// <summary>
        /// 业务重写，打包完成后需要做的工作
        ///  打包业务特殊异常，请使用PackBizException包装
        /// 根据需要，可以在此项目引用其它业务DLL
        /// </summary>
        protected override void AfterPackage()
        {
            //打包完成后删除临时文件
            PackageStruct.ListPackageCatalog.ForEach(listPackageCatalog =>
           {
               listPackageCatalog.ListPackageFileInfo.ForEach(
                   listPackageFileInfo =>
                   {
                       string tempPath = listPackageFileInfo.Path;
                       if (File.Exists(tempPath))
                           File.Delete(tempPath);
                   });
           });
        }
    }
}