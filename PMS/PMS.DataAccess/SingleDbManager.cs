using System;
using System.Data;

using PMS.Data.Pool;
using PMS.Metadata;

namespace PMS.DataAccess
{
    internal sealed class SingleDbManager : MarshalByRefObject, IDbManager
    {
        private IDbConnection connection = null;
        private IDbTransaction trans = null;
        private IConnectionPool pool = null;
        private bool isInit = false;

        private static IDbManager _instance = null;

        public bool InTransaction = false;

        /// <summary>
        /// Return singleton instance of SingleDbManager
        /// </summary>
        public static IDbManager Instance {
            get {
                if (_instance == null) {
                    _instance = new SingleDbManager();
                }

                return _instance;
            }
        }

        private SingleDbManager()
        {
        }

        private IDbConnection GetConnection()
        {
            if (InTransaction == false) {
                if (connection != null)
                    pool.ReturnConnection(connection);

                return (connection = pool.GetConnection());
            }

            return trans.Connection;
        }

        #region Transactions
        public bool BeginTransaction()
        {
            if (trans != null)
                return false;

            if (connection == null)
                connection = pool.GetConnection();

            trans = connection.BeginTransaction();


            return (InTransaction = true);
        }

        public bool RollbackTransaction()
        {
            if (trans == null || InTransaction == false)
                return false;

            try {
                trans.Rollback();
            } catch (Exception) {
                return false;
            }

            return true;
        }

        public bool CommitTransaction()
        {
            if (trans == null || InTransaction == false)
                return false;

            try {
                trans.Commit();
            } catch (Exception) {
                return false;
            }

            return true;
        } 
        #endregion

        #region GetCommand
        /// <summary>
        /// Retrieve IDbCommand at from pool based on AccessMode (Read|Write)
        /// and sets the IDbCommand.CommandText = sql
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="mode">AccessMode</param>
        /// <returns>IDbCommand instance</returns>
        public IDbCommand GetCommand(string sql, AccessMode mode)
        {
            IDbCommand cmd = GetConnection().CreateCommand();
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
            return GetConnection().CreateCommand();
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

            //connection = pool.GetConnection();

            isInit = true;
        }

        /// <summary>
        /// Closes the database pool if it has been Start()ed
        /// </summary>
        public void Stop()
        {
            if (connection != null) {
                pool.ReturnConnection(connection);
            }

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
