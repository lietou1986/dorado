using Dorado.Core;
using Dorado.Core.Logger;
using System;
using System.ServiceModel.Dispatcher;

namespace Dorado.ESB.Extensions
{
    public class CacheProvider : IOperationInvoker
    {
        private IOperationInvoker innerOperationInvoker;
        private DispatchOperation dispatchOperation;
        private TimeSpan expirationTime;

        public CacheProvider(IOperationInvoker innerOperationInvoker, DispatchOperation dispatchOperation, int expirationTime)
        {
            this.expirationTime = new TimeSpan(0, expirationTime, 0);
            this.dispatchOperation = dispatchOperation;
            this.innerOperationInvoker = innerOperationInvoker;
        }

        #region IOperationInvoker Members

        public object[] AllocateInputs()
        {
            return this.innerOperationInvoker.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            LoggerWrapper.Logger.Info("+++++++++" + this.expirationTime.ToString() + "+++++++++");

            object realReturn = this.innerOperationInvoker.Invoke(instance, inputs, out outputs);
            return realReturn;
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs,
            AsyncCallback callback, object state)
        {
            return this.innerOperationInvoker.InvokeBegin(
                instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            return this.innerOperationInvoker.InvokeEnd(
                instance, out outputs, result);
        }

        public bool IsSynchronous
        {
            get { return innerOperationInvoker.IsSynchronous; }
        }

        #endregion IOperationInvoker Members
    }
}