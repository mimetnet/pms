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
        private static readonly log4net.ILog log = 
            log4net.LogManager.GetLogger("PMS.Metadata.MetaObjectEnumerator");
        protected static readonly Type DBNullType = typeof(DBNull);

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

            Type dbType = null;
            FieldInfo finfo = null;
            Object dbColumn = null;

            foreach (Field f in cdesc.Fields) {

                //Console.WriteLine("----------");
                //Console.WriteLine(f);

                finfo = cdesc.Type.GetField(f.Name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

                if (finfo == null) {
                    log.ErrorFormat("Field '{0}' not found for Column '{1}'", f.Name, f.Column);
                    continue;
                }

                try {
                    dbColumn = reader[f.Column];
                } catch (Exception) {
                    continue;
                }

                if (null == dbColumn || DBNull.Value.Equals(dbColumn))
                    continue;

                if (DBNullType == (dbType = dbColumn.GetType()))
                    continue;

                //Console.WriteLine("Field '{0}' ({1}) => {2}", f.Name, dbColumn.GetType().ToString(), dbColumn);

                if (finfo.FieldType == dbType && finfo.FieldType.IsInstanceOfType(dbColumn)) {
                    try {
                        finfo.SetValue(obj, dbColumn);
                    } catch (Exception e) {
                        log.Warn("PopulateObject: Unexpected Error -> ", e);
                    }
                } else {
                    try {
                        finfo.SetValue(obj, Type.DefaultBinder.ChangeType(dbColumn, finfo.FieldType, null));
                    } catch (Exception e) {
                        log.Warn("PopulateObject: Failed to set "+typeof(T).Name+"."+f.Name+"::"+finfo.FieldType.Name+" = "+dbColumn+"::"+dbType.Name+" ("+f.Column+"::"+f.DbType+")", e);
                    }
                }
            }

            return obj;
        }
    }
}
