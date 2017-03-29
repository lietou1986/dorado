
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace Dorado.Data
{
    public class SafeProcedureAsync
    {
        public static IAsyncResult BeginExecuteReader(AsyncCallback callback, Database db, string procName, params object[] parameterValues)
        {
            SqlConnection connection = db.GetAsyncConnection();
            SqlCommand command = CommandFactory.CreateCommand(connection, db.InstanceName, procName, parameterValues);
            IAsyncResult result;
            try
            {
                connection.Open();
                result = command.BeginExecuteReader(callback, command, CommandBehavior.CloseConnection);
            }
            catch (Dorado.Data.Exceptions.DataException)
            {
                CloseAsyncConnection(command);
                throw;
            }
            return result;
        }

        public static void EndExecuteAndMapResults(IAsyncResult ar, Action<IRecordSet> result)
        {
            SqlCommand command = (SqlCommand)ar.AsyncState;
            try
            {
                SqlDataReader reader = command.EndExecuteReader(ar);
                result(new DataRecord(reader));
            }
            catch (Dorado.Data.Exceptions.DataException)
            {
                throw;
            }
            finally
            {
                CloseAsyncConnection(ar);
            }
        }

        public static T EndExecuteAndGetInstance<T>(IAsyncResult ar, Action<IRecord,T> recordMapper) where T : new()
        {
            T objectInstance = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
            SqlCommand command = (SqlCommand)ar.AsyncState;
            using (SqlDataReader reader = command.EndExecuteReader(ar))
            {
                recordMapper(new DataRecord(reader), objectInstance);
            }
            return objectInstance;
        }

        public static IAsyncResult BeginExecuteNonQuery(AsyncCallback callback, Database db, string procName, params object[] parameterValues)
        {
            SqlConnection connection = db.GetAsyncConnection();
            SqlCommand command = CommandFactory.CreateCommand(connection, db.InstanceName, procName, parameterValues);
            IAsyncResult result;
            try
            {
                connection.Open();
                result = command.BeginExecuteNonQuery(callback, command);
            }
            catch (Dorado.Data.Exceptions.DataException)
            {
                CloseAsyncConnection(command);
                throw;
            }
            return result;
        }

        public static int EndExecuteNonQuery(IAsyncResult ar)
        {
            SqlCommand command = (SqlCommand)ar.AsyncState;
            int result = 0;
            try
            {
                result = command.EndExecuteNonQuery(ar);
            }
            catch (Dorado.Data.Exceptions.DataException)
            {
                throw;
            }
            finally
            {
                CloseAsyncConnection(ar);
            }
            return result;
        }

        public static void CloseAsyncConnection(SqlCommand command)
        {
            command.Connection.Close();
            command.Connection.Dispose();
        }

        public static void CloseAsyncConnection(IAsyncResult ar)
        {
            CloseAsyncConnection((SqlCommand)ar.AsyncState);
        }

        public static void ExecuteAndMapResults(Database database, string procedureName, Action<IParameterSet> parameterMapper, Action<IRecordSet> resultMapper)
        {
            SqlConnection connection = database.GetAsyncConnection();
            SqlCommand command = CommandFactory.CreateParameterMappedCommand(connection, database.InstanceName, procedureName, parameterMapper);
            bool isCompleted = false;
            try
            {
                command.Connection.Open();
                command.BeginExecuteReader(delegate(IAsyncResult result)
                {
                    try
                    {
                        SqlDataReader reader = command.EndExecuteReader(result);
                        resultMapper(new DataRecord(reader));
                    }
                    finally
                    {
                        CloseAsyncConnection(command);
                        isCompleted = true;
                    }
                }
                , command);
                if (isCompleted)
                {
                    return;
                }
            }
            catch
            {
                CloseAsyncConnection(command);
                throw;
            }
            Thread.Sleep(200);
        }

        public static int ExecuteNonQuery(Database database, string procedureName, Action<IParameterSet> parameterMapper)
        {
            SqlConnection connection = database.GetAsyncConnection();
            SqlCommand command = CommandFactory.CreateParameterMappedCommand(connection, database.InstanceName, procedureName, parameterMapper);
            bool isCompleted = false;
            int result = 0;
            try
            {
                connection.Open();
                command.BeginExecuteNonQuery(delegate(IAsyncResult ar)
                {
                    SqlCommand locCommand = ar.AsyncState as SqlCommand;
                    try
                    {
                        result = locCommand.EndExecuteNonQuery(ar);
                    }
                    finally
                    {
                        CloseAsyncConnection(locCommand);
                        isCompleted = true;
                    }
                }
                , command);
                if (isCompleted)
                {
                    return result;
                }
            }
            catch
            {
                CloseAsyncConnection(command);
                throw;
            }
            Thread.Sleep(200);
            return result;
        }
    }
}