using System;
using System.Collections;
using System.Data;
using System.Security.Principal;

using PMS.Data;

using PMS.Metadata;
using PMS.Query;

namespace PMS.DataAccess
{
    /// <summary>
    /// Manages access to DbManager instance objects
    /// </summary>
    public sealed class DbEngine : MarshalByRefObject
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static IDbManager dbManager = null;

        private DbEngine()
        {
        }

        #region Transactions
        /// <summary>
        /// Begin transaction with Dbmanager
        /// </summary>
        /// <returns>status</returns>
        public static bool BeginTransaction(IPrincipal principal)
        {
            return dbManager.BeginTransaction(principal);
        }

        /// <summary>
        /// Rollback transaction with Dbmanager
        /// </summary>
        /// <returns>status</returns>
        public static bool RollbackTransaction(IPrincipal principal)
        {
            return dbManager.RollbackTransaction(principal);
        }

        /// <summary>
        /// Commit transaction with Dbmanager
        /// </summary>
        /// <returns>status</returns>
        public static bool CommitTransaction(IPrincipal principal)
        {
            return dbManager.CommitTransaction(principal);
        } 
        #endregion

        
        /// <summary>
        /// Execute SQL built by IQuery and return instantiated class
        /// </summary>
        /// <param name="query">Query to perform</param>
        /// <returns>Instantiated class or null</returns>
        public static object ExecuteSelectObject(IQuery query)
        {
            IDataReader reader = null;
            IDbCommand cmd = null;
            MetaObject mobj = null;
            Object obj = null;

            if (query == null) throw new ArgumentNullException("Parameter cannot be null");

            try {
                mobj = new MetaObject(query.Type);
                if (mobj.Exists) {
                    cmd = dbManager.GetCommand(query.Select(), AccessMode.Read);
                    reader = cmd.ExecuteReader();
                    obj = mobj.Materialize(reader);
                } else if (log.IsErrorEnabled) {
                    log.Error("ExecuteSelectObject", new ClassNotFoundException(query.Type));
                }
            } catch (InvalidOperationException e) {
                log.Error("ExecuteSelectObject", e);
            } finally {
                if (cmd != null) {
                    if (log.IsDebugEnabled)
                        log.Debug("SQL = " + cmd.CommandText);
                    dbManager.ReturnCommand(cmd);
                }

                if (reader != null) {
                    reader.Close();
                    reader = null;
                }

                query = null;
                mobj = null;
            }

            return obj;
        }

        /// <summary>
        /// Execute SQL built by IQuery and return instantiated classes
        /// </summary>
        /// <param name="query">Query to perform</param>
        /// <returns>Instantiated classes or null</returns>
        public static object[] ExecuteSelectArray(IQuery query)
        {
            IDataReader reader = null;
            IDbCommand cmd = null;
            MetaObject mobj = null;
            object[] list = null;

            if (query == null) throw new ArgumentNullException("Parameter cannot be null");

            try {
                mobj = new MetaObject(query.Type);
                if (mobj.Exists) {
                    cmd = dbManager.GetCommand(query.Select(),
                                               AccessMode.Read);
                    reader = cmd.ExecuteReader();
                    list = mobj.MaterializeArray(reader);
                } else if (log.IsErrorEnabled) {
                    log.Error("ExecuteSelectObject", new ClassNotFoundException(query.Type));
                }
            } catch (Exception e) {
                log.Error("ExecuteSelectArray", e);
            } finally {
                if (cmd != null) {
                    if (log.IsDebugEnabled)
                        log.Debug("SQL = " + cmd.CommandText);
                    dbManager.ReturnCommand(cmd);
                }
                if (reader != null) {
                    reader.Close();
                    reader = null;
                }
                query = null;
            }

            return list;
        }

        /// <summary>
        /// Execute SQL built by IQuery and return instantiated classes inside CollectionBase
        /// </summary>
        /// <param name="query">Query to perform</param>
        /// <returns>CollectionBase of instantiated classes</returns>
        public static IList ExecuteSelectList(IQuery query)
        {
            IDataReader reader = null;
            IDbCommand cmd = null;
            MetaObject mobj = null;
            IList list = null;

            if (query == null) throw new ArgumentNullException("Parameter cannot be null");

            try {
                mobj = new MetaObject(query.Type);
                if (mobj.Exists) {
                    cmd = dbManager.GetCommand(query.Select(), AccessMode.Read);
                    reader = cmd.ExecuteReader();
                    list = mobj.MaterializeList(reader);
                } else if (log.IsErrorEnabled) {
                    log.Error("ExecuteSelectObject", new ClassNotFoundException(query.Type));
                }
            } catch (Exception e) {
                log.Error("ExecuteSelectList", e);
            } finally {
                if (cmd != null) {
                    if (log.IsDebugEnabled)
                        log.Debug("SQL = " + cmd.CommandText);
                    dbManager.ReturnCommand(cmd);
                }
                if (reader != null) {
                    reader.Close();
                    reader = null;
                }
                query = null;
            }

            return list;
        }

        /// <summary>
        /// Delete object based on IQuery
        /// </summary>
        /// <param name="query">IQuery to call .Delete() on</param>
        /// <returns>DbResult</returns>
        public static DbResult ExecuteDelete(IQuery query)
        {
            if (query == null) throw new ArgumentNullException("Parameter cannot be null");

            if (query.IsValid)
                return ExecuteNonQuery(query.Delete());

            return new DbResult(query.ValidationException);
        }

        /// <summary>
        /// Delete object
        /// </summary>
        /// <param name="obj">Object to delete via QueryByObject</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecuteDelete(object obj)
        {
            if (obj == null) throw new ArgumentNullException("Parameter cannot be null");

            return ExecuteDelete(new QueryByObject(obj));
        }

        /// <summary>
        /// Delete all objects of this type
        /// </summary>
        /// <param name="type">Type to delete via QueryByType</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecuteDelete(Type type)
        {
            if (type == null) throw new ArgumentNullException("Parameter cannot be null");

            return ExecuteDelete(new QueryByType(type));
        }

        /// <summary>
        /// Insert or update object based on count retrieved
        /// </summary>
        /// <param name="obj">Object to save</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecutePersist(object obj)
        {
            IQuery query = null;
            DbResult result = null;

            if (obj == null) throw new ArgumentNullException("Parameter cannot be null");

            query = new QueryByObject(obj);

            if (query.IsValid) {
                result = ExecuteScalar(query.Count());

                if (result.Count >= 1) {
                    return ExecuteNonQuery(query.Update());
                } else {
                    return ExecuteNonQuery(query.Insert());
                }
            }

            return new DbResult(query.ValidationException);
        }

        /// <summary>
        /// Insert object
        /// </summary>
        /// <param name="obj">Object to insert</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecuteInsert(object obj)
        {
            if (obj == null) throw new ArgumentNullException("Parameter cannot be null");

            IQuery query = new QueryByObject(obj);

            if (query.IsValid)
                return ExecuteNonQuery(query.Insert());

            return new DbResult(query.ValidationException);
        }

        /// <summary>
        /// Update object
        /// </summary>
        /// <param name="obj">Object to update</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecuteUpdate(object obj)
        {
            if (obj == null) throw new ArgumentNullException("Parameter cannot be null");

            IQuery query = new QueryByObject(obj);

            return (query.IsValid) ?
                ExecuteScalar(query.Update()) : new DbResult(query.ValidationException);
        }

        /// <summary>
        /// Update object
        /// </summary>
        /// <param name="obj">Object to update</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecuteUpdate(object oldObj, object newObj)
        {
            if (oldObj == null) throw new ArgumentNullException("Parameter 1 cannot be null");
            if (newObj == null) throw new ArgumentNullException("Parameter 2 cannot be null");

            return ExecuteNonQuery((new QueryByObjectDiff(oldObj, newObj)).Update());
        }

        /// <summary>
        /// Perform SQL "SELECT * FROM obj;"
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DbResult ExecuteCount(object obj)
        {
            if (obj == null) throw new ArgumentNullException("Parameter cannot be null");

            IQuery query = new QueryByObject(obj);

            return (query.IsValid) ?
                ExecuteScalar(query.Count()) : new DbResult(query.ValidationException);
        }

        #region Execution
        /// <summary>
        /// Perform IDbCommand.ExecuteNonQuery()
        /// </summary>
        /// <param name="sql">SQL to execute</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecuteNonQuery(string sql)
        {
            IDbCommand cmd = null;
            DbResult result = null;

            if (sql == null)
                throw new ArgumentNullException("Parameter cannot be null");
            if (sql == String.Empty)
                throw new ArgumentNullException("Parameter cannot be String.Empty");

            try {
                cmd = dbManager.GetCommand(sql, AccessMode.Write);
                result = new DbResult(cmd.ExecuteNonQuery(), sql);
            } catch (Exception ex) {
                result = new DbResult(sql, ex);
            } finally {
                if (log.IsDebugEnabled)
                    log.Debug(result);
                if (log.IsErrorEnabled && result.Exception != null)
                    log.Error("ExecuteNonQuery", result.Exception);
                if (cmd != null)
                    dbManager.ReturnCommand(cmd);
            }

            return result;
        }

        /// <summary>
        /// Perform IDbCommand.ExecuteScalar()
        /// </summary>
        /// <param name="sql">SQL to execute</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecuteScalar(string sql)
        {
            IDbCommand cmd = null;
            DbResult result = null;

            if (sql == null)
                throw new ArgumentNullException("Parameter cannot be null");
            if (sql == String.Empty)
                throw new ArgumentNullException("Parameter cannot be String.Empty");

            try {
                cmd = dbManager.GetCommand(sql, AccessMode.Read);
                object count = cmd.ExecuteScalar();
                result = new DbResult(Convert.ToInt32(count), sql);
            } catch (InvalidOperationException ex) {
                result = new DbResult(sql, ex);
            } finally {
                if (log.IsDebugEnabled)
                    log.Debug(result);
                if (log.IsErrorEnabled && result.Exception != null)
                    log.Error("ExecuteNonQuery", result.Exception);
                if (cmd != null)
                    dbManager.ReturnCommand(cmd);
            }

            return result;
        }

        public static IDataReader ExecuteReader(string sql)
        {
            IDbCommand cmd = null;
            IDataReader reader = null;

            if (sql == null)
                throw new ArgumentNullException("Parameter cannot be null");
            if (sql == String.Empty)
                throw new ArgumentNullException("Parameter cannot be String.Empty");

            try {
                cmd = dbManager.GetCommand(sql, AccessMode.Read);
                reader = cmd.ExecuteReader();
            } catch (Exception ex) {
                log.Error("ExecuteReader(): " + new DbResult(sql, ex).ToString());
            } finally {
                if (log.IsDebugEnabled)
                    log.Debug("SQL = " + sql);
                if (cmd != null)
                    dbManager.ReturnCommand(cmd);
            }

            return reader;
        } 
        #endregion

        #region Control
        /// <summary>
        /// Start DbEngine with specified DbManagerMode
        /// </summary>
        /// <param name="mode">Mode to operate under</param>
        /// <returns>status</returns>
        public static bool Start(DbManagerMode mode)
        {
            if (DbManagerMode.Single == mode) {
                if (log.IsDebugEnabled)
                    log.Debug("DBEngine.Start(" + mode + ")");
                dbManager = new SingleDbManager();
                dbManager.Start();

                return true;
            }

            throw new ApplicationException("DbManagerMode Not Currently Support: " + mode);
        }

        /// <summary>
        /// Shutdown engine and subsequent DbManager
        /// </summary>
        /// <returns>status</returns>
        public static bool Stop()
        {
            if (dbManager != null) {
                if (log.IsDebugEnabled)
                    log.Debug("DBEngine.Stop()");
                dbManager.Stop();
                dbManager = null;
            }

            return true;
        } 
        #endregion

    }
}
