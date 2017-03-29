using Dorado.ESB.Common.Utility;
using System;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;

namespace Dorado.ESB.Extensions.Auth
{
    public enum ErrorCode
    {
        SignatureError = 1000,
        TokenNotExist = 1001,
        HeaderIsNull = 1002,
        BusinessError = 1003,
        UnLaw = 1004,
        TokenTimeout = 1005,
        NoPermission = 1006,
        EncryptError = 1007,
        ProductNotExist = 1008,
        TokenStop = 1009,
    }

    public class HeaderContext
    {
        public string ProductName
        {
            get;
            set;
        }

        public string MethodName
        {
            get;
            set;
        }

        public string Token
        {
            get;
            set;
        }

        public string Timestamp
        {
            get;
            set;
        }

        public string Nonce
        {
            get;
            set;
        }

        public string Signature
        {
            get;
            set;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizationAttribute : Attribute, IOperationBehavior
    {
        public AuthorizationAttribute(bool isAuthorization)
        {
            this.IsAuthorization = isAuthorization;
        }

        public bool IsAuthorization
        {
            get;
            set;
        }

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            if (IsAuthorization)
            {
                dispatchOperation.Invoker = new AuthorizationOperationInvoker(dispatchOperation.Invoker);
            }
        }

        public void Validate(OperationDescription operationDescription)
        {
        }
    }

    public class AuthorizationOperationInvoker : IOperationInvoker
    {
        private const string AUTHORIZATION = "Authorization";
        private const string AUTHORIZATION_NAMESPACE = "api.Dorado.com";
        private IOperationInvoker innerOperationInvoker;

        public AuthorizationOperationInvoker(IOperationInvoker innerOperationInvoker)
        {
            this.innerOperationInvoker = innerOperationInvoker;
        }

        private string EnumToString(ErrorCode errorCode)
        {
            string errorStr = String.Format("{0},{1}", ((int)errorCode).ToString(), errorCode.ToString());
            return errorStr;
        }

        private void CheckHeaderContext(HeaderContext headerContext)
        {
            if (headerContext == null)
            {
                throw new System.ServiceModel.FaultException<CustomFault>(new CustomFault() { Code = (int)ErrorCode.HeaderIsNull, Reason = ErrorCode.HeaderIsNull.ToString() }, EnumToString(ErrorCode.HeaderIsNull));
            }
        }

        private HeaderContext GetHeaderContext(string headerValue)
        {
            if (string.IsNullOrEmpty(headerValue)) return null;
            HeaderContext headerContext = null;
            string[] arrayValues = headerValue.Split('&');
            if (arrayValues.Length == 4)
            {
                headerContext = new HeaderContext();
                headerContext.Token = arrayValues[0].ToLower();
                headerContext.Timestamp = arrayValues[1];
                headerContext.Nonce = arrayValues[2];
                headerContext.Signature = arrayValues[3];
            }

            return headerContext;
        }

        private string GetProtocolName()
        {
            string protocolName = string.Empty;
            if (OperationContext.Current != null && OperationContext.Current.EndpointDispatcher != null
               && OperationContext.Current.EndpointDispatcher.ChannelDispatcher != null)
            {
                protocolName = OperationContext.Current.EndpointDispatcher.ChannelDispatcher.BindingName;
            }
            else
            {
                throw new System.ServiceModel.FaultException<CustomFault>(new CustomFault() { Code = (int)ErrorCode.BusinessError, Reason = ErrorCode.BusinessError.ToString() }, EnumToString(ErrorCode.BusinessError));
            }
            return protocolName;
        }

        private string[] GetStrings(string source)
        {
            string[] array = source.Split('/');

            return array;
        }

        private void ParseUrl(HeaderContext headerContext, string source)
        {
            if (headerContext != null)
            {
                string[] strings = GetStrings(source);
                headerContext.ProductName = strings[4];
                headerContext.MethodName = strings[5];
            }
        }

        private void WebHttpBindingAuthorization()
        {
            string headerValue = string.Empty;
            try
            {
                IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
                WebHeaderCollection header = request.Headers;
                if (header.AllKeys.Contains(AUTHORIZATION))
                {
                    headerValue = header[AUTHORIZATION];
                    HeaderContext headerContext = GetHeaderContext(headerValue);
                    string source = OperationContext.Current.RequestContext.RequestMessage.Headers.To.ToString();
                    if (source.IndexOf("?") > -1)
                        source = source.Substring(0, headerContext.ProductName.IndexOf("?"));
                    ParseUrl(headerContext, source);
                    CheckHeaderContext(headerContext);
                }
                else
                {
                    throw new System.ServiceModel.FaultException<CustomFault>(new CustomFault() { Code = (int)ErrorCode.HeaderIsNull, Reason = ErrorCode.HeaderIsNull.ToString() }, EnumToString(ErrorCode.HeaderIsNull));
                }
            }
            catch (FaultException<CustomFault> fc)
            {
                throw fc;
            }
            catch (Exception ex)
            {
                throw new System.ServiceModel.FaultException<CustomFault>(new CustomFault() { Code = (int)ErrorCode.BusinessError, Reason = ErrorCode.BusinessError.ToString() }, String.Format("{0},{1}", EnumToString(ErrorCode.BusinessError), ex.Message));
            }
        }

        private void BasicHttpBindingAuthorization()
        {
            try
            {
                string headerValue = string.Empty;
                MessageHeaders currentHeaders = OperationContext.Current.IncomingMessageHeaders;
                if (currentHeaders != null && currentHeaders.Count() > 0)
                {
                    int index = currentHeaders.FindHeader(AUTHORIZATION, AUTHORIZATION_NAMESPACE);
                    if (index > -1)
                    {
                        headerValue = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>(AUTHORIZATION, AUTHORIZATION_NAMESPACE);
                        HeaderContext headerContext = GetHeaderContext(headerValue);
                        if (headerContext != null)
                        {
                            headerContext.ProductName = OperationContext.Current.EndpointDispatcher.ContractName;
                            headerContext.MethodName = OperationContext.Current.RequestContext.RequestMessage.Headers.Action;
                        }
                        CheckHeaderContext(headerContext);
                    }
                    else
                    {
                        throw new System.ServiceModel.FaultException<CustomFault>(new CustomFault() { Code = (int)ErrorCode.HeaderIsNull, Reason = ErrorCode.HeaderIsNull.ToString() }, EnumToString(ErrorCode.HeaderIsNull));
                    }
                }
            }
            catch (FaultException fe)
            {
                throw fe;
            }
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            string protocolName = GetProtocolName().ToLower();
            if (protocolName.IndexOf("basichttpbinding") != -1)
            {
                BasicHttpBindingAuthorization();
            }
            else if (protocolName.IndexOf("webhttpbinding") != -1)
            {
                WebHttpBindingAuthorization();
            }
            return this.innerOperationInvoker.Invoke(instance, inputs, out outputs);
        }

        public object[] AllocateInputs()
        {
            return innerOperationInvoker.AllocateInputs();
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            return innerOperationInvoker.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            return innerOperationInvoker.InvokeEnd(instance, out outputs, result);
        }

        public bool IsSynchronous
        {
            get { return innerOperationInvoker.IsSynchronous; }
        }
    }
}