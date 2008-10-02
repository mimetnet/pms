using System;
using System.Collections;
using System.Data;
using System.Security.Principal;

using PMS.Data;

using PMS.Metadata;

using PMS.Query;

namespace PMS.DataAccess
{
    public static class DbEngine
    {
        private static readonly log4net.ILog log = 
			log4net.LogManager.GetLogger("PMS.DataAccess.DbEngine");
        private static IDbManager dbManager = null;

        #region Transactions
        public static void BeginTransaction()
        {
            dbManager.BeginTransaction();
        }

        public static void RollbackTransaction()
        {
            dbManager.RollbackTransaction();
        }

        public static void CommitTransaction()
        {
            dbManager.CommitTransaction();
        } 
        #endregion
        
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
						result = new DbResult(((obj == null)? 0 : 1), cmd.CommandText);
					}
				} catch (RepositoryDefinitionException rde) {
					throw rde;
				} catch (Exception e) {
					result = new DbResult(cmd.CommandText, e);

					if (cmd.Transaction != null)
						throw e;
				} finally {
					if (result != null) {
						if (result.Exception != null) {
							log.Error("ExecuteSelectObject: " + result.ToString(false), result.Exception);
						} else if (log.IsDebugEnabled) {
							log.Debug(result.ToString());
						}
					}
				}
			}

            return obj;
        }

		public static T ExecuteSelectObject<T>(IQuery query)
		{
			if (query == null) throw new ArgumentNullException("IQuery cannot be null");

            T obj = default(T);
			DbResult result = null;
			PMS.Metadata.Generic.MetaObject<T> mobj = new PMS.Metadata.Generic.MetaObject<T>();

			using (IDbCommand cmd = dbManager.GetCommand(query.Select())) {
				try {
					query.Criteria.Limit = 1;
					using (IDataReader reader = cmd.ExecuteReader()) {
						obj = mobj.Materialize(reader);
						result = new DbResult(((obj == null)? 0 : 1), cmd.CommandText);
					}
				} catch (RepositoryDefinitionException rde) {
					throw rde;
				} catch (Exception e) {
					result = new DbResult(cmd.CommandText, e);

					if (cmd.Transaction != null)
						throw e;
				} finally {
					if (result != null) {
						if (result.Exception != null) {
							log.Error("ExecuteSelectObject<T>: " + result.ToString(false), result.Exception);
						} else if (log.IsDebugEnabled) {
							log.Debug(result.ToString());
						}
					}
				}
			}

            return obj;
        }

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
						result = new DbResult(list.Length, cmd.CommandText);
					}
				} catch (RepositoryDefinitionException rde) {
					throw rde;
				} catch (Exception e) {
					result = new DbResult(cmd.CommandText, e);

					if (cmd.Transaction != null)
						throw e;
				} finally {
					if (result != null) {
						if (result.Exception != null) {
							log.Error("ExecuteSelectArray: " + result.ToString(false), result.Exception);
						} else if (log.IsDebugEnabled) {
							log.Debug(result.ToString());
						}
					}
				}
			}

            return list;
        }

        public static T[] ExecuteSelectArray<T>(IQuery query, QueryCallback<T> callback)
		{
			if (query == null) throw new ArgumentNullException("IQuery cannot be null");

            T[] list = null;
			DbResult result = null;
			PMS.Metadata.Generic.MetaObject<T> mobj = new PMS.Metadata.Generic.MetaObject<T>();

			using (IDbCommand cmd = dbManager.GetCommand(query.Select())) {
				try {
					using (IDataReader reader = cmd.ExecuteReader()) {
						list = mobj.MaterializeArray(reader, callback);
						result = new DbResult(list.Length, cmd.CommandText);
					}
				} catch (RepositoryDefinitionException rde) {
					throw rde;
				} catch (Exception e) {
					result = new DbResult(cmd.CommandText, e);

					if (cmd.Transaction != null)
						throw e;
				} finally {
					if (result != null) {
						if (result.Exception != null) {
							log.Error("ExecuteSelectArray<T>: " + result.ToString(false), result.Exception);
						} else if (log.IsDebugEnabled) {
							log.Debug(result.ToString());
						}
					}
				}
			}

            return list;
        }

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
						result = new DbResult(list.Count, cmd.CommandText);
					}
				} catch (RepositoryDefinitionException rde) {
					throw rde;
				} catch (Exception e) {
					result = new DbResult(cmd.CommandText, e);

					if (cmd.Transaction != null)
						throw e;
				} finally {
					if (result != null) {
						if (result.Exception != null) {
							log.Error("ExecuteSelectList: " + result.ToString(false), result.Exception);
						} else if (log.IsDebugEnabled) {
							log.Debug(result.ToString());
						}
					}
				}
			}

            return list;
        }

		public static IList ExecuteSelectList<T>(IQuery query, QueryCallback<T> callback)
        {
			if (query == null) throw new ArgumentNullException("IQuery cannot be null");

            IList list = null;
			DbResult result = null;
			PMS.Metadata.Generic.MetaObject<T> mobj = new PMS.Metadata.Generic.MetaObject<T>();

			using (IDbCommand cmd = dbManager.GetCommand(query.Select())) {
				try {
					using (IDataReader reader = cmd.ExecuteReader()) {
						list = mobj.MaterializeList(reader, callback);
						result = new DbResult(list.Count, cmd.CommandText);
					}
				} catch (RepositoryDefinitionException rde) {
					throw rde;
				} catch (Exception e) {
					result = new DbResult(cmd.CommandText, e);

					if (cmd.Transaction != null)
						throw e;
				} finally {
					if (result != null) {
						if (result.Exception != null) {
							log.Error("ExecuteSelectList<T>: " + result.ToString(false), result.Exception);
						} else if (log.IsDebugEnabled) {
							log.Debug(result.ToString());
						}
					}
				}
			}

            return list;
        }

        public static DbResult ExecuteDelete(IQuery query)
        {
            if (query == null) throw new ArgumentNullException("IQuery cannot be null");

			return ExecuteNonQuery(query.Delete());
        }

        public static DbResult ExecuteDelete(object obj)
        {
            return ExecuteDelete(new QueryByObject(obj));
        }

        public static DbResult ExecuteDelete(Type type)
        {
            if (type == null) throw new ArgumentNullException("Type cannot be null");

            return ExecuteDelete(new QueryByType(type));
        }

        public static DbResult ExecutePersist(object obj)
        {
			DbResult result = null;
			IQuery query = new QueryByObject(obj);

			try {
				result = ExecuteScalar(query.Count());

				if (result.Count >= 1) {
					result = ExecuteNonQuery(query.Update());
				} else {
					result = ExecuteNonQuery(query.Insert());
				}

			} finally {
				if (result != null) {
					if (result.Exception != null) {
						log.Error("ExecutePersist: " + result.ToString(false), result.Exception);
					} else if (log.IsDebugEnabled) {
						log.Debug(result.ToString());
					}
				}
			}

			return result;

        }

        /// <summary>
        /// Insert object
        /// </summary>
        /// <param name="obj">Object to insert</param>
        /// <returns>Result of query</returns>
        public static DbResult ExecuteInsert(object obj)
        {
			String sql = null;
			DbResult result = null;

			try {
				sql = new QueryByObject(obj).Insert();
				result = ExecuteNonQuery(sql);
			} catch (RepositoryDefinitionException rde) {
					throw rde;
			} catch (Exception e) {
				result = new DbResult(sql, e);
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
			return ExecuteUpdate(new QueryByObject(obj));
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
			} catch (RepositoryDefinitionException rde) {
				throw rde;
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
			return ExecuteScalar(new QueryByObject(obj).Count());
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
				} catch (RepositoryDefinitionException rde) {
					throw rde;
				} catch (Exception e) {
					result = new DbResult(sql, e);

					if (cmd.Transaction != null)
						throw e;
				} finally {
					if (result != null) {
						if (result.Exception != null) {
							log.Error("ExecuteNonQuery: " + result.ToString(false), result.Exception);
						} else if (log.IsDebugEnabled) {
							log.Debug(result.ToString());
						}
					}
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
				} catch (RepositoryDefinitionException rde) {
					throw rde;
				} catch (Exception e) {
					result = new DbResult(sql, e);

					if (cmd.Transaction != null)
						throw e;
				} finally {
					if (result != null) {
						if (result.Exception != null) {
							log.Error("ExecuteScalar: " + result.ToString(false), result.Exception);
						} else if (log.IsDebugEnabled) {
							log.Debug(result.ToString());
						}
					}
				}
			}

            return result;           
        }

        public static IDataReader ExecuteReader(string sql)
        {
            IDataReader reader = null;

            if (sql == null)
                throw new ArgumentNullException("SQL cannot be null");

            if (sql == String.Empty)
                throw new ArgumentNullException("SQL cannot be Empty");

			using (IDbCommand cmd = dbManager.GetCommand(sql)) {
				try {
					reader = cmd.ExecuteReader();
				} catch (RepositoryDefinitionException rde) {
					throw rde;
				} catch (Exception e) {
					log.Error("ExecuteReader", e);

					if (cmd.Transaction != null)
						throw e;
				} finally {
					if (log.IsDebugEnabled)
						log.Debug("ExecuteReader.SQL = " + sql);
				}
			}

            return reader;
        } 
        #endregion

        #region Control
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
