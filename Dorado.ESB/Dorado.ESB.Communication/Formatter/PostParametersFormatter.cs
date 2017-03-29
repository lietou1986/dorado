using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Dorado.ESB.Communication
{
    public abstract class PostParametersFormatter : IDispatchMessageFormatter
    {
        #region fields

        protected string operation;
        protected Dictionary<string, ParameterInfo> paramInfos;

        #endregion fields

        #region ctor

        public PostParametersFormatter(OperationDescription description)
        {
            operation = description.Name;
            InitParamInfos(description);
        }

        private void InitParamInfos(OperationDescription description)
        {
            var paras = description.SyncMethod.GetParameters();
            paramInfos = new Dictionary<string, ParameterInfo>(paras.Length, StringComparer.OrdinalIgnoreCase);
            foreach (var para in paras)
                paramInfos.Add(para.Name, para);
        }

        #endregion ctor

        #region IDispatchMessageFormatter Members

        public void DeserializeRequest(Message message, object[] parameters)
        {
            if (parameters.Length <= 0 || message.IsEmpty) return;
            DoDeserializeRequest(message, parameters);
        }

        protected abstract void DoDeserializeRequest(Message message, object[] parameters);

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            throw new NotImplementedException();
        }

        #endregion IDispatchMessageFormatter Members
    }
}