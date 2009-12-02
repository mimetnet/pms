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

            Type dbType;
            FieldInfo finfo;
            Object dbColumn;

			//DateTime now = DateTime.Now;

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

				if (dbColumn == null || DBNull.Value.Equals(dbColumn))
					continue;

                if (typeof(DBNull) == (dbType = dbColumn.GetType()))
                    continue;

				//Console.WriteLine("Field '{0}' ({1}) => {2}", f.Name, dbColumn.GetType().ToString(), dbColumn);

				try {
                    if (finfo.FieldType == dbType) {
                        finfo.SetValue(obj, dbColumn);
                    } else {
                        finfo.SetValue(obj, provider.ConvertToType(f.DbType, dbColumn));
                    }
				} catch (Exception e) {
					log.Warn("PopulateObject: Assignment " + cdesc.Table + "." + f.Name + " >> " + dbColumn + " >> " + f.DbType + " || " + e.Message);
				}
            }

            return obj;
        }
    }
}
