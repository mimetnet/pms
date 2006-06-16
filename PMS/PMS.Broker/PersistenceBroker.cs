using System;
using System.Collections;
using System.Configuration;

using PMS.Data;
using PMS.DataAccess;
using PMS.Metadata;
using PMS.Query;

namespace PMS.Broker
{
    [Serializable]
    public class PersistenceBroker : IPersistenceBroker
    {
        private bool isOpen = false;

        private string R_FILE = "repository.xml";
        private const string R_KEY = "PMS.Repository";

        private static PersistenceBroker _instance = null;

        public static IPersistenceBroker Instance {
            get {
                return (_instance != null) 
                    ? _instance : (_instance = new PersistenceBroker());
            }
        }

        private PersistenceBroker()
        {
        }

        ~PersistenceBroker()
        {
            Close();
        }

        public bool BeginTransaction()
        {
            return DbEngine.BeginTransaction();
        }

        public bool RollbackTransaction()
        {
            return DbEngine.RollbackTransaction();
        }

        public bool CommitTransaction()
        {
            return DbEngine.CommitTransaction();
        }

        public object GetObject(IQuery query)
        {
            return DbEngine.ExecuteSelectObject(query);
        }

        public object[] GetObjectList(IQuery query)
        {
            return DbEngine.ExecuteSelectList(query);
        }

        public object[] GetObjectList(Type type)
        {
            return DbEngine.ExecuteSelectList(new QueryByObject(type));
        }

        public DbResult Persist(object obj)
        {
            return DbEngine.ExecutePersist(obj);
        }

        public DbResult Count(object obj)
        {
            return DbEngine.ExecuteCount(obj);
        }

        public DbResult Insert(object obj)
        {
            return DbEngine.ExecuteInsert(obj);
        }

        public DbResult Update(object obj)
        {
            return DbEngine.ExecuteUpdate(obj);
        }

        public DbResult Update(object oldObj, object newObj)
        {
            return DbEngine.ExecuteUpdate(oldObj, newObj);
        }

        public DbResult Delete(object obj)
        {
            return DbEngine.ExecuteDelete(obj);
        }

        public DbResult Delete(object[] list)
        {
            DbResult result = new DbResult();
            foreach (object obj in list)
                result += Delete(obj);

            return result;
        }

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

        public bool Load(string fileName)
        {
            return PMS.Metadata.RepositoryManager.Load(fileName);
        }

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

        public bool Close()
        {
            if (isOpen) {
                DbEngine.Stop();
            }

            return true;
        }

        public bool IsOpen {
            get { return isOpen; }
        }

        public bool IsLoaded {
            get { return RepositoryManager.IsLoaded; }
        }

        public string Version {
            get { 
                return "0.5.122222"; 
            }
        }
    }
}
