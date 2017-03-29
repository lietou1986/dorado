using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Dorado.ESB.Json
{
    public class JsonClientMessageFormatter : IClientMessageFormatter
    {
        private IClientMessageFormatter innerFormatter;

        private Uri baseUri;
        private string operation;
        private Uri destinationUri;
        private ParameterInfo[] paramInfos;
        private Type returnType;
        private bool isWebGetMethod;

        public JsonClientMessageFormatter(Uri baseUri, OperationDescription description, IClientMessageFormatter innerFormatter)
        {
            if (innerFormatter == null) throw new ArgumentNullException("innerFormatter");

            this.innerFormatter = innerFormatter;
            this.baseUri = baseUri;
            this.destinationUri = GetDestinationUri(baseUri, description);

            operation = description.Name;
            paramInfos = description.SyncMethod.GetParameters();
            returnType = description.SyncMethod.ReturnType;

            isWebGetMethod = JsonFormatHelper.IsWebGetMethod(description);
        }

        #region IClientMessageFormatter Members

        public object DeserializeReply(Message message, object[] parameters)
        {
            string jsonString = JsonMessage.GetJsonString(message);
            if (jsonString == null) return null;

            return JsonObjectSerializer.Instance.Deserialize(jsonString, returnType);
        }

        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            if (isWebGetMethod)
            {
                return innerFormatter.SerializeRequest(messageVersion, parameters);
            }

            Dictionary<string, object> paramDictionary = FormatRequestParameters(parameters);
            Message message = Message.CreateMessage(messageVersion, null, JsonBodyWriter.GetBodyWriter(paramDictionary));
            JsonFormatHelper.AttachBodyFormatProperty(message, true);
            message.Headers.To = destinationUri;
            return message;
        }

        private Dictionary<string, object> FormatRequestParameters(object[] parameters)
        {
            Dictionary<string, object> paramDictionary = new Dictionary<string, object>(parameters.Length);

            for (int i = 0; i < paramInfos.Length; ++i)
            {
                paramDictionary.Add(paramInfos[i].Name, parameters[i]);
            }

            return paramDictionary;
        }

        private Uri GetDestinationUri(Uri baseUri, OperationDescription description)
        {
            string basePath = baseUri.ToString();
            if (!basePath.EndsWith("/"))
            {
                basePath = basePath + "/";
            }

            return new Uri(basePath + JsonFormatHelper.GetUriTemplate(description));
        }

        #endregion IClientMessageFormatter Members
    }
}