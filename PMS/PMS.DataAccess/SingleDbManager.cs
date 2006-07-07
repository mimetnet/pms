using System;
using System.Data;

using PMS.Data.Pool;
using PMS.Metadata;

namespace PMS.DataAccess
{
    internal sealed class SingleDbManager : IDbManager
    {
        private IDbConnection connection = null;
        private IDbTransaction trans = null;
        private ConnectionPool pool = null;
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

        /// <summary>
        /// Retrieve IDbCommand at from pool based on AccessMode (Read|Write)
        /// and sets the IDbCommand.CommandText = sql
        /// </summary>
        /// <param name="sql">SQL</param>
        /// <param name="mode">AccessMode</param>
        /// <returns>IDbCommand instance</returns>
        public IDbCommand GetCommand(string sql, AccessMode mode)
        {
            IDbCommand cmd = CurrentConnection.CreateCommand();
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
            return CurrentConnection.CreateCommand();
        }
        
        private IDbConnection CurrentConnection {
            get {
                if (InTransaction == false) {
                    pool.ReturnConnection(connection);

                    return (connection = pool.GetConnection());
                } else {
                    return trans.Connection;
                }
            }
        }

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

        /// <summary>
        /// Pull default connection information from repository and initialize
        /// the database pool.
        /// </summary>
        public void Start()
        {
            //Console.WriteLine("SingleDbManager.Start()");

            if (isInit)
                Stop();

            Connection conn = RepositoryManager.DefaultConnection;
            pool = new ConnectionPool(conn.Type, conn.Value, conn.PoolSize);
            pool.Open();

            isInit = true;

            connection = pool.GetConnection();
        }

        /// <summary>
        /// Closes the database pool if it has been Start()ed
        /// </summary>
        public void Stop()
        {
            //Console.WriteLine("SingleDbManager.Stop()");

            if (connection != null) {
                pool.ReturnConnection(connection);
            }
            
            if (pool != null)
                pool.Close();

            isInit = false;
        }

        public bool IsInitialized {
            get { return isInit; }
        }
    }
}
