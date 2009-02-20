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
            this.conn = this.db.GetConnection();
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

        public Query<T> Query<T>() where T : new()
        {
            return this.connDesc.Provider.CreateQuery<T>(
                this.repository.GetClass(typeof(T)), 
                this.conn);
        }

        //public DbResult Persist(object obj)
        //{
        //    return DbEngine.ExecutePersist(obj);
        //}

        //public DbResult Count(object obj)
        //{
        //    return DbEngine.ExecuteCount(obj);
        //}

        //public DbResult Insert(object obj)
        //{
        //    return DbEngine.ExecuteInsert(obj);
        //}

        //public DbResult Update(object obj)
        //{
        //    return DbEngine.ExecuteUpdate(obj);
        //}

        //public DbResult Update(IQuery query)
        //{
        //    return DbEngine.ExecuteUpdate(query);
        //}

        //public DbResult Delete(IList list)
        //{
        //    DbResult result = new DbResult();

        //    foreach (object obj in list)
        //        result += this.Delete(obj);

        //    return result;
        //} 

        //public DbResult Delete(object[] list)
        //{
        //    DbResult result = new DbResult();

        //    foreach (object obj in list)
        //        result += this.Delete(obj);

        //    return result;
        //} 

        //public DbResult Delete(object obj)
        //{
        //    return DbEngine.ExecuteDelete(obj);
        //}

		/* Control {{{ */
		public void Close()
		{
			this.Dispose();
		} 
		/* }}} */

        #region IDisposable Members

        public void Dispose()
        {
            if (this.db != null) {
                this.db.ReturnConnection(this.conn);
                this.db = null;
            }

            this.conn = null;
            this.repository = null;
        }

        #endregion
    }
}
