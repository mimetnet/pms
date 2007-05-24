using System;
using System.Data;
using System.ComponentModel;
using System.Threading;

namespace PMS.Data
{
    public sealed class DbConnectionProxy : Component, IDbConnection
    {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger("PMS.Data.DbConnectionProxy");
        internal IDbConnection _Connection = null;
        private IDbTransaction trans = null;
		private Thread hasLock = null;
		private Object myLock = new Object();

		public const int LockTimeout = 30000;

        internal DbConnectionProxy(IDbConnection connection)
        {
            this._Connection = connection;
        }

        public DbConnectionProxy(Type typeOfConnection)
        {
            this._Connection = (IDbConnection) Activator.CreateInstance(typeOfConnection);
        }

        internal IDbConnection RealConnection {
            get { return this._Connection; }
        }

        #region IDbConnection Members

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return (trans = this._Connection.BeginTransaction(il));
        }

        public IDbTransaction BeginTransaction()
        {
            return (trans = this._Connection.BeginTransaction());
        }

        internal void CommitTransation()
        {
            if (trans != null) {
                trans.Commit();
            }
        }

        internal void RollbackTransaction()
        {
            if (trans != null) {
                trans.Rollback();
            }
        }

        public void ChangeDatabase(string databaseName)
        {
            this._Connection.ChangeDatabase(databaseName);
        }

        public string ConnectionString {
            get { return this._Connection.ConnectionString; }
            set { this._Connection.ConnectionString = value; }
        }

        public int ConnectionTimeout {
            get { return this._Connection.ConnectionTimeout; }
        }

        public IDbCommand CreateCommand()
        {
            DbCommandProxy proxy = new DbCommandProxy(this);
            if (trans != null) {
                proxy.Transaction = trans;
			}

            return proxy;
        }

        public string Database {
            get { return this._Connection.Database; }
        }

        public void Open()
        {
            this._Connection.Open();
        }

        public void Close()
        {
			try {
				this._Connection.Close();
			} finally {
				ReleaseLock();
			}
		}

        public ConnectionState State
        {
            get { return this._Connection.State; }
        }
        #endregion

        #region IDisposable Members
        public new void Dispose()
        {
			try {
				this._Connection.Dispose();
			} finally {
				ReleaseLock();
			}
        }
        #endregion

		internal bool AcquireLock()
		{
			if (Monitor.TryEnter(this._Connection, LockTimeout)) {
				lock (myLock) {
					hasLock = Thread.CurrentThread;
				}

				//log.InfoFormat("({0}): AcquireLock", GetHashCode());
				return true;
			}

			log.WarnFormat("({0}): Failed to obtain lock within {1} seconds", GetHashCode(), LockTimeout / 1000);

			return false;
		}

		internal void ReleaseLock()
		{
			lock (myLock) {
				if (hasLock != Thread.CurrentThread) return;
				Monitor.Exit(this._Connection);
				//log.InfoFormat("({0}): ReleaseLock", GetHashCode());
				hasLock = null;
			}
		}
	}
}
