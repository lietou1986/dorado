using Dorado.Data.Exceptions;

using System;
using System.Data;
using System.Data.SqlClient;

namespace Dorado.Data
{
    public static class Procedure
    {
        private const string LOG_PREFIX = "DB_CALL_LOG - Procedure";

        public static IRecordSet Execute<T>(Database database, string procedureName, Action<IParameterSet,T> parameterMapper, T objectInstance)
        {
            SqlConnection connection = database.GetConnection();
            SqlCommand command = CommandFactory.CreateParameterMappedCommand<T>(connection, database.InstanceName, procedureName, parameterMapper, objectInstance);

            IRecordSet result;
            try
            {
                command.Connection.Open();
                IRecordSet record = new DataRecord(command.ExecuteReader(CommandBehavior.CloseConnection));
                result = record;
            }
            catch (Exception ex)
            {
                command.Connection.Close();
                throw new DatabaseExecutionException(database, procedureName, command, ex);
            }
            return result;
        }

        public static IRecordSet Execute(Database database, string procedureName, Action<IParameterSet> parameterMapper)
        {
            SqlConnection connection = database.GetConnection();
            SqlCommand command = CommandFactory.CreateParameterMappedCommand(connection, database.InstanceName, procedureName, parameterMapper);

            IRecordSet result;
            try
            {
                command.Connection.Open();
                IRecordSet record = new DataRecord(command.ExecuteReader(CommandBehavior.CloseConnection));
                result = record;
            }
            catch (Exception ex)
            {
                command.Connection.Close();
                throw new DatabaseExecutionException(database, procedureName, command, ex);
            }
            return result;
        }

        public static IRecordSet Execute(Database database, string procedureName, params object[] parameters)
        {
            SqlConnection connection = database.GetConnection();
            SqlCommand command = CommandFactory.CreateCommand(connection, database.InstanceName, procedureName, parameters);
            IRecordSet result;
            try
            {
                connection.Open();
                IRecordSet recordSet = new DataRecord(command.ExecuteReader(CommandBehavior.CloseConnection));
                result = recordSet;
            }
            catch (Exception ex)
            {
                connection.Close();
                throw new DatabaseExecutionException(database, procedureName, command, ex);
            }
            return result;
        }

        public static IRecordSet Execute(Database database, string procedureName, SqlParameter[] parameters)
        {
            SqlConnection connection = database.GetConnection();
            SqlCommand command = CommandFactory.CreateCommand(connection, database.InstanceName, procedureName, parameters);
            IRecordSet result;
            try
            {
                connection.Open();
                IRecordSet recordSet = new DataRecord(command.ExecuteReader(CommandBehavior.CloseConnection));
                result = recordSet;
            }
            catch (Exception ex)
            {
                connection.Close();
                throw new DatabaseExecutionException(database, procedureName, command, ex);
            }
            return result;
        }

        internal static IDataReader ExecuteReader(Database database, string procedureName, params object[] parameters)
        {
            SqlConnection connection = database.GetConnection();
            SqlCommand command = CommandFactory.CreateCommand(connection, database.InstanceName, procedureName, parameters);
            IDataReader result;
            try
            {
                connection.Open();
                IDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                result = reader;
            }
            catch (Exception ex)
            {
                connection.Close();
                throw new DatabaseExecutionException(database, procedureName, command, ex);
            }
            return result;
        }

        internal static IDataReader ExecuteReader(Database database, string procedureName, Action<IParameterSet> parameterMapper)
        {
            SqlConnection connection = database.GetConnection();
            SqlCommand command = CommandFactory.CreateParameterMappedCommand(connection, database.InstanceName, procedureName, parameterMapper);

            IDataReader result;
            try
            {
                command.Connection.Open();
                IDataReader record = command.ExecuteReader(CommandBehavior.CloseConnection);
                result = record;
            }
            catch (Exception ex)
            {
                command.Connection.Close();
                throw new DatabaseExecutionException(database, procedureName, command, ex);
            }
            return result;
        }
    }
}