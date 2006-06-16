using System;
using System.Collections;
using System.Configuration;

using PMS.Data;
using PMS.DataAccess;
using PMS.Metadata;
using PMS.Query;

namespace PMS.Broker
{
    public interface IPersistenceBroker
    {
        object GetObject(IQuery query);
        object[] GetObjectList(IQuery query);

        //object GetObject(object obj);
        //object[] GetObjectList(object obj);

        DbResult Persist(object obj);
        DbResult Count(object obj);
        DbResult Insert(object obj);
        DbResult Update(object obj);
        DbResult Update(object oldObj, object newObj);
        DbResult Delete(object obj);
        DbResult Delete(object[] list);

        bool Load(string repositoryFile);
        bool Load();
        bool Open();
        bool Close();

        bool BeginTransaction();
        bool RollbackTransaction();
        bool CommitTransaction();

        bool IsOpen { get; }
        bool IsLoaded { get; }

        string Version { get; }
    }
}
