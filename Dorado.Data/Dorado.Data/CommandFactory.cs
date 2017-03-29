using Dorado.Data.Exceptions;

using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dorado.Data
{
    internal class CommandFactory
    {
        private static CommandCache commandCache = new CommandCache();
        private static Regex forbiddenVarchars = new Regex("[^\t-ÿ]", RegexOptions.Compiled);
        private static readonly string forbiddenVarcharsReplacement = "?";

        private static void MapParameters(SqlCommand command, object[] parameters)
        {
            int returnValueOffset = 1;
            if (parameters == null)
            {
                AssertParameterCount(command.Parameters.Count, 0, returnValueOffset, command.CommandText);
                return;
            }
            AssertParameterCount(command.Parameters.Count, parameters.Length, returnValueOffset, command.CommandText);
            int i = returnValueOffset;
            int j = parameters.Length;
            while (i <= j)
            {
                object parameterValue = parameters[i - 1] ?? DBNull.Value;
                command.Parameters[i].Value = parameterValue;
                i++;
            }
        }

        private static void MapParameters(SqlCommand command, StoredProcedureParameterList parameters)
        {
            int returnValueOffset = 1;
            int parameterCount = parameters.Count;
            if (parameters.Any(spp => spp.ParameterDirection == ParameterDirectionWrap.ReturnValue))
            {
                parameterCount--;
            }
            AssertParameterCount(command.Parameters.Count, parameterCount, returnValueOffset, command.CommandText);
            int i = returnValueOffset;
            int j = parameters.Count;
            while (i <= j)
            {
                SqlParameter sqlParameter;
                StoredProcedureParameter spp = parameters[i - 1];
                sqlParameter = spp.Key != null ? command.Parameters[spp.Key] : command.Parameters[i];
                sqlParameter.Value = spp.Value ?? DBNull.Value;
                switch (spp.ParameterDirection)
                {
                    case ParameterDirectionWrap.Input:
                        sqlParameter.Direction = ParameterDirection.Input;
                        break;

                    case ParameterDirectionWrap.Output:
                        sqlParameter.Direction = ParameterDirection.Output;
                        break;

                    case ParameterDirectionWrap.InputOutput:
                        sqlParameter.Direction = ParameterDirection.InputOutput;
                        break;

                    case ParameterDirectionWrap.ReturnValue:
                        sqlParameter.Direction = ParameterDirection.ReturnValue;
                        break;

                    default:
                        throw new ArgumentException("Unknow parameter direction specified: " + spp.ParameterDirection.ToString());
                }
                if (spp.Size.HasValue)
                {
                    sqlParameter.Size = spp.Size.Value;
                }
                i++;
            }
        }

        private static void AssertParameterCount(int numProcedureParameters, int numPassedParameters, int returnValueOffset, string procedureName)
        {
            if (numProcedureParameters < numPassedParameters + returnValueOffset)
            {
                throw new ArgumentException(string.Format("Too many parameters parameters were supplied to the procedure {0}.  The number supplied was: {1}.  The number expected is: {2}.", procedureName, numPassedParameters, numProcedureParameters - returnValueOffset));
            }
        }

        internal static SqlCommand CreateParameterizedCommand(SqlConnection connection, string databaseInstanceName, string commandName)
        {
            if (commandName.IndexOf("dbo", StringComparison.OrdinalIgnoreCase) == -1)
            {
                throw new NoDboException(connection.Database, commandName);
            }
            SqlCommand command = commandCache.GetCommandCopy(connection, databaseInstanceName, commandName);

            return command;
        }

        internal static SqlCommand CreateParameterMappedCommand(SqlConnection connection, string databaseInstanceName, string procedureName, Action<IParameterSet> parameterMapper)
        {
            if (procedureName.IndexOf("dbo", StringComparison.OrdinalIgnoreCase) == -1)
            {
                throw new NoDboException(connection.Database, procedureName);
            }
            SqlCommand command = connection.CreateCommand();
            command.CommandText = procedureName;
            command.CommandType = CommandType.StoredProcedure;
            if (parameterMapper != null)
            {
                ParameterSet pSet = new ParameterSet(command.Parameters);
                parameterMapper(pSet);
            }
            ApplySecurity(command, commandCache.GetCommandCopy(connection, databaseInstanceName, procedureName).Parameters);

            return command;
        }

        internal static SqlCommand CreateParameterMappedCommand<T>(SqlConnection connection, string databaseInstanceName, string procedureName, Action<IParameterSet, T> parameterMapper, T objectInstance)
        {
            if (procedureName.IndexOf("dbo", StringComparison.OrdinalIgnoreCase) == -1)
            {
                throw new NoDboException(connection.Database, procedureName);
            }
            SqlCommand command = connection.CreateCommand();
            command.CommandText = procedureName;
            command.CommandType = CommandType.StoredProcedure;
            if (parameterMapper != null)
            {
                ParameterSet pSet = new ParameterSet(command.Parameters);
                parameterMapper(pSet, objectInstance);
            }
            ApplySecurity(command, commandCache.GetCommandCopy(connection, databaseInstanceName, procedureName).Parameters);

            return command;
        }

        internal static SqlCommand CreateCommand(SqlConnection connection, string databaseInstanceName, string commandName, params object[] parameterValues)
        {
            SqlCommand command = CreateParameterizedCommand(connection, databaseInstanceName, commandName);
            MapParameters(command, parameterValues);
            ApplySecurity(command, commandCache.GetCommandCopy(connection, databaseInstanceName, commandName).Parameters);

            return command;
        }

        internal static SqlCommand CreateCommand(SqlConnection connection, string databaseInstanceName, string commandName, SqlParameter[] parameterValues)
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = commandName;
            command.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter p in parameterValues)
            {
                command.Parameters.Add(p);
            }
            ApplySecurity(command, commandCache.GetCommandCopy(connection, databaseInstanceName, commandName).Parameters);

            return command;
        }

        internal static SqlCommand CreateCommand(SqlConnection connection, string databaseInstanceName, string commandName, StoredProcedureParameterList parameterList)
        {
            SqlCommand command = CreateParameterizedCommand(connection, databaseInstanceName, commandName);
            MapParameters(command, parameterList);
            ApplySecurity(command, commandCache.GetCommandCopy(connection, databaseInstanceName, commandName).Parameters);

            return command;
        }

        private static void ApplySecurity(SqlCommand command, SqlParameterCollection parameterTypes)
        {
            foreach (SqlParameter parameter in parameterTypes)
            {
                if (parameter.DbType != DbType.AnsiString) continue;
                string parameterName = parameter.ParameterName.Replace("@", "").ToLower();
                foreach (SqlParameter commandParameter in command.Parameters.Cast<SqlParameter>().Where(commandParameter => commandParameter.ParameterName.Replace("@", "").ToLower() == parameterName && commandParameter.Value != null && commandParameter.Value != DBNull.Value))
                {
                    commandParameter.Value = forbiddenVarchars.Replace(commandParameter.Value.ToString(), forbiddenVarcharsReplacement);
                }
            }
        }
    }
}