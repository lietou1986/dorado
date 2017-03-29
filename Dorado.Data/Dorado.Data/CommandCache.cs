using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace Dorado.Data
{
    internal class CommandCache : Dictionary<string, SqlCommand>
    {
        internal SqlCommand GetCommandCopy(SqlConnection connection, string databaseInstanceName, string procedureName)
        {
            string commandCacheKey = databaseInstanceName + procedureName;
            if (!ContainsKey(commandCacheKey))
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = procedureName;
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                SqlCommandBuilder.DeriveParameters(command);
                connection.Close();
                Monitor.Enter(this);
                try
                {
                    base[commandCacheKey] = command;
                }
                finally
                {
                    Monitor.Exit(this);
                }
            }
            SqlCommand copiedCommand = base[commandCacheKey].Clone();
            copiedCommand.Connection = connection;
            return copiedCommand;
        }
    }
}