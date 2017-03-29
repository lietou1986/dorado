using Dorado.Package.ServiceInterface.Model;
using System.ServiceModel;

namespace Dorado.Package.ServiceInterface
{
    [ServiceContract]
    public interface IPackageProvider
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="productType">productType 1 assessment,2 recruit,3 survey,4 360 ,5 succession,6 reportengine</param>
        /// <param name="packageStruct"></param>
        /// <param name="packageNotice"></param>
        /// <returns></returns>
        [OperationContract(Action = "RequestPackage")]
        string RequestPackage(int productType, PackageStruct packageStruct, PackageNotice packageNotice);

        [OperationContract(Action = "RequestPackageNotNotice")]
        string RequestPackageNotNotice(int productType, PackageStruct packageStruct);

        [OperationContract(Action = "GetPackageStatus")]
        PackageStatus GetPackageStatus(string packageNo);

        [OperationContract(Action = "GetPackageUrl")]
        string GetPackageUrl(string packageNo);
    }
}