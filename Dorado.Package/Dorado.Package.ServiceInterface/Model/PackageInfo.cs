namespace Dorado.Package.ServiceInterface.Model
{
    public class PackageInfo
    {
        public string PackageNo { get; set; }

        public int ProductType { get; set; }

        public PackageStruct PackageStruct { get; set; }

        public PackageNotice PackageNotice { get; set; }

        public PackagePriority PackagePriority { get; set; }

        public string PackageType { get; set; }
    }
}