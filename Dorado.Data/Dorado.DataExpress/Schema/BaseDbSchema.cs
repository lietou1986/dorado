using System;

namespace Dorado.DataExpress.Schema
{
    [Serializable]
    public class BaseDbSchema : IDbSchema
    {
        public string Owner
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string FullName
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Alias
        {
            get;
            set;
        }

        public BaseDbSchema()
        {
            this.Alias = string.Empty;
            this.Description = string.Empty;
            this.FullName = string.Empty;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}