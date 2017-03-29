namespace Dorado.DataExpress.Schema
{
    public interface IDbSchema
    {
        string Name
        {
            get;
            set;
        }

        string FullName
        {
            get;
            set;
        }

        string Description
        {
            get;
            set;
        }

        string Alias
        {
            get;
            set;
        }
    }
}