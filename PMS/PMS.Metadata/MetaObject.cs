using System;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;

using PMS.Data;

namespace PMS.Metadata
{
    [Serializable]
    internal sealed class MetaObject
    {
        private static readonly log4net.ILog log = 
            log4net.LogManager.GetLogger("PMS.Metadata.MetaObject");

        //private static Hashtable fieldCache = new Hashtable();
        private IProvider provider = null;
		private Class cdesc = null;

        public MetaObject(Type type)
        {
			this.cdesc = RepositoryManager.GetClass(type);
			this.provider = ProviderFactory.Factory(RepositoryManager.CurrentConnection.Type);

			if (this.cdesc == null) {
				throw new Exception("type not found in repository");
			}
        }

        public MetaObject(object obj)
        {
            if (obj == null) {
                throw new NoNullAllowedException("Parameter obj cannot be null");
            }

			this.cdesc = RepositoryManager.GetClass(obj.GetType());
			this.provider = ProviderFactory.Factory(RepositoryManager.CurrentConnection.Type);

			if (this.cdesc == null) {
				throw new Exception("obj.GetType() not found in repository");
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
			
            try {
				if (cdesc.ListType == null) {
                    if (log.IsErrorEnabled)
                        log.Error("MaterializeList:GetClassListType(" + cdesc.Type + ") == NULL");
                    return list;
                }

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

        private object PopulateObject(object obj, IDataReader reader)
        {
			FieldInfo finfo;
			object dbColumn;
			//bool verb = (log.IsDebugEnabled && Environment.GetEnvironmentVariable("PMS_MAPPING") == null) ? false : true;
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

				if (dbColumn == null)
					continue;

				try {
					finfo.SetValue(obj, provider.ConvertToType(f.DbType, dbColumn));
				} catch (Exception e) {
					log.Warn("PopulateObject: Assignment " + f.Name + " >> " + dbColumn + " >> " + f.DbType + " || " + e.Message);
				}

			}

			/**
            Field field;
            string column;

            DataTable table = reader.GetSchemaTable();

            foreach (DataRow row in table.Rows) {
                column = (string)row["ColumnName"];
                field = cdesc.GetFieldByColumn(column);

                if (field != null) {
                    finfo = cdesc.Type.GetField(field.Name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

					if (finfo == null) {
						log.ErrorFormat("Field '{0}' not found for Column '{1}'", field.Name, field.Column);
						continue;
					}

                    try {
                        dbColumn = provider.ConvertToType(field.DbType, reader[column]);
                        finfo.SetValue(obj, dbColumn);

						//if (verb) {
						//    log.DebugFormat("{0} - {1} = {2}", cdesc.Type, finfo.Name, dbColumn);
						//}
                    } catch (Exception) {
                        log.ErrorFormat("Column '{0}' failed to convert from '{1}' to '{2}'",
                                        column, field.DbType, finfo.FieldType.ToString());
                    }
                } else {
					log.WarnFormat("Obj({0}) Type({1}) - Column '{2}' not in repo ({3})", obj.GetType(), cdesc.Type, column, this.mode);
				}
            }
			**/

			//if (verb) {
			//    log.Debug("");
			//}

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

        public static Type LoadType(string fullTypeName)
        {
            // Type
            // Type, Assembly
            // Type, Assembly, Version, Info
            Regex reg = new Regex("^(?<T>[\\d\\w.]+)(,\\s*(?<A>[\\d\\w.]+))?");
            Match match = reg.Match(fullTypeName);

            if (!match.Success) {
                if (log.IsWarnEnabled)
                    log.Warn("LoadType: failed to match regex");

                throw new TypeLoadException("LoadType: failed to match regex - " + fullTypeName);
            }

            Type type = null;
            Assembly ass = null;
            String sType = null;
            String sAssembly = null; ;

            if (match.Groups["T"].Success) {
                sType = match.Groups["T"].Value;
            }

            if (match.Groups["A"].Success) {
                sAssembly = match.Groups["A"].Value;
            }

			try {
				if (!String.IsNullOrEmpty(sAssembly)) {
					if ((ass = Assembly.Load(sAssembly)) != null) {
						if ((type = ass.GetType(sType, false)) != null) {
							return type;
						}
					}
				}
			} catch (Exception) {
				return null;
			}

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
                //Console.WriteLine("a = " + a.GetName().Name);
                if (a.GetName().Name.StartsWith(sType.Split('.')[0])) {
                    //Console.WriteLine("checking " + a.FullName);
                    foreach (Type t in a.GetTypes()) {
                        if (t.FullName == sType) {
                            return t;
                        }
                    }
                }
            }
            
            string msg = sType;
            msg += " not found in AppDomain.CurrentDomain. ";
            msg += "Please add a reference in /repository/assemblies/add/@assembly";
            log.Error(msg);

            throw new TypeLoadException(msg);
        }
    }
}
