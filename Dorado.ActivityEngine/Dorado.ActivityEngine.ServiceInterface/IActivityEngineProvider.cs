using System.ServiceModel;
using System.ServiceModel.Web;

namespace Dorado.ActivityEngine.ServiceInterface
{
    [ServiceContract]
    public interface IActivityEngineProvider
    {
        [WebInvoke(Method = "POST", UriTemplate = "json/ActivityEngine/RaiseActivity", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json), OperationContract(Action = "RaiseActivity")]
        void RaiseActivity(Activity activity);
    }
}