
using Dorado.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dorado
{
    [DataContract]
    public class Result
    {
        public static readonly Result OK = new Result(ResultStatus.OK);
        public static readonly Result BAD_REQUEST = new Result(ResultStatus.BadRequest);
        public static readonly Result UNAUTHORIZED = new Result(ResultStatus.Unauthorized);
        public static readonly Result FORBIDDEN = new Result(ResultStatus.Forbidden);
        public static readonly Result NOT_FOUND = new Result(ResultStatus.NotFound);
        public static readonly Result ERROR = new Result(ResultStatus.Error);
        public static readonly Result NOT_SUPPORTED = new Result(ResultStatus.NotSupported);
        public static readonly Result CONFIG_ERROR = new Result(ResultStatus.ConfigError);
        public static readonly Result THIRD_PARTY_API_ERROR = new Result(ResultStatus.ThirdPartyApiError);

        [DataMember]
        public int Code { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public List<string> DebugInfo { get; set; }

        public Result() : this(ResultStatus.OK)
        {

        }

        public Result(int code, string message)
        {
            this.Code = code;
            this.Message = message;
            this.DebugInfo = new List<string>();
        }

        public Result(ResultStatus resultStatus) : this(resultStatus.Int(), resultStatus.GetDefaultDescription())
        {

        }

        public Result(ResultStatus resultStatus, string message) : this(resultStatus.Int(), message)
        {

        }

        public bool IsOk()
        {
            return Code == 200;
        }

        public bool IsFail()
        {
            return !IsOk();
        }

        public static Result OfSuccess(string message)
        {
            Result result = new Result();
            result.Code = 200;
            result.Message = message;
            return result;
        }

        public static Result OfFail(string message)
        {
            return OfFail(500, message);
        }

        public static Result OfFail(int code, string message)
        {
            Result result = new Result();
            result.Code = code;
            result.Message = message;
            return result;
        }

        public static Result OfException(Exception exception)
        {
            return OfException(500, exception);
        }

        public static Result OfException(int code, Exception exception)
        {
            Result result = new Result();
            result.Code = code;
            result.Message = exception.GetType().Name + ", " + exception.Message;
            return result;
        }

    }

    [DataContract]
    public class Result<T> : Result
    {
        public new static readonly Result<T> OK = new Result<T>(ResultStatus.OK);
        public new static readonly Result<T> BAD_REQUEST = new Result<T>(ResultStatus.BadRequest);
        public new static readonly Result<T> UNAUTHORIZED = new Result<T>(ResultStatus.Unauthorized);
        public new static readonly Result<T> FORBIDDEN = new Result<T>(ResultStatus.Forbidden);
        public new static readonly Result<T> NOT_FOUND = new Result<T>(ResultStatus.NotFound);
        public new static readonly Result<T> ERROR = new Result<T>(ResultStatus.Error);
        public new static readonly Result<T> NOT_SUPPORTED = new Result<T>(ResultStatus.NotSupported);
        public new static readonly Result<T> CONFIG_ERROR = new Result<T>(ResultStatus.ConfigError);
        public new static readonly Result<T> THIRD_PARTY_API_ERROR = new Result<T>(ResultStatus.ThirdPartyApiError);

        [DataMember]
        public T Data { get; set; }

        public Result(T data = default(T)) : base()
        {
            this.Data = data;
        }

        public Result(int code, string message, T data = default(T)) : base(code, message)
        {
            this.Data = data;
        }

        public Result(ResultStatus resultStatus, T data = default(T)) : this(resultStatus.Int(), resultStatus.GetDefaultDescription(), data)
        {

        }

        public Result(ResultStatus resultStatus, string message, T data = default(T)) : this(resultStatus.Int(), message, data)
        {

        }

        public static Result<T> OfSuccess(T data)
        {
            Result<T> result = new Result<T>();
            result.Code = 200;
            result.Data = data;
            return result;
        }

        public static Result<T> OfSuccess(T data, string message)
        {
            Result<T> result = new Result<T>();
            result.Code = 200;
            result.Data = data;
            result.Message = message;
            return result;
        }
    }
}
