using Dorado.Package.Component.Base;
using Dorado.Package.Component.Recruit;
using Dorado.Package.ServiceInterface.Model;

namespace Dorado.Package.ServiceImp.Controllers
{
    internal class PackageDispatcher : BasePackageDispatcher
    {
        #region singleton

        private static readonly PackageDispatcher instance = new PackageDispatcher();

        public static PackageDispatcher Instance
        {
            get { return instance; }
        }

        #endregion singleton

        public PackageDispatcher()
            : base()
        {
        }

        protected override BasePackage GeneratePackageInstance(string packageType, PackageStruct packageStruct, PackageNotice packageNotice)
        {
            BasePackage basePackage = null;
            switch (packageType)
            {
                case "recruit":
                    basePackage = new RecruitDocsPackage(packageStruct, packageNotice);
                    break;
            }
            return basePackage;
        }
    }
}