using System;
using System.Data;
using System.ComponentModel;

namespace PMS.Data
{
    public sealed class DbCommandProxy : Component, IDbCommand
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("PMS.Data.DbCommandProxy");
		private DbConnectionProxy conn = null;
        private IDbCommand cmd = null;

        internal DbCommandProxy(DbConnectionProxy conn)
        {
			this.conn = conn;
            this.cmd = this.conn._Connection.CreateCommand();
        }

        #region IDbCommand Members

        public string CommandText {
            get { return this.cmd.CommandText; }
            set { this.cmd.CommandText = value; }
        }

        public int CommandTimeout {
            get { return this.cmd.CommandTimeout; }
            set { this.cmd.CommandTimeout = value; }
        }

        public CommandType CommandType {
            get { return this.cmd.CommandType; }
            set  { this.cmd.CommandType = value; }
        }

        public IDbConnection Connection {
            get { return this.conn; }
            set { throw new NotImplementedException("Assignment of DbCommandProxy.Connection is prohibted"); }
        }

        public IDbTransaction Transaction {
            get { return this.cmd.Transaction; }
            set { this.cmd.Transaction = value; }
        }

        public UpdateRowSource UpdatedRowSource {
            get { return this.cmd.UpdatedRowSource; }
            set { this.cmd.UpdatedRowSource = value; }
        }
        public IDataParameterCollection Parameters {
            get { return this.cmd.Parameters; }
        }

        public IDbDataParameter CreateParameter()
        {
            return this.cmd.CreateParameter();
        }

        public int ExecuteNonQuery()
        {
			int x = 0;

			if (!this.conn.AcquireLock())
				throw new ApplicationException("ExecuteNonQuery: Failed to lock connection");

			try {
				x = this.cmd.ExecuteNonQuery();
			} catch (NotSupportedException) {
				if (this.CanReopenConnection())
					x = this.cmd.ExecuteNonQuery();
			} finally {
				this.conn.ReleaseLock();
			}

			return x;
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            IDataReader reader = null;

			if (!this.conn.AcquireLock())
				throw new ApplicationException("ExecuteReader(CommandBehavior): Failed to lock connection");

			try {
				reader = this.cmd.ExecuteReader(behavior);
			} catch (NotSupportedException) {
				if (this.CanReopenConnection())
					reader = this.cmd.ExecuteReader();
			}

            return new DbDataReaderProxy(reader, this.conn);
        }

        public IDataReader ExecuteReader()
        {
            IDataReader reader = null;

			if (!this.conn.AcquireLock())
				throw new ApplicationException("ExecuteReader: Failed to lock connection");

			try {
				reader = this.cmd.ExecuteReader();
			} catch (NotSupportedException) {
				if (this.CanReopenConnection())
					reader = this.cmd.ExecuteReader();
			}

            return new DbDataReaderProxy(reader, this.conn);
        }

		public object ExecuteScalar()
        {
            Object obj = null;

			if (!this.conn.AcquireLock())
				throw new ApplicationException("ExecuteScalar: Failed to lock connection");

			try {
				obj = this.cmd.ExecuteScalar();
			} catch (NotSupportedException) {
				if (this.CanReopenConnection())
					obj = this.cmd.ExecuteScalar();
			} finally {
				this.conn.ReleaseLock();
			}

            return obj;
        }

        public void Prepare()
        {
            this.cmd.Prepare();
        }


        public void Cancel()
        {
            this.cmd.Cancel();
        }

        #endregion

        #region IDisposable Members

        public new void Dispose()
        {
            this.cmd.Dispose();
        }

        #endregion

		private bool CanReopenConnection()
		{
			try {
				this.cmd.Connection.Close();
			} catch (Exception) {
				log.Error("CanReopenConnection: Failed to Close in order to reopen it");
				return false;
			}

			try {
				this.cmd.Connection.Open();
			} catch (Exception) {
				log.Error("CanReopenConnection: Failed to Open connection, it appears dead!");
				return false;
			}

			log.Info("Connection was lost but is once again found");

			return true;
		}
	}
}
