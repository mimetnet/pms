using System;
using System.Data;
using System.ComponentModel;
using System.Threading;

namespace PMS.Data
{
    public sealed class DbConnectionProxy : Component, IDbConnection
    {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger("PMS.Data.DbConnectionProxy");
        private IDbTransaction trans = null;
		private Semaphore sema = new Semaphore(1, 1);

        internal IDbConnection _Connection = null;

		public const int LockTimeout = 10000;

        internal DbConnectionProxy(IDbConnection connection)
        {
            this._Connection = connection;
        }

        public DbConnectionProxy(Type typeOfConnection)
        {
            this._Connection = (IDbConnection) Activator.CreateInstance(typeOfConnection);
        }

        #region IDbConnection Members

        public string ConnectionString {
            get { return this._Connection.ConnectionString; }
            set { this._Connection.ConnectionString = value; }
        }

        public int ConnectionTimeout {
            get { return this._Connection.ConnectionTimeout; }
        }
        public string Database {
            get { return this._Connection.Database; }
        }

        public ConnectionState State {
            get { return this._Connection.State; }
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return (trans = this._Connection.BeginTransaction(il));
        }

        public IDbTransaction BeginTransaction()
        {
            return (trans = this._Connection.BeginTransaction());
        }

        internal void CommitTransaction()
        {
            if (trans != null) {
                trans.Commit();
				trans = null;
            }
        }

        internal void RollbackTransaction()
        {
            if (trans != null) {
                trans.Rollback();
				trans = null;
            }
        }

        public void ChangeDatabase(string databaseName)
        {
            this._Connection.ChangeDatabase(databaseName);
        }

        public IDbCommand CreateCommand()
        {
            DbCommandProxy proxy = new DbCommandProxy(this);

            if (trans != null) {
                proxy.Transaction = trans;
			}

            return proxy;
        }

        public void Open()
        {
            this._Connection.Open();
        }

        public void Close()
        {
			try {
				this.RollbackTransaction();
			} catch {}

			try {
				this._Connection.Close();
			} catch {}

			//log.Info("Conn.Close");
		}
        #endregion

        #region IDisposable Members
        public new void Dispose()
        {
			try {
				Close();

				if (this._Connection != null)
					this._Connection.Dispose();

				base.Dispose();
			} catch {}
        }
        #endregion

		internal bool AcquireLock()
		{
			//log.Info("Try to get lock");
			if (sema.WaitOne(LockTimeout, false)) {
				//log.Info("Got Lock");
				return true;
			}

			log.WarnFormat("({0}): Failed to obtain lock within {1} seconds", GetHashCode(), LockTimeout / 1000);

			return false;
		}

		internal void ReleaseLock()
		{
			try {
				//log.Info("Try release");
				sema.Release();
				//log.Info("Released");
			} catch (SemaphoreFullException) {
				log.Info("ReleaseLock: SemaphoreFullException");
			}
		}
	}
}
