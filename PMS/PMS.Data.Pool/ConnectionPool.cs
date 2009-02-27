using System;
using System.Collections;
using System.Data;
using System.Security.Principal;
using System.Runtime.CompilerServices;

using PMS.Collections.Pool;
using PMS.Data;

namespace PMS.Data.Pool
{
    internal sealed class ConnectionPool : ManagedObjectPool<IDbConnection>
    {
        private String sConn;
		//private IProvider provider;

		/* {{{ Constructors */
        public ConnectionPool(IProvider provider, string sConn) : base(provider.Type, 0, 1000, "Close")
        {
            this.sConn = sConn;
        }

        public ConnectionPool(IProvider provider, string sConn, int minimum, int maximum) : base(provider.Type, minimum, maximum, "Close")
        {
            this.sConn = sConn;
        }

        ~ConnectionPool()
        {
            Close();
        } 
		/*}}}*/

		/* IDbConnection Methods {{{ */
        public DbConnectionProxy GetConnection()
        {
            DbConnectionProxy conn = (DbConnectionProxy)this.Borrow();
            conn.Pool = this;

			switch (conn.State) {
				case ConnectionState.Open:
					return conn;

				case ConnectionState.Closed:
					conn.ConnectionString = this.sConn;
					conn.Open();
					return conn;

				case ConnectionState.Broken:
					conn.Close(); conn.Open();
					return conn;

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
		/* }}} */

		/* {{{ Transaction Methods */
        public void BeginTransaction()
        {
   			DbConnectionProxy conn = this.GetConnection();

			try {
				conn.BeginTransaction();
			} finally {
				ReturnConnection(conn);
			}
        }

        public void CommitTransaction()
        {
   			DbConnectionProxy conn = this.GetConnection();

			try {
				conn.CommitTransaction();
			} finally {
				ReturnConnection(conn);
			}
		}

		public void RollbackTransaction()
		{
			DbConnectionProxy conn = this.GetConnection();

			try {
				conn.RollbackTransaction();
			} finally {
				ReturnConnection(conn);
			}
		}
		/*}}}*/
	}
}
// vim:foldmethod=marker:foldlevel=0:
