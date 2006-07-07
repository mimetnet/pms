using System;
using System.Collections;
using System.Configuration;

using PMS.Data;
using PMS.DataAccess;
using PMS.Metadata;
using PMS.Query;

namespace PMS.Broker
{
    /// <summary>
    /// Use this class to control all marshalling of data to/from the database.
    /// </summary>
    [Serializable]
    public sealed class PersistenceBroker : IPersistenceBroker
    {
        #region variables
        private bool isOpen = false;
        private string R_FILE = "repository.xml";
        private const string R_KEY = "PMS.Repository";
        private static PersistenceBroker _instance = null;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        #region Construct/Destruct
        /// <summary>
        /// Return Singleton
        /// </summary>
        public static IPersistenceBroker Instance {
            get  {
                return (_instance != null) ?
                    _instance : (_instance = new PersistenceBroker());
            }
        }

        private PersistenceBroker()
        {
            System.IO.FileInfo file = new System.IO.FileInfo("PMS.dll.config");
            if (file.Exists)
                log4net.Config.XmlConfigurator.Configure(file);
            else
                Console.WriteLine("NOCONFIG: " + file.FullName);

            if (log.IsInfoEnabled)
                log.Info("Version " + Version);
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
        /// <returns>success status</returns>
        public bool BeginTransaction()
        {
            return DbEngine.BeginTransaction();
        }

        /// <summary>
        /// Rollback transaction
        /// </summary>
        /// <returns>success status</returns>
        public bool RollbackTransaction()
        {
            return DbEngine.RollbackTransaction();
        }

        /// <summary>
        /// Commit transaction
        /// </summary>
        /// <returns>success status</returns>
        public bool CommitTransaction()
        {
            return DbEngine.CommitTransaction();
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
            return DbEngine.ExecuteSelectObject(query);
        }

        /// <summary>
        /// Retrieve multiple instances of a class based on IQuery
        /// </summary>
        /// <param name="query">Query to search for one or more objects</param>
        /// <returns>List of instantiated classes</returns>
        public object[] GetObjectArray(IQuery query)
        {
            return DbEngine.ExecuteSelectArray(query);
        }

        /// <summary>
        /// Retrieve multiple instances of a class based on type, which maps to a table
        /// </summary>
        /// <param name="type">Type that maps to a database table</param>
        /// <returns>List of instantiated classes</returns>
        public object[] GetObjectArray(Type type)
        {
            return DbEngine.ExecuteSelectArray(new QueryByObject(type));
        }

        /// <summary>
        /// Retrieve CollectionBase IList of classes based on IQuery provided
        /// </summary>
        /// <param name="query">Query to search for one or more objects</param>
        /// <returns>CollectionBase of instantiated classes</returns>
        public IList GetObjectList(IQuery query)
        {
            return DbEngine.ExecuteSelectList(query);
        }

        /// <summary>
        /// Retrieve CollectionBase IList of classes based on Type provided
        /// </summary>
        /// <param name="type">Type that maps to a database table</param>
        /// <returns>CollectionBase of instantiated classes</returns>
        public IList GetObjectList(Type type)
        {
            return DbEngine.ExecuteSelectList(new QueryByObject(type));
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
            return DbEngine.ExecuteCount(obj);
        }

        /// <summary>
        /// Insert object based on its set properties
        /// </summary>
        /// <param name="obj">Instance of class to INSERT</param>
        /// <returns>Result holding Count and executed SQL</returns>
        public DbResult Insert(object obj)
        {
            return DbEngine.ExecuteInsert(obj);
        }

        /// <summary>
        /// Update object based on its set properties
        /// </summary>
        /// <param name="obj">Instance of class to UPDATE</param>
        /// <returns>Result holding Count and executed SQL</returns>
        public DbResult Update(object obj)
        {
            return DbEngine.ExecuteUpdate(obj);
        }

        /// <summary>
        /// Update object based on difference between oldObj and newObj
        /// </summary>
        /// <param name="oldObj">Original class</param>
        /// <param name="newObj">Modified class</param>
        /// <returns>Result holding Count and executed SQL</returns>
        public DbResult Update(object oldObj, object newObj)
        {
            return DbEngine.ExecuteUpdate(oldObj, newObj);
        }

        /// <summary>
        /// Delete object based on its properties
        /// </summary>
        /// <param name="obj">Instance of class to delete</param>
        /// <returns>Result holding Count and executed SQL</returns>
        public DbResult Delete(object obj)
        {
            return DbEngine.ExecuteDelete(obj);
        }

        /// <summary>
        /// Delete object based on its properties
        /// </summary>
        /// <param name="list">List of classes to delete</param>
        /// <returns>Result holding Count and executed SQL</returns>
        public DbResult Delete(object[] list)
        {
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
            string fileName = null;

#if NET_2_0
            if ((fileName = ConfigurationManager.AppSettings[R_KEY]) != null)
#else
            if ((fileName = ConfigurationSettings.AppSettings[R_KEY]) != null)
#endif
                R_FILE = fileName;

            return Load(R_FILE);
        }

        /// <summary>
        /// Load configuration file at specified location
        /// </summary>
        /// <param name="fileName">Repository file to load</param>
        /// <returns>success status</returns>
        public bool Load(string fileName)
        {
            return PMS.Metadata.RepositoryManager.Load(fileName);
        }

        /// <summary>
        /// Starts DbEngine based on Load()'ed repository.xml
        /// </summary>
        /// <returns>success status</returns>
        public bool Open()
        {
            if (IsOpen == false) {
                if (IsLoaded == false)
                    PersistenceBroker.Instance.Load();

                DbEngine.Start(RepositoryManager.Repository.DbManagerMode);
                isOpen = true;
            }

            return IsOpen;
        }

        /// <summary>
        /// Closes DbEngine
        /// </summary>
        /// <returns>success status</returns>
        public bool Close()
        {
            if (isOpen) {
                DbEngine.Stop();
            }

            return true;
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
