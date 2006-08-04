using System;
using System.Collections;
using System.Data;
using System.Security.Principal;

using PMS.Collections.Pool;

namespace PMS.Data.Pool
{
    internal sealed class ConnectionPool : ManagedObjectPool
    {
        public const Int32 DEFAULT_MAX = 20;
        public const Int32 DEFAULT_MIN = 2;
        private String sConn;

        #region Constructors

        public ConnectionPool(Type type, string sConn) :
            this(type, sConn, ConnectionPool.DEFAULT_MIN, ConnectionPool.DEFAULT_MAX)
        {
        }

        public ConnectionPool(Type type, string sConn, int min) :
            this(type, sConn, min, DEFAULT_MAX)
        {
        }

        public ConnectionPool(Type type, string sConn, int min, int max) :
            base(type, min, max, "Close")
        {
            this.sConn = sConn;
        }

        ~ConnectionPool()
        {
            Close();
        } 
        #endregion // Constructors

        #region Connection Operations

        public IDbConnection GetConnection()
        {
            IDbConnection conn = null;

            conn = (IDbConnection)this.Borrow();

            switch (conn.State) {
                case ConnectionState.Open:
                    return conn;

                case ConnectionState.Closed:
                    conn.ConnectionString = this.sConn;
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

        public void ReturnConnection(IDbConnection conn)
        {
            this.Return(conn);
        }

        public void DestroyConnection(IDbConnection conn)
        {
            if (conn != null) {
                Console.WriteLine("\n\n\nDestroyConnection");
                conn.Close();
                this.Remove(conn);
                if (this.Count < this.Min) {
                    Console.WriteLine("Adding new to replace");
                    this.Add();
                }
            }
        }

        #endregion

        #region Transactions
        public bool BeginTransaction(IPrincipal principal)
        {
            return true;
        }

        public bool CommitTransaction(IPrincipal principal)
        {
            return true;
        }

        public bool RollbackTransaction(IPrincipal principal)
        {
            return true;
        }
        #endregion
    }
}
