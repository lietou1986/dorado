using System.Collections.Generic;
using System.Linq;
using Dorado.Package.ServiceInterface.Model;

namespace Dorado.Package.ClientDemo
{
    internal class Program
    {
        private static void Main( string[ ] args )
        {
            PackageStruct packageStruct = Get_PackageStruct( );

            PackageNotice packageNotice = Get_PackageNotice( );

            //string packNo1 = PackageProvider.Instance.RequestPackage(1, packageStruct, packageNotice);
            //string packNo2 = PackageProvider.Instance.RequestPackageNotNotice(1, packageStruct);
            //PackageStatus packageStatus = PackageProvider.Instance.GetPackageStatus("673FBA19-9FDB-42A9-9079-C36AC7286AF3");
            //string result  = PackageProvider.Instance.GetPackageUrl("673FBA19-9FDB-42A9-9079-C36AC7286AF3");

            Dorado.Package.ServiceImp.PackageProvider.Instance.Running( );
        }

        private static PackageNotice Get_PackageNotice( )
        {
            PackageNotice packageNotice = new PackageNotice( );
            packageNotice.Subject = "打包";
            packageNotice.Content = "打包了，查看";
            packageNotice.ReceiverId = "1000";
            packageNotice.TenantID = 8888;
            return packageNotice;
        }

        private static PackageStruct Get_PackageStruct( )
        {
            List<string> listReportNo = new List<string>( );
            listReportNo.Add( "672b1b06-1739-4af9-819b-52a6400dac6f" );
            listReportNo.Add( "84084070-7d1e-40fd-837a-a33918200f31" );

            PackageStruct packageStruct = new PackageStruct( );
            packageStruct.PackName = "张三报告";//zip包名前缀
            packageStruct.PackagePriority = PackagePriority.Highest;
            packageStruct.PackageType = PackageType.Report;
            List<PackageCatalog> listPackageCatalog = new List<PackageCatalog>( );

            PackageCatalog reportPackCategory = new PackageCatalog( );

            //reportPackCategory.CatalogLevelStringName = "测评/报告";

            int len = listReportNo.Count( );
            for ( int i = 0 ; i < len ; i++ )
            {
                PackageFileInfo packageFileInfo = new PackageFileInfo( );
                packageFileInfo.IdentID = listReportNo[ i ];
                reportPackCategory.ListPackageFileInfo.Add( packageFileInfo );
            }
            listPackageCatalog.Add( reportPackCategory );
            packageStruct.ListPackageCatalog.Add( reportPackCategory );
            return packageStruct;
        }
    }
}