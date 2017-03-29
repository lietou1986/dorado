using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Dorado.ESB.Json
{
    public class JsonMessageFormatter : IDispatchMessageFormatter
    {
        #region fields

        private string operationName;
        private Uri baseAddress;
        private UriTemplate uriTemplate;
        private int utVarCount;
        private Dictionary<int, string> pathMapping;
        private Dictionary<int, KeyValuePair<string, Type>> queryMapping;
        private QueryStringConverter queryStringConverter;
        private IDispatchMessageFormatter jsonParametersFormatter;
        private IDispatchMessageFormatter innerFormatter;

        #endregion fields

        #region ctor

        public JsonMessageFormatter(OperationDescription operationDescription, Uri baseAddress, IDispatchMessageFormatter innerFormatter)
        {
            this.operationName = operationDescription.Name;
            this.baseAddress = baseAddress;
            this.uriTemplate = JsonFormatHelper.GetUriTemplate(operationDescription);
            this.queryStringConverter = new QueryStringConverter();

            JsonFormatHelper.Populate(operationDescription, uriTemplate, queryStringConverter,
                out utVarCount, out pathMapping, out queryMapping);

            this.jsonParametersFormatter = new JsonParametersFormatter(operationDescription);
            this.innerFormatter = innerFormatter;
        }

        #endregion ctor

        #region IDispatchMessageFormatter Members

        public void DeserializeRequest(Message message, object[] parameters)
        {
            if (parameters.Length == 0) return;

            object[] objects = new object[parameters.Length - utVarCount];
            if (objects.Length > 0)
            {
                jsonParametersFormatter.DeserializeRequest(message, objects);
            }

            UriTemplateMatch match = JsonFormatHelper.GetUriTemplateMatch(message, uriTemplate, baseAddress);
            NameValueCollection values = (match == null) ? new NameValueCollection() : match.BoundVariables;

            int index = 0;
            for (int i = 0; i < parameters.Length; ++i)
            {
                if (pathMapping.ContainsKey(i) && match != null)
                {
                    parameters[i] = values[pathMapping[i]];
                }
                else if (queryMapping.ContainsKey(i) && match != null)
                {
                    KeyValuePair<string, Type> pair = queryMapping[i];
                    parameters[i] = queryStringConverter.ConvertStringToValue(values[pair.Key], pair.Value);
                }
                else
                {
                    parameters[i] = objects[index];
                    ++index;
                }
            }
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            Message message;

            if (JsonFormattingConfig.Instance.MethodInIgnoreList(operationName))
            {
                message = innerFormatter.SerializeReply(messageVersion, parameters, result);
                JsonFormatHelper.AttachEncoderProperty(message, operationName);
            }
            else
            {
                message = Message.CreateMessage(messageVersion, null, JsonBodyWriter.GetBodyWriter(result));
                if (result == null)
                {
                    JsonFormatHelper.SuppressReplyEntityBody(message);
                }

                JsonFormatHelper.AttachBodyFormatProperty(message, false);
            }

            return message;
        }

        #endregion IDispatchMessageFormatter Members
    }
}