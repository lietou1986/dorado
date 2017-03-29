namespace Dorado.Data.Exceptions
{
    public class SqlParameterException : DataException
    {
        public SqlParameterException(string paramName, string paramValue)
            : base(string.Format("Parameter value contains illegal words, parameter name: {0}, parameter value: '{1}'", paramName, paramValue))
        {
        }
    }
}