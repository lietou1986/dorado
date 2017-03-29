using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using HelloWorldApp.Contracts.DataContracts;
using System.ServiceModel.Web;

namespace HelloWorldApp.Contracts.ServiceContracts
{
    [ServiceContract]
    
    public interface IHelloWorldService
    {
        #region 
        //[OperationContract(Action = "GetHelloWorldApp", IsOneWay = false)]
        //string GetHelloWorldApp();
        //[OperationContract(Action = "GetUserInfo", IsOneWay = false)]
        //UserInfo GetUserInfo(string userName);
        //[OperationContract(Action = "Ping", IsOneWay = false)]
        //bool Ping();

        //[OperationContract(Action = "Ping1", IsOneWay = false, Name = "MyPing1")]
        //bool Ping(int x);

        //[OperationContract(Action = "Ping2", IsOneWay = false, Name = "MyPing2")]
        //bool Ping(string y);
        #endregion



        #region WebGet/WebInvoke


        [WebGet(UriTemplate = "json/GetHelloWorldApp", ResponseFormat = WebMessageFormat.Json), OperationContract(Action = "GetHelloWorldApp", IsOneWay = false)]
        string GetHelloWorldApp();

        [WebGet(UriTemplate = "json/Ping", ResponseFormat = WebMessageFormat.Json), OperationContract(Action = "Ping", IsOneWay = false)]
        bool Ping();

        [WebInvoke(Method = "POST", UriTemplate = "json/GetUserInfo/{userName}", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json), OperationContract(Action = "GetUserInfo", IsOneWay = false)]
        UserInfo GetUserInfo(string userName);

        [OperationContract(Action = "GetUserInfo5", IsOneWay = false), WebGet(ResponseFormat = WebMessageFormat.Json)]
        UserInfo GetUserInfo5(string userName);


        [OperationContract(Action = "SetUserInfo2", IsOneWay = false), WebInvoke(Method = "POST", UriTemplate = "json/info2", ResponseFormat = WebMessageFormat.Json)]
        UserInfo SetUserInfo2(UserInfo userinfo);

        [OperationContract(Action = "SetUserInfo3", IsOneWay = false), WebInvoke(Method = "POST", UriTemplate = "json/info3", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        UserInfo SetUserInfo3(UserInfo userinfo, string country, Test test);

        [OperationContract(Action = "SetUserInfo4", IsOneWay = false), WebInvoke(Method = "POST", UriTemplate = "json/info4", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        UserInfo SetUserInfo4(UserInfo userinfo, string country);

        [OperationContract(Action = "SetUserInfo5", IsOneWay = false), WebInvoke(Method = "POST", UriTemplate = "json/info5", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        UserInfo SetUserInfo5(List<UserInfo> userinfoList, State state,DateTime dt);


        [OperationContract(Action = "SetUserInfo6", IsOneWay = false), WebInvoke(Method = "POST", UriTemplate = "json/info6", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        DateTime SetUserInfo6(DateTime dt);

        [OperationContract(Action = "SetUserInfo7", IsOneWay = false), WebInvoke(Method = "POST", UriTemplate = "json/info7", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        State SetUserInfo7(State st);
        #endregion
      

    }
}
