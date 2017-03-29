using Dorado.DataExpress.Dialect;
using Dorado.DataExpress.Driver;
using Dorado.DataExpress.Schema;
using System.Threading;

namespace Dorado.DataExpress
{
    public abstract class BaseQuery
    {
        protected int ParamCount;

        public BaseQuery Parent
        {
            get;
            set;
        }

        protected string InitialeSql
        {
            get;
            set;
        }

        protected string FinalizeSql
        {
            get;
            set;
        }

        public SqlStatement Statement
        {
            get;
            set;
        }

        public BaseDialect Dialect
        {
            get
            {
                return this.Statement.Database.Dialect;
            }
        }

        public BaseDriver Driver
        {
            get
            {
                return this.Statement.Database.Driver;
            }
        }

        public TableSchema Table
        {
            get;
            set;
        }

        public string GetParameterName()
        {
            Monitor.Enter(this);
            string result;
            try
            {
                this.ParamCount++;
                if (this.Parent != null)
                {
                    result = this.Parent.GetParameterName() + "_" + this.ParamCount;
                }
                else
                {
                    result = "p" + this.ParamCount;
                }
            }
            finally
            {
                Monitor.Exit(this);
            }
            return result;
        }

        public virtual void SetInitialSql(string sql)
        {
            this.InitialeSql = sql;
        }

        public virtual void SetFinalizeSql(string sql)
        {
            this.FinalizeSql = sql;
        }

        public abstract string GenerateSql();

        public virtual void ProcessParameters()
        {
        }
    }
}