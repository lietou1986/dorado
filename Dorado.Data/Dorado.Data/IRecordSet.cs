using System;
using System.Data;

namespace Dorado.Data
{
    public interface IRecordSet : IDisposable, IRecord
    {
        int Depth
        {
            get;
        }

        bool IsClosed
        {
            get;
        }

        int RecordsAffected
        {
            get;
        }

        void Close();

        DataTable GetSchemaTable();

        bool NextResult();

        bool Read();
    }
}