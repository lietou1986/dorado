using System;
using System.Runtime.Serialization;

namespace Dorado.ESB.Common.Utility
{
    /// <summary>
    /// ESB异常信息的基类,目标是，当业务系统实现了此基类后，可以由框架来统一做系统的异常处理机制，业务服务不用做异常处理。
    /// </summary>
    [DataContract(Namespace = "http://www.Dorado.com")]
    public abstract class ESBExceptionBase : ApplicationException
    {
        /// <summary>
        /// 异常所对应的错误代码。
        /// </summary>
        public virtual int ErrorCode
        {
            get
            {
                return 0;
            }
        }
    }

    public abstract class ESBExceptionFactory
    {
        public abstract ESBExceptionBase GetException(int errorCode);
    }
}