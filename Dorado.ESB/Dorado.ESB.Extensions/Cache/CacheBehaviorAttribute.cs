using System;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Dorado.ESB.Extensions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheManager : Attribute, IOperationBehavior
    {
        #region IOperationBehavior 成员

        public static readonly int DefaultExpirationMinute = 5;

        private int expirationMinute;

        public int ExpirationMinute
        {
            get { return expirationMinute; }
            set { expirationMinute = value; }
        }

        public CacheManager()
            : this(DefaultExpirationMinute)
        {
        }

        public CacheManager(int expirationMinute)
        {
            this.expirationMinute = expirationMinute;
        }

        public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.Invoker = new CacheProvider(dispatchOperation.Invoker, dispatchOperation, this.expirationMinute);
        }

        public void Validate(OperationDescription operationDescription)
        {
        }

        #endregion IOperationBehavior 成员
    }
}