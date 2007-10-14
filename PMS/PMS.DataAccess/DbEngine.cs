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
        private static readonly log4net.ILog log = 
			log4net.LogManager.GetLogger("PMS.DataAccess.DbEngine");
        private static IDbManager dbManager = null;

        private DbEngine()
        {
        }

        #region Transactions
        /// <summary>
        /// Begin transaction with Dbmanager
        /// </summary>
        public static void BeginTransaction()
        {
            dbManager.BeginTransaction();
        }

        /// <summary>
        /// Rollback transaction with Dbmanager
        /// </summary>
        public static void RollbackTransaction()
        {
            dbManager.RollbackTransaction();
        }

        /// <summary>
        /// Commit transaction with Dbmanager
        /// </summary>
        public static void CommitTransaction()
        {
            dbManager.CommitTransaction();
        } 
        #endregion
        
        /// <summary>
        /// Execute SQL built by IQuery and return instantiated class
        /// </summary>
        /// <remarks>Selecting a single object implies LIMIT 1, feel free to change offset</remarks>
        /// <param name="query">Query to perform</param>
        /// <returns>Instantiated class or null</returns>
        public static object ExecuteSelectObject(IQuery query)
        {
			if (query == null) throw new ArgumentNullException("IQuery cannot be null");

            Object obj = null;
			DbResult result = null;
			MetaObject mobj = new MetaObject(query.Type);

			using (IDbCommand cmd = dbManager.GetCommand(query.Select())) {
				try {
					query.Criteria.Limit = 1;
					using (IDataReader reader = cmd.ExecuteReader()) {
						obj = mobj.Materialize(reader);
						result = new DbResult(((obj == null)? 0 : 1), query.Select());
					}
				} catch (Exception e) {
					result = new DbResult(query.Select(), e);
				} finally {
					if (result.Exception != null) {
						log.Error("ExecuteSelectObject: " + result);
					} else if (log.IsDebugEnabled) {
						log.Debug(result);
					}
					dbManager.ReturnCommand(cmd);
				}
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
			if (query == null) throw new ArgumentNullException("IQuery cannot be null");

            object[] list = null;
			DbResult result = null;
			MetaObject mobj = new MetaObject(query.Type);

			using (IDbCommand cmd = dbManager.GetCommand(query.Select())) {
				try {
					using (IDataReader reader = cmd.ExecuteReader()) {
						list = mobj.MaterializeArray(reader);
						result = new DbResult(list.Length, query.Select());
					}
				} catch (Exception e) {
					result = new DbResult(query.Select(), e);
				} finally {
					if (result.Exception != null) {
						log.Error("ExecuteSelectArray: " + result);
					} else if (log.IsDebugEnabled) {
						log.Debug(result);
					}
					dbManager.ReturnCommand(cmd);
				}
			}

            return list;
        }

        /// <summary>
        /// Execute SQL built by IQuery and return instantiated classes inside IList
        /// </summary>
        /// <param name="query">Query to perform</param>
        /// <returns>CollectionBase of instantiated classes</returns>
        public static IList ExecuteSelectList(IQuery query)
        {
			if (query == null) throw new ArgumentNullException("IQuery cannot be null");

            IList list = null;
			DbResult result = null;
			MetaObject mobj = new MetaObject(query.Type);

			using (IDbCommand cmd = dbManager.GetCommand(query.Select())) {
				try {
					using (IDataReader reader = cmd.ExecuteReader()) {
						list = mobj.MaterializeList(reader);
						result = new DbResult(list.Count, query.Select());
					}
				} catch (Exception e) {
					result = new DbResult(query.Select(), e);
				} finally {
					if (result.Exception != null) {
						log.Error("ExecuteSelectList: " + result);
					} else if (log.IsDebugEnabled) {
						log.Debug(result);
					}
					dbManager.ReturnCommand(cmd);
				}
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
            if (query == null) throw new ArgumentNullException("IQuery cannot be null");

            try {
				return ExecuteNonQuery(query.Delete());
			} catch (Exception e) {
				return new DbResult(query.Delete(), e);
			}
        }

        /// <summary>
        /// Delete object
        /// </summary>
        /// <param name="obj">Object to delete via QueryByObject</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecuteDelete(object obj)
        {
            if (obj == null) throw new ArgumentNullException("Object cannot be null");

            return ExecuteDelete(new QueryByObject(obj));
        }

        /// <summary>
        /// Delete all objects of this type
        /// </summary>
        /// <param name="type">Type to delete via QueryByType</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecuteDelete(Type type)
        {
            if (type == null) throw new ArgumentNullException("Type cannot be null");

            return ExecuteDelete(new QueryByType(type));
        }

        /// <summary>
        /// Insert or update object based on count retrieved
        /// </summary>
        /// <param name="obj">Object to save</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecutePersist(object obj)
        {
			if (obj == null) throw new ArgumentNullException("Object cannot be null");

			DbResult result = null;
			IQuery query = new QueryByObject(obj);

			try {
				result = ExecuteScalar(query.Count());

				if (result.Count >= 1) {
					return ExecuteNonQuery(query.Update());
				} else {
					return ExecuteNonQuery(query.Insert());
				}
			} catch (Exception e) {
				return new DbResult(e);
			}
        }

        /// <summary>
        /// Insert object
        /// </summary>
        /// <param name="obj">Object to insert</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecuteInsert(object obj)
        {
            if (obj == null) throw new ArgumentNullException("Object cannot be null");

            IQuery query = null;
			DbResult result = null;

			try {
				query = new QueryByObject(obj);
				result = ExecuteNonQuery(query.Insert());
			} catch (Exception e) {
				result = (query == null)? (new DbResult(e)) : (new DbResult(query.Insert(), e));
			}

			return result;
        }

        /// <summary>
        /// Update object
        /// </summary>
        /// <param name="obj">Object to update</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecuteUpdate(object obj)
        {
            if (obj == null) throw new ArgumentNullException("Object cannot be null");

            IQuery query = null;

			try {
				query = new QueryByObject(obj);
				return ExecuteNonQuery(query.Update());
			} catch (Exception e) {
				return new DbResult(e);
			}
        }

        /// <summary>
        /// Update record based on IQuery
        /// </summary>
        /// <param name="obj">IQuery Update to perform</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecuteUpdate(IQuery query)
        {
            if (query == null) throw new ArgumentNullException("IQuery cannot be null");

			try {
				return ExecuteNonQuery(query.Update());
			} catch (Exception e) {
				return new DbResult(e);
			}
        }

        /// <summary>
        /// Perform SQL "SELECT * FROM obj;"
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DbResult ExecuteCount(object obj)
        {
            if (obj == null) throw new ArgumentNullException("Object cannot be null");

			IQuery query = null;

			try {
				query = new QueryByObject(obj);
				return ExecuteScalar(query.Count());
			} catch (Exception e) {
				return new DbResult(e);
			}
        }

        #region Execution
        /// <summary>
        /// Perform IDbCommand.ExecuteNonQuery()
        /// </summary>
        /// <param name="sql">SQL to execute</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecuteNonQuery(string sql)
        {
            DbResult result = null;

            if (sql == null)
                throw new ArgumentNullException("SQL cannot be null");

            if (sql == String.Empty)
                throw new ArgumentException("SQL cannot be Empty");

			using (IDbCommand cmd = dbManager.GetCommand(sql)) {
				try {
					result = new DbResult(cmd.ExecuteNonQuery(), sql);
				} catch (Exception ex) {
					result = new DbResult(sql, ex);
				} finally {
					if (result.Exception != null) {
						log.Error("ExecuteNonQuery: " + result);
					} else if (log.IsDebugEnabled) {
						log.Debug(result);
					}
					dbManager.ReturnCommand(cmd);
				}
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
            DbResult result = null;

            if (sql == null)
                throw new ArgumentNullException("SQL cannot be null");

            if (sql == String.Empty)
                throw new ArgumentException("SQL cannot be Empty");

 			using (IDbCommand cmd = dbManager.GetCommand(sql)) {
				try {
					result = new DbResult(cmd.ExecuteScalar(), sql);
				} catch (Exception ex) {
					result = new DbResult(sql, ex);
				} finally {
					if (result.Exception != null) {
						log.Error("ExecuteScalar: " + result);
					} else if (log.IsDebugEnabled) {
						log.Debug(result);
					}
					dbManager.ReturnCommand(cmd);
				}
			}

            return result;           
        }

        public static IDataReader ExecuteReader(string sql)
        {
            IDbCommand cmd = null;
            IDataReader reader = null;

            if (sql == null)
                throw new ArgumentNullException("SQL cannot be null");

            if (sql == String.Empty)
                throw new ArgumentNullException("SQL cannot be Empty");

            try {
                cmd = dbManager.GetCommand(sql);
                reader = cmd.ExecuteReader();
            } catch (Exception ex) {
                log.Error("ExecuteReader", ex);
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

                return dbManager.Start();
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
