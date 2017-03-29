using Dorado.DataExpress.Schema;

namespace Dorado.DataExpress
{
    public struct ColumnWithValue
    {
        public ColumnSchema Column
        {
            get;
            set;
        }

        public object Value
        {
            get;
            set;
        }
    }
}