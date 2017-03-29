using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace Dorado.ESB.Json
{
    public class JsonParametersFormatter : IDispatchMessageFormatter
    {
        #region fields

        private string operation;
        private Dictionary<string, ParameterInfo> paramInfos;
        private DataContractJsonSerializer dataContractJsonSerializer;
        private string paraName;

        #endregion fields

        #region ctor

        public JsonParametersFormatter(OperationDescription opd)
        {
            operation = opd.Name;
            InitParamInfos(opd);
        }

        private void InitParamInfos(OperationDescription opd)
        {
            var v = opd.SyncMethod.GetParameters();

            paramInfos = new Dictionary<string, ParameterInfo>(v.Length, StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < v.Length; ++i)
            {
                paramInfos.Add(v[i].Name, v[i]);
            }

            if (JsonFormattingConfig.Instance.MethodInIgnoreList(operation) && (v.Length == 1))
            {
                dataContractJsonSerializer = new DataContractJsonSerializer(v[0].ParameterType);
                paraName = v[0].Name;
            }
        }

        #endregion ctor

        #region IDispatchMessageFormatter Members

        public void DeserializeRequest(Message message, object[] parameters)
        {
            if (parameters.Length <= 0) return;
            if (message.IsEmpty) return;

            string jsonString = JsonMessage.GetJsonString(message);
            if (dataContractJsonSerializer != null)
            {
                parameters[0] = GetObject(jsonString);
                return;
            }

            if (JsonFormattingConfig.Instance.MethodInIgnoreList(operation))
            {
                jsonString = JsonFormatHelper.GetStandardJsonString(jsonString);
            }

            using (StringReader reader = new StringReader(jsonString))
            {
                using (JsonTextReader jsonReader = new JsonTextReader(reader))
                {
                    jsonReader.Read();
                    if (jsonReader.TokenType != JsonToken.StartObject)
                        throw new FormatException("Should start with '{', line:" + jsonReader.LineNumber + ", position:" + jsonReader.LinePosition);

                    while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndObject)
                    {
                        if (jsonReader.TokenType != JsonToken.PropertyName)
                            throw new FormatException("Should be propertyName, line:" + jsonReader.LineNumber + ", position:" + jsonReader.LinePosition);

                        string propName = (string)jsonReader.Value;

                        ParameterInfo pi;
                        if (!paramInfos.TryGetValue(propName, out pi))
                            throw new FormatException(string.Format("Parameter '{0}' not found, line: {1}, position: {2}", propName, jsonReader.LineNumber, jsonReader.LinePosition));

                        try
                        {
                            parameters[pi.Position] = JsonObjectSerializer.Instance.Deserialize(jsonReader, pi.ParameterType);
                        }
                        catch (Exception ex)
                        {
                            string msg = string.Format("Parameter '{0}' format exception: {1}, line: {2}, position: {3}",
                                propName, ex.Message, jsonReader.LineNumber, jsonReader.LinePosition);
                            throw new FormatException(msg, ex);
                        }
                    }
                }
            }
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            throw new NotSupportedException();
        }

        #endregion IDispatchMessageFormatter Members

        #region helper

        public object GetObject(string json)
        {
            json = json.Replace("{\"" + paraName + "\":", "");
            json = json.Remove(json.Length - 1);
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return dataContractJsonSerializer.ReadObject(stream);
            }
        }

        #endregion helper
    }
}