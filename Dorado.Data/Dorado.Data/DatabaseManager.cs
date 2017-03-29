using System.Threading;

namespace Dorado.Data
{
    internal class DatabaseManager
    {
        private static DatabaseManager instance = new DatabaseManager();
        private DatabaseCollection databases;

        internal static DatabaseManager Instance
        {
            get
            {
                return instance;
            }
        }

        internal DatabaseCollection Databases
        {
            get
            {
                return this.databases;
            }
        }

        internal DatabaseManager()
        {
            this.databases = new DatabaseCollection();
        }

        internal Database GetDatabase(string instanceName)
        {
            if (this.databases.Contains(instanceName)) return this.databases[instanceName];
            DatabaseCollection obj;
            Monitor.Enter(obj = this.databases);
            try
            {
                Database database = new Database(instanceName);
                if (!this.databases.Contains(instanceName))
                {
                    this.databases.Add(database);
                }
            }
            finally
            {
                Monitor.Exit(obj);
            }
            return this.databases[instanceName];
        }
    }
}