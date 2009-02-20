using System;
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

        public IDataReader Reader()
        {
            return this.Reader(query.ToString(SqlCommand.Select));
        }

        public IDataReader Reader(string sqlOverride)
        {
            IDbCommand cmd = query.Connection.CreateCommand();
            cmd.CommandText = sqlOverride;

            foreach (IDataParameter p in query.Parameters)
                cmd.Parameters.Add(p);

            return cmd.ExecuteReader();
        }

        public TResult Scalar<TResult>(String sqlOverride)
        {
            Object o = null;
            
            if ((o = this.Scalar(sqlOverride)) != null && o is TResult)
                return (TResult) o;

            return default(TResult);
        }

        public Object Scalar(String sqlOverride)
        {
            using (IDbCommand cmd = query.Connection.CreateCommand()) {
                cmd.CommandText = sqlOverride;
                Console.WriteLine("scalar: " + query.Parameters.Count + " " + sqlOverride);
                foreach (IDataParameter p in query.Parameters)
                    cmd.Parameters.Add(p);

                return cmd.ExecuteScalar();
            }
        }

        public bool Exists()
        {
            using (IEnumerator<T> list = this.GetEnumerator())
                if (list.MoveNext())
                    return true;

            return false;
        }

        public int Count()
        {
            return this.Scalar<int>(query.ToString(SqlCommand.Count));
        }

        public TResult Count<TResult>()
        {
            return this.Scalar<TResult>(query.ToString(SqlCommand.Count));
        }

        public T Object()
        {
            return this.First();
        }

        public T Single()
        {
            return this.First();
        }

        public T First()
        {
            //this.SetLimit(1);
            using (IEnumerator<T> list = this.GetEnumerator())
                if (list.MoveNext())
                    return list.Current;

            return default(T);
        }

        public T Last()
        {
            T current = default(T);

            using (IEnumerator<T> list = this.GetEnumerator())
                while (list.MoveNext())
                    current = list.Current;

            return current;
        }

        #region Objects<TList> with QueryCallback
        public TList Objects<TList>() where TList : IList<T>, new()
        {
            return this.Reader2List<TList>(query.ToString(SqlCommand.Select), null);
        }

        public TList Objects<TList>(QueryCallback<T> callback) where TList : IList<T>, new()
        {
            return this.Reader2List<TList>(query.ToString(SqlCommand.Select), callback);
        }
        #endregion

        #region Objects<TList> with SQL override
        public TList Objects<TList>(string sqlOverride) where TList : IList<T>, new()
        {
            return this.Reader2List<TList>(sqlOverride, null);
        }

        public TList Objects<TList>(string sqlOverride, QueryCallback<T> callback) where TList : IList<T>, new()
        {
            return this.Reader2List<TList>(sqlOverride, callback);
        }
        #endregion

        private TList Reader2List<TList>(string sqlOverride, QueryCallback<T> callback) where TList : IList<T>, new()
        {
            TList list = Activator.CreateInstance<TList>();
            IEnumerator<T> enumerator = this.GetEnumerator(sqlOverride);

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
            return new MetaObjectEnumerator<T>(query.Class, query.Provider, this.Reader(query.ToString(SqlCommand.Select)));
        }

        public IEnumerator<T> GetEnumerator(string sql)
        {
            return new MetaObjectEnumerator<T>(query.Class, query.Provider, this.Reader(sql));
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
