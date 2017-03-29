using Dorado.Core;
using Dorado.Core.Logger;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web;

namespace Dorado.ESB.ClientProxyFactory.Behaviors.EndpointBehaviors
{
    #region EpInfoMessageInspector

    public class EpInfoCheckerInspector : IClientMessageInspector
    {
        #region IClientMessageInspector Members

        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
        {
            string BSSaaS_Cookie = string.Empty;
            #region
            try
            {
                HttpContext context = HttpContext.Current;
                if (context != null)
                {
                    HttpCookie authCookie = context.Request.Cookies["BS_auth"];
                    if (authCookie != null)
                        BSSaaS_Cookie = authCookie.Value;
                }
                MessageHeader messageHeader = MessageHeader.CreateHeader("BS_auth", "http://Dorado", BSSaaS_Cookie);
                request.Headers.Add(messageHeader);
            }
            catch
            {
                LoggerWrapper.Logger.Info("Not Found Tenant Information");
            }

            #endregion IClientMessageInspector Members

            return null;
        }

        #endregion EpInfoMessageInspector
    }

    #endregion

    #region DoradoClientMessageInspector

    public class DoradoClientMessageInspector : IClientMessageInspector
    {
        public const string Header_CLIENT_IP = "CLIENT-IP";
        public const string Header_CLIENT_MachineName = "CLIENT-MachineName";
        #region IClientMessageInspector Members

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        public string GetIP() //获取IP
        {
            string HostName = Dns.GetHostName(); //得到主机名
            IPHostEntry IpEntry = Dns.GetHostEntry(HostName); //得到主机IP
            string strIPAddr = IpEntry.AddressList[0].ToString();
            return (strIPAddr);
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            if (request != null)
            {
                MessageHeaders headers = request.Headers;
                if (headers != null)
                {
                    if (headers.From == null)
                    {
                        if (headers.MessageVersion.Addressing == AddressingVersion.None)//AddressingVersion.None can't not SetFrom
                        {
                            if (headers.MessageVersion.Envelope != EnvelopeVersion.None)
                            {
                                MessageHeader mh = MessageHeader.CreateHeader(Header_CLIENT_IP, "http://Dorado", GetIP());
                                headers.Add(mh);

                                mh = MessageHeader.CreateHeader(Header_CLIENT_MachineName, "http://Dorado", System.Environment.MachineName);
                                headers.Add(mh);
                            }
                        }
                        else
                            headers.From = new EndpointAddress("http://" + System.Environment.MachineName);
                    }
                }
            }
            return null;
        }

        #endregion
    }

    #endregion

    public class DoradoClientEndpointBehavior : IEndpointBehavior
    {
        #region IEndpointBehavior Members

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new DoradoClientMessageInspector());
            clientRuntime.MessageInspectors.Add(new EpInfoCheckerInspector());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        #endregion

        #region 配置文件使用

        //public override Type BehaviorType
        //{
        //    get { return typeof(DoradoClientEndpointBehavior); }
        //}
        //protected override object CreateBehavior()
        //{
        //    return new DoradoClientEndpointBehavior();
        //}
        #endregion
    }
}