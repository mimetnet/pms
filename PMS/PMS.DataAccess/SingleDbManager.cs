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
        public bool BeginTransaction(IPrincipal principal)
        {
            return pool.BeginTransaction(principal);
        }

        public bool CommitTransaction(IPrincipal principal)
        {
            return pool.CommitTransaction(principal);
        }

        public bool RollbackTransaction(IPrincipal principal)
        {
            return pool.RollbackTransaction(principal);
        }
        #endregion

        #region IDbCommand Methods
        /// <summary>
        /// Retrieve IDbCommand at from pool based on AccessMode (Read|Write)
        /// and sets the IDbCommand.CommandText = sql
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="mode">AccessMode</param>
        /// <returns>IDbCommand instance</returns>
        public IDbCommand GetCommand(string sql, AccessMode mode)
        {
            IDbCommand cmd = this.pool.GetConnection().CreateCommand();
            cmd.CommandText = sql;

            return cmd;
        }

        /// <summary>
        /// Retrieve IDbCommand at from pool based on AccessMode (Read|Write)
        /// </summary>
        /// <param name="mode">AccessMode</param>
        /// <returns>IDbCommand instance</returns>
        public IDbCommand GetCommand(AccessMode mode)
        {
            return this.pool.GetConnection().CreateCommand();
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
            pool = new ConnectionPool(conn.Type, conn.Value, conn.PoolSize);
            pool.Open();

            isInit = true;
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
