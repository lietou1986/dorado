using System;
using System.Collections.Generic;
using Dorado.Package.ServiceImp.Persistence;
using Dorado.Package.ServiceInterface.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dorado.Test.Package
{
    /// <summary>
    ///这是 PackageDaoTest 的测试类，旨在
    ///包含所有 PackageDaoTest 单元测试
    ///</summary>
    [TestClass()]
    public class PackageDaoTest
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

        private Guid packageNo = new Guid("AF3E9885-64A1-43AD-813A-9475C979D441");

        /// <summary>
        ///测试Package数据所有存储过程
        ///</summary>
        [TestMethod()]
        public void TestPackageAll()
        {
            DeleteReportDataTest();
            AddPackageTest();
            GetPackageTaskTest();
            GetPackageDataByNoTest();
            UpdatePackageStatusTest();
            GetPackageStatusByNoTest();
            UpdatePackageDataByNoTest();
            GetPackageAddressByNoTest();
            DeleteReportDataTest();
        }

        private void GetPackageTaskTest()
        {
            int packagePriority = 1;
            List<PackageInfo> actual = PackageDao.GetPackageTask(packagePriority);
            Assert.IsTrue(actual.Exists(c => c.PackageNo == packageNo.ToString()));
        }

        private void AddPackageTest()
        {
            int productType = 1;
            string packageType = "Report";
            string packageStruct = @"<PackageStruct>
                                                          <PackName>张三报告</PackName>
                                                          <PackagePriority>Highest</PackagePriority>
                                                          <PackageType>Report</PackageType>
                                                          <ListPackageCatalog>
                                                            <PackageCatalog>
                                                              <CatalogLevelStringName>测评/报告</CatalogLevelStringName>
                                                              <ListPackageFileInfo>
                                                                <PackageFileInfo>
                                                                  <IdentID>672b1b06-1739-4af9-819b-52a6400dac6f</IdentID>
                                                                </PackageFileInfo>
                                                                <PackageFileInfo>
                                                                  <IdentID>84084070-7d1e-40fd-837a-a33918200f31</IdentID>
                                                                </PackageFileInfo>
                                                              </ListPackageFileInfo>
                                                            </PackageCatalog>
                                                          </ListPackageCatalog>
                                                        </PackageStruct>";
            string packageNotice = @"<PackageNotice>
                                                          <Subject>打包</Subject>
                                                          <Content>打包了，查看</Content>
                                                          <ReceiverId>1000</ReceiverId>
                                                          <TenantID>8888</TenantID>
                                                        </PackageNotice>";
            int packageStatus = 0;
            int packagePriority = 1;

            Assert.IsTrue(PackageDao.AddPackage(packageNo, productType, packageType, packageStruct, packageNotice, packageStatus, packagePriority));
        }

        private void GetPackageAddressByNoTest()
        {
            string expected = @"D:\Package\Zip\2010-12-16\2010-12-16-14-58-14-张三报告-be72516c-6a09-444b-b3bd-0e6c42773a2c.zip";
            string actual;
            actual = PackageDao.GetPackageAddressByNo(packageNo);
            Assert.AreEqual(expected, actual);
        }

        private void GetPackageDataByNoTest()
        {
            PackageInfo actual = PackageDao.GetPackageDataByNo(packageNo);
            Assert.AreEqual(actual.PackageType, "Report");
            Assert.AreEqual(actual.ProductType, 1);
            Assert.AreEqual(actual.PackageNo, packageNo.ToString());
        }

        public void UpdatePackageStatusTest()
        {
            int packageStatus = 2;

            PackageDao.UpdatePackageStatus(packageNo, packageStatus);
        }

        private void GetPackageStatusByNoTest()
        {
            PackageStatus expected = PackageStatus.Processed;
            PackageStatus actual;
            actual = PackageDao.GetPackageStatusByNo(packageNo);
            Assert.AreEqual(expected, actual);
        }

        private void UpdatePackageDataByNoTest()
        {
            string packageAddress = @"D:\Package\Zip\2010-12-16\2010-12-16-14-58-14-张三报告-be72516c-6a09-444b-b3bd-0e6c42773a2c.zip";
            PackageDao.UpdatePackageDataByNo(packageNo, packageAddress);
        }

        private void DeleteReportDataTest()
        {
            Assert.IsTrue(PackageDao.DeletePackageByNo(packageNo));
        }
    }
}