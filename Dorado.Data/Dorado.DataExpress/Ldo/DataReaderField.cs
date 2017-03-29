using System;

namespace Dorado.DataExpress.Ldo
{
    public class DataReaderField
    {
        public int Ordinal
        {
            get;
            set;
        }

        public string FieldName
        {
            get;
            set;
        }

        public Type DataType
        {
            get;
            set;
        }
    }
}