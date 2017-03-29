using Dorado.Core;
using Dorado.Core.Logger;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Xml;

//using Dorado.Common;

namespace Dorado.ESB.ServiceHost
{
    /// <summary>
    /// 继承ServiceHostFactory。可动态创建主机实例以响应传入消息的托管宿主环境中提供 ServiceHost 的实例的工厂。
    /// </summary>
    public class WebScriptHandlerFactory1 : ServiceHostFactory
    {
        /// <summary>
        /// 缺省需要调入的Assemblies
        /// </summary>
        private static List<string> defaultRefedAssemblies = new List<string>() {
                                    "System",
                                    "System.ServiceModel",
                                    "System.ServiceModel.Web",
                                    "System.Runtime.Serialization",
                                    "System.Web",
                                    "System.Xml",
                                    "Dorado.ESB.ServiceHost",
                               };

        /// <summary>
        /// 包装服务
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        private static Type WrapServiceType(Type serviceType)
        {
            //代码生成器,得到代码流

            //----------------------------------------
            //Modify by jiangsong 2009-11-13
            //string generatedClass = CodeGenerator.Generate(serviceType);
            string generatedClass = "";

            //----------------------------------------

            CSharpCodeProvider csCodeProvider = new CSharpCodeProvider();

            //将名字空间调入referedAssemblies List<string> 。将生成代码作准备
            List<string> referedAssemblies = new List<string>();
            foreach (string assemblyName in defaultRefedAssemblies)
            {
                //根据名字空间取的路径地址
                //----------------------------------------
                //Modify by jiangsong 2009-11-13
                //string assemblyLocation = CodeGenerateHelper.GetAssemblyPath(assemblyName)

                ;
                string assemblyLocation = "";

                //----------------------------------------

                if (!string.IsNullOrEmpty(assemblyLocation))
                {
                    referedAssemblies.Add(assemblyLocation);
                }
            }

            //添加serviceType的Assembly地址
            referedAssemblies.Add(serviceType.Assembly.Location);

            //从serviceType遍历读取所有Assemblie，由CodeGenerateHelper取得路径添加到referedAssemblies List<string>
            foreach (AssemblyName assemblyName in serviceType.Assembly.GetReferencedAssemblies())
            {
                //System.CodeDom.CodeNamespaceImport import = new System.CodeDom.CodeNamespaceImport();
                //System.CodeDom.CodeNamespace nm = new System.CodeDom.CodeNamespace();
                //nm.Imports.Add(import);
                //System.CodeDom.CodeSnippetCompileUnit unit = new System.CodeDom.CodeSnippetCompileUnit();

                //----------------------------------------
                //Modify by jiangsong 2009-11-13
                //string assemblyLocation = CodeGenerateHelper.GetAssemblyPath(assemblyName.Name);
                string assemblyLocation = "";

                //----------------------------------------

                if (!string.IsNullOrEmpty(assemblyLocation))
                {
                    referedAssemblies.Add(assemblyLocation);
                }
            }

            //CompilerParameters表示用于调用编译器的参数,将referedAssemblies List<string> 加入到编码器参数中
            CompilerParameters parameters = new CompilerParameters(referedAssemblies.ToArray());

            //在内存中生成输出。
            parameters.GenerateInMemory = true;

            //指示不生成可执行文件。
            parameters.GenerateExecutable = false;

            //传入编译参数和源代码，取的编译器返回的编译结果。
            CompilerResults compilerResult = csCodeProvider.CompileAssemblyFromSource(parameters, new string[] { generatedClass });

            ///获取编译器错误和警告的集合。
            if (compilerResult.Errors.Count > 0)
            {
                foreach (CompilerError err in compilerResult.Errors)
                {
                    //将详细的错误信息记录到日志，并retrun
                    LoggerWrapper.Logger.Info("Dorado.ESB.ServiceHost:" + err.ToString());
                }
                return null;
            }

            //得到已编译的程序集。
            Assembly assembly = compilerResult.CompiledAssembly;
            Type[] types = assembly.GetTypes();
            if (types.Length > 0)
            {
                return types[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 重写ServiceHost基类中的CreateServiceHost方法
        /// </summary>
        /// <param name="serviceType">指定要承载的服务的类型</param>
        /// <param name="baseAddresses">类型为 System.Uri 且包含所承载服务的基址的 System.Array</param>
        /// <returns>使用特定基址指定的该类型服务的 System.ServiceModel.ServiceHost。</returns>
        protected override System.ServiceModel.ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            //应该是从ServiceHostConfig配置文件实例中读取地址

            //----------------------------------------
            //Modify by jiangsong 2009-11-13
            //无 ServiceHostConfig类
            /*
            if (!ServiceHostConfig.Instance.BaseAddress.EndsWith("/"))
            {
                //不包含创建
                baseAddresses = new Uri[] { new Uri(ServiceHostConfig.Instance.BaseAddress + "/" + serviceType.Name + ".svc") };
            }
            else
            {
                baseAddresses = new Uri[] { new Uri(ServiceHostConfig.Instance.BaseAddress + serviceType.Name + ".svc") };
            }
             */
            baseAddresses = null;

            //----------------------------------------

            //改变type类型了，无代码看不出,应该是转成可以适应WebServiceHost的类型
            serviceType = WrapServiceType(serviceType);
            if (serviceType == null)
                return null;

            ///WebServiceHost是一个 ServiceHost 派生类，它是对 Windows Communication Foundation (WCF) Web 编程模型的补充。
            WebServiceHost host = new WebServiceHost(serviceType, baseAddresses);

            //添加服务的自定义行为
            host.Description.Behaviors.Add(new ServiceErrorBehavior1());

            // host.Description.Behaviors.Add(new WebFilterServiceBehavior());
            foreach (Uri uri in baseAddresses)
            {
                // add json endpoint
                Uri listenUri = new Uri(uri.ToString() + "/json");

                //在host中添加自定义json binding

                //----------------------------------------
                //Modify by jiangsong 2009-11-13
                //ServiceEndpoint ep = host.AddServiceEndpoint(serviceType, new JsonpBinding(), string.Empty, listenUri);
                ServiceEndpoint ep = null;

                //----------------------------------------

                //添加WebHttpBehavior行为
                ep.Behaviors.Add(new WebHttpBehavior());

                //添加JsonpEndpointBehavior扩展行为
                ep.Behaviors.Add(new JsonpEndpointBehavior());

                //添加OperationSelectEndpointBehavior扩展行为
                ep.Behaviors.Add(new OperationSelectEndpointBehavior());

                // add xml endpoint
                //这里的endpoing支持2种一种为json,一种为xml
                listenUri = new Uri(uri.ToString() + "/xml");
                ep = host.AddServiceEndpoint(serviceType, new WebHttpBinding(), string.Empty, listenUri);
                ep.Behaviors.Add(new WebHttpBehavior());
                ep.Behaviors.Add(new OperationSelectEndpointBehavior());
            }
            return host;
        }
    }

    public class OperationSelectEndpointBehavior : IEndpointBehavior
    {
        #region IEndpointBehavior Members

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            if (endpoint == null)
            {
                throw new ArgumentNullException("endpoint");
            }
            if (endpointDispatcher == null)
            {
                throw new ArgumentNullException("endpointDispatcher");
            }

            Dictionary<string, string> operationNameDictionary = new Dictionary<string, string>();
            foreach (OperationDescription operation in endpoint.Contract.Operations)
            {
                try
                {
                    operationNameDictionary.Add(operation.Name.ToLower(), operation.Name);
                }
                catch (ArgumentException)
                {
                    throw new Exception(String.Format("More than one operation named {0}", operation.Name));
                }
            }

            //*
            Type contractType = endpoint.Contract.ContractType;
            endpointDispatcher.DispatchRuntime.OperationSelector = new UriOperationSelector(
                    endpoint.ListenUri.ToString(), operationNameDictionary, contractType);
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        #endregion IEndpointBehavior Members
    }

    //定义协定，该协定将传入消息与本地操作相关联，来自定义服务执行行为。
    public class UriOperationSelector : IDispatchOperationSelector
    {
        private Uri endpointListenUri;
        private string endpointType;
        private Dictionary<string, string> operationNameDictionary;
        private Type contractType;

        //操作类型选择器。根据endpoint的后缀，确定endpointType
        public UriOperationSelector(string endpointListenUri,
                    Dictionary<string, string> operationNameDictionary, Type contractType)
        {
            if (endpointListenUri == null)
            {
                throw new ArgumentNullException("endpointListenUri");
            }
            if (operationNameDictionary == null)
            {
                throw new ArgumentNullException("operationNameDictionary");
            }
            if (contractType == null)
            {
                throw new ArgumentNullException("contractType");
            }

            endpointListenUri = endpointListenUri.ToLower();
            if (!endpointListenUri.EndsWith("/"))
            {
                endpointListenUri += "/";
            }
            this.endpointListenUri = new Uri(endpointListenUri);
            if (endpointListenUri.Contains("/xml/"))
            {
                this.endpointType = "xml";
            }
            else if (endpointListenUri.Contains("/json/"))
            {
                this.endpointType = "json";
            }
            else
            {
                throw new Exception(string.Format("Url must contain xml or json: {0}", endpointListenUri));
            }

            this.operationNameDictionary = operationNameDictionary;
            this.contractType = contractType;
        }

        #region IDispatchOperationSelector Members

        /// <summary>
        /// 根据Message中的调用方法
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public string SelectOperation(ref Message message)
        {
            string operationName = null;
            Uri via = null;
            if (message.Properties.ContainsKey("Via"))
            {
                via = message.Properties["Via"] as Uri;
            }

            if (via != null && !string.IsNullOrEmpty(via.ToString()))
            {
                Uri operationUri = new Uri(via.GetLeftPart(UriPartial.Path).ToString().ToLower());
                if (endpointListenUri.IsBaseOf(operationUri))
                {
                    string relativeUri = endpointListenUri.MakeRelativeUri(operationUri).ToString();
                    string name = null;
                    if (relativeUri.Contains("/"))
                    {
                        name = relativeUri.Substring(0, relativeUri.IndexOf('/'));
                    }
                    else
                    {
                        name = relativeUri;
                    }

                    if (!string.IsNullOrEmpty(name))
                    {
                        HttpRequestMessageProperty prop = JsonpDispatchMessageInspector.GetHttpRequestMessageProperty(message);
                        if (prop.Method.ToUpper() == "POST")
                        {
                            string contentType = GetRequestHeader(message, "Content-Type");
                            if (!string.IsNullOrEmpty(contentType) && contentType.ToLower().IndexOf("application/x-www-form-urlencoded") == 0)
                            {
                                name = string.Format("Form_Post_{0}", name).ToLower();
                            }
                            else
                            {
                                name = string.Format("{0}_{1}_{2}", this.endpointType, prop.Method, name).ToLower();
                            }
                        }
                        else
                        {
                            name = string.Format("{0}_{1}_{2}", this.endpointType, prop.Method, name).ToLower();
                        }

                        operationNameDictionary.TryGetValue(name, out operationName);
                    }
                }
            }

            if (!string.IsNullOrEmpty(operationName))
            {
                CheckRefer(message, operationName);
                return operationName;
            }
            else
            {
                string errString = String.Format("Missing or unknown operation name in URL {0}", via);
                string httpHeader = GetHttpHeaderString(message);

                //Log.Write(Log.Status.Warn, errString + "\r\n" + httpHeader);
                throw new Exception(errString);
            }
        }

        #endregion IDispatchOperationSelector Members

        //根据message中的信息了得发送post方法名，类型、相关串
        private string GetHttpHeaderString(Message message)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Http Request Header:");

            HttpRequestMessageProperty prop = JsonpDispatchMessageInspector.GetHttpRequestMessageProperty(message);
            if (prop != null && prop.Headers != null)
            {
                sb.AppendLine("Method: " + prop.Method);
                sb.AppendLine("Content Type: " + GetRequestHeader(message, "Content-Type"));
                sb.AppendLine("Query String: " + prop.QueryString);
                sb.AppendLine(prop.Headers.ToString());
            }
            else
            {
                sb.AppendLine("null");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 从message中取得指定headername的value
        /// </summary>
        /// <param name="message"></param>
        /// <param name="headerName"></param>
        /// <returns></returns>
        private string GetRequestHeader(Message message, string headerName)
        {
            try
            {
                HttpContext context = HttpContext.Current;
                if (context != null)
                {
                    return context.Request.Headers[headerName];
                }
                else
                {
                    HttpRequestMessageProperty prop = JsonpDispatchMessageInspector.GetHttpRequestMessageProperty(message);
                    return prop.Headers[headerName];
                }
            }
            catch (Exception ex)
            {
                LoggerWrapper.Logger.Error("Dorado.ESB.ServiceHost", ex);
                return null;
            }
        }

        private void CheckRefer(Message request, string requestedOpeartion)
        {
            //----------------------------------------
            //Modify by jiangsong 2009-11-13
            //*
            //bool requireCheck = true;
            //if ( !GetMethodCheckReferSetting(requestedOpeartion, ref requireCheck) )
            //{
            //    requireCheck = ReferFilter.Instance.Enabled;
            //}

            //if (requireCheck)
            //{
            //    string refer = GetRequestHeader(request, "Referer");
            //    if ( !ReferFilter.Instance.Contains(refer) )
            //        throw new UntrustReferException(refer);
            //}

            //----------------------------------------
        }

        private bool GetClassCheckReferSetting(ref bool requireCheck)
        {
            //*
            bool success = false;

            //----------------------------------------
            //Modify by jiangsong 2009-11-13
            //object[] attrObjects = contractType.GetCustomAttributes(typeof(SOAOperationAttribute), true);
            //if (attrObjects.Length > 0)
            //{
            //    SOAOperationAttribute attribute = attrObjects[0] as SOAOperationAttribute;
            //    if (attribute != null)
            //    {
            //        requireCheck = attribute.CheckRefer;
            //        success = true;
            //    }
            //}
            success = true;

            //----------------------------------------

            return success;
        }

        private bool GetMethodCheckReferSetting(string method, ref bool requireCheck)
        {
            //*
            bool success = false;

            //----------------------------------------
            //Modify by jiangsong 2009-11-13
            //MemberInfo[] memberInfo = contractType.GetMember(method);
            //if (memberInfo.Length > 0)
            //{
            //    object[] attrObjects = memberInfo[0].GetCustomAttributes(typeof(SOAOperationAttribute), true);
            //    if (attrObjects.Length > 0)
            //    {
            //        SOAOperationAttribute attribute = attrObjects[0] as SOAOperationAttribute;
            //        if (attribute != null)
            //        {
            //            requireCheck = attribute.CheckRefer;
            //            success = true;
            //        }
            //    }
            //}
            success = true;

            //----------------------------------------
            return success;
        }
    }

    /// <summary>
    /// WebFiltert自定义行为
    /// </summary>
    public class WebFilterServiceBehavior : IServiceBehavior
    {
        #region IServiceBehavior Members

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher chDisp in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher epDisp in chDisp.Endpoints)
                {
                    epDisp.DispatchRuntime.MessageInspectors.Add(new WebFilterDispatchMessageInspector());
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        #endregion IServiceBehavior Members
    }

    /// <summary>
    /// WebFilter截获器
    /// </summary>
    public class WebFilterDispatchMessageInspector : IDispatchMessageInspector
    {
        #region IDispatchMessageInspector Members

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            HttpRequestMessageProperty prop = JsonpDispatchMessageInspector.GetHttpRequestMessageProperty(request);
            string refer = prop.Headers[HttpRequestHeader.Referer];

            //----------------------------------------
            //Modify by jiangsong 2009-11-13
            //if (ReferFilter.Instance.Enabled && !ReferFilter.Instance.Contains(refer))
            //    throw new UntrustReferException(refer);

            //----------------------------------------

            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
        }

        #endregion IDispatchMessageInspector Members
    }

    /// <summary>
    /// IEndpoint截获器
    /// </summary>
    public class JsonpEndpointBehavior : IEndpointBehavior
    {
        //*
        public JsonpEndpointBehavior()
        {
        }

        #region IEndpointBehavior Members

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new JsonpDispatchMessageInspector());
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        #endregion IEndpointBehavior Members
    }

    public class JsonpDispatchMessageInspector : IDispatchMessageInspector
    {
        public JsonpDispatchMessageInspector()
        {
        }

        //*

        internal static HttpRequestMessageProperty GetHttpRequestMessageProperty(Message msg)
        {
            object obj;
            if (msg.Properties.TryGetValue(HttpRequestMessageProperty.Name, out obj))
                return (HttpRequestMessageProperty)obj;
            else
                return null;
        }

        internal static HttpResponseMessageProperty GetHttpResponseMessageProperty(Message msg)
        {
            object obj;
            if (msg.Properties.TryGetValue(HttpResponseMessageProperty.Name, out obj))
                return (HttpResponseMessageProperty)obj;
            else
                return null;
        }

        #region IDispatchMessageInspector Members

        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request,
            IClientChannel channel,
            InstanceContext instanceContext)
        {
            System.Collections.Specialized.NameValueCollection nvc = HttpUtility.ParseQueryString(request.Properties.Via.Query);
            return nvc;
        }

        private static bool IsValidCallback(string callback)
        {
            if (string.IsNullOrEmpty(callback)) return false;
            callback = callback.ToLower();
            foreach (char c in callback)
            {
                if (c == '_' || c == '$' || c == '.') continue;
                else if (c >= 'a' && c <= 'z') continue;
                else if (c >= '0' && c <= '9') continue;
                else return false;
            }

            return true;
        }

        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
            System.Collections.Specialized.NameValueCollection nvc = (System.Collections.Specialized.NameValueCollection)correlationState;
            if (nvc != null)
            {
                string callback = nvc["callback"];
                if (IsValidCallback(callback))
                {
                    //----------------------------------------
                    //Modify by jiangsong 2009-11-13

                    //reply.Properties[JsonpEncoder.JsonpCallbackKey] = callback;

                    //----------------------------------------

                    HttpResponseMessageProperty prop = GetHttpResponseMessageProperty(reply);
                    if (prop != null)
                    {
                        //----------------------------------------
                        //Modify by jiangsong 2009-11-13
                        //prop.Headers[HttpResponseHeader.ContentType] = JsonpEncoder.JsonpContentType;
                        //----------------------------------------
                    }
                }
            }
        }

        #endregion IDispatchMessageInspector Members
    }

    public class FormPostMessage : Message
    {
        public override MessageHeaders Headers
        {
            get { throw new NotImplementedException(); }
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            throw new NotImplementedException();
        }

        public override MessageProperties Properties
        {
            get { throw new NotImplementedException(); }
        }

        public override MessageVersion Version
        {
            get { throw new NotImplementedException(); }
        }
    }
}