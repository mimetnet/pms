using System;
using System.Collections;
using System.Data;
using System.Security.Principal;

using PMS.Collections.Pool;

namespace PMS.Data.Pool
{
    internal sealed class ConnectionPool : PrincipalObjectPool
    {
        public const Int32 DEFAULT_MAX = 30;
        public const Int32 DEFAULT_MIN = 0;
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
            base(typeof(DbConnectionProxy), new object[]{type}, min, max, "Close")
        {
            this.sConn = sConn;
        }

        ~ConnectionPool()
        {
            Close();
        } 
        #endregion // Constructors

        #region Connection Operations

        public DbConnectionProxy GetConnection()
        {
            DbConnectionProxy conn = null;

            lock (ilock) {
                conn = (DbConnectionProxy)this.Borrow();

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
                lock (ilock) {
                    Console.WriteLine("\n\n\nDestroyConnection");
                    conn.Close();
                    this.Remove(conn);
                    if (this.Count < this.Min) {
                        Console.WriteLine("Adding new to replace");
                        this.Add();
                    }
                }
            }
        }

        #endregion

        #region Transactions
        public void BeginTransaction()
        {
            this.GetConnection().BeginTransaction();
        }

        public void CommitTransaction()
        {
            this.GetConnection().CommitTransation();
        }

        public void RollbackTransaction()
        {
            this.GetConnection().RollbackTransaction();
        }
        #endregion
    }
}
