using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Dorado.ESB.Communication
{
    public class CustomOperationSelector : IDispatchOperationSelector
    {
        private Uri endpointListenUri;
        private Dictionary<string, string> operationNameDictionary;

        public CustomOperationSelector(ServiceEndpoint endpoint)
        {
            if (endpoint == null) throw new ArgumentNullException("endpoint");

            endpointListenUri = endpoint.ListenUri;
            operationNameDictionary = new Dictionary<string, string>(endpoint.Contract.Operations.Count);
            foreach (OperationDescription op in endpoint.Contract.Operations)
            {
                try
                {
                    operationNameDictionary.Add(op.Name.ToLower(), op.Name);
                }
                catch (Exception ex)
                {
                    throw new DuplicateOperationException(op.Name, ex);
                }
            }
        }

        #region IDispatchOperationSelector Members

        public string SelectOperation(ref Message message)
        {
            string operation = string.Empty;

            Uri destination = message.Headers.To;
            if (endpointListenUri.IsBaseOf(destination))
            {
                string relativeUri = endpointListenUri.MakeRelativeUri(destination).ToString().ToLower();
                string name;
                if (relativeUri.Contains("/"))
                    name = relativeUri.Substring(0, relativeUri.IndexOf('/'));
                else
                    name = relativeUri;
                operationNameDictionary.TryGetValue(name, out operation);
            }

            return operation;
        }

        #endregion IDispatchOperationSelector Members
    }
}