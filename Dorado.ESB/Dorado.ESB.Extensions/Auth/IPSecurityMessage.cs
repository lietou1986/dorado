using System;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;

namespace Dorado.Services.Behaviors
{
    public class IPSecurityMessage : Message
    {
        #region Properties

        private HttpStatusCode statusCode;

        public HttpStatusCode StatusCode { get { return statusCode; } set { statusCode = value; } }

        private string errorMessage;

        public string ErrorMessage { get { return errorMessage; } set { errorMessage = value; } }

        private MessageHeaders headers;
        private MessageProperties properties;
        private MessageVersion version;

        #endregion Properties

        #region CTors

        public IPSecurityMessage(IPSecurityException restException)
            : this(restException.StatusCode, restException.StatusDescription, restException.StatusDescription) { }

        public IPSecurityMessage(HttpStatusCode statusCode, string errorMessage, string statusDescription)
        {
            this.StatusCode = statusCode;
            this.ErrorMessage = errorMessage;
            version = MessageVersion.None;
            headers = new MessageHeaders(version);
            properties = new MessageProperties();
            if (WebOperationContext.Current != null)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = statusCode;
                if (!string.IsNullOrEmpty(statusDescription))
                    WebOperationContext.Current.OutgoingResponse.StatusDescription = statusDescription;
                WebOperationContext.Current.OutgoingResponse.SuppressEntityBody = false;
            }
            else
            {
                HttpResponseMessageProperty prop = new HttpResponseMessageProperty();
                prop.StatusCode = statusCode;
                this.Properties[HttpResponseMessageProperty.Name] = prop;
            }
        }

        #endregion CTors

        #region Virtual Overrides

        public override MessageHeaders Headers
        {
            get { return headers; }
        }

        protected override void OnWriteBodyContents(System.Xml.XmlDictionaryWriter writer)
        {
            writer.WriteStartElement("error");
            writer.WriteAttributeString("xmlns", "Dorado.PlatformService.WebHttp");
            writer.WriteElementString("statuscode", string.Format("{0}", (int)statusCode));
            writer.WriteElementString("message", errorMessage);
            writer.WriteEndElement();
        }

        public override MessageProperties Properties
        {
            get { return properties; }
        }

        public override MessageVersion Version
        {
            get { return version; }
        }

        #endregion Virtual Overrides
    }

    public class IPSecurityException : Exception
    {
        private bool suppressEntityBody = true;

        public HttpStatusCode StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public IPSecurityException(Exception ex)
            : this(ex, HttpStatusCode.InternalServerError)
        {
        }

        public IPSecurityException(Exception ex, HttpStatusCode httpStatusCode)
            : this(ex, httpStatusCode, null)
        {
        }

        public IPSecurityException(Exception ex, HttpStatusCode httpStatusCode, string statusDescription)
            : base(ex.Message, ex)
        {
            this.StatusCode = httpStatusCode;
            this.StatusDescription = statusDescription;
        }

        public void SetResponse(bool suppressEntityBody)
        {
            this.suppressEntityBody = suppressEntityBody;
            SetResponse();
        }

        public virtual void SetResponse()
        {
            WebOperationContext.Current.OutgoingResponse.StatusCode = this.StatusCode;
            WebOperationContext.Current.OutgoingResponse.StatusDescription = this.StatusDescription;
            WebOperationContext.Current.OutgoingResponse.SuppressEntityBody = suppressEntityBody;
        }
    }

    public class PermissionException : IPSecurityException
    {
        public PermissionException(Exception ex) :
            base(ex, HttpStatusCode.Unauthorized, string.Format("Not Allow! {0}", ex.Message))
        {
        }

        public PermissionException(string statusDescription) :
            base(new Exception(statusDescription), HttpStatusCode.Unauthorized, statusDescription)
        {
        }

        public override void SetResponse()
        {
            base.SetResponse();
            WebOperationContext.Current.OutgoingResponse.Headers[HttpResponseHeader.WwwAuthenticate] = "Please check your request";
        }
    }
}