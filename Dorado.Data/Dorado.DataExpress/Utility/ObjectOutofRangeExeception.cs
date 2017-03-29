using System;

namespace Dorado.DataExpress.Utility
{
    [Serializable]
    public class ObjectOutofRangeExeception : ApplicationException
    {
        public ObjectOutofRangeExeception(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}