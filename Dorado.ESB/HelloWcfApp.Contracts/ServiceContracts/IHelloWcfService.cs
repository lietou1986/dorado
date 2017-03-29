using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using HelloWcfApp.Contracts.DataContracts;




namespace HelloWcfApp.Contracts.ServiceContracts
{
    [ServiceContract]
    public interface IHelloWcfService
    {
        [OperationContract(Action = "GetHelloWcfApp", IsOneWay = false)]
        string GetHelloWcfApp();
        [OperationContract(Action = "GetAccountInfo", IsOneWay = false)]
        AccountInfo GetAccountInfo(string userName);
        [OperationContract(Action = "Ping", IsOneWay = false)]
        bool Ping();
    }
}
