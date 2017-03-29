using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;

namespace Dorado.ESB.Communication
{
    public class JsonParametersFormatter : PostParametersFormatter
    {
        public JsonParametersFormatter(OperationDescription operation)
            : base(operation)
        {
        }

        protected override void DoDeserializeRequest(Message message, object[] parameters)
        {
            string json = GetJsonString(message);
            using (StringReader sr = new StringReader(json))
            using (JsonTextReader reader = new JsonTextReader(sr))
            {
                reader.Read();
                if (reader.TokenType != JsonToken.StartObject)
                    throw new FormatException("Json should start with '{', line: " + reader.LineNumber + ", position: " + reader.LinePosition);

                while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                {
                    if (reader.TokenType != JsonToken.PropertyName)
                        throw new FormatException("Property name expected, line: " + reader.LineNumber + ", position: " + reader.LinePosition);

                    string prop = (string)reader.Value;
                    ParameterInfo pi;
                    if (!paramInfos.TryGetValue(prop, out pi))
                        throw new FormatException("Unexpected parameter '" + prop + "', line: " + reader.LineNumber + ", position: " + reader.LinePosition);

                    try
                    {
                        parameters[pi.Position] = JsonObjectSerializer.Instance.Deserialize(reader, pi.ParameterType);
                    }
                    catch (Exception ex)
                    {
                        string error = string.Format("Parameter '{0}' format exception: {1}, line: {2}, position: {3}",
                            prop, ex.Message, reader.LineNumber, reader.LinePosition);
                        throw new FormatException(error, ex);
                    }
                }
            }
        }

        private string GetJsonString(Message message)
        {
            InMessage inMessage = message as InMessage;
            if (inMessage == null)
                return null;
            else
                return Encoding.UTF8.GetString(inMessage.Data);
        }
    }
}