using System;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Dorado.ESB.Json
{
    [DataContract]
    public class JsonError
    {
        [DataMember]
        public string __Error; // error message

        public JsonError()
        {
        }

        public JsonError(string error)
        {
            __Error = error;
        }
    }

    public class JsonErrorHandler : IErrorHandler
    {
        #region IErrorHandler Members

        public bool HandleError(Exception error)
        {
            return false;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            JsonError jsonError = new JsonError(error.Message);
            fault = Message.CreateMessage(version, null, JsonBodyWriter.GetBodyWriter(jsonError));
        }

        #endregion IErrorHandler Members
    }
}