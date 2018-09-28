using System.Collections.Generic;

namespace Dorado
{
    public class ResultList<T> : Result<List<T>>
    {

        public ResultList() : this(new List<T>())
        {

        }

        public ResultList(List<T> data) : base(data)
        {

        }

        public ResultList(int code, string message) : base(code, message)
        {

        }

        public ResultList(int code, string message, List<T> data) : base(code, message, data)
        {

        }

        public ResultList(ResultStatus status) : base(status)
        {

        }

        public ResultList(ResultStatus status, string message) : base(status, message)
        {

        }

        public ResultList(ResultStatus status, List<T> data) : base(status, data)
        {

        }

        public ResultList(ResultStatus status, string message, List<T> data) : base(status, message, data)
        {

        }

    }
}