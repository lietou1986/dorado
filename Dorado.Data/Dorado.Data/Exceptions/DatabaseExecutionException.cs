using System;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace Dorado.Data.Exceptions
{
    [Serializable]
    public class DatabaseExecutionException : DataException
    {
        protected Database database;
        private SqlCommand command;

        public Database Database
        {
            get
            {
                return this.database;
            }
            set
            {
                this.database = value;
            }
        }

        public SqlCommand Command
        {
            get
            {
                return this.command;
            }
            set
            {
                this.command = value;
            }
        }

        private static string GetParameterString(SqlCommand c)
        {
            string ret = string.Empty;
            string empty;
            try
            {
                if (c == null)
                {
                    empty = string.Empty;
                    return empty;
                }
                foreach (SqlParameter p in c.Parameters)
                {
                    object obj = ret;
                    ret = string.Concat(new object[]
					{
						obj,
						"{@",
						p.ParameterName,
						"}={",
						p.Value,
						"}"
					});
                }
            }
            catch (Exception)
            {
            }
            return ret;
        }

        public DatabaseExecutionException(Database database, string procedureName, Exception innerException)
            : base(string.Concat(new string[]
		{
			"Database Exception Against Database: ",
			database.InstanceName,
			" \n \n With procedure: ",
			procedureName,
			" \n \n With inner exception message: ",
			innerException.Message
		}), innerException)
        {
            this.database = database;
            this.procedureName = procedureName;
            this.innerException = innerException;
            this.instanceName = database.InstanceName;
            this.shortMessage = "Database Exception Against Database: " + database.InstanceName + "\n With inner exception message: " + innerException.Message;
        }

        public DatabaseExecutionException(Database database, string procedureName, SqlCommand command, Exception innerException)
            : base(string.Concat(new string[]
		{
			"Database Exception Against Database: ",
			database.InstanceName,
			" \n \n With procedure: ",
			procedureName,
			" \n \n With parameters: ",
			DatabaseExecutionException.GetParameterString(command),
			"\n With inner exception message: ",
			innerException.Message
		}), innerException)
        {
            this.database = database;
            this.procedureName = procedureName;
            this.command = command;
            this.innerException = innerException;
            this.instanceName = database.InstanceName;
            this.shortMessage = "Database Exception Against Database: " + database.InstanceName + "\n With inner exception message: " + innerException.Message;
        }

        public DatabaseExecutionException(Database database, string procedureName, string contextInfo, Exception innerException)
            : base(string.Concat(new string[]
		{
			"Database Exception Against Database: ",
			database.InstanceName,
			" \n \n With procedure: ",
			procedureName,
			" \n \n With context info: ",
			contextInfo,
			" \n \n With inner exception message: ",
			innerException.Message
		}), innerException)
        {
            this.database = database;
            this.procedureName = procedureName;
            this.innerException = innerException;
            this.instanceName = database.InstanceName;
            this.shortMessage = "Database Exception Against Database: " + database.InstanceName + "\n With inner exception message: " + innerException.Message;
        }

        protected DatabaseExecutionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.database = (Database)info.GetValue("Database", typeof(Database));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Database", this.database);
            base.GetObjectData(info, context);
        }
    }
}