using System;
using System.Data;
using System.ComponentModel;

namespace PMS.Data
{
    public delegate void DbCommandExecuted(IDbConnection command);

    public sealed class DbCommandProxy : Component, IDbCommand
    {
        object hold = new object();
        IDbCommand cmd = null;
        internal event DbCommandExecuted Executed;

        internal DbCommandProxy(IDbCommand command)
        {
            this.cmd = command;
        }

        #region IDbCommand Members

        public void Cancel()
        {
            this.cmd.Cancel();
        }

        public string CommandText
        {
            get {
                return this.cmd.CommandText;
            }
            set {
                this.cmd.CommandText = value;
            }
        }

        public int CommandTimeout
        {
            get {
                return this.cmd.CommandTimeout;
            }
            set {
                this.cmd.CommandTimeout = value;
            }
        }

        public CommandType CommandType
        {
            get {
                return this.cmd.CommandType;
            }
            set  {
                this.cmd.CommandType = value;
            }
        }

        public IDbConnection Connection
        {
            get {
                if (this.cmd.Connection is DbConnectionProxy)
                    return this.cmd.Connection;

                return new DbConnectionProxy(this.cmd.Connection);
            }
            set {
                this.cmd.Connection = value;
            }
        }

        public IDbDataParameter CreateParameter()
        {
            return this.cmd.CreateParameter();
        }

        public int ExecuteNonQuery()
        {
            if (this.Executed != null)
                this.Executed(this.cmd.Connection);

            return this.cmd.ExecuteNonQuery();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            IDataReader reader = null;

            if (this.Executed != null)
                this.Executed(this.cmd.Connection);

            lock (hold) {
                reader = this.cmd.ExecuteReader(behavior);
            }

            return reader;
        }

        public IDataReader ExecuteReader()
        {
            IDataReader reader = null;

            if (this.Executed != null)
                this.Executed(this.cmd.Connection);

            lock (hold) {
                reader = this.cmd.ExecuteReader();
            }

            return reader;
        }

        public object ExecuteScalar()
        {
            Object obj = null;

            if (this.Executed != null)
                this.Executed(this.cmd.Connection);

            lock (hold) {
                obj = this.cmd.ExecuteScalar();
            }

            return obj;
        }

        public IDataParameterCollection Parameters
        {
            get { return this.cmd.Parameters; }
        }

        public void Prepare()
        {
            this.cmd.Prepare();
        }

        public IDbTransaction Transaction
        {
            get {
                return this.cmd.Transaction;
            }
            set {
                lock (hold) {
                    this.cmd.Transaction = value;
                }
            }
        }

        public UpdateRowSource UpdatedRowSource
        {
            get {
                return this.cmd.UpdatedRowSource;
            }
            set {
                this.cmd.UpdatedRowSource = value;
            }
        }

        #endregion

        #region IDisposable Members

        public new void Dispose()
        {
            this.cmd.Dispose();
        }

        #endregion
    }
}
