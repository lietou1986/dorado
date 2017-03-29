using System;
using System.Collections.Generic;

namespace Dorado.DataExpress.Schema
{
    public class KeySchema : BaseDbSchema
    {
        public Dictionary<string, ColumnSchema> Columns
        {
            get;
            set;
        }

        public KeySchema()
        {
            this.Columns = new Dictionary<string, ColumnSchema>(StringComparer.OrdinalIgnoreCase);
        }
    }
}