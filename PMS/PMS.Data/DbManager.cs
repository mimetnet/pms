using System;
using System.Data;
using System.Security.Principal;

using PMS.Data;
using PMS.Data.Pool;
using PMS.Metadata;

namespace PMS.Data
{
    internal sealed class DbManager : PMS.DataAccess.IDbManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("PMS.Data.DbManager");
        private ConnectionPool pool = null;
        private bool isOpen = false;

        public DbManager(IProvider provider, string connectionString)
        {
            this.pool = new ConnectionPool(provider, connectionString);
        }

		~DbManager()
		{
			this.Close();
		}

        public bool IsOpen {
            get { return this.isOpen; }
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

        public IDbConnection GetConnection()
        {
            return this.pool.GetConnection();
        }
        
        public void ReturnConnection(IDbConnection connection)
        {
            this.pool.ReturnConnection(connection);
        }
        #endregion

        #region Control
        public bool Open()
        {
            if (this.pool.Open())
				return this.isOpen = CanSendHello();

            return false;
        }
        public void Close()
        {
            if (pool != null)
                pool.Close();
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
    }
}
