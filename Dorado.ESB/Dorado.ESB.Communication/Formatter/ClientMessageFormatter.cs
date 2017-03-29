using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Dorado.ESB.Communication
{
    public class ClientMessageFormatter : IClientMessageFormatter
    {
        #region fields

        private string operation;
        private Uri destination;
        private ParameterInfo[] paramInfos;
        private Type returnType;
        private bool isGetMethod;

        private IObjectSerializer serializer;
        private IClientMessageFormatter innerFormatter;

        #endregion fields

        #region ctor

        public ClientMessageFormatter(OperationDescription description, Uri baseAddress,
            IObjectSerializer serializer, IClientMessageFormatter originFormatter)
        {
            if (serializer == null) throw new ArgumentNullException("serializer");

            //if (originFormatter == null) throw new ArgumentNullException("originFormatter");

            this.serializer = serializer;
            this.innerFormatter = originFormatter;

            this.operation = description.Name;
            this.destination = GetDestination(baseAddress, description);
            this.paramInfos = description.SyncMethod.GetParameters();
            this.returnType = description.SyncMethod.ReturnType;
            this.isGetMethod = MessageFormatHelper.IsWebGetMethod(description);
        }

        #endregion ctor

        #region IClientMessageFormatter Members

        public object DeserializeReply(Message message, object[] parameters)
        {
            InMessage inMessage = message as InMessage;
            if (inMessage != null && !inMessage.IsEmpty)
                return serializer.Deserialize(inMessage.Data, returnType);
            return null;
        }

        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            if (isGetMethod)
                return innerFormatter.SerializeRequest(messageVersion, parameters);

            Dictionary<string, object> paras = BuildRequestParameters(parameters);
            Message message = new OutMessage(messageVersion, paras, this.serializer);
            message.Headers.To = destination;
            return message;
        }

        #endregion IClientMessageFormatter Members

        #region helper

        private Dictionary<string, object> BuildRequestParameters(object[] parameters)
        {
            var result = new Dictionary<string, object>(parameters.Length);
            for (int i = 0; i < paramInfos.Length; ++i)
                result.Add(paramInfos[i].Name, parameters[i]);
            return result;
        }

        private Uri GetDestination(Uri baseAddress, OperationDescription description)
        {
            return new Uri(baseAddress, UriTemplateHelper.GetUriTemplate(description).ToString());
        }

        #endregion helper
    }
}