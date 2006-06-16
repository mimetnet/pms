using System;
using System.Collections;
using System.Data;

using PMS.Collections.Pool;

namespace PMS.Data.Pool
{
    public class ConnectionPool
    {
        private ManagedObjectPool pool;

        private IProvider provider;
        private string connString;
        private int defaultMaxConns = 5;

        public ConnectionPool(Type type, string connectionString)
        {
            provider = PMS.Data.ProviderFactory.Factory(type);
            connString = connectionString;
        }

        public ConnectionPool(Type type, string sConn, int max)
        {
            provider = PMS.Data.ProviderFactory.Factory(type);
            connString = sConn;
            defaultMaxConns = max;           
        }

        public IDbConnection GetConnection()
        {
            IDbConnection conn = (IDbConnection) pool.Borrow();
            
            switch (conn.State) {
            case ConnectionState.Open:
                return conn;

            case ConnectionState.Closed:
                conn.Open();
                return conn;

            case ConnectionState.Broken:
                DestroyConnection(conn);
                break;

            default:
                ReturnConnection(conn);
                break;
            }
            
            return GetConnection();
        }

        public void DestroyConnection(IDbConnection conn)
        {
            if (conn != null) {
                conn.Close();
                if (pool.Count < defaultMaxConns)
                    pool.Remove(CreateConnection());
            }
        }

        public void ReturnConnection(IDbConnection conn)
        {
            try {
                pool.Return(conn);
            } catch (Exception e) {
				Console.WriteLine(e);
            }
        }

        private IDbConnection CreateConnection()
        {
            IDbConnection connection = provider.GetConnection(connString);
            connection.Open();

            return connection;
        }

        public void Open()
        {
            int i;

            pool = new ManagedObjectPool(defaultMaxConns);

            for (i=0; i < defaultMaxConns; i++)
                pool.Add(CreateConnection());
        }

        public void Close()
        {
            if (pool != null)
                pool.Close();
        }

        ~ConnectionPool()
        {
            Close();
            pool = null;
            connString = null;
        }
    }
}
