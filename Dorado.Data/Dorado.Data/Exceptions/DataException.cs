using System;
using System.Runtime.Serialization;

namespace Dorado.Data.Exceptions
{
    [Serializable]
    public class DataException : Exception
    {
        protected string instanceName;
        protected string procedureName;
        protected string shortMessage;
        protected Exception innerException;

        public string DBInstanceName
        {
            get
            {
                return this.instanceName;
            }
            set
            {
                this.instanceName = value;
            }
        }

        public string ProcedureName
        {
            get
            {
                return this.procedureName;
            }
            set
            {
                this.procedureName = value;
            }
        }

        public string ShortMessage
        {
            get
            {
                return this.shortMessage;
            }
            set
            {
                this.shortMessage = value;
            }
        }

        public new Exception InnerException
        {
            get
            {
                return this.innerException;
            }
            set
            {
                this.innerException = value;
            }
        }

        internal DataException(string message)
            : base(message)
        {
        }

        internal DataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DataException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.instanceName = info.GetString("DBInstanceName");
            this.procedureName = info.GetString("ProcedureName");
            this.shortMessage = info.GetString("ShortMessage");
            this.innerException = (Exception)info.GetValue("_InnerException", typeof(Exception));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("DBInstanceName", this.instanceName);
            info.AddValue("ProcedureName", this.procedureName);
            info.AddValue("ShortMessage", this.shortMessage);
            info.AddValue("_InnerException", this.innerException);
            base.GetObjectData(info, context);
        }
    }
}