using System;
using System.Collections;
using System.Data;

using PMS.Data;

using PMS.Metadata;
using PMS.Query;

namespace PMS.DataAccess
{
    public class DbEngine
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static IDbManager dbManager = null;

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
                if (cmd != null) {
                    Console.WriteLine("SQL = " + cmd.CommandText);
                }
                if (reader != null) {
                    reader.Close();
                    reader = null;
                }

                query = null;
                metaObject = null;
            }
        }

        public static object[] ExecuteSelectList(IQuery query)
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
                
                list = metaObject.MaterializeList(reader);

                return list;
            } catch (InvalidOperationException) {
                return null;
            } finally {
                if (cmd != null) {
                    Console.WriteLine("SQL = " + cmd.CommandText);
                }
                if (reader != null) {
                    reader.Close();
                    reader = null;
                }
                query  = null;
            }
        }

        public static DbResult ExecuteDelete(object obj)
        {
            IQuery query = null;

            try {
                query = new QueryByObject(obj);

                return ExecuteNonQuery(query.Delete());
            } catch (Exception e) {
                throw e;
            } 
        }

        public static DbResult ExecuteDelete(Type type)
        {
            IQuery query = null;

            try {
                query = new QueryByType(type);

                return ExecuteNonQuery(query.Delete());
            } catch (Exception e) {
                throw e;
            } 
        }

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

        public static DbResult ExecuteUpdate(object obj)
        {
            IQuery query = new QueryByObject(obj);

            return ExecuteNonQuery(query.Update());
        }

        public static DbResult ExecuteUpdate(object oldObj, object newObj)
        {
            IQuery query = new QueryByObjectDiff(oldObj, newObj);

            return ExecuteNonQuery(query.Update());
        }

        public static DbResult ExecuteCount(object obj)
        {
            IQuery query = new QueryByObject(obj);

            return ExecuteScalar(query.Count());
        }        

        public static DbResult ExecuteNonQuery(string sql)
        {
            IDbCommand cmd   = null;
            DbResult result = null;

            try {
                cmd = dbManager.GetCommand(sql, AccessMode.Write);
                int x = cmd.ExecuteNonQuery();
                result = new DbResult(x, sql);
                Console.WriteLine(result);
                return result;
            } catch (Exception) {
                result = new DbResult(sql);
                return result;
            } finally {
                result = null;
            }
        }

        public static DbResult ExecuteScalar(string sql)
        {
            IDbCommand cmd   = null;
            DbResult result = null;

            try {
                cmd = dbManager.GetCommand(sql, AccessMode.Read);
                object count = cmd.ExecuteScalar();
                result = new DbResult(Convert.ToInt32(count), sql);
                Console.WriteLine(result);
                return result;
            } catch (InvalidOperationException) {
                result = new DbResult(sql);

                return result;
            } finally {
                result = null;
            }
        }
        
        public static bool Start(DbManagerMode mode)
        {
            if (DbManagerMode.Single == mode) {
                Console.WriteLine("PMS Start`ing");
                dbManager = SingleDbManager.Instance;
                dbManager.Start();

                return true;
            }

//            throw new ApplicationException(Resource.Format("PMS001", mode));
            throw new ApplicationException("DbManagerMode Not Currently Support: " + mode);
        }

        public static bool Stop()
        {
            if (dbManager != null) {
                Console.WriteLine("PMS Stop`ing");
                dbManager.Stop();
                dbManager = null;
            }

            return true;
        }

        public static IDbCommand GetCommand(string s, AccessMode mode)
        {
            return dbManager.GetCommand(s, mode);
        }

        public static IDbCommand GetCommand(AccessMode mode)
        {
            return dbManager.GetCommand(mode);
        }

        public static IDbCommand GetCommand(string s)
        {
            return dbManager.GetCommand(s, AccessMode.Read);
        }

        public static IDbCommand GetCommand()
        {
            return dbManager.GetCommand(AccessMode.Read);
        }
    }
}
