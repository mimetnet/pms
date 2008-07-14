using System;
using System.Collections;
using System.Configuration;
using System.Security.Principal;

using PMS.Data;
using PMS.DataAccess;
using PMS.Metadata;
using PMS.Query;

namespace PMS.Broker
{
    public sealed class PersistenceBroker : MarshalByRefObject, IPersistenceBroker
    {
        #region variables
        private bool isOpen = false;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("PMS.Broker.PersistenceBroker");
        
        public const string REPOSITORY_FILE = "repository.xml";
        #endregion

        #region Construct/Destruct
        internal PersistenceBroker()
        {
            System.IO.FileInfo file = new System.IO.FileInfo("PMS.dll.config");
            if (file.Exists) {
                log4net.Config.XmlConfigurator.ConfigureAndWatch(file);
			}

            if (log.IsInfoEnabled) {
                log.Info("Version " + this.Version);
			}
        }

        ~PersistenceBroker()
        {
            Close();
        } 
        #endregion

        #region Properties
        public bool IsOpen {
            get { return isOpen; }
        }

        public bool IsLoaded {
            get { return RepositoryManager.IsLoaded; }
        }

        public string Version {
            get {
                return System.Reflection.Assembly.GetCallingAssembly().GetName().Version.ToString();
            }
        }
        #endregion

        #region Transaction
        public void BeginTransaction()
        {
            DbEngine.BeginTransaction();
        }

        public void RollbackTransaction()
        {
            DbEngine.RollbackTransaction();
        }

        public void CommitTransaction()
        {
            DbEngine.CommitTransaction();
        } 
        #endregion

        #region CRUD
        public object GetObject(IQuery query)
        {
			CheckStatus();

            return DbEngine.ExecuteSelectObject(query);
        }

        public object[] GetObjectArray(IQuery query)
        {
			CheckStatus();

            return DbEngine.ExecuteSelectArray(query);
        }

        public object[] GetObjectArray(Type type)
        {
			CheckStatus();

            return DbEngine.ExecuteSelectArray(new QueryByObject(type));
        }

        public IList GetObjectList(IQuery query)
        {
			CheckStatus();

            return DbEngine.ExecuteSelectList(query);
        }

        public IList GetObjectList(Type type)
        {
			CheckStatus();

            return this.GetObjectList(new QueryByType(type));
        }

        public DbResult Persist(object obj)
        {
			CheckStatus();

            return DbEngine.ExecutePersist(obj);
        }

        public DbResult Count(object obj)
        {
			CheckStatus();

            return DbEngine.ExecuteCount(obj);
        }

        public DbResult Insert(object obj)
        {
			CheckStatus();

            return DbEngine.ExecuteInsert(obj);
        }

        public DbResult Update(object obj)
        {
			CheckStatus();

            return DbEngine.ExecuteUpdate(obj);
        }


		public DbResult Update(IQuery query)
		{
			CheckStatus();

			return DbEngine.ExecuteUpdate(query);
		}

        //public DbResult Update(object oldObj, object newObj)
        //{
		//    CheckStatus();
		//
        //    return DbEngine.ExecuteUpdate(oldObj, newObj);
        //}

		public DbResult Delete(IList list)
        {
			CheckStatus();

            DbResult result = new DbResult();

            foreach (object obj in list)
                result += this.Delete(obj);

            return result;
        } 

        public DbResult Delete(object[] list)
        {
			CheckStatus();

            DbResult result = new DbResult();

            foreach (object obj in list)
                result += this.Delete(obj);

            return result;
        } 

        public DbResult Delete(object obj)
        {
			CheckStatus();

            return DbEngine.ExecuteDelete(obj);
        }
        #endregion

        #region Control
        public bool Load()
        {
            return Load(REPOSITORY_FILE);
        }

        public bool Load(string fileName)
        {
            return PMS.Metadata.RepositoryManager.Load(fileName);
        }

        public bool Load(System.IO.FileInfo file)
        {
            return PMS.Metadata.RepositoryManager.Load(file);
        }

        public void Clear()
        {
            PMS.Metadata.RepositoryManager.Close();
        }

        public bool Open()
        {
            if (IsLoaded == true && IsOpen == false) {
                isOpen = DbEngine.Start(RepositoryManager.Repository.DbManagerMode);
            }

            return IsOpen;
        }

        public void Close()
        {
            if (isOpen) {
                isOpen = !(DbEngine.Stop());
            }
        } 
        #endregion

		private void CheckStatus()
		{
            if (!IsOpen)
                throw new RepositoryNotFoundException("... called before .Open()");

            if (!IsLoaded)
                throw new DbEngineNotStartedException("... called before .Load()");
		}
    }
}
