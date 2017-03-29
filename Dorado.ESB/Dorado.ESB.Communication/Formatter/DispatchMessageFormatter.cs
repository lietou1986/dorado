using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Dorado.ESB.Communication
{
    public class DispatchMessageFormatter : IDispatchMessageFormatter
    {
        #region fields

        private string operation;
        private int templateVarCount;

        private IObjectSerializer serializer;

        private UriParametersFormatter uriFormatter;
        private PostParametersFormatter postFormatter;

        #endregion fields

        #region ctor

        public DispatchMessageFormatter(OperationDescription description, Uri baseAddress,
            IObjectSerializer serializer, PostParametersFormatter postFormatter)
        {
            if (serializer == null) throw new ArgumentNullException("serializer");
            if (postFormatter == null) throw new ArgumentNullException("postFormatter");

            this.operation = description.Name;

            UriTemplate template = UriTemplateHelper.GetUriTemplate(description);
            this.templateVarCount = template.PathSegmentVariableNames.Count + template.QueryValueVariableNames.Count;

            this.uriFormatter = new UriParametersFormatter(description, baseAddress, template);
            this.postFormatter = postFormatter;

            this.serializer = serializer;
        }

        #endregion ctor

        #region IDispatchMessageFormatter Members

        public void DeserializeRequest(Message message, object[] parameters)
        {
            if (parameters.Length == 0) return;

            List<int> notfound = uriFormatter.DeserializeRequest(message, parameters);
            object[] objects = new object[parameters.Length - templateVarCount];
            if (objects.Length > 0)
            {
                postFormatter.DeserializeRequest(message, objects);
                int index = 0;
                foreach (int i in notfound)
                {
                    parameters[i] = objects[index];
                    ++index;
                }
            }
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            Message message = new OutMessage(messageVersion, result, serializer);
            if (result == null)
                MessageFormatHelper.SuppressReplyEntityBody(message);
            return message;
        }

        #endregion IDispatchMessageFormatter Members
    }
}