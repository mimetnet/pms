using System;
using System.Collections;
using System.Data;
using System.Reflection;

using PMS.Data;

namespace PMS.Metadata
{
    [Serializable]
    internal sealed class MetaObject
    {
        private static readonly log4net.ILog log = 
            log4net.LogManager.GetLogger("PMS.Metadata.MetaObject");

        private IProvider provider = null;
		private Class cdesc = null;

        public MetaObject(Type type)
        {
			this.cdesc = RepositoryManager.GetClass(type);
			this.provider = RepositoryManager.CurrentConnection.Provider;

			if (this.cdesc == null) {
				throw new Exception("type '" + type + "' not found in repository");
			}
        }

        public MetaObject(object obj)
        {
            if (obj == null) {
                throw new NoNullAllowedException("Parameter obj cannot be null");
            }

			this.cdesc = RepositoryManager.GetClass(obj.GetType());
			this.provider = RepositoryManager.CurrentConnection.Provider;

			if (this.cdesc == null) {
				throw new Exception("obj.GetType(" + obj.GetType() + ") not found in repository");
			}
        }

        public bool Exists {
			get { return (cdesc != null) ? true : false; }
        }

        public Type Type {
            get { return this.cdesc.Type; }
        }

        /// <summary>
        /// Converts IDataReader into instantiated object
        /// </summary>
        /// <param name="reader">IDataReader containing SQL SELECT results</param>
        /// <returns>Instantiated Object</returns>
        public object Materialize(IDataReader reader)
        {
            if (reader.Read()) {
                return PopulateObject(CreateObject(), reader);
            }

            return null;
        }

        /// <summary>
        /// Convert IDataReader to object[]
        /// </summary>
        /// <param name="reader">List of results to create objects from</param>
        /// <returns>Empty or full Object[] list</returns>
        public object[] MaterializeArray(IDataReader reader)
        {
            ArrayList list = new ArrayList();

            try {
                while (reader.Read())
                    list.Add(PopulateObject(CreateObject(), reader));
            } catch (Exception e) {
                if (log.IsErrorEnabled)
                    log.Error("MaterializeList", e);
            }

            return (object[])list.ToArray(cdesc.Type);
        }

        /// <summary>
        /// Convert IDataReader to IList of Objects
        /// </summary>
        /// <param name="reader">List of Results to create objects from</param>
        /// <returns>IList is null if list-type attribute is invalid, otherwise its always an IList</returns>
        public IList MaterializeList(IDataReader reader)
        {
            IList list = null;
			
			if (cdesc.ListType == null) {
				throw new RepositoryDefinitionException("DbEngine.RetrieveList requires a valid @list-type for <class/> for " + cdesc.Type);
			}


            try {
				list = (IList)Activator.CreateInstance(cdesc.ListType);
            } catch (Exception e) {
                if (log.IsErrorEnabled)
                    log.Error("MaterializeList:GetClassListType", e);
                return list;
            }

            try {
                while (reader.Read()) {
                    list.Add(PopulateObject(CreateObject(), reader));
                }
            } catch (Exception e) {
                if (log.IsErrorEnabled)
                    log.Error("MaterializeList:reader.Read()", e);
            }

            return list;
        }

        private object CreateObject()
        {
			return Activator.CreateInstance(cdesc.Type);
        }

        private Object PopulateObject(Object obj, IDataReader reader)
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

            return obj;
        }
    }
}
