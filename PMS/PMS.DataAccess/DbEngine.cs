using System;
using System.Collections;
using System.Data;

using PMS.Data;

using PMS.Metadata;
using PMS.Query;

namespace PMS.DataAccess
{
    /// <summary>
    /// Manages access to DbManager instance objects
    /// </summary>
    public sealed class DbEngine
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
        public static bool BeginTransaction()
        {
            return dbManager.BeginTransaction();
        }

        /// <summary>
        /// Rollback transaction with Dbmanager
        /// </summary>
        /// <returns>status</returns>
        public static bool RollbackTransaction()
        {
            return dbManager.RollbackTransaction();
        }

        /// <summary>
        /// Commit transaction with Dbmanager
        /// </summary>
        /// <returns>status</returns>
        public static bool CommitTransaction()
        {
            return dbManager.CommitTransaction();
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
            MetaObject metaObject = null;

            try {
                cmd = dbManager.GetCommand(query.Select(), AccessMode.Read);
                reader = cmd.ExecuteReader();
                metaObject = new MetaObject(query.Type);
                
                return metaObject.Materialize(reader);
            } catch (InvalidOperationException) {
                return null;
            } finally {
                if (cmd != null)
                    if (log.IsDebugEnabled)
                        log.Debug("SQL = " + cmd.CommandText);

                if (reader != null) {
                    reader.Close();
                    reader = null;
                }

                query = null;
                metaObject = null;
            }
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
            MetaObject metaObject = null;
            Object[] list;

            try {
                cmd = dbManager.GetCommand(query.Select(),
                                           AccessMode.Read);
                reader = cmd.ExecuteReader();
                metaObject = new MetaObject(query.Type);
                
                return metaObject.MaterializeArray(reader);
            } catch (Exception e) {
                log.Error("ExecuteSelectArray", e);
                return null;
            } finally {
                if (cmd != null) {
                    if (log.IsDebugEnabled)
                        log.Debug("SQL = " + cmd.CommandText);
                }
                if (reader != null) {
                    reader.Close();
                    reader = null;
                }
                query  = null;
            }
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
            MetaObject metaObject = null;

            try {
                cmd = dbManager.GetCommand(query.Select(), AccessMode.Read);
                reader = cmd.ExecuteReader();
                metaObject = new MetaObject(query.Type);

                return metaObject.MaterializeList(reader);
            } catch (Exception e) {
                log.Error("ExecuteSelectArray", e);
                return null;
            } finally {
                if (cmd != null) {
                    if (log.IsDebugEnabled)
                        log.Debug("SQL = " + cmd.CommandText);
                }
                if (reader != null) {
                    reader.Close();
                    reader = null;
                }
                query = null;
            }
        }

        /// <summary>
        /// Delete object
        /// </summary>
        /// <param name="obj">Object to delete</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecuteDelete(object obj)
        {
            return ExecuteNonQuery((new QueryByObject(obj)).Delete());
        }

        /// <summary>
        /// Delete all objects of this type
        /// </summary>
        /// <param name="type">type to delete</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecuteDelete(Type type)
        {
            IQuery query = query = new QueryByType(type);

            return ExecuteNonQuery(query.Delete());
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
            
            try {
                query = new QueryByObject(obj);
                result = ExecuteScalar(query.Count());

                if (result.Count >= 1) {
                    return ExecuteNonQuery(query.Update());
                } else {
                    return ExecuteNonQuery(query.Insert());
                }
            } catch (Exception e) {
                throw e;
            }
        }

        /// <summary>
        /// Insert object
        /// </summary>
        /// <param name="obj">Object to insert</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecuteInsert(object obj)
        {
            IQuery query = null;

            try {
                query = new QueryByObject(obj);
                return ExecuteNonQuery(query.Insert());
            } catch (Exception e) {
                throw e;
            }
        }

        /// <summary>
        /// Update object
        /// </summary>
        /// <param name="obj">Object to update</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecuteUpdate(object obj)
        {
            return ExecuteNonQuery((new QueryByObject(obj)).Update());
        }

        /// <summary>
        /// Update object
        /// </summary>
        /// <param name="obj">Object to update</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecuteUpdate(object oldObj, object newObj)
        {
            return ExecuteNonQuery((new QueryByObjectDiff(oldObj, newObj)).Update());
        }

        /// <summary>
        /// Perform SQL "SELECT * FROM obj;"
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DbResult ExecuteCount(object obj)
        {
            return ExecuteScalar((new QueryByObject(obj)).Count());
        }

        /// <summary>
        /// Perform IDbCommand.ExecuteNonQuery()
        /// </summary>
        /// <param name="sql">SQL to execute</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecuteNonQuery(string sql)
        {
            IDbCommand cmd   = null;
            DbResult result = null;

            try {
                cmd = dbManager.GetCommand(sql, AccessMode.Write);
                result = new DbResult(cmd.ExecuteNonQuery(), sql);
            } catch (Exception ex) {
                result = new DbResult(sql, ex);
            } finally {
                if (log.IsDebugEnabled)
                    log.Debug(result);
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
            IDbCommand cmd   = null;
            DbResult result = null;

            try {
                cmd = dbManager.GetCommand(sql, AccessMode.Read);
                object count = cmd.ExecuteScalar();
                result = new DbResult(Convert.ToInt32(count), sql);
            } catch (InvalidOperationException ex) {
                result = new DbResult(sql, ex);
            } finally {
                if (log.IsDebugEnabled)
                    log.Debug(result);
            }

            return result;
        }
        
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
                dbManager = SingleDbManager.Instance;
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

        /// <summary>
        /// Retrieve IDbCommand from pool with sql for specified AccessMode
        /// </summary>
        /// <param name="sql">SQL to execute</param>
        /// <param name="mode">AccessMode (is command reading or writing)</param>
        /// <returns>IDbCommand</returns>
        public static IDbCommand GetCommand(string sql, AccessMode mode)
        {
            return dbManager.GetCommand(sql, mode);
        }

        /// <summary>
        /// Retrieve IDbCommand from pool for specified AccessMode
        /// </summary>
        /// <param name="mode">AccessMode (is command reading or writing)</param>
        /// <returns>IDbCommand</returns>
        public static IDbCommand GetCommand(AccessMode mode)
        {
            return dbManager.GetCommand(mode);
        }

        /// <summary>
        /// Retieve IDbCommand from pool where access mode defaults to Read
        /// </summary>
        /// <param name="sql">SQL to execute</param>
        /// <returns>IDbCommand</returns>
        public static IDbCommand GetCommand(string sql)
        {
            return dbManager.GetCommand(sql, AccessMode.Read);
        }

        /// <summary>
        /// Retrieves IDbCommand from pool where access mode defaults to Read
        /// </summary>
        /// <returns>IDbCommand</returns>
        public static IDbCommand GetCommand()
        {
            return dbManager.GetCommand(AccessMode.Read);
        }
    }
}
