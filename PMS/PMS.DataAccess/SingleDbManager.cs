using System;
using System.Data;
using System.Security.Principal;

using PMS.Data;
using PMS.Data.Pool;
using PMS.Metadata;

namespace PMS.DataAccess
{
    internal sealed class SingleDbManager : MarshalByRefObject, IDbManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("PMS.DataAccess.SingleDbManager");
        private ConnectionPool pool = null;
        private bool isInit = false;

        public SingleDbManager()
        {
        }

		~SingleDbManager()
		{
			this.Stop();
		}

        #region Transactions
        public void BeginTransaction()
        {
            pool.BeginTransaction();
        }

        public void CommitTransaction()
        {
            pool.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            pool.RollbackTransaction();
        }
        #endregion

        #region IDbCommand Methods
        public IDbCommand GetCommand(string sql)
        {
            DbCommandProxy cmd = (DbCommandProxy) this.pool.GetConnection().CreateCommand();
			cmd.Manager = this;
            cmd.CommandText = sql;
            return cmd;
        }

		public void ReturnCommand(IDbCommand cmd)
		{
			if (cmd != null) {
				pool.ReturnConnection(cmd.Connection);
			} else {
				log.Warn("ReturnCommand was given a null command >>");
				log.Warn(new System.Diagnostics.StackTrace());
			}
        }
        #endregion

        #region Control
        public bool Start()
        {
            if (isInit)
                Stop();

            Connection conn = RepositoryManager.DefaultConnection;
            pool = new ConnectionPool(conn.Provider, conn.Value);

            if ((isInit = pool.Open()))
				return CanSendHello();

			return false;
        }

        public void Stop()
        {
            if (pool != null)
                pool.Close();

            isInit = false;
        } 

		private bool CanSendHello()
		{
			try {
				pool.ReturnConnection(pool.GetConnection());
			} catch (Exception e) {
				log.Error("CanSendHello: " + e.Message);
				return false;
			}

			return true;
		}
        #endregion

        public bool IsInitialized {
            get { return isInit; }
        }
    }
}
