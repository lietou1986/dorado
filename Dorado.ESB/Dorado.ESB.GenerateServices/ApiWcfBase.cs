using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.ServiceModel;

namespace Dorado.ESB.GenerateServices
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall)]
    public class ApiWcfBase
    {
        public ApiWcfBase()
        {
        }

        public static void LogException(Exception ex, string category)
        {
            string errorInfo = String.Format("Action:{0},Address:{1}", OperationContext.Current.RequestContext.RequestMessage.Headers.Action, OperationContext.Current.RequestContext.RequestMessage.Headers.To.ToString());

            LoggerWrapper.Logger.Error(category + " && " + errorInfo, ex);
        }
    }
}