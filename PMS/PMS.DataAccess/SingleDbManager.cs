using System;
using System.Data;
using System.Security.Principal;

using PMS.Data.Pool;
using PMS.Metadata;

namespace PMS.DataAccess
{
    internal sealed class SingleDbManager : MarshalByRefObject, IDbManager
    {
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
        public void Start()
        {
            if (isInit)
                Stop();

            Connection conn = RepositoryManager.DefaultConnection;
            pool = new ConnectionPool(conn.Type, conn.Value);
            isInit = pool.Open();
        }

		public void Start(Type type, string connectionString)
		{
			if (isInit)
				Stop();

			pool = new ConnectionPool(type, connectionString);
			isInit = pool.Open();
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
        #endregion

        public bool IsInitialized {
            get { return isInit; }
        }
    }
}
