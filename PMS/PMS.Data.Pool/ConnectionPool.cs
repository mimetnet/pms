using System;
using System.Collections;
using System.Data;
using System.Security.Principal;
using System.Runtime.CompilerServices;

using PMS.Collections.Pool;
using PMS.Data;

namespace PMS.Data.Pool
{
    internal sealed class ConnectionPool : PrincipalObjectPool
    {
        private String sConn;
		//private IProvider provider;

		/* {{{ Constructors */
        public ConnectionPool(IProvider provider, string sConn) : base(provider.Type, "Close")
        {
            this.sConn = sConn;
			//this.provider = provider;
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
	
		/* ObjectPool Overrides {{{ */
		/*
		[MethodImpl(MethodImplOptions.Synchronized)]
		protected override long Add()
		{
			Item obj = null;

			lock (ilock) {
				pool.Add((obj = new Item(this.provider.GetConnection())));
			}

			if (log.IsDebugEnabled)
				log.DebugFormat("ObjectPool.Add(new {0}())", obj.Object.GetType());

			return pool.Count;
		}
		*/

		/*
		[MethodImpl(MethodImplOptions.Synchronized)]
		public override void CleanObject(ref Object obj)
		{
			try {
				if (cleanup != null) {
					cleanup.Invoke(obj, null);

					if (log.IsDebugEnabled) {
						log.DebugFormat("{0}.{1}(OID={2})", type, cleanup, obj.GetHashCode());
					}
				}
			} finally {
				obj = null;
			}
		}
		*/
		/* }}} */
	}
}
// vim:foldmethod=marker:foldlevel=0:
