using Dorado.DataExpress.Ldo;
using System;

namespace Dorado.DataExpress
{
    public class DataAccessBase : IDisposable, IDataAccess
    {
        private Database _dataContext;
        private bool _isDisposing;

        public Database DataContext
        {
            get
            {
                this._dataContext.UpdateIdle();
                return this._dataContext;
            }
            protected set
            {
                this._dataContext = value;
            }
        }

        public DataAccessBase()
        {
            this._dataContext = this.GetDataContext();
        }

        public DataAccessBase(string dbName)
        {
            this._dataContext = this.GetDataContext(dbName);
        }

        public LdoEntityInfo Entity<T>() where T : new()
        {
            return BinderManager<T>.Binder.EntifyInfo;
        }

        public string Field<T>(string propertyName) where T : new()
        {
            return this.Entity<T>()[propertyName];
        }

        ~DataAccessBase()
        {
            this.Dispose();
        }

        public virtual void Dispose()
        {
            if (this._isDisposing)
            {
                return;
            }
            this._isDisposing = true;
            if (this._dataContext != null)
            {
                this._dataContext.Close();
            }
        }
    }
}