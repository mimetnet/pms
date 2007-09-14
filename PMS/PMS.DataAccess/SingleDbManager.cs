using System;
using System.Data;
using System.Security.Principal;

using PMS.Data.Pool;
using PMS.Metadata;

namespace PMS.DataAccess
{
    internal sealed class SingleDbManager : MarshalByRefObject, IDbManager
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ConnectionPool pool = null;
        private bool isInit = false;

        public SingleDbManager()
        {
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
        /// <summary>
        /// Retrieve IDbCommand at from pool based on AccessMode (Read|Write)
        /// and sets the IDbCommand.CommandText = sql
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <returns>IDbCommand instance</returns>
        public IDbCommand GetCommand(string sql)
        {
            IDbCommand cmd = this.pool.GetConnection().CreateCommand();
            cmd.CommandText = sql;

            return cmd;
        }

        public void ReturnCommand(IDbCommand command)
        {
            pool.ReturnConnection(command.Connection);
        }
        #endregion

        #region Control
        /// <summary>
        /// Pull default connection information from repository and initialize
        /// the database pool.
        /// </summary>
        public bool Start()
        {
            if (isInit)
                Stop();

            Connection conn = RepositoryManager.DefaultConnection;
            pool = new ConnectionPool(conn.Provider.Type, conn.Value);

            if ((isInit = pool.Open()))
				return CanSendHello();

			return false;
        }

        /// <summary>
        /// Closes the database pool if it has been Start()ed
        /// </summary>
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
