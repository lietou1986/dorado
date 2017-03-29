using System;

namespace Dorado.DataExpress
{
    public class ColumnsNotFoundExcpetion : ApplicationException
    {
        public ColumnsNotFoundExcpetion(string msg)
            : base(msg)
        {
        }
    }
}