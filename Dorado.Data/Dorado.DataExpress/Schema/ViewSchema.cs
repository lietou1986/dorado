namespace Dorado.DataExpress.Schema
{
    public class ViewSchema : TableSchema
    {
        public ViewSchema(string name, string fullName, string desc, string alias)
        {
            base.Name = name;
            base.FullName = fullName;
            base.Description = desc;
            base.Alias = alias;
        }

        public ViewSchema(string name, string fullName, string desc)
        {
            base.Name = name;
            base.FullName = fullName;
            base.Description = desc;
        }

        public ViewSchema(string name, string fullName)
        {
            base.Name = name;
            base.FullName = fullName;
        }

        public ViewSchema()
        {
        }

        public ViewSchema(string name)
        {
            base.Name = name;
        }
    }
}