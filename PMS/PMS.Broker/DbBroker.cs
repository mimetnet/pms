namespace PMS.Broker
{
    using System;
    using System.Data;
    using System.IO;
    using System.Collections.Generic;

    using PMS.Data;
    using PMS.DataAccess;
    using PMS.Metadata;
    using PMS.Query;

    public class DbBroker : IDisposable
    {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger("PMS.Broker.Session");
        private RepositoryManager repository = null;
        private Connection connDesc = null;
        private IDbManager db = null;
        private IDbConnection conn = null;

        /* octors {{{ */
        static DbBroker()
        {
            FileInfo file = new FileInfo("PMS.dll.config");
            
			if (file.Exists)
				log4net.Config.XmlConfigurator.ConfigureAndWatch(file);

			if (log.IsInfoEnabled)
				log.Info("Version " + Version);
        }

        public DbBroker() : this("repository.xml", null)
        {
        }

        public DbBroker(string connectionID) : this("repository.xml", connectionID)
		{
		}

		public DbBroker(string repository, string connectionID)
		{
            this.repository = RepositoryManagerFactory.Factory(repository);
            
            if (!String.IsNullOrEmpty(connectionID)) {
                this.connDesc = this.repository.GetDescriptor(connectionID);
            } else {
                this.connDesc = this.repository.GetDescriptor();
            }
            
            this.db = DbManagerFactory.Factory(this.connDesc);
		}

		~DbBroker()
		{
			this.Close();
		} 
		/* }}} */

		/* Properties {{{ */
		public static string Version {
			get {
				return System.Reflection.Assembly.GetCallingAssembly().GetName().Version.ToString();
			}
		}
		/* }}} */

		/* Transactions {{{ */
		public IDbTransaction Begin()
		{
			return (this.conn.BeginTransaction());
		}

        public void Rollback(IDbTransaction transaction)
		{
			if (transaction != null)
                transaction.Rollback();
		}

        public void Commit(IDbTransaction transaction)
		{
			if (transaction != null)
                transaction.Commit();
		}
		/* }}} */

        public DbExecutor<T> Exec<T>() where T : new()
        {
            return this.Query<T>().Exec();
        }

        public DbExecutor<T> Exec<T>(T record) where T : new()
        {
            return this.Query<T>().Set(record).Exec();
        }

        public Query<T> Query<T>() where T : new()
        {
            //if (this.conn != null)
            //    this.db.ReturnConnection(this.conn);

            //this.conn = this.db.GetConnection();

            return this.connDesc.Provider.CreateQuery<T>(
                this.repository.GetClass(typeof(T)), 
                this.db.GetConnection());
                //this.conn);
        }

		public void Close()
		{
			this.Dispose();
		}

        public void Dispose()
        {
            lock (this) {
                if (this.db != null && this.conn != null)
                    this.db.ReturnConnection(this.conn);

                this.db = null;
                this.conn = null;
                this.repository = null;
            }
        }
    }
}
