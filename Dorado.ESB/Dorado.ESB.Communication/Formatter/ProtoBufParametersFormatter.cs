using ProtoBuf;
using System;
using System.IO;
using System.Reflection;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Dorado.ESB.Communication
{
    public class ProtoBufParametersFormatter : PostParametersFormatter
    {
        public ProtoBufParametersFormatter(OperationDescription operation)
            : base(operation)
        {
        }

        protected override void DoDeserializeRequest(Message message, object[] parameters)
        {
            using (Stream stream = GetMessageStream(message))
            {
                object name, value;
                while (Serializer.NonGeneric.TryDeserializeWithLengthPrefix(stream, PrefixStyle.Base128,
                    delegate(int tag) { return typeof(string); }, out name))
                {
                    ParameterInfo pi;
                    if (!paramInfos.TryGetValue((string)name, out pi))
                        throw new FormatException("Unexpected parameter '" + name + "'");
                    try
                    {
                        if (Serializer.NonGeneric.TryDeserializeWithLengthPrefix(stream, PrefixStyle.Base128,
                            delegate(int tag) { return pi.ParameterType; }, out value))
                        {
                            parameters[pi.Position] = value;
                        }
                    }
                    catch (Exception ex)
                    {
                        string error = string.Format("Parameter '{0}' format exception: {1}", name, ex.Message);
                        throw new FormatException(error, ex);
                    }
                }
            }
        }

        private Stream GetMessageStream(Message message)
        {
            InMessage inMessage = message as InMessage;
            if (inMessage == null || inMessage.IsEmpty)
                return new MemoryStream();
            else
                return new MemoryStream(inMessage.Data);
        }
    }
}