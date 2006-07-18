using System;
using System.Collections;
using System.Data;

using PMS.Collections.Pool;

namespace PMS.Data.Pool
{
    internal sealed class ConnectionPool
    {
        private ManagedObjectPool pool;
        private const Int32 DEFAULT_OPEN = 5;
        private IProvider provider;
        private String sConn;
        private Type type = null;
        private Int32 maxOpen = ConnectionPool.DEFAULT_OPEN;

        public ConnectionPool(Type type, string sConn)
            : this(type, sConn, ConnectionPool.DEFAULT_OPEN)
        {
        }

        public ConnectionPool(Type type, string sConn, int max)
        {
            this.provider = PMS.Data.ProviderFactory.Factory(type);
            this.sConn = sConn;
            this.maxOpen = max;
            this.type = type;
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
                Console.WriteLine("\n\n\nDestroyConnection");
                conn.Close();
                pool.Remove(conn);
                if (pool.Count < maxOpen) {
                    Console.WriteLine("Adding new to replace");
                    pool.Add(CreateConnection());
                }
            }
        }

        public void ReturnConnection(IDbConnection conn)
        {
            pool.Return(conn);
        }

        private IDbConnection CreateConnection()
        {
            IDbConnection connection = provider.GetConnection(sConn);
            //connection.Open();

            return connection;
        }

        public void Open()
        {
            pool = new ManagedObjectPool(maxOpen, "Close");
            for (int i=0; i < maxOpen; i++) {
                pool.Add(CreateConnection());
            }
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
            sConn = null;
        }
    }
}
