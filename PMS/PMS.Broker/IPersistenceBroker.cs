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
    public interface IPersistenceBroker
    {
        #region Properties
        bool IsOpen { get; }

        bool IsLoaded { get; }

        string Version { get; }
        #endregion

        #region CRUD
        object GetObject(IQuery query);

        IList GetObjectList(IQuery query);
        IList GetObjectList(Type type);

        object[] GetObjectArray(IQuery query);
        object[] GetObjectArray(Type type);

        DbResult Persist(object obj);

        DbResult Count(object obj);

        DbResult Insert(object obj);

        DbResult Update(object obj);
        DbResult Update(IQuery query);

        //DbResult Update(object oldObj, object newObj);

		DbResult Delete(IList list);
        DbResult Delete(object[] list); 
        DbResult Delete(object obj);
        #endregion

        #region Control
        bool Load();
        bool Load(string repositoryFile);
        bool Load(System.IO.FileInfo file);

        void Clear();

        bool Open();

        void Close(); 
        #endregion

        #region Transactions
        void BeginTransaction();
        void RollbackTransaction();
        void CommitTransaction(); 
        #endregion
    }
}
