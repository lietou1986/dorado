namespace Dorado.DataExpress
{
    public class DataAccessObject<T> : DataAccessBase where T : class, new()
    {
        public DataAccessObject()
        {
        }

        public DataAccessObject(string databaseName)
            : base(databaseName)
        {
        }
    }
}