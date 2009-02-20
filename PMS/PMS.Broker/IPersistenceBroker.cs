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
        bool IsOpen { get; }
        bool IsLoaded { get; }
        string Version { get; }

        object GetObject(IQuery query);

        IList GetObjectList(IQuery query);
        IList GetObjectList(Type type);

        object[] GetObjectArray(IQuery query);
        object[] GetObjectArray(Type type);

#if !MONO_1_1
        T GetObject<T>(IQuery query) where T : new();

        IList GetObjectList<T>() where T : new(); // Type
        IList GetObjectList<T>(QueryCallback<T> callback) where T : new(); // Type w/ callback
        IList GetObjectList<T>(IQuery query) where T : new();
        IList GetObjectList<T>(IQuery query, QueryCallback<T> callback) where T : new();

        T[] GetObjectArray<T>() where T : new();
        T[] GetObjectArray<T>(QueryCallback<T> callback) where T : new();
        T[] GetObjectArray<T>(IQuery query) where T : new();
        T[] GetObjectArray<T>(IQuery query, QueryCallback<T> callback) where T : new();
#endif

        DbResult Persist(object obj);

        DbResult Count(object obj);

        DbResult Insert(object obj);

        DbResult Update(object obj);
        DbResult Update(IQuery query);

        //DbResult Update(object oldObj, object newObj);

		DbResult Delete(IList list);
        DbResult Delete(object[] list); 
        DbResult Delete(object obj);

        bool Load();
        bool Load(string repositoryFile);
        bool Load(System.IO.FileInfo file);

        void Clear();
        bool Open();
        void Close(); 

        void BeginTransaction();
        void RollbackTransaction();
        void CommitTransaction(); 
    }
}
