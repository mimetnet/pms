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
        #region CRUD
        /// <summary>
        /// Retrieve multiple instances of a class based on IQuery
        /// </summary>
        /// <param name="query">Query to search for one or more objects</param>
        /// <returns>List of instantiated classes</returns>
        object GetObject(IQuery query);

        /// <summary>
        /// Retrieve multiple instances of a class based on type, which maps to a table
        /// </summary>
        /// <param name="type">Type that maps to a database table</param>
        /// <returns>List of instantiated classes</returns>
        IList GetObjectList(IQuery query);

        /// <summary>
        /// Retrieve multiple instances of a class based on type, which maps to a table
        /// </summary>
        /// <param name="type">Type that maps to a database table</param>
        /// <returns>List of instantiated classes</returns>
        IList GetObjectList(Type type);

        /// <summary>
        /// Retrieve Object[] of classes based on IQuery provided
        /// </summary>
        /// <param name="query">Query to search for one or more objects</param>
        /// <returns>Object[] of instantiated classes</returns>
        object[] GetObjectArray(IQuery query);

        /// <summary>
        /// Retrieve Object[] of classes based on Type provided
        /// </summary>
        /// <param name="type">Type that maps to a database table</param>
        /// <returns>Object[] of instantiated classes</returns>
        object[] GetObjectArray(Type type);

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
        DbResult Persist(object obj);

        /// <summary>
        /// Count number of records matching object properties
        /// </summary>
        /// <param name="obj">Instance of class to count</param>
        /// <remarks>SELECT count(*) FROM table;</remarks>
        /// <returns>Result holding Count and executed SQL</returns>
        DbResult Count(object obj);

        /// <summary>
        /// Insert object based on its set properties
        /// </summary>
        /// <param name="obj">Instance of class to INSERT</param>
        /// <returns>Result holding Count and executed SQL</returns>
        DbResult Insert(object obj);

        /// <summary>
        /// Update object based on its set properties
        /// </summary>
        /// <param name="obj">Instance of class to UPDATE</param>
        /// <returns>Result holding Count and executed SQL</returns>
        DbResult Update(object obj);

        /// <summary>
        /// Update object based on difference between oldObj and newObj
        /// </summary>
        /// <param name="oldObj">Original class</param>
        /// <param name="newObj">Modified class</param>
        /// <returns>Result holding Count and executed SQL</returns>
        //DbResult Update(object oldObj, object newObj);

        /// <summary>
        /// Delete object based on its properties
        /// </summary>
        /// <param name="obj">Instance of class to delete</param>
        /// <returns>Result holding Count and executed SQL</returns>
        DbResult Delete(object obj);

        /// <summary>
        /// Delete object based on its properties
        /// </summary>
        /// <param name="list">List of classes to delete</param>
        /// <returns>Result holding Count and executed SQL</returns>
        DbResult Delete(object[] list); 
        #endregion

        #region Control
        /// <summary>
        /// Load configuration file (defaults to repository.xml in PATH)
        /// </summary>
        /// <returns>success status</returns>
        bool Load();

        /// <summary>
        /// Load configuration file at specified location
        /// </summary>
        /// <param name="fileName">Path to the repository to load</param>
        /// <returns>success status</returns>
        bool Load(string repositoryFile);

        /// <summary>
        /// Load configuration file at specified location
        /// </summary>
        /// <param name="fileName">Repository file to load</param>
        /// <returns>success status</returns>
        bool Load(System.IO.FileInfo file);

        /// <summary>
        /// Closes all Load()'ed repositories
        /// </summary>
        void Clear();

        /// <summary>
        /// Starts DbEngine based on Load()'ed repository.xml
        /// </summary>
        /// <returns>success status</returns>
        bool Open();

        /// <summary>
        /// Closes DbEngine
        /// </summary>
        void Close(); 
        #endregion

        #region Transactions

        /// <summary>
        /// Begin transaction
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Rollback transaction
        /// </summary>
        void RollbackTransaction();

        /// <summary>
        /// Commit transaction
        /// </summary>
        void CommitTransaction(); 

        #endregion

        #region Properties
        /// <summary>
        /// Has PersistenceBroker been Open()'ed successfully?
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Has PersistenceBroker been LOad()'ed successfully?
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        /// Current Version of PMS
        /// </summary>
        string Version { get; }

        #endregion
    }
}
