using System;
using System.Collections.Generic;
using System.Data;

using PMS.Data;
using PMS.Query;
using System.Reflection;

namespace PMS.Metadata
{
    public class MetaObjectEnumerator <T> : IEnumerator<T> where T : new()
    {
        //private static readonly log4net.ILog log =
        //    log4net.LogManager.GetLogger("PMS.Metadata.MetaObjectEnumerator");
        //protected static readonly Type DBNullType = typeof(DBNull);

        private T current = default(T);
        private Class cdesc = null;
        private IProvider provider = null;
        private IDataReader reader = null;

        public MetaObjectEnumerator(Class cdesc, IProvider provider, IDataReader reader)
        {
            this.cdesc = cdesc;
            this.provider = provider;
            this.reader = reader;
        }

        ~MetaObjectEnumerator()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (this.reader != null) {
                this.reader.Close();
                this.reader.Dispose();
                this.reader = null;
            }
        }

        public T Current {
            get {
                if (this.current != null)
                    return this.current;

                return (this.current = CreateObject());
            }
        }

        object System.Collections.IEnumerator.Current {
            get { throw new NotImplementedException(); }
        }

        public bool MoveNext()
        {
            this.current = default(T);

            return this.reader.Read();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        private T CreateObject()
        {
            T obj = new T();

            foreach (Field f in cdesc.Fields) {
                try {
                    f.SetValue(obj, reader[f.Column]);
                } catch (Exception) {
                }
            }

            return obj;
        }
    }
}
