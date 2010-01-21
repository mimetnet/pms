using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using PMS.Metadata;
using PMS.Query;

namespace PMS.Data
{
    public class DbExecutor <T> : IEnumerable<T> where T : new()
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("DbConverter");
        protected Query<T> query = null;

        public DbExecutor(Query<T> query)
        {
            this.query = query;
        }

        public int Insert()
        {
            //return this.Scalar<int>(query.ToString(SqlCommand.Insert) + ";SELECT CAST(SCOPE_IDENTITY() AS int)");
            return this.NonQuery(query.ToString(SqlCommand.Insert));
        }

        public int Update()
        {
            return this.NonQuery(query.ToString(SqlCommand.Update));
        }

        public int Delete()
        {
            return this.NonQuery(query.ToString(SqlCommand.Delete));
        }

        public bool Create()
        {
            return (0 < this.NonQuery(query.ToString(SqlCommand.Create)));
        }

        public bool Drop()
        {
            return (0 < this.NonQuery(query.ToString(SqlCommand.Drop)));
        }

        public int NonQuery(string sqlOverride)
        {
            return NonQuery(sqlOverride, query.CommandType, null);
        }

        public int NonQuery(string sqlOverride, params object[] parameters)
        {
            return NonQuery(sqlOverride, query.CommandType, parameters);
        }

        public int NonQuery(string sqlOverride, CommandType type)
        {
            return NonQuery(sqlOverride, type, null);
        }

        public int NonQuery(string sqlOverride, CommandType type, params object[] parameters)
        {
            this.query.AddParameters(parameters);
            this.query.LoadParameters();

            using (IDbCommand cmd = this.query.Connection.CreateCommand()) {
                cmd.CommandText = sqlOverride;
                cmd.CommandType = type;

                foreach (IDataParameter p in this.query.Parameters) {
                    cmd.Parameters.Add(p);
                }

                return cmd.ExecuteNonQuery();
            }
        }

        public IDataReader Reader()
        {
            return this.Reader(query.ToString(SqlCommand.Select), query.CommandType, null);
        }

        public IDataReader Reader(string sqlOverride)
        {
            return Reader(sqlOverride, query.CommandType, null);
        }

        public IDataReader Reader(string sqlOverride, params object[] parameters)
        {
            return Reader(sqlOverride, query.CommandType, parameters);
        }

        public IDataReader Reader(string sqlOverride, CommandType type)
        {
            return Reader(sqlOverride, type, null);
        }

        public IDataReader Reader(string sqlOverride, CommandType type, params object[] parameters)
        {
            this.query.AddParameters(parameters);
            this.query.LoadParameters();

            IDbCommand cmd = this.query.Connection.CreateCommand();
            cmd.CommandText = sqlOverride;
            cmd.CommandType = type;

            foreach (IDataParameter p in this.query.Parameters) {
                cmd.Parameters.Add(p);
            }

            return cmd.ExecuteReader();
        }

        public TResult Scalar<TResult>(String sqlOverride)
        {
            return Scalar<TResult>(sqlOverride, query.CommandType, null);
        }

        public TResult Scalar<TResult>(String sqlOverride, params object[] parameters)
        {
            return Scalar<TResult>(sqlOverride, query.CommandType, parameters);
        }

        public TResult Scalar<TResult>(String sqlOverride, CommandType type)
        {
            return Scalar<TResult>(sqlOverride, type, null);
        }

        public TResult Scalar<TResult>(String sqlOverride, CommandType type, params object[] parameters)
        {
            Object o = null;

            if ((o = this.Scalar(sqlOverride, type, parameters)) != null && o is TResult)
                return (TResult) o;

            return default(TResult);
        }

        public Object Scalar(String sqlOverride)
        {
            return Scalar(sqlOverride, query.CommandType, null);
        }

        public Object Scalar(String sqlOverride, params object[] parameters)
        {
            return Scalar(sqlOverride, query.CommandType, parameters);
        }

        public Object Scalar(String sqlOverride, CommandType type)
        {
            return Scalar(sqlOverride, type, null);
        }

        public Object Scalar(String sqlOverride, CommandType type, params object[] parameters)
        {
            this.query.AddParameters(parameters);
            this.query.LoadParameters();

            using (IDbCommand cmd = this.query.Connection.CreateCommand()) {
                cmd.CommandText = sqlOverride;
                cmd.CommandType = type;

                foreach (IDataParameter p in this.query.Parameters) {
                    cmd.Parameters.Add(p);
                }

                return cmd.ExecuteScalar();
            }
        }

        public bool Exists()
        {
            return (0 < this.Count());
        }

        public Int64 Count()
        {
            return this.Scalar<Int64>(query.ToString(SqlCommand.Count));
        }

        public TResult Count<TResult>()
        {
            return this.Scalar<TResult>(query.ToString(SqlCommand.Count));
        }

        public T Object()
        {
            return this.First();
        }

        public T Object(string sqlOverride)
        {
            return this.First(sqlOverride);
        }

        public T Object(string sqlOverride, params object[] parameters)
        {
            return this.First(sqlOverride, parameters);
        }

        public T Single()
        {
            return this.First();
        }

        public T Single(string sqlOverride)
        {
            return this.First(sqlOverride);
        }

        public T Single(string sqlOverride, params object[] parameters)
        {
            return this.First(sqlOverride, parameters);
        }

        public T First()
        {
            return First(query.ToString(SqlCommand.Select));
        }

        public T First(string sqlOverride)
        {
            return this.First(sqlOverride, null);
        }

        public T First(string sqlOverride, params object[] parameters)
        {
            using (IEnumerator<T> list = this.GetEnumerator(sqlOverride, parameters))
                if (list.MoveNext())
                    return list.Current;

            return default(T);
        }

        public T Last()
        {
            return Last(query.ToString(SqlCommand.Select));
        }

        public T Last(string sqlOverride)
        {
            return this.Last(sqlOverride, null);
        }

        public T Last(string sqlOverride, params object[] parameters)
        {
            T current = default(T);

            using (IEnumerator<T> list = this.GetEnumerator(sqlOverride, parameters))
                while (list.MoveNext())
                    current = list.Current;

            return current;
        }

        #region Objects<TList> with QueryCallback
        public TList Objects<TList>() where TList : IList, new()
        {
            return this.Reader2List<TList>(query.ToString(SqlCommand.Select), null);
        }

        public TList Objects<TList>(QueryCallback<T> callback) where TList : IList, new()
        {
            return this.Reader2List<TList>(query.ToString(SqlCommand.Select), callback);
        }
        #endregion

        #region Objects<TList> with SQL override
        public TList Objects<TList>(string sqlOverride) where TList : IList, new()
        {
            return this.Reader2List<TList>(sqlOverride, null, null);
        }

        public TList Objects<TList>(string sqlOverride, params object[] parameters) where TList : IList, new()
        {
            return this.Reader2List<TList>(sqlOverride, null, parameters);
        }

        public TList Objects<TList>(string sqlOverride, QueryCallback<T> callback) where TList : IList, new()
        {
            return this.Reader2List<TList>(sqlOverride, callback, null);
        }

        public TList Objects<TList>(string sqlOverride, QueryCallback<T> callback, params object[] parameters) where TList : IList, new()
        {
            return this.Reader2List<TList>(sqlOverride, callback, parameters);
        }
        #endregion

        protected TList Reader2List<TList>(string sqlOverride, QueryCallback<T> callback, params object[] parameters) where TList : IList, new()
        {
            TList list = Activator.CreateInstance<TList>();
            IEnumerator<T> enumerator = this.GetEnumerator(sqlOverride, parameters);

            try {
                if (callback == null) {
                    while (enumerator.MoveNext())
                        list.Add(enumerator.Current);
                } else {
                    while (enumerator.MoveNext()) {
				        try {
					        if (callback(enumerator.Current))
                                list.Add(enumerator.Current);
				        } catch (Exception ce) {
					        log.WarnFormat("QueryCallback<" + typeof(T).FullName + "> failure => ", ce);
				        }
                    }
			    }
            } finally {
                if (enumerator != null)
                    enumerator.Dispose();
            }

            return list;
        }

        #region IEnumerable Members
        public IEnumerator<T> GetEnumerator()
        {
            return new MetaObjectEnumerator<T>(query.Class, query.Provider, this.Reader(query.ToString(SqlCommand.Select), query.CommandType));
        }

        public IEnumerator<T> GetEnumerator(string sql)
        {
            return new MetaObjectEnumerator<T>(query.Class, query.Provider, this.Reader(sql, query.CommandType));
        }

        public IEnumerator<T> GetEnumerator(string sql, params object[] parameters)
        {
            return new MetaObjectEnumerator<T>(query.Class, query.Provider, this.Reader(sql, query.CommandType, parameters));
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
