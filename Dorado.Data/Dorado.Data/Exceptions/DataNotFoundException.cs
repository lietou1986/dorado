namespace Dorado.Data.Exceptions
{
    public class DataNotFoundException : DataException
    {
        public DataNotFoundException(string message)
            : base(message)
        {
        }

        public DataNotFoundException(string message, string dbInstanceName)
            : base(message)
        {
            base.DBInstanceName = dbInstanceName;
        }

        public DataNotFoundException(string message, string dbInstanceName, string procedureName)
            : base(message)
        {
            base.DBInstanceName = dbInstanceName;
            this.procedureName = procedureName;
        }
    }
}