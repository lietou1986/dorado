using Dorado.Data.Exceptions;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Dorado.Data
{
    public class SafeProcedure
    {
        private const string LOG_PREFIX = "DB_CALL_LOG - SafeProcedure";

        public static int ExecuteNonQuery(Database database, string procedureName, Action<IParameterSet> parameterMapper, Action<IParameterSet> outputMapper)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }
            if (procedureName == null)
            {
                throw new ArgumentNullException("procedureName");
            }
            int result = 0;
            try
            {
                using (SqlConnection connection = database.GetOpenConnection())
                {
                    result = ExecuteNonQuery(database, connection, procedureName, parameterMapper, outputMapper);
                }
            }
            catch (SafeProcedureException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, ex);
            }
            return result;
        }

        public static int ExecuteNonQuery(Database database, SqlConnection connection, string procedureName, Action<IParameterSet> parameterMapper, Action<IParameterSet> outputMapper)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if (procedureName == null)
            {
                throw new ArgumentNullException("procedureName");
            }
            int result = 0;
            try
            {
                SqlCommand command = CommandFactory.CreateParameterMappedCommand(connection, database.InstanceName, procedureName, parameterMapper);

                bool doClose = false;
                if (connection.State != ConnectionState.Open)
                {
                    doClose = true;
                    connection.Open();
                }
                result = command.ExecuteNonQuery();
                if (doClose)
                {
                    connection.Close();
                }
                if (outputMapper != null)
                {
                    ParameterSet outputParams = new ParameterSet(command.Parameters);
                    outputMapper(outputParams);
                }
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, ex);
            }
            return result;
        }

        public static int ExecuteNonQuery(Database database, string procedureName, Action<IParameterSet> parameterMapper)
        {
            return ExecuteNonQuery(database, procedureName, parameterMapper, null);
        }

        public static int ExecuteNonQuery<T>(Database database, string procedureName, Action<IParameterSet,T> parameterMapper, T objectInstance)
        {
            int result = 0;
            try
            {
                using (SqlConnection connection = database.GetConnection())
                {
                    SqlCommand command = CommandFactory.CreateParameterMappedCommand<T>(connection, database.InstanceName, procedureName, parameterMapper, objectInstance);

                    connection.Open();
                    result = command.ExecuteNonQuery();
                    connection.Close();
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, ex);
            }
        }

        public static int ExecuteNonQuery(Database database, string procedureName, params object[] parameterValues)
        {
            int result = 0;
            try
            {
                using (SqlConnection connection = database.GetConnection())
                {
                    SqlCommand command = CommandFactory.CreateCommand(connection, database.InstanceName, procedureName, parameterValues);

                    connection.Open();
                    result = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, ex);
            }
            return result;
        }

        public static object ExecuteScalar(Database database, string procedureName, Action<IParameterSet> parameterMapper)
        {
            return ExecuteScalar(database, procedureName, parameterMapper, null);
        }

        public static object ExecuteScalar(Database database, string procedureName, Action<IParameterSet> parameterMapper, Action<IParameterSet> outputMapper)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }
            if (procedureName == null)
            {
                throw new ArgumentNullException("procedureName");
            }
            object result;
            try
            {
                using (SqlConnection connection = database.GetOpenConnection())
                {
                    result = ExecuteScalar(database, connection, procedureName, parameterMapper, outputMapper);
                }
            }
            catch (SafeProcedureException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, ex);
            }
            return result;
        }

        public static object ExecuteScalar(Database database, SqlConnection connection, string procedureName, Action<IParameterSet> parameterMapper, Action<IParameterSet> outputMapper)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            if (procedureName == null)
            {
                throw new ArgumentNullException("procedureName");
            }
            object result;
            try
            {
                SqlCommand command = CommandFactory.CreateParameterMappedCommand(connection, database.InstanceName, procedureName, parameterMapper);

                bool doClose = false;
                if (connection.State != ConnectionState.Open)
                {
                    doClose = true;
                    connection.Open();
                }
                result = command.ExecuteScalar();
                if (doClose)
                {
                    connection.Close();
                }
                if (outputMapper != null)
                {
                    ParameterSet outputParams = new ParameterSet(command.Parameters);
                    outputMapper(outputParams);
                }
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, ex);
            }
            return result;
        }

        public static object ExecuteScalar(Database database, string procedureName, params object[] parameterValues)
        {
            object result;
            try
            {
                using (SqlConnection connection = database.GetConnection())
                {
                    SqlCommand command = CommandFactory.CreateCommand(connection, database.InstanceName, procedureName, parameterValues);

                    connection.Open();
                    result = command.ExecuteScalar();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, ex);
            }
            return result;
        }

        public static DataTable Execute(Database database, string procedureName, params object[] parameters)
        {
            DataTable dt;
            try
            {
                dt = new DataTable(procedureName);
                using (IDataReader reader = Procedure.ExecuteReader(database, procedureName, parameters))
                {
                    dt.Load(reader, LoadOption.OverwriteChanges);
                }
            }
            catch (Dorado.Data.Exceptions.DataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, ex);
            }
            return dt;
        }

        public static DataTable Execute(Database database, string procedureName, Action<IParameterSet> parameterMapper)
        {
            DataTable dt;
            try
            {
                dt = new DataTable(procedureName);
                using (IDataReader reader = Procedure.ExecuteReader(database, procedureName, parameterMapper))
                {
                    dt.Load(reader, LoadOption.OverwriteChanges);
                }
            }
            catch (Dorado.Data.Exceptions.DataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, ex);
            }
            return dt;
        }

        public static void ExecuteAndMapResults(Database database, string procedureName, StoredProcedureParameterList parameterList, Action<IRecordSet> result, Action<IParameterSet> outputMapper)
        {
            try
            {
                using (SqlConnection connection = database.GetConnection())
                {
                    SqlCommand command = CommandFactory.CreateCommand(connection, database.InstanceName, procedureName, parameterList);

                    connection.Open();
                    IRecordSet reader = new DataRecord(command.ExecuteReader(CommandBehavior.CloseConnection));
                    result(reader);
                    connection.Close();
                    if (outputMapper != null)
                    {
                        outputMapper(new ParameterSet(command.Parameters));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, ex);
            }
        }

        public static void ExecuteAndMapResults(Database database, string procedureName, Action<IParameterSet> parameterMapper, Action<IRecordSet> result)
        {
            try
            {
                using (IRecordSet reader = Procedure.Execute(database, procedureName, parameterMapper))
                {
                    result(reader);
                }
            }
            catch (Dorado.Data.Exceptions.DataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, ex);
            }
        }

        public static void ExecuteAndMapResults(Database database, string procedureName, Action<IRecordSet> result, params object[] parameters)
        {
            try
            {
                using (IRecordSet reader = Procedure.Execute(database, procedureName, parameters))
                {
                    result(reader);
                }
            }
            catch (Dorado.Data.Exceptions.DataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, ex);
            }
        }

        public static void ExecuteAndMapRecords(Database database, string procedureName, Action<IParameterSet> parameterMapper, Action<IRecord> recordMapper)
        {
            try
            {
                using (IRecordSet reader = Procedure.Execute(database, procedureName, parameterMapper))
                {
                    while (reader.Read())
                    {
                        recordMapper(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, ex);
            }
        }

        public static void ExecuteAndMapRecords(Database database, string procedureName, Action<IParameterSet> parameterMapper, Action<IRecord> recordMapper, Action<IParameterSet> outputMapper)
        {
            try
            {
                using (SqlConnection connection = database.GetConnection())
                {
                    SqlCommand command = CommandFactory.CreateParameterMappedCommand(connection, database.InstanceName, procedureName, parameterMapper);

                    connection.Open();
                    IRecordSet reader = new DataRecord(command.ExecuteReader(CommandBehavior.CloseConnection));
                    while (reader.Read())
                    {
                        recordMapper(reader);
                    }
                    connection.Close();
                    if (outputMapper != null)
                    {
                        outputMapper(new ParameterSet(command.Parameters));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, ex);
            }
        }

        public static void ExecuteAndMapRecords(Database database, string procedureName, Action<IRecord> recordMapper, params object[] parameters)
        {
            try
            {
                using (IRecordSet reader = Procedure.Execute(database, procedureName, parameters))
                {
                    while (reader.Read())
                    {
                        recordMapper(reader);
                    }
                }
            }
            catch (Dorado.Data.Exceptions.DataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, ex);
            }
        }

        public static bool ExecuteAndHydrateInstance<T>(T objectInstance, Database database, string procedureName, Action<IParameterSet,T> parameterMapper, Action<IRecord,T> recordMapper)
        {
            bool result = false;
            try
            {
                using (IRecordSet reader = Procedure.Execute<T>(database, procedureName, parameterMapper, objectInstance))
                {
                    if (reader.Read())
                    {
                        recordMapper(reader, objectInstance);
                        result = true;
                    }
                }
            }
            catch (Dorado.Data.Exceptions.DataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, typeof(T), ex);
            }
            return result;
        }

        public static bool ExecuteAndHydrateInstance<T>(T objectInstance, Database database, string procedureName, Action<IParameterSet> parameterMapper, Action<IRecord,T> recordMapper)
        {
            bool result = false;
            try
            {
                using (IRecordSet reader = Procedure.Execute(database, procedureName, parameterMapper))
                {
                    if (reader.Read())
                    {
                        recordMapper(reader, objectInstance);
                        result = true;
                    }
                }
            }
            catch (Dorado.Data.Exceptions.DataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, typeof(T), ex);
            }
            return result;
        }

        public static bool ExecuteAndHydrateInstance<T>(T objectInstance, Database database, string procedureName, Action<IParameterSet> parameterMapper, Action<IRecord> recordMapper)
        {
            return ExecuteAndHydrateInstance<T>(objectInstance, database, procedureName, delegate(IParameterSet parameterSet, T instance)
            {
                parameterMapper(parameterSet);
            }
            , delegate(IRecord reader, T instance)
            {
                recordMapper(reader);
            }
            );
        }

        public static bool ExecuteAndHydrateInstance<T>(T objectInstance, Database database, string procedureName, Action<IRecord,T> recordMapper, params object[] parameters)
        {
            bool result = false;
            try
            {
                using (IRecordSet reader = Procedure.Execute(database, procedureName, parameters))
                {
                    if (reader.Read())
                    {
                        recordMapper(reader, objectInstance);
                        result = true;
                    }
                }
            }
            catch (Dorado.Data.Exceptions.DataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, typeof(T), ex);
            }
            return result;
        }

        public static bool ExecuteAndHydrateInstance<T>(T objectInstance, Database database, string procedureName, Action<IRecord> recordMapper, params object[] parameters)
        {
            return ExecuteAndHydrateInstance<T>(objectInstance, database, procedureName, delegate(IRecord reader, T instance)
            {
                recordMapper(reader);
            }
            , parameters);
        }

        public static T ExecuteAndGetInstance<T>(Database database, string procedureName, Action<IParameterSet> parameterMapper, Action<IRecord,T> recordMapper) where T : new()
        {
            T objectInstance = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
            if (!ExecuteAndHydrateInstance<T>(objectInstance, database, procedureName, parameterMapper, recordMapper))
            {
                return default(T);
            }
            return objectInstance;
        }

        public static T ExecuteAndGetInstance<T>(Database database, string procedureName, Action<IRecord,T> recordMapper, params object[] parameters) where T : new()
        {
            T objectInstance = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
            if (!ExecuteAndHydrateInstance<T>(objectInstance, database, procedureName, recordMapper, parameters))
            {
                return default(T);
            }
            return objectInstance;
        }

        public static List<T> ExecuteAndGetInstanceList<T>(Database database, string procedureName, Action<IParameterSet> parameterMapper, Action<IRecord,T> recordMapper) where T : new()
        {
            List<T> instanceList = new List<T>();
            ExecuteAndHydrateInstanceList<T>(instanceList, database, procedureName, parameterMapper, recordMapper);
            return instanceList;
        }

        public static List<T> ExecuteAndGetInstanceList<T>(Database database, string procedureName, Action<IRecord,T> recordMapper, params object[] parameters) where T : new()
        {
            List<T> instanceList = new List<T>();
            ExecuteAndHydrateInstanceList<T>(instanceList, database, procedureName, recordMapper, parameters);
            return instanceList;
        }

        public static List<T> ExecuteAndGetInstanceList<T>(Database database, string procedureName, Action<IRecord,T> recordMapper, SqlParameter[] parameters) where T : new()
        {
            List<T> instanceList = new List<T>();
            ExecuteAndHydrateInstanceList<T>(instanceList, database, procedureName, recordMapper, parameters);
            return instanceList;
        }

        public static void ExecuteAndHydrateInstanceList<T>(List<T> instanceList, Database database, string procedureName, Action<IParameterSet> parameterMapper, Action<IRecord,T> recordMapper) where T : new()
        {
            try
            {
                using (IRecordSet reader = Procedure.Execute(database, procedureName, parameterMapper))
                {
                    while (reader.Read())
                    {
                        T objectInstance = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
                        recordMapper(reader, objectInstance);
                        instanceList.Add(objectInstance);
                    }
                }
            }
            catch (Dorado.Data.Exceptions.DataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, typeof(T), ex);
            }
        }

        public static void ExecuteAndHydrateInstanceList<TConcrete, TList>(ICollection<TList> instanceList, Database database, string procedureName, Action<IParameterSet> parameterMapper, Action<IRecord,TConcrete> recordMapper) where TConcrete : TList, new()
        {
            try
            {
                using (IRecordSet reader = Procedure.Execute(database, procedureName, parameterMapper))
                {
                    while (reader.Read())
                    {
                        TConcrete objectInstance = (default(TConcrete) == null) ? Activator.CreateInstance<TConcrete>() : default(TConcrete);
                        recordMapper(reader, objectInstance);
                        instanceList.Add((TList)objectInstance);
                    }
                }
            }
            catch (Dorado.Data.Exceptions.DataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, typeof(TConcrete), ex);
            }
        }

        public static void ExecuteAndHydrateInstanceList<T>(List<T> instanceList, Database database, string procedureName, Action<IRecord,T> recordMapper, params object[] parameters) where T : new()
        {
            try
            {
                using (IRecordSet reader = Procedure.Execute(database, procedureName, parameters))
                {
                    while (reader.Read())
                    {
                        T objectInstance = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
                        recordMapper(reader, objectInstance);
                        instanceList.Add(objectInstance);
                    }
                }
            }
            catch (Dorado.Data.Exceptions.DataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, typeof(T), ex);
            }
        }

        public static void ExecuteAndHydrateInstanceList<T>(List<T> instanceList, Database database, string procedureName, Action<IRecord,T> recordMapper, SqlParameter[] parameters) where T : new()
        {
            try
            {
                using (IRecordSet reader = Procedure.Execute(database, procedureName, parameters))
                {
                    while (reader.Read())
                    {
                        T objectInstance = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
                        recordMapper(reader, objectInstance);
                        instanceList.Add(objectInstance);
                    }
                }
            }
            catch (Dorado.Data.Exceptions.DataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, typeof(T), ex);
            }
        }

        public static void ExecuteAndHydrateInstanceList<T>(List<T> instanceList, Database database, string procedureName, Action<IRecord> recordMapper, params object[] parameters) where T : new()
        {
            try
            {
                using (IRecordSet reader = Procedure.Execute(database, procedureName, parameters))
                {
                    while (reader.Read())
                    {
                        T objectInstance = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
                        recordMapper(reader);
                        instanceList.Add(objectInstance);
                    }
                }
            }
            catch (Dorado.Data.Exceptions.DataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, typeof(T), ex);
            }
        }

        public static void ExecuteAndHydrateGenericInstance<T>(T objectInstance, Database database, string procedureName, Action<IParameterSet> parameterMapper, Action<IRecord,T>[] recordMappers) where T : new()
        {
            try
            {
                using (IRecordSet reader = Procedure.Execute(database, procedureName, parameterMapper))
                {
                    int mapperIndex = 0;
                    while (mapperIndex < recordMappers.Length)
                    {
                        while (reader.Read())
                        {
                            recordMappers[mapperIndex](reader, objectInstance);
                        }
                        mapperIndex++;
                        if (!reader.NextResult())
                        {
                            break;
                        }
                    }
                }
            }
            catch (Dorado.Data.Exceptions.DataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, typeof(T), ex);
            }
        }

        public static void ExecuteAndHydrateGenericInstance<T>(T objectInstance, Database database, string procedureName, Action<IRecord,T>[] recordMappers, params object[] parameters) where T : new()
        {
            try
            {
                using (IRecordSet reader = Procedure.Execute(database, procedureName, parameters))
                {
                    int mapperIndex = 0;
                    while (mapperIndex < recordMappers.Length)
                    {
                        while (reader.Read())
                        {
                            recordMappers[mapperIndex](reader, objectInstance);
                        }
                        mapperIndex++;
                        if (!reader.NextResult())
                        {
                            break;
                        }
                    }
                }
            }
            catch (Dorado.Data.Exceptions.DataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, typeof(T), ex);
            }
        }

        public static Dictionary<int, T> ExecuteAndGetDictionary<T>(Database database, string procedureName, Action<IParameterSet> parameterMapper, Action<IRecord,T> recordMapper) where T : new()
        {
            Dictionary<int, T> result;
            try
            {
                Dictionary<int, T> dictionaryList = new Dictionary<int, T>();
                using (IRecordSet reader = Procedure.Execute(database, procedureName, parameterMapper))
                {
                    while (reader.Read())
                    {
                        T objectInstance = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
                        recordMapper(reader, objectInstance);
                        dictionaryList[reader.GetInt32(0)] = objectInstance;
                    }
                }
                result = dictionaryList;
            }
            catch (Dorado.Data.Exceptions.DataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, typeof(T), ex);
            }
            return result;
        }

        public static Dictionary<int, T> ExecuteAndGetDictionary<T>(Database database, string procedureName, Action<IRecord,T> recordMapper, params object[] parameters) where T : new()
        {
            Dictionary<int, T> result;
            try
            {
                Dictionary<int, T> dictionaryList = new Dictionary<int, T>();
                using (IRecordSet reader = Procedure.Execute(database, procedureName, parameters))
                {
                    while (reader.Read())
                    {
                        T objectInstance = (default(T) == null) ? Activator.CreateInstance<T>() : default(T);
                        recordMapper(reader, objectInstance);
                        dictionaryList[reader.GetInt32(0)] = objectInstance;
                    }
                }
                result = dictionaryList;
            }
            catch (Dorado.Data.Exceptions.DataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SafeProcedureException(database, procedureName, typeof(T), ex);
            }
            return result;
        }
    }
}