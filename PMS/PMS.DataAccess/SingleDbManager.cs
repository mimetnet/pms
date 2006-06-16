using System;
using System.Data;

using PMS.Data.Pool;
using PMS.Metadata;

namespace PMS.DataAccess
{
    internal sealed class SingleDbManager : IDbManager
    {
        private IDbConnection connection = null;
        private ConnectionPool pool = null;
        private bool isInit = false;

        private static IDbManager _instance = null;

        public bool InTransaction = false;

        public static IDbManager Instance {
            get {
                if (_instance == null) {
                    _instance = new SingleDbManager();
                }

                return _instance;
            }
        }

        public IDbCommand GetCommand(string sql, AccessMode mode)
        {
            IDbCommand cmd = CurrentConnection.CreateCommand();
            cmd.CommandText = sql;

            return cmd;
        }

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
                    return connection;
                }
            }
        }

        public void Start()
        {
            if (isInit)
                Stop();

            Connection conn = RepositoryManager.DefaultConnection;
            pool = new ConnectionPool(conn.Type, conn.String, conn.PoolSize);
            pool.Open();

            isInit = true;

            connection = pool.GetConnection();
        }

        public void Stop()
        {
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
