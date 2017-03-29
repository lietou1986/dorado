using System;
using System.Configuration;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace Dorado.ESB.Json
{
    public class JsonpBinding : Binding
    {
        private HttpTransportBindingElement httpTransportBindingElement;
        private JsonpEncodingBindingElement jsonpEncodingBindingElement;

        internal static MessageVersion messageVersion = MessageVersion.None;

        #region properties

        public bool AllowCookies
        {
            get
            {
                return this.httpTransportBindingElement.AllowCookies;
            }
            set
            {
                this.httpTransportBindingElement.AllowCookies = value;
            }
        }

        public bool BypassProxyOnLocal
        {
            get
            {
                return this.httpTransportBindingElement.BypassProxyOnLocal;
            }
            set
            {
                this.httpTransportBindingElement.BypassProxyOnLocal = value;
            }
        }

        public HostNameComparisonMode HostNameComparisonMode
        {
            get
            {
                return this.httpTransportBindingElement.HostNameComparisonMode;
            }
            set
            {
                this.httpTransportBindingElement.HostNameComparisonMode = value;
            }
        }

        public long MaxBufferPoolSize
        {
            get
            {
                return this.httpTransportBindingElement.MaxBufferPoolSize;
            }
            set
            {
                this.httpTransportBindingElement.MaxBufferPoolSize = value;
            }
        }

        public int MaxBufferSize
        {
            get
            {
                return this.httpTransportBindingElement.MaxBufferSize;
            }
            set
            {
                this.httpTransportBindingElement.MaxBufferSize = value;
            }
        }

        public long MaxReceivedMessageSize
        {
            get
            {
                return this.httpTransportBindingElement.MaxReceivedMessageSize;
            }
            set
            {
                this.httpTransportBindingElement.MaxReceivedMessageSize = value;
            }
        }

        public Uri ProxyAddress
        {
            get
            {
                return this.httpTransportBindingElement.ProxyAddress;
            }
            set
            {
                this.httpTransportBindingElement.ProxyAddress = value;
            }
        }

        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get
            {
                return this.jsonpEncodingBindingElement.ReaderQuotas;
            }
            set
            {
                this.jsonpEncodingBindingElement.ReaderQuotas = value;
            }
        }

        public TransferMode TransferMode
        {
            get
            {
                return this.httpTransportBindingElement.TransferMode;
            }
            set
            {
                this.httpTransportBindingElement.TransferMode = value;
            }
        }

        public bool UseDefaultWebProxy
        {
            get
            {
                return this.httpTransportBindingElement.UseDefaultWebProxy;
            }
            set
            {
                this.httpTransportBindingElement.UseDefaultWebProxy = value;
            }
        }

        #endregion properties

        public JsonpBinding()
            : this(WebHttpSecurityMode.None)
        {
        }

        public JsonpBinding(WebHttpSecurityMode securityMode)
        {
            this.jsonpEncodingBindingElement = new JsonpEncodingBindingElement();
            this.jsonpEncodingBindingElement.MessageVersion = JsonpBinding.messageVersion;

            this.httpTransportBindingElement = new HttpTransportBindingElement();
            this.httpTransportBindingElement.ManualAddressing = true;
            this.httpTransportBindingElement.AuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous;
            this.httpTransportBindingElement.ProxyAuthenticationScheme = System.Net.AuthenticationSchemes.Anonymous;
            this.httpTransportBindingElement.Realm = "";
        }

        public override BindingElementCollection CreateBindingElements()
        {
            BindingElementCollection elements = new BindingElementCollection();
            elements.Add(this.jsonpEncodingBindingElement);
            elements.Add(this.httpTransportBindingElement);
            return elements.Clone();
        }

        public override string Scheme
        {
            get
            {
                return "http";
            }
        }

        public JsonpBinding(string configurationName)
            : this()
        {
            ApplyConfiguration(configurationName);
        }

        protected virtual void ApplyConfiguration(string configurationName)
        {
            JsonpBindingCollectionElement section = JsonpBindingCollectionElement.GetBindingCollectionElement();
            JsonpBindingConfigurationElement element = section.Bindings[configurationName];
            if (element == null)
            {
                throw new ConfigurationErrorsException("Count not found config section: " + configurationName);
            }
            else
            {
                element.ApplyConfiguration(this);
            }
        }
    }

    public class JsonpEncodingBindingElement : MessageEncodingBindingElement
    {
        public XmlDictionaryReaderQuotas ReaderQuotas
        {
            get
            {
                return this.bindingElm.ReaderQuotas;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                value.CopyTo(this.bindingElm.ReaderQuotas);
            }
        }

        public JsonpEncodingBindingElement()
        {
            bindingElm = new WebMessageEncodingBindingElement();
            bindingElm.ReaderQuotas.MaxStringContentLength = 1024 * 1024;
            bindingElm.MessageVersion = JsonpBinding.messageVersion;
        }

        private JsonpEncodingBindingElement(JsonpEncodingBindingElement elm)
        {
            this.bindingElm = (WebMessageEncodingBindingElement)elm.bindingElm.Clone();
            this.msgVersion = elm.msgVersion;
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            context.BindingParameters.Add(this);
            return context.BuildInnerChannelListener<TChannel>();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            context.BindingParameters.Add(this);
            return context.CanBuildInnerChannelFactory<TChannel>();
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            context.BindingParameters.Add(this);
            return context.CanBuildInnerChannelListener<TChannel>();
        }

        public override T GetProperty<T>(BindingContext context)
        {
            return bindingElm.GetProperty<T>(context);
        }

        private WebMessageEncodingBindingElement bindingElm;

        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            return new JsonpEncoderFactory(bindingElm.CreateMessageEncoderFactory());
        }

        private MessageVersion msgVersion;

        public override MessageVersion MessageVersion
        {
            get
            {
                return msgVersion;
            }
            set
            {
                msgVersion = value;
            }
        }

        public override BindingElement Clone()
        {
            return new JsonpEncodingBindingElement(this);
        }
    }

    public class JsonpEncoderFactory : MessageEncoderFactory
    {
        private JsonpEncoder encoder;

        public override MessageEncoder Encoder
        {
            get
            {
                return encoder;
            }
        }

        public override MessageVersion MessageVersion
        {
            get
            {
                return encoder.MessageVersion;
            }
        }

        public JsonpEncoderFactory(MessageEncoderFactory factory)
        {
            this.encoder = new JsonpEncoder(factory.Encoder);
        }
    }

    public class JsonpEncoder : MessageEncoder
    {
        public override bool IsContentTypeSupported(string contentType)
        {
            contentType = contentType.ToLower();
            if (contentType.IndexOf("application/json") == 0)
                return true;
            else if (contentType.IndexOf("text/javascript;") == 0)
                return true;
            else if (contentType.IndexOf("application/x-www-form-urlencoded") == 0)
                return true;
            else if (contentType.IndexOf("text/html") == 0)
                return true;
            else
                return false;
        }

        private MessageEncoder jsonEncoder;

        public JsonpEncoder(MessageEncoder jsonEncoder)
        {
            this.jsonEncoder = jsonEncoder;
        }

        public const string JsonpContentType = "application/json; charset=utf-8";
        public const string JsonpMediaType = "application/json";

        public override string ContentType
        {
            get { return JsonpContentType; }
        }

        public override string MediaType
        {
            get { return JsonpMediaType; }
        }

        public override MessageVersion MessageVersion
        {
            get
            {
                return jsonEncoder.MessageVersion;
            }
        }

        public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
        {
            byte[] data = new byte[buffer.Count];
            if (buffer.Count != 0)
            {
                Array.Copy(buffer.Array, data, buffer.Count);
            }

            JsonMessage message = new JsonMessage(data);
            message.Properties.Encoder = this;
            JsonFormatHelper.AttachBodyFormatProperty(message, true);

            return message;
        }

        public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
        {
            return jsonEncoder.ReadMessage(stream, maxSizeOfHeaders, contentType);
        }

        public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                WriteMessage(message, stream);

                byte[] messageBytes = stream.GetBuffer();
                int messageLength = (int)stream.Position;
                stream.Close();

                int totalLength = messageLength + messageOffset;
                byte[] totalBytes = bufferManager.TakeBuffer(totalLength);
                Array.Copy(messageBytes, 0, totalBytes, messageOffset, messageLength);

                ArraySegment<byte> byteArray = new ArraySegment<byte>(totalBytes, messageOffset, messageLength);
                return byteArray;
            }
        }

        internal const string JsonpCallbackKey = "jsonp_callback";
        private static readonly byte[] postFix = Encoding.ASCII.GetBytes(");");

        public override void WriteMessage(Message message, Stream stream)
        {
            object callback;
            message.Properties.TryGetValue(JsonpCallbackKey, out callback);

            if (callback != null)
            {
                byte[] buf = Encoding.ASCII.GetBytes((string)callback);
                stream.Write(buf, 0, buf.Length);
                stream.WriteByte((byte)'(');
            }

            if (JsonFormatHelper.ShouldUseDefaultEncoder(message))
            {
                jsonEncoder.WriteMessage(message, stream);
            }
            else
            {
                WriteJsonMessage(message, stream);
            }

            if (callback != null)
                stream.Write(postFix, 0, postFix.Length);
        }

        private void WriteJsonMessage(Message message, Stream stream)
        {
            message.Properties.Encoder = this;

            JsonWriter writer = GetJsonWriter(stream);
            writer.WriteStartDocument();
            message.WriteMessage(writer);
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
        }

        private JsonWriter GetJsonWriter(Stream stream)
        {
            JsonWriter writer = new JsonWriter();
            writer.SetOutput(stream, Encoding.UTF8, false);
            return writer;
        }
    }
}