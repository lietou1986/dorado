using System;
using System.Data;

namespace Dorado.Spider.Workload.Sql.Oracle
{
    /// <summary>
    /// OracleWorkloadManager: Contains a workload manager
    /// customized to Oracle.
    /// </summary>
    internal class OracleWorkloadManager : SqlWorkloadManager
    {
        /// <summary>
        /// Creates an instance of the OracleHolder class.
        /// </summary>
        /// <returns>An SqlHolder derived object.</returns>
        public override SqlHolder CreateSqlHolder()
        {
            return new OracleHolder();
        }

        /// <summary>
        /// Return the size of the specified column.  Oracle
        /// requires the "%" parameter to be specified.
        /// </summary>
        /// <param name="table">The table that contains the column.</param>
        /// <param name="column">The column to get the size for.</param>
        /// <returns>The size of the column.</returns>
        public override int GetColumnSize(String table, String column)
        {
            String[] restriction = { null, null, table, "%" };
            DataTable dt = this.Connection.GetSchema("Columns", restriction);
            foreach (System.Data.DataRow row in dt.Rows)
            {
                if (String.Compare(row["column_name"].ToString(), column, true) == 0)
                {
                    return int.Parse(row["column_size"].ToString());
                }
            }

            return -1;
        }
    }
}