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
			if (query == null) throw new ArgumentNullException("Parameter cannot be null");

            IDataReader reader = null;
            IDbCommand cmd = null;
			MetaObject mobj = new MetaObject(query.Type);
            Object obj = null;
            DbResult result = null;

            try {
                query.Criteria.Limit = 1;
                cmd = dbManager.GetCommand(query.Select());
				if ((reader = cmd.ExecuteReader()) != null) {
					obj = mobj.Materialize(reader);
					result = new DbResult(((obj == null)? 0 : 1), query.Select());
				}
			} catch (Exception e) {
                result = new DbResult(query.Select(), e);
                log.Error("ExecuteSelectObject: " + result);
            } finally {
                if (cmd != null) {
                    if (log.IsDebugEnabled)
                        log.Debug(result);
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
			if (query == null) throw new ArgumentNullException("Parameter cannot be null");

            IDataReader reader = null;
            IDbCommand cmd = null;
			MetaObject mobj = new MetaObject(query.Type);
            DbResult result = null;
            object[] list = null;

            try {
				cmd = dbManager.GetCommand(query.Select());
				if ((reader = cmd.ExecuteReader()) != null) {
					list = mobj.MaterializeArray(reader);
					result = new DbResult(list.Length, query.Select());
				}
            } catch (Exception e) {
                result = new DbResult(query.Select(), e);
                log.Error("ExecuteSelectArray: " + result);
            } finally {
                if (cmd != null) {
                    if (log.IsDebugEnabled)
                        log.Debug(result);
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
        /// Execute SQL built by IQuery and return instantiated classes inside IList
        /// </summary>
        /// <param name="query">Query to perform</param>
        /// <returns>CollectionBase of instantiated classes</returns>
        public static IList ExecuteSelectList(IQuery query)
        {
			if (query == null) throw new ArgumentNullException("Parameter cannot be null");

            IDataReader reader = null;
            IDbCommand cmd = null;
			MetaObject mobj = new MetaObject(query.Type);
            IList list = null;
            DbResult result = null;

            try {
                cmd = dbManager.GetCommand(query.Select());
				if ((reader = cmd.ExecuteReader()) != null) {
					list = mobj.MaterializeList(reader);
					result = new DbResult(list.Count, query.Select());
				}
            } catch (Exception e) {
                result = new DbResult(query.Select(), e);
                log.Error("ExecuteSelectList: " + result);
            } finally {
                if (cmd != null) {
                    if (log.IsDebugEnabled)
                        log.Debug(result);
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
			if (obj == null) throw new ArgumentNullException("Parameter cannot be null");

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
            if (obj == null) throw new ArgumentNullException("Parameter cannot be null");

            IQuery query = null;

			try {
				query = new QueryByObject(obj);
				return ExecuteNonQuery(query.Insert());
			} catch (Exception e) {
				if (query == null)
					return new DbResult(e);
				else 
					return new DbResult(query.Insert(), e);
			}
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

			try {
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
            if (query == null) throw new ArgumentNullException("Parameter cannot be null");

			try {
				return ExecuteNonQuery(query.Update());
			} catch (Exception e) {
				return new DbResult(e);
			}
        }

		/*
        public static DbResult ExecuteUpdate(object oldObj, object newObj)
        {
            if (oldObj == null) throw new ArgumentNullException("Parameter 1 cannot be null");
            if (newObj == null) throw new ArgumentNullException("Parameter 2 cannot be null");

            return ExecuteNonQuery((new QueryByObjectDiff(oldObj, newObj)).Update());
        }
		*/

        /// <summary>
        /// Perform SQL "SELECT * FROM obj;"
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DbResult ExecuteCount(object obj)
        {
            if (obj == null) throw new ArgumentNullException("Parameter cannot be null");

			IQuery query = new QueryByObject(obj);

			try {
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
            IDbCommand cmd = null;
            DbResult result = null;

            if (sql == null)
                throw new ArgumentNullException("Parameter cannot be null");
            if (sql == String.Empty)
                throw new ArgumentNullException("Parameter cannot be String.Empty");

            try {
                cmd = dbManager.GetCommand(sql);
                result = new DbResult(cmd.ExecuteNonQuery(), sql);
            } catch (Exception ex) {
                result = new DbResult(sql, ex);
            } finally {
                if (result.Exception != null) {
                    log.Error("ExecuteNonQuery");
					log.Error(result);
				} else if (log.IsDebugEnabled) {
                    log.Debug(result);
				}

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
                cmd = dbManager.GetCommand(sql);
                result = new DbResult(cmd.ExecuteScalar(), sql);
            } catch (Exception ex) {
                result = new DbResult(0, sql, ex);
            } finally {
                if (result.Exception != null) {
                    log.Error("ExecuteScalar");
					log.Error(result);
				} else if (log.IsDebugEnabled) {
                    log.Debug(result);
				}

                if (cmd != null) {
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
                throw new ArgumentNullException("Parameter cannot be null");
            if (sql == String.Empty)
                throw new ArgumentNullException("Parameter cannot be String.Empty");

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
