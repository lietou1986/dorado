using System.Runtime.Serialization;

namespace Dorado.ESB.Common.Utility
{
    /// <summary>
    /// 分布式访问的结果，包含了返回的结果数据以及错误信息数据
    /// </summary>
    /// <typeparam name="TReturnData">结果数据的类型</typeparam>
    [DataContract(Namespace = "http://www.Dorado.com")]
    public class RequestResult<TReturnData>
    {
        public RequestResult(TReturnData resultData)
        {
            this.ResultData = resultData;
            this.Succeed = true;
        }

        public RequestResult(int errorCode, string message)
        {
            this.Succeed = false;
            this.ErrorCode = errorCode;
            this.Message = message;
        }

        /// <summary>
        /// 调用是否成功
        /// </summary>
        [DataMember]
        public bool Succeed
        {
            get;
            set;
        }

        /// <summary>
        /// 调用的消息
        /// </summary>
        [DataMember]
        public string Message
        {
            get;
            set;
        }

        /// <summary>
        /// 调用失败的消息代码
        /// </summary>
        [DataMember]
        public int ErrorCode
        {
            get;
            set;
        }

        /// <summary>
        /// 调用返回的结果数据
        /// </summary>
        [DataMember]
        public TReturnData ResultData
        {
            get;
            set;
        }
    }
}