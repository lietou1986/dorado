using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;

namespace Dorado.ESB.Communication
{
    public static class MessageFormatHelper
    {
        public static bool IsWebGetMethod(OperationDescription operation)
        {
            WebGetAttribute get = operation.Behaviors.Find<WebGetAttribute>();
            if (get != null) return true;
            return false;
        }

        public static void SuppressReplyEntityBody(Message message)
        {
            if (WebOperationContext.Current != null)
                WebOperationContext.Current.OutgoingResponse.SuppressEntityBody = true;
            else
            {
                object obj = null;
                message.Properties.TryGetValue(HttpResponseMessageProperty.Name, out obj);
                HttpResponseMessageProperty property = obj as HttpResponseMessageProperty;
                if (property == null)
                {
                    property = new HttpResponseMessageProperty();
                    message.Properties[HttpResponseMessageProperty.Name] = property;
                }
                property.SuppressEntityBody = true;
            }
        }

        public static void Populate(OperationDescription operation, UriTemplate template, QueryStringConverter converter,
            out Dictionary<int, string> pathMapping, out Dictionary<int, KeyValuePair<string, Type>> queryMapping)
        {
            int templateVarCount = template.PathSegmentVariableNames.Count + template.QueryValueVariableNames.Count;
            pathMapping = new Dictionary<int, string>(template.PathSegmentVariableNames.Count);
            queryMapping = new Dictionary<int, KeyValuePair<string, Type>>(template.QueryValueVariableNames.Count);

            List<string> pathVarNames = new List<string>(template.PathSegmentVariableNames);
            List<string> queryVarNames = new List<string>(template.QueryValueVariableNames);
            Dictionary<string, byte> alreadyFoundNames = new Dictionary<string, byte>(templateVarCount, StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < operation.Messages[0].Body.Parts.Count; ++i)
            {
                MessagePartDescription part = operation.Messages[0].Body.Parts[i];
                string name = part.Name;
                if (alreadyFoundNames.ContainsKey(name))
                    throw new InvalidOperationException("Duplicate Uri tempalte var name found: " + name);

                int index = pathVarNames.FindIndex(str => str.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    if (part.Type != typeof(string))
                        throw new InvalidOperationException("Uri template path var must be a string");
                    pathMapping.Add(i, name);
                    alreadyFoundNames.Add(name, 0);
                    pathVarNames.RemoveAt(index);
                    continue;
                }

                index = queryVarNames.FindIndex(str => str.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (index >= 0)
                {
                    if (!converter.CanConvert(part.Type))
                        throw new InvalidOperationException("Unexpected type " + part.Type + ", Uri template query var must be convertible");
                    queryMapping.Add(i, new KeyValuePair<string, Type>(name, part.Type));
                    alreadyFoundNames.Add(name, 0);
                    queryVarNames.RemoveAt(index);
                }
            }

            if (pathVarNames.Count != 0 || queryVarNames.Count != 0)
                throw new InvalidOperationException("Uri template missing var");
        }
    }
}