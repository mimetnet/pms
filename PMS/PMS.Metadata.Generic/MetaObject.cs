using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace PMS.Metadata.Generic
{
	using PMS.Data;
	using PMS.Query;

    [Serializable]
    internal sealed class MetaObject <T> where T : new()
    {
        private static readonly log4net.ILog log = 
            log4net.LogManager.GetLogger("PMS.Metadata.MetaObject");

        private IProvider provider = null;
		private Class cdesc = null;

        public MetaObject(Class cdesc, IProvider provider)
        {
			this.cdesc = cdesc;
			this.provider = provider;

			if (this.cdesc == null) {
				throw new Exception("type '" + typeof(T) + "' not found in repository");
			}
        }

        public bool Exists {
			get { return (cdesc != null); }
        }

        public Type Type {
            get { return this.cdesc.Type; }
        }

        public T Materialize(IDataReader reader)
        {
            if (reader.Read())
                return PopulateObject(CreateObject(), reader, null);

            return default(T);
        }

        public T[] MaterializeArray(IDataReader reader)
		{
			return this.MaterializeArray(reader, null);
		}

        public T[] MaterializeArray(IDataReader reader, QueryCallback<T> callback)
        {
            List<T> list = new List<T>();

            try {
                while (reader.Read())
                    list.Add(PopulateObject(CreateObject(), reader, callback));
            } catch (Exception e) {
                if (log.IsErrorEnabled)
                    log.Error("MaterializeArray: ", e);
            }

            return list.ToArray();
        }

        public TList MaterializeList<TList>(IDataReader reader) where TList : IList<T>, new()
		{
			return this.MaterializeList<TList>(reader, null);
		}

        public TList MaterializeList<TList>(IDataReader reader, QueryCallback<T> callback) where TList : IList<T>, new()
        {
            TList list = default(TList);
            
            try {
				list = (TList) Activator.CreateInstance(typeof(TList));
            } catch (Exception e) {
                if (log.IsErrorEnabled)
                    log.Error("MaterializeList:GetClassListType", e);
                return list;
            }

            try {
                while (reader.Read()) {
                    list.Add(PopulateObject(CreateObject(), reader, callback));
                }
            } catch (Exception e) {
                if (log.IsErrorEnabled)
                    log.Error("MaterializeList:reader.Read()", e);
            }

            return list;
        }

        private T CreateObject()
        {
			return (T) Activator.CreateInstance<T>();
        }

        private T PopulateObject(T obj, IDataReader reader, QueryCallback<T> callback)
        {
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

				if (dbColumn == null || dbColumn.GetType() == typeof(DBNull))
					continue;

				//Console.WriteLine("Field '{0}' ({1}) => {2}", f.Name, dbColumn.GetType().ToString(), dbColumn);

				try {
					finfo.SetValue(obj, provider.ConvertToType(f.DbType, dbColumn));
				} catch (Exception e) {
					log.Warn("PopulateObject: Assignment " + cdesc.Table + "." + f.Name + " >> " + dbColumn + " >> " + f.DbType + " || " + e.Message);
				}
			}

            //foreach (Field f in cdesc.Fields) {
            //    if (f.HasReference) {
            //        if ((f.Reference.Auto & Auto.Retrieve) != 0) {
            //            PMS.Query.IQuery q =
            //                    new PMS.Query.QueryBySql(f.Reference.Type,
            //                                             f.Reference.Sql,
            //                                             obj);
            //            Console.WriteLine(q.Select());
            //        }
            //    }
            //}
			
			//Console.WriteLine("CREATE TIME = " + (DateTime.Now - now));
			if (callback != null) {
				try {
					callback(obj);
				} catch (Exception ce) {
					log.Warn("PopulateObject: Callback failure => ", ce);
				}
			}

            return obj;
        }

        
    }
}
