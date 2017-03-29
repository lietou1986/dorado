using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Dorado.ESB.Communication
{
    public class UriParametersFormatter
    {
        #region fields

        private string operation;
        private Uri baseAddress;
        private UriTemplate template;
        private Dictionary<int, string> pathMapping;
        private Dictionary<int, KeyValuePair<string, Type>> queryMapping;
        private QueryStringConverter queryStringConverter;

        #endregion fields

        public UriParametersFormatter(OperationDescription description, Uri baseAddress, UriTemplate template)
        {
            this.operation = description.Name;
            this.baseAddress = baseAddress;
            this.template = template;
            this.queryStringConverter = new QueryStringConverter();

            MessageFormatHelper.Populate(description, template, queryStringConverter, out pathMapping, out queryMapping);
        }

        public List<int> DeserializeRequest(Message message, object[] parameters)
        {
            List<int> notfound = new List<int>();

            UriTemplateMatch match = UriTemplateHelper.GetUriTemplateMatch(message, template, baseAddress);
            NameValueCollection values = (match == null) ? new NameValueCollection() : match.BoundVariables;
            for (int i = 0; i < parameters.Length; ++i)
            {
                if (pathMapping.ContainsKey(i) && match != null)
                    parameters[i] = values[pathMapping[i]];
                else if (queryMapping.ContainsKey(i) && match != null)
                {
                    KeyValuePair<string, Type> pair = queryMapping[i];
                    parameters[i] = queryStringConverter.ConvertStringToValue(values[pair.Key], pair.Value);
                }
                else
                    notfound.Add(i);
            }

            return notfound;
        }
    }
}