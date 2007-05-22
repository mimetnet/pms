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
		private bool hasLock = false;
		private DateTime lockStamp;
		private int lockCnt = 0;

		public const int LockTimeout = 20000;

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
				lockStamp = DateTime.Now;
				lockCnt++;
				return (hasLock = true);
			}

			log.WarnFormat("DbConnectionProxy: Failed to obtain lock within {0} seconds", LockTimeout / 1000);
			log.WarnFormat("DbConnectionProxy: Connection locked @ " + lockStamp + " : Cnt=" + lockCnt);

			return false;
		}

		internal void ReleaseLock()
		{
			if (hasLock) {
				hasLock = false;
				lockCnt--;
				lockStamp = DateTime.MinValue;
				Monitor.Exit(this._Connection);
			}   
		}   
	}
}
