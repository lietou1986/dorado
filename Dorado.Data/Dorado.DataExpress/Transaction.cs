using System;
using System.Data.Common;

namespace Dorado.DataExpress
{
    public class Transaction : IDisposable
    {
        private TransactionState _state;
        private bool _isDisposing;

        public Database Database
        {
            get;
            set;
        }

        public DbTransaction InnerTransaction
        {
            get;
            set;
        }

        public TransactionState State
        {
            get
            {
                return this._state;
            }
            set
            {
                this._state = value;
            }
        }

        public Transaction()
        {
            this.InnerTransaction = null;
            this.Database = null;
        }

        public Transaction(Database database)
        {
            this.Database = database;
            this.InnerTransaction = database.Connection.BeginTransaction();
        }

        private void AssertDb()
        {
            if (this.Database == null || this.InnerTransaction == null)
            {
                throw new NullReferenceException("数据库或日志为空");
            }
        }

        public void Commit()
        {
            this.AssertDb();
            if (this.Database.Closed)
            {
                return;
            }
            this.Database.UpdateIdle();
            this.State = TransactionState.Error;
            this.Database.ActiveTransactions.Remove(this);
            this.InnerTransaction.Commit();
            this.State = TransactionState.Commited;
        }

        public void Rollback()
        {
            this.AssertDb();
            if (this.Database.Closed)
            {
                return;
            }
            this.Database.UpdateIdle();
            this.State = TransactionState.Error;
            this.Database.ActiveTransactions.Remove(this);
            this.InnerTransaction.Rollback();
            this.State = TransactionState.Rollbacked;
        }

        ~Transaction()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (this._isDisposing)
            {
                return;
            }
            this._isDisposing = true;
            if (this.Database != null && this.InnerTransaction != null && this.Database.ActiveTransactions.Contains(this))
            {
                this.Database.ActiveTransactions.Remove(this);
                if (this.State == TransactionState.InUsing)
                {
                    this.Rollback();
                }
            }
            if (this.InnerTransaction != null)
            {
                this.InnerTransaction.Dispose();
            }
        }
    }
}