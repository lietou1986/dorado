using System;
using System.Collections.Generic;
using Dorado.Package.ServiceImp;
using Dorado.Package.ServiceImp.Persistence;
using Dorado.Package.ServiceImp.Utility;
using Dorado.Package.ServiceInterface.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dorado.Test.Package
{
    /// <summary>
    ///这是 PackageProviderTest 的测试类，旨在
    ///包含所有 PackageProviderTest 单元测试
    ///</summary>
    [TestClass()]
    public class PackageProviderTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        private PackageProvider target = new PackageProvider();
        private PackageStruct packageStruct = Get_PackageStruct();
        private PackageNotice packageNotice = Get_PackageNotice();
        private string packageNo1, packageNo2;

        #region 附加测试特性

        //
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion 附加测试特性

        private static PackageNotice Get_PackageNotice()
        {
            PackageNotice packageNotice = new PackageNotice();
            packageNotice.Subject = "打包";
            packageNotice.Content = "打包了，查看";
            packageNotice.ReceiverId = "1000";
            packageNotice.TenantID = 8888;
            return packageNotice;
        }

        private static PackageStruct Get_PackageStruct()
        {
            List<string> listReportNo = new List<string>();
            listReportNo.Add("672b1b06-1739-4af9-819b-52a6400dac6f");
            listReportNo.Add("84084070-7d1e-40fd-837a-a33918200f31");

            PackageStruct packageStruct = new PackageStruct();
            packageStruct.PackName = "张三报告";//zip包名前缀
            packageStruct.PackagePriority = PackagePriority.Highest;
            packageStruct.PackageType = PackageType.Report;
            List<PackageCatalog> listPackageCatalog = new List<PackageCatalog>();

            PackageCatalog reportPackCategory = new PackageCatalog();

            //reportPackCategory.CatalogLevelStringName = "测评/报告";

            int len = listReportNo.Count;
            for (int i = 0; i < len; i++)
            {
                PackageFileInfo packageFileInfo = new PackageFileInfo();
                packageFileInfo.IdentID = listReportNo[i];
                reportPackCategory.ListPackageFileInfo.Add(packageFileInfo);
            }
            listPackageCatalog.Add(reportPackCategory);
            packageStruct.ListPackageCatalog.Add(reportPackCategory);
            return packageStruct;
        }

        /// <summary>
        ///整个打包服务完整测试
        ///</summary>
        [TestMethod()]
        public void PackageTest()
        {
            RequestPackageTest();
            RequestPackageNotNoticeTest();
            GetPackageStatusTest(PackageStatus.UnProcessed);

            //****无法中断RunningTest*****
            //RunningTest();//
            //StopTest();
            //GetPackageStatusTest(PackageStatus.Processed);
            //GetPackageUrlTest();
            //DeletePackage();
        }

        /// <summary>
        ///RequestPackage 的测试
        ///</summary>
        private void RequestPackageTest()
        {
            int productType = 1;
            string actual = target.RequestPackage(productType, packageStruct, packageNotice);
            bool isGuid = UtilityHelper.IsGuid(actual);
            packageNo1 = actual;
            Assert.IsTrue(isGuid);
        }

        /// <summary>
        ///RequestPackageNotNotice 的测试
        ///</summary>
        private void RequestPackageNotNoticeTest()
        {
            int productType = 1;
            string actual = target.RequestPackageNotNotice(productType, packageStruct);
            bool isGuid = UtilityHelper.IsGuid(actual);
            packageNo2 = actual;
            Assert.IsTrue(isGuid);
        }

        private void DeletePackage()
        {
            Assert.IsTrue(PackageDao.DeletePackageByNo(new Guid(packageNo1)));
            Assert.IsTrue(PackageDao.DeletePackageByNo(new Guid(packageNo2)));
        }

        /// <summary>
        ///GetPackageStatus 的测试
        ///</summary>
        private void GetPackageStatusTest(PackageStatus expected)
        {
            PackageStatus actual;
            actual = target.GetPackageStatus(packageNo1);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///GetPackageUrl 的测试
        ///</summary>
        private void GetPackageUrlTest()
        {
            string actual = target.GetPackageUrl(packageNo1);
            Assert.IsTrue(string.IsNullOrEmpty(actual));
        }

        /// <summary>
        ///Running 的测试
        ///</summary>
        private void RunningTest()
        {
            target.Running();
            System.Threading.Thread.Sleep(5000);
        }

        /// <summary>
        ///Stop 的测试
        ///</summary>
        public void StopTest()
        {
            target.Stop();
        }
    }
}