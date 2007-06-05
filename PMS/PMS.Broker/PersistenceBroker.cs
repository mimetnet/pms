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
    /// <summary>
    /// Use this class to control all marshalling of data to/from the database.
    /// </summary>
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

        #region Transaction
        /// <summary>
        /// Begin transaction
        /// </summary>
        public void BeginTransaction()
        {
            DbEngine.BeginTransaction();
        }

        /// <summary>
        /// Rollback transaction
        /// </summary>
        public void RollbackTransaction()
        {
            DbEngine.RollbackTransaction();
        }

        /// <summary>
        /// Commit transaction
        /// </summary>
        public void CommitTransaction()
        {
            DbEngine.CommitTransaction();
        } 
        #endregion

        #region CRUD
        /// <summary>
        /// Retrieve instance of class based on IQuery
        /// </summary>
        /// <param name="query">Query to search for single object</param>
        /// <returns>single instance of class or null</returns>
        public object GetObject(IQuery query)
        {
            if (!IsOpen) {
                log.Warn("GetObject(query) called before .Open()");
                throw new RepositoryNotFoundException("GetObject(query) called before .Open()");
            }

            if (!IsLoaded) {
                log.Warn("GetObject(query) called before .Load()");
                throw new DbEngineNotStartedException("GetObject(query) called before .Load()");
            }

            return DbEngine.ExecuteSelectObject(query);
        }

        /// <summary>
        /// Retrieve multiple instances of a class based on IQuery
        /// </summary>
        /// <param name="query">Query to search for one or more objects</param>
        /// <returns>List of instantiated classes</returns>
        public object[] GetObjectArray(IQuery query)
        {
            if (!IsOpen) {
                log.Warn("GetObjectArray(query) called before .Open()");
                throw new RepositoryNotFoundException("GetObject(query) called before .Open()");
            }

            if (!IsLoaded) {
                log.Warn("GetObjectArray(query) called before .Load()");
                throw new DbEngineNotStartedException("GetObject(query) called before .Load()");
            }

            return DbEngine.ExecuteSelectArray(query);
        }

        /// <summary>
        /// Retrieve multiple instances of a class based on type, which maps to a table
        /// </summary>
        /// <param name="type">Type that maps to a database table</param>
        /// <returns>List of instantiated classes</returns>
        public object[] GetObjectArray(Type type)
        {
            if (!IsOpen) {
                log.Warn("GetObjectArray(type) called before .Open()");
                throw new RepositoryNotFoundException("GetObject(query) called before .Open()");
            }

            if (!IsLoaded) {
                log.Warn("GetObjectArray(type) called before .Load()");
                throw new DbEngineNotStartedException("GetObject(query) called before .Load()");
            }

            return DbEngine.ExecuteSelectArray(new QueryByObject(type));
        }

        /// <summary>
        /// Retrieve CollectionBase IList of classes based on IQuery provided
        /// </summary>
        /// <param name="query">Query to search for one or more objects</param>
        /// <returns>CollectionBase of instantiated classes</returns>
        public IList GetObjectList(IQuery query)
        {
            if (!IsOpen) {
                log.Warn("GetObjectList(query) called before .Open()");
                throw new RepositoryNotFoundException("GetObject(query) called before .Open()");
            }

            if (!IsLoaded) {
                log.Warn("GetObjectList(query) called before .Load()");
                throw new DbEngineNotStartedException("GetObject(query) called before .Load()");
            }

            return DbEngine.ExecuteSelectList(query);
        }

        /// <summary>
        /// Retrieve CollectionBase IList of classes based on Type provided
        /// </summary>
        /// <param name="type">Type that maps to a database table</param>
        /// <returns>CollectionBase of instantiated classes</returns>
        public IList GetObjectList(Type type)
        {
            if (!IsOpen) {
                log.Warn("GetObjectList(type) called before .Open()");
                throw new RepositoryNotFoundException("GetObject(query) called before .Open()");
            }

            if (!IsLoaded) {
                log.Warn("GetObjectList(type) called before .Load()");
                throw new DbEngineNotStartedException("GetObject(query) called before .Load()");
            }

            return this.GetObjectList(new QueryByType(type));
        }

        /// <summary>
        /// Perform PersistenceBroker.Count and based on result do Insert(obj) or Update(obj)
        /// </summary>
        /// <param name="obj">Instance of class to save</param>
        /// <remarks>
        /// cnt = SELECT count(*) FROM table;
        /// if cnt 0 then UPDATE
        /// else INSERT
        /// </remarks>
        /// <returns>Result holding Count and executed SQL</returns>
        public DbResult Persist(object obj)
        {
            if (!IsOpen) {
                log.Warn("Persist(obj) called before .Open()");
                throw new RepositoryNotFoundException("GetObject(query) called before .Open()");
            }

            if (!IsLoaded) {
                log.Warn("Persist(obj) called before .Load()");
                throw new DbEngineNotStartedException("GetObject(query) called before .Load()");
            }

            return DbEngine.ExecutePersist(obj);
        }

        /// <summary>
        /// Count number of records matching object properties
        /// </summary>
        /// <param name="obj">Instance of class to count</param>
        /// <remarks>SELECT count(*) FROM table;</remarks>
        /// <returns>Result holding Count and executed SQL</returns>
        public DbResult Count(object obj)
        {
            if (!IsOpen) {
                log.Warn("Count(obj) called before .Open()");
                throw new RepositoryNotFoundException("GetObject(query) called before .Open()");
            }

            if (!IsLoaded) {
                log.Warn("Count(obj) called before .Load()");
                throw new DbEngineNotStartedException("GetObject(query) called before .Load()");
            }

            return DbEngine.ExecuteCount(obj);
        }

        /// <summary>
        /// Insert object based on its set properties
        /// </summary>
        /// <param name="obj">Instance of class to INSERT</param>
        /// <returns>Result holding Count and executed SQL</returns>
        public DbResult Insert(object obj)
        {
            if (!IsOpen) {
                log.Warn("Insert(obj) called before .Open()");
                throw new RepositoryNotFoundException("GetObject(query) called before .Open()");
            }

            if (!IsLoaded) {
                log.Warn("Insert(obj) called before .Load()");
                throw new DbEngineNotStartedException("GetObject(query) called before .Load()");
            }

            return DbEngine.ExecuteInsert(obj);
        }

        /// <summary>
        /// Update object based on its set properties
        /// </summary>
        /// <param name="obj">Instance of class to UPDATE</param>
        /// <returns>Result holding Count and executed SQL</returns>
        public DbResult Update(object obj)
        {
            if (!IsOpen) {
                log.Warn("Update(obj) called before .Open()");
                throw new RepositoryNotFoundException("Update(obj) called before .Open()");
            }

            if (!IsLoaded) {
                log.Warn("Update(obj) called before .Load()");
                throw new DbEngineNotStartedException("Update(obj) called before .Load()");
            }

            return DbEngine.ExecuteUpdate(obj);
        }


		public DbResult Update(IQuery query)
		{
			if (!IsOpen) {
				log.Warn("Update(obj) called before .Open()");
				throw new RepositoryNotFoundException("Update(query) called before .Open()");
			}

			if (!IsLoaded) {
				log.Warn("Update(obj) called before .Load()");
				throw new DbEngineNotStartedException("Update(query) called before .Load()");
			}

			return DbEngine.ExecuteUpdate(query);
		}

        //public DbResult Update(object oldObj, object newObj)
        //{
        //    if (!IsOpen) {
        //        log.Warn("Update(obj) called before .Open()");
        //        throw new RepositoryNotFoundException("GetObject(query) called before .Open()");
        //    }

        //    if (!IsLoaded) {
        //        log.Warn("Update(obj) called before .Load()");
        //        throw new DbEngineNotStartedException("GetObject(query) called before .Load()");
        //    }

        //    return DbEngine.ExecuteUpdate(oldObj, newObj);
        //}

        /// <summary>
        /// Delete object based on its properties
        /// </summary>
        /// <param name="obj">Instance of class to delete</param>
        /// <returns>Result holding Count and executed SQL</returns>
        public DbResult Delete(object obj)
        {
            if (!IsOpen) {
                log.Warn("Delete(obj) called before .Open()");
                throw new RepositoryNotFoundException("GetObject(query) called before .Open()");
            }

            if (!IsLoaded) {
                log.Warn("Delete(obj) called before .Load()");
                throw new DbEngineNotStartedException("GetObject(query) called before .Load()");
            }

            return DbEngine.ExecuteDelete(obj);
        }

        /// <summary>
        /// Delete object based on its properties
        /// </summary>
        /// <param name="list">List of classes to delete</param>
        /// <returns>Result holding Count and executed SQL</returns>
        public DbResult Delete(object[] list)
        {
            if (!IsOpen) {
                log.Warn("Delete(list) called before .Open()");
                throw new RepositoryNotFoundException("GetObject(query) called before .Open()");
            }

            if (!IsLoaded) {
                log.Warn("Delete(list) called before .Load()");
                throw new DbEngineNotStartedException("GetObject(query) called before .Load()");
            }

            DbResult result = new DbResult();
            foreach (object obj in list)
                result += Delete(obj);

            return result;
        } 
        #endregion

        #region Control
        /// <summary>
        /// Load configuration file (defaults to repository.xml in PATH)
        /// </summary>
        /// <returns>success status</returns>
        public bool Load()
        {
            return Load(REPOSITORY_FILE);
        }

        /// <summary>
        /// Load configuration file at specified location
        /// </summary>
        /// <param name="fileName">Path to the repository to load</param>
        /// <returns>success status</returns>
        public bool Load(string fileName)
        {
            
            return PMS.Metadata.RepositoryManager.Load(fileName);
        }

        /// <summary>
        /// Load configuration file at specified location
        /// </summary>
        /// <param name="file">Repository file to load</param>
        /// <returns>success status</returns>
        public bool Load(System.IO.FileInfo file)
        {
            return PMS.Metadata.RepositoryManager.Load(file);
        }

        /// <summary>
        /// Closes all Load()'ed repositories
        /// </summary>
        public void Clear()
        {
            PMS.Metadata.RepositoryManager.Close();
        }

        /// <summary>
        /// Starts DbEngine based on Load()'ed repository.xml
        /// </summary>
        /// <returns>success status</returns>
        public bool Open()
        {
            if (IsLoaded == true && IsOpen == false) {
                isOpen = DbEngine.Start(RepositoryManager.Repository.DbManagerMode);
            }

            return IsOpen;
        }

        /// <summary>
        /// Closes DbEngine
        /// </summary>
        public void Close()
        {
            if (isOpen) {
                isOpen = !(DbEngine.Stop());
            }
        } 
        #endregion

        #region Properties

        /// <summary>
        /// Has PersistenceBroker been Open()'ed successfully?
        /// </summary>
        public bool IsOpen {
            get { return isOpen; }
        }

        /// <summary>
        /// Has PersistenceBroker been LOad()'ed successfully?
        /// </summary>
        public bool IsLoaded {
            get { return RepositoryManager.IsLoaded; }
        }

        /// <summary>
        /// Current Version of PMS
        /// </summary>
        public string Version {
            get {
                return System.Reflection.Assembly.GetCallingAssembly().GetName().Version.ToString();
            }
        }

        #endregion
    }
}
