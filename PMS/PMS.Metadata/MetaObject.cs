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

        private static Hashtable fieldCache = new Hashtable();
        private IProvider provider = null;
        private Object obj = null;
        private Type type = null;

        public MetaObject(Type type)
        {
            this.type = type;
        }

        public MetaObject(object obj)
        {
            
            if ((this.obj = obj) == null) {
                throw new NoNullAllowedException("Parameter obj cannot be null");
            }

            this.type = obj.GetType();
        }

        public bool Exists {
            get { return RepositoryManager.Exists(this.type); }
        }

        public IProvider Provider {
            get {
                if (provider == null)
                    provider = ProviderFactory.Factory(RepositoryManager.CurrentConnection.Type);

                return provider;
            }
        }

        public object BaseObject {
            get { return this.obj; }
        }

        public Type Type {
            get { return this.type; }
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

            return (object[])list.ToArray(type);
        }

        /// <summary>
        /// Convert IDataReader to IList of Objects
        /// </summary>
        /// <param name="reader">List of Results to create objects from</param>
        /// <returns>IList is null if list-type attribute is invalid, otherwise its always an IList</returns>
        public IList MaterializeList(IDataReader reader)
        {
            Type listType = null;
            IList list = null;

            try {
                listType = RepositoryManager.GetClassListType(this.type);

                if (listType == null) {
                    if (log.IsErrorEnabled)
                        log.Error("MaterializeList:GetClassListType(" + type + ") == NULL");
                    return list;
                }

                list = (IList)Activator.CreateInstance(listType);
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
            return Activator.CreateInstance(type);
        }

        private object PopulateObject(object obj, IDataReader reader)
        {
            Type type = obj.GetType();
            Class cdesc = RepositoryManager.GetClass(type);
            FieldInfo finfo;
            Field field;
            object dbColumn;
            string column;

            DataTable table = reader.GetSchemaTable();

            foreach (DataRow row in table.Rows) {
                column = (string)row["ColumnName"];
                field = cdesc.GetFieldByColumn(column);

                if (field != null) {
                    finfo = type.GetField(field.Name,
                                          BindingFlags.NonPublic |
                                          BindingFlags.Instance |
                                          BindingFlags.Public);

                    try {
                        dbColumn = Provider.ConvertToType(field.DbType, reader[column]);
                        finfo.SetValue(obj, dbColumn);
                    } catch (Exception) {
                        log.ErrorFormat("Column '{0}' failed to convert from '{1}' to '{2}'",
                                        column, field.DbType, finfo.FieldType.ToString());
                    }
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

            return obj;
        }

        private FieldInfo[] TypeFields {
            get {
                if (fieldCache.ContainsKey(type.ToString()))
                    return (FieldInfo[])fieldCache[type.ToString()];

                FieldInfo[] _fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                fieldCache[type.ToString()] = _fields;

                return _fields;
            }
        }

        public string Table  {
            get {
                return RepositoryManager.GetClass(type).Table;
            }
        }

        public FieldCollection Columns  {
            get {
                FieldCollection list = new FieldCollection();

                foreach (Field field in RepositoryManager.GetClass(type).Fields) {
                    if (field.PrimaryKey == false) {
                        list.Add(field);
                    }
                }

                return list;
            }
        }

        public FieldCollection PrimaryKeys {
            get {
                FieldCollection keys = new FieldCollection();

                foreach (Field field in RepositoryManager.GetClass(type).Fields) {
                    if (field.PrimaryKey == true) {
                        keys.Add(field);
                    }
                }

                return keys;
            }
        }

        public object GetColumnValue(Field field)
        {
            foreach (FieldInfo fi in this.TypeFields)
                if (fi.Name == field.Name)
                    return fi.GetValue(obj);

            return null;
        }

        public string GetSqlValue(Field field)
        {
            return Provider.PrepareSqlValue(field.DbType, this.GetColumnValue(field));
        }

        public bool IsFieldSet(Field field)
        {
            return IsFieldSet(field, GetColumnValue(field));
        }

        public bool IsFieldSet(Field field, object value)
        {
            //Console.WriteLine("   Column: " + field.Column);
            //Console.WriteLine("    Value: " + value);

            if (value != null) {

                //Console.WriteLine("   DbType: " + field.DbType);
                //Console.WriteLine("  Default: " + init);
                //Console.WriteLine("   Ignore: " + field.IgnoreDefault);
                //Console.WriteLine("IsIgnored: " + value.Equals(init));
                //Console.WriteLine();

                if (field.IgnoreDefault && value.Equals(Provider.GetTypeInit(field.DbType)))
                    return false;

                return true;
            }

            return false;
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
            string sType = null;
            string sAssembly = null; ;

            if (match.Groups["T"].Success) {
                sType = match.Groups["T"].Value;
            }

            if (match.Groups["A"].Success) {
                sAssembly = match.Groups["A"].Value;
            }

            if (sAssembly != null && sAssembly != String.Empty) {
                if ((ass = Assembly.Load(sAssembly)) != null) {
                    if ((type = ass.GetType(sType, false)) != null) {
                        return type;
                    }
                }
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
