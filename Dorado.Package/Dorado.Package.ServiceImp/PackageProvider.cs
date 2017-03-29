using Dorado.Package.ServiceImp.Persistence;
using Dorado.Package.ServiceImp.Utility;
using Dorado.Package.ServiceInterface.Model;
using Dorado.Utils;
using System;
using Dorado.Package.ServiceImp.Controllers;

namespace Dorado.Package.ServiceImp
{
    public class PackageProvider : Dorado.Package.ServiceInterface.IPackageProvider
    {
        #region singleton

        private static readonly PackageProvider instance = new PackageProvider();

        public static PackageProvider Instance
        {
            get { return instance; }
        }

        #endregion singleton

        public string RequestPackage(int productType, ServiceInterface.Model.PackageStruct packageStruct, PackageNotice packageNotice)
        {
            Guard.ArgumentPositive(productType, "productType");
            Guard.ArgumentPositive(packageStruct.ListPackageCatalog.Count, "packageStruct");
            Guard.ArgumentNotEmpty(packageStruct.PackageType.ToString(), "packageType");
            Guard.ArgumentNotEmpty(packageStruct.PackagePriority.ToString(), "packagePriority");
            Guard.ArgumentNotEmpty(packageStruct.PackName, "packName");
            string packageNo = Guid.NewGuid().ToString();
            PackageDao.AddPackage(new Guid(packageNo), productType, packageStruct.PackageType.ToString(), SerializeUtility.SerializeIt(packageStruct), packageNotice != null ? SerializeUtility.SerializeIt(packageNotice) : "", 0,
                                  (int)packageStruct.PackagePriority);
            return packageNo;
        }

        public string RequestPackageNotNotice(int productType, ServiceInterface.Model.PackageStruct packageStruct)
        {
            return this.RequestPackage(productType, packageStruct, null);
        }

        public ServiceInterface.Model.PackageStatus GetPackageStatus(string packageNo)
        {
            Guard.ArgumentNotEmpty(packageNo, "packageNo");
            Guard.ArgumentIsGuid(packageNo);
            return PackageDao.GetPackageStatusByNo(new Guid(packageNo));
        }

        public string GetPackageUrl(string packageNo)
        {
            Guard.ArgumentNotEmpty(packageNo, "packageNo");
            Guard.ArgumentIsGuid(packageNo);
            string localPath = PackageDao.GetPackageAddressByNo(new Guid(packageNo));
            string packageUrl = string.Empty;
            if (!string.IsNullOrEmpty(localPath))
            {
                string fileName = UtilityHelper.GetPackagePartPath(localPath);
                packageUrl = String.Concat(PackageSettings.Instance.HttpBaseAddress, fileName);
            }
            return packageUrl;
        }

        public void Running()
        {
            PackageDispatcher.Instance.Running();
        }

        public void Stop()
        {
            PackageDispatcher.Instance.Stop();
        }
    }
}