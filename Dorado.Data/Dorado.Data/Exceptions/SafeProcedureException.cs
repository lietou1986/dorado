using System;
using System.Runtime.Serialization;

namespace Dorado.Data.Exceptions
{
    [Serializable]
    public class SafeProcedureException : DatabaseExecutionException
    {
        private Type instanceType;

        public Type InstanceType
        {
            get
            {
                return this.instanceType;
            }
            set
            {
                this.instanceType = value;
            }
        }

        public SafeProcedureException(Database database, string procedureName, Exception innerException)
            : base(database, procedureName, innerException)
        {
        }

        public SafeProcedureException(Database database, string procedureName, Type instanceType, Exception innerException)
            : base(database, procedureName, "Instance type: " + instanceType.ToString(), innerException)
        {
            this.InstanceType = instanceType;
        }

        protected SafeProcedureException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.instanceType = (Type)info.GetValue("InstanceType", typeof(Type));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("InstanceType", this.instanceType);
            base.GetObjectData(info, context);
        }
    }
}