using System;
using System.Data;
using System.ComponentModel;

using PMS.DataAccess;

namespace PMS.Data
{
    public class DbCommandProxy : Component, IDbCommand
    {
        //private static readonly log4net.ILog log = log4net.LogManager.GetLogger("PMS.Data.DbCommandProxy");
		protected DbConnectionProxy connection = null;
        protected IDbCommand command = null;
		private IDbManager mgr = null;

        public DbCommandProxy(DbConnectionProxy conn)
        {
			this.connection = conn;
            this.command = this.connection.CreateRealCommand();
        }

		internal IDbManager Manager {
			get { return mgr; }
			set { mgr = value; }
		}

        #region IDbCommand Members

        public string CommandText {
            get { return this.command.CommandText; }
            set { this.command.CommandText = value; }
        }

        public int CommandTimeout {
            get { return this.command.CommandTimeout; }
            set { this.command.CommandTimeout = value; }
        }

        public CommandType CommandType {
            get { return this.command.CommandType; }
            set  { this.command.CommandType = value; }
        }

        public IDbConnection Connection {
            get { return this.connection; }
            set { throw new NotImplementedException("Assignment of DbCommandProxy.Connection is prohibted"); }
        }

        public IDbTransaction Transaction {
            get { return this.command.Transaction; }
            set { this.command.Transaction = value; }
        }

        public UpdateRowSource UpdatedRowSource {
            get { return this.command.UpdatedRowSource; }
            set { this.command.UpdatedRowSource = value; }
        }
        public IDataParameterCollection Parameters {
            get { return this.command.Parameters; }
        }

        public IDbDataParameter CreateParameter()
        {
            return this.command.CreateParameter();
        }

        public int ExecuteNonQuery()
        {
			int x = 0;

			if (!this.connection.AcquireLock())
				throw new ApplicationException("ExecuteNonQuery: Failed to lock connection");

			try {
				x = this.command.ExecuteNonQuery();
			} catch (Exception e) {
				if (this.CanReopenConnection(e))
					x = this.command.ExecuteNonQuery();
			} finally {
				this.connection.ReleaseLock();
                Console.WriteLine("ExecuteNonQuery("+this.command.Parameters.Count+"): " + this.command.CommandText);
			}

			return x;
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            IDataReader reader = null;

			if (!this.connection.AcquireLock())
				throw new ApplicationException("ExecuteReader(CommandBehavior): Failed to lock connection");

			try {
				reader = this.command.ExecuteReader();
			} catch (Exception e) {
				if (this.CanReopenConnection(e)) {
					reader = this.command.ExecuteReader();
				} else {
					throw e;
				}
			} finally {
				if (reader == null) {
					this.connection.ReleaseLock();
					this.connection = null;
				}
                Console.WriteLine("ExecuteReader("+this.command.Parameters.Count+"): " + this.command.CommandText);
			}
			
            return new DbDataReaderProxy(reader, this.connection);
        }

        public IDataReader ExecuteReader()
        {
            IDataReader reader = null;

			if (!this.connection.AcquireLock())
				throw new ApplicationException("ExecuteReader: Failed to lock connection");

			try {
				reader = this.command.ExecuteReader();
			} catch (Exception e) {
				if (this.CanReopenConnection(e)) {
					reader = this.command.ExecuteReader();
				} else {
					throw e;
				}
			} finally {
				if (reader == null) {
					this.connection.ReleaseLock();
					this.connection = null;
				}
                Console.WriteLine("ExecuteReader("+this.command.Parameters.Count+"): " + this.command.CommandText);
			}

            return new DbDataReaderProxy(reader, this.connection);
        }

		public object ExecuteScalar()
        {
            Object obj = null;

			if (!this.connection.AcquireLock())
				throw new ApplicationException("ExecuteScalar: Failed to lock connection");

			try {
				obj = this.command.ExecuteScalar();
			} catch (Exception e) {
				if (this.CanReopenConnection(e))
					obj = this.command.ExecuteScalar();
			} finally {
				this.connection.ReleaseLock();
                Console.WriteLine("ExecuteScalar("+this.command.Parameters.Count+"): " + this.command.CommandText);
			}

            return obj;
        }

        public void Prepare()
        {
            this.command.Prepare();
        }

        public void Cancel()
        {
            this.command.Cancel();
        }

        #endregion

        #region IDisposable Members

        public new void Dispose()
        {
			if (this.command != null) {
				if (this.mgr != null)
					this.mgr.ReturnCommand(this.command);

				this.command.Dispose();
			}

			if (this.connection != null) {
				this.connection = null;
			}
        }

        #endregion

		protected virtual bool CanReopenConnection(Exception ex)
		{
			return this.connection.CanReopen(ex);
		}
	}
}
