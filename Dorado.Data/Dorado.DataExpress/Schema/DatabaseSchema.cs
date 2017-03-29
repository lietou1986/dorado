using System.Collections.Generic;

namespace Dorado.DataExpress.Schema
{
    public class DatabaseSchema : BaseDbSchema
    {
        public List<TableSchema> Tables = new List<TableSchema>();

        public DatabaseSchema(string name, string fullName, string desc, string alias)
        {
            base.Name = name;
            base.FullName = fullName;
            base.Description = desc;
            base.Alias = alias;
        }

        public DatabaseSchema(string name, string fullName, string desc)
        {
            base.Name = name;
            base.FullName = fullName;
            base.Description = desc;
        }

        public DatabaseSchema(string name, string fullName)
        {
            base.Name = name;
            base.FullName = fullName;
        }

        public DatabaseSchema()
        {
        }
    }
}