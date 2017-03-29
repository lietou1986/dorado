namespace Dorado.Data.Exceptions
{
    public class BadDataException : DataException
    {
        public BadDataException(string message)
            : base(message)
        {
        }

        public BadDataException(string message, string dbInstanceName)
            : base(message)
        {
            base.DBInstanceName = dbInstanceName;
        }

        public BadDataException(string message, string dbInstanceName, string procedureName)
            : base(message)
        {
            base.DBInstanceName = dbInstanceName;
            this.procedureName = procedureName;
        }
    }
}