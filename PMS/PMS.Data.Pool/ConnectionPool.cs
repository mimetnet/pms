using System;
using System.Collections;
using System.Data;

using PMS.Collections.Pool;

namespace PMS.Data.Pool
{
    internal sealed class ConnectionPool
    {
        private ManagedObjectPool pool;
        private const Int32 DEFAULT_NUM = 5;
        private IProvider provider;
        private String connString;
        private Type type = null;
        private Int32 defaultMaxConns = ConnectionPool.DEFAULT_NUM;

        public ConnectionPool(Type type, string sConn)
            : this(type, sConn, ConnectionPool.DEFAULT_NUM)
        {
        }

        public ConnectionPool(Type type, string sConn, int max)
        {
            this.provider = PMS.Data.ProviderFactory.Factory(type);
            this.connString = sConn;
            this.defaultMaxConns = max;
            this.type = type;

            //Console.WriteLine("ConnectionPool({0}, '*****', {1})", type, max);
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
            } catch (Exception) {
				//Console.WriteLine(e);
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
            pool = new ManagedObjectPool(defaultMaxConns, "Close");
            //Console.WriteLine("ConnectionPool.Open()");
            for (int i=0; i < defaultMaxConns; i++) {
                pool.Add(CreateConnection());
            }
        }

        public void Close()
        {
            //Console.WriteLine("ConnectionPool.Close()");
            if (pool != null) pool.Close();
        }

        ~ConnectionPool()
        {
            Close();
            pool = null;
            connString = null;
        }
    }
}
