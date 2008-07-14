using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Threading;

namespace PMS.Data
{
    public class DbConnectionProxy : Component, IDbConnection
    {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger("PMS.Data.DbConnectionProxy");
		private Semaphore sema = new Semaphore(1, 1);
        protected IDbTransaction transaction = null;

        protected IDbConnection connection = null;

		public const int LockTimeout = 10000;

		/* {{{ Constructors */
        public DbConnectionProxy(IDbConnection connection)
        {
            this.connection = connection;
        }

        public DbConnectionProxy(Type typeOfConnection)
        {
            this.connection = (IDbConnection) Activator.CreateInstance(typeOfConnection);
        }
		/*}}}*/

        /* {{{ IDbConnection Members */
        public string ConnectionString {
            get { return this.connection.ConnectionString; }
            set { this.connection.ConnectionString = value; }
        }

        public int ConnectionTimeout {
            get { return this.connection.ConnectionTimeout; }
        }
        public string Database {
            get { return this.connection.Database; }
        }

        public ConnectionState State {
            get { return this.connection.State; }
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return (transaction = this.connection.BeginTransaction(il));
        }

        public IDbTransaction BeginTransaction()
        {
            return (transaction = this.connection.BeginTransaction());
        }

        internal void CommitTransaction()
        {
            if (transaction != null) {
                transaction.Commit();
				transaction = null;
            }
        }

        internal void RollbackTransaction()
        {
            if (transaction != null) {
                transaction.Rollback();
				transaction = null;
            }
        }

        public void ChangeDatabase(string databaseName)
        {
            this.connection.ChangeDatabase(databaseName);
        }

		internal IDbCommand CreateRealCommand()
		{
			return this.connection.CreateCommand();
		}

        public IDbCommand CreateCommand()
        {
            DbCommandProxy proxy = new DbCommandProxy(this);

            if (transaction != null) {
                proxy.Transaction = transaction;
			}

            return proxy;
        }

        public void Open()
        {
            this.connection.Open();
        }

        public void Close()
        {
			try {
				this.RollbackTransaction();
			} catch {}

			try {
				this.connection.Close();
			} catch {}

			//log.Info("Conn.Close");
		}
		/*}}}*/

        public new void Dispose()
        {
			try {
				this.Close();
			} finally {
				try {
					if (this.connection != null)
						this.connection.Dispose();
				} finally {
					base.Dispose();
				}
			}
        }

		/* {{{ Lock Management */
		public bool AcquireLock()
		{
			//log.Info("Try to get lock");
			if (sema.WaitOne(LockTimeout, false)) {
				//log.Info("Got Lock");
				return true;
			}

			log.WarnFormat("({0}): Failed to obtain lock within {1} seconds", GetHashCode(), LockTimeout / 1000);

			return false;
		}

		public void ReleaseLock()
		{
			try {
				//log.Info("Try release");
				sema.Release();
				//log.Info("Released");
			} catch (SemaphoreFullException) {
				log.Info("ReleaseLock: SemaphoreFullException");
			}
		}
		/*}}}*/

		public virtual bool CanReopen(Exception ex)
		{
			if (ex != null)
				return false;

			if (ex.GetType() == typeof(IOException) || (ex.InnerException != null && ex.InnerException.GetType() == typeof(IOException)))
				return this.Reopen();

			if (ex.InnerException == null)
				return false;

			return false;
		}

		public virtual bool Reopen()
		{
			try {
				this.Close();
			} catch (Exception) {
				log.Error("CanReopenConnection: Failed to Close in order to reopen it");
			}

			try {
				this.Open();
			} catch (Exception) {
				log.Error("CanReopenConnection: Failed to Open connection, it appears dead!");
				return false;
			}

			log.Info("Connection was lost but is once again found");

			return true;
		}
	}
}
// vim:foldmethod=marker:foldlevel=0:
