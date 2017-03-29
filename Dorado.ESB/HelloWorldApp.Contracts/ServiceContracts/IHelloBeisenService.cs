using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using System.ServiceModel;
using HelloWorldApp.Contracts.DataContracts;

namespace HelloWorldApp.Contracts.ServiceContracts
{
    [ServiceContract]
    public interface IHelloBeisenService
    {
        [OperationContract(Action = "SetUserInfo8", IsOneWay = false), WebInvoke(Method = "POST", UriTemplate = "json/info8", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        State SetUserInfo8(State st);
    }
}
