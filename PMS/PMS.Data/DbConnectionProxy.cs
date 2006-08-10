using System;
using System.Data;
using System.ComponentModel;

namespace PMS.Data
{
    public sealed class DbConnectionProxy : Component, IDbConnection
    {
        IDbConnection conn = null;
        IDbTransaction trans = null;

        internal DbConnectionProxy(IDbConnection connection)
        {
            this.conn = connection;
        }

        public DbConnectionProxy(Type typeOfConnection)
        {
            this.conn = (IDbConnection) Activator.CreateInstance(typeOfConnection);
        }

        internal IDbConnection RealConnection {
            get { return this.conn; }
        }

        #region IDbConnection Members

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return (trans = this.conn.BeginTransaction(il));
        }

        public IDbTransaction BeginTransaction()
        {
            return (trans = this.conn.BeginTransaction());
        }

        internal void CommitTransation()
        {
            if (trans != null) {
                trans.Commit();
            }
        }

        internal void RollbackTransaction()
        {
            if (trans != null) {
                trans.Rollback();
            }
        }

        public void ChangeDatabase(string databaseName)
        {
            this.conn.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            this.conn.Close();
        }

        public string ConnectionString
        {
            get {
                return this.conn.ConnectionString;
            }
            set {
                this.conn.ConnectionString = value;
            }
        }

        public int ConnectionTimeout
        {
            get { return this.conn.ConnectionTimeout; }
        }

        public IDbCommand CreateCommand()
        {
            DbCommandProxy proxy = new DbCommandProxy(this.conn.CreateCommand());
            //proxy.Executed += new DbCommandExecuted(CommandProxyExecuted);
            if (trans != null)
                proxy.Transaction = trans;

            return proxy;
        }

        private void CommandProxyExecuted(IDbConnection connection)
        {
            Console.WriteLine("HOLY SHIT");
        }

        public string Database
        {
            get { return this.conn.Database; }
        }

        public void Open()
        {
            this.conn.Open();
        }

        public ConnectionState State
        {
            get { return this.conn.State; }
        }

        #endregion

        #region IDisposable Members

        public new void Dispose()
        {
            this.conn.Dispose();
        }

        #endregion
    }
}
