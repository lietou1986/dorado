using Dorado.Configuration;
using Dorado.Data.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Threading;

namespace Dorado.Data
{
    [Serializable]
    public class Database
    {
        private string connectionString;
        private string connectionStringAsync;
        private long lastTimeout;
        private int timeoutCount;
        private int connectionsDenied;
        private long lastConnectionDenied;
        private int totalTimeoutCount;
        private int totalConnectionsServed;
        private int totalConnectionsDenied;
        private string exceptionLog = string.Empty;
        private object padLock = new object();
        private string instanceName;

        public ConnectivityState ConnectivityState
        {
            get
            {
                if (timeoutCount <= 0) return ConnectivityState.Up;
                long downTicks = 1200000000L;
                if (lastTimeout + downTicks > DateTime.Now.Ticks)
                {
                    return ConnectivityState.Down;
                }
                ResetState();
                return ConnectivityState.Up;
            }
        }

        public string InstanceName
        {
            get
            {
                return instanceName;
            }
        }

        internal void SetConnectionString(string connString)
        {
            connectionString = connString;
            connectionStringAsync = connString + ";async=true;";
        }

        internal Database(string instanceName)
        {
            try
            {
                SetConnectionString(ConnectionStringProvider.Get(instanceName));
            }
            catch
            {
                throw new DatabaseNotConfiguredException(instanceName);
            }
        }

        private Database()
        {
        }

        public void RegisterSqlTimeout(Exception e)
        {
            exceptionLog = string.Concat(new string[]
			{
				e.ToString(),
				" : ",
				e.Message,
				" : ",
				e.StackTrace
			});
            if ((e is InvalidOperationException && e.Message.StartsWith("Timeout expired.")) || (e.InnerException != null && e.InnerException is SqlException && (e.Message.StartsWith("SQL Server does not exist or access denied.") || e.InnerException.Message.StartsWith("An error has occurred while establishing a connection to the server."))) || (e is SqlException && (e.Message.StartsWith("SQL Server does not exist or access denied.") || e.Message.StartsWith("An error has occurred while establishing a connection to the server."))))
            {
                object obj;
                Monitor.Enter(obj = padLock);
                try
                {
                    lastTimeout = DateTime.Now.Ticks;
                    timeoutCount++;
                    totalTimeoutCount++;
                    SqlConnection.ClearPool(new SqlConnection(GetConnectionString()));
                }
                finally
                {
                    Monitor.Exit(obj);
                }

                throw new DatabaseDownException("Database " + instanceName + " is down.");
            }
        }

        private void CheckConnectivity()
        {
            if (ConfigurationManager.AppSettings["DatabaseConnectivityState"] == "enabled" && ConnectivityState == ConnectivityState.Down)
            {
                connectionsDenied++;
                totalConnectionsDenied++;
                lastConnectionDenied = DateTime.Now.Ticks;
                throw new DatabaseDownException("Database " + instanceName + " is down.");
            }
            totalConnectionsServed++;
        }

        public void ResetState()
        {
            timeoutCount = 0;
            connectionsDenied = 0;
        }

        public string GetConnectionString()
        {
            return connectionString;
        }

        public string GetAsyncConnectionString()
        {
            return connectionStringAsync;
        }

        public SqlConnection GetAsyncConnection()
        {
            CheckConnectivity();
            return new SqlConnection(GetAsyncConnectionString());
        }

        public SqlConnection GetConnection()
        {
            CheckConnectivity();
            return new SqlConnection(GetConnectionString());
        }

        public SqlConnection GetOpenConnection()
        {
            CheckConnectivity();
            SqlConnection connection = new SqlConnection(GetConnectionString());
            connection.Open();
            return connection;
        }

        public static Database GetDatabase(string instanceName)
        {
            return DatabaseManager.Instance.GetDatabase(instanceName);
        }

        public static List<Database> GetDatabases()
        {
            return new List<Database>(DatabaseManager.Instance.Databases);
        }

        public static SqlConnection GetConnection(string instanceName)
        {
            return GetDatabase(instanceName).GetConnection();
        }

        public string GetHtmlStatus(StringBuilder sb)
        {
            sb.Append(ConnectivityState == ConnectivityState.Down
                ? "<dl class=\"dangerousServer\">"
                : "<dl class=\"happyServer\">");
            AddHeaderLine(sb, InstanceName);
            AddPropertyLine(sb, "Total Exceptions", totalTimeoutCount);
            AddPropertyLine(sb, "Total Connections Served", totalConnectionsServed);
            AddPropertyLine(sb, "Total Connections Denied", totalConnectionsDenied);
            if (timeoutCount > 0 && connectionsDenied > 0)
            {
                sb.Append("<hr/>");
                AddPropertyLine(sb, "Connections Denied Since Downed", connectionsDenied);
                if (connectionsDenied > 0)
                {
                    AddPropertyLine(sb, "Last Connection Denied", new DateTime(lastConnectionDenied).ToString());
                }
            }
            if (lastTimeout > 0L)
            {
                sb.Append("<hr/>");
                AddPropertyLine(sb, "Last Exception Caught", new DateTime(lastTimeout).ToString());
                sb.AppendFormat("<dd class=\"exception\">{0}</dd>", exceptionLog);
            }
            sb.Append("</dl>");
            return sb.ToString();
        }

        private void AddPropertyLine(StringBuilder sb, string propName, object propValue)
        {
            if (propValue.ToString() == "0")
            {
                return;
            }
            sb.AppendFormat("<dt>{0}:</dt><dd>{1}</dd>", propName, propValue);
        }

        private void AddHeaderLine(StringBuilder sb, object propValue)
        {
            sb.AppendFormat("<h3>{0}:</h3>", propValue);
        }
    }
}