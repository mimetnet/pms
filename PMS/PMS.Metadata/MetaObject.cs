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
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static Hashtable fieldCache = new Hashtable();
        private IProvider provider = null;
        private Object obj = null;
        private Type type = null;

        public MetaObject()
        {
        }

        public MetaObject(Type type)
        {
            this.type = type;
        }

        public MetaObject(object obj)
        {
            this.obj = obj;
            
            if (this.obj == null) {
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
                    provider = 
                        PMS.Data.ProviderFactory.Factory(RepositoryManager.CurrentConnection.Type);

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
        /// <returns>IList is null if list/@type is invalid, otherwise its always an IList</returns>
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

                list = (IList) Activator.CreateInstance(listType);
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
            Class classDesc = RepositoryManager.GetClass(type);
            string column, sField, cType;
            FieldInfo field;
            object dbColumn;

            DataTable table = reader.GetSchemaTable();

            foreach (DataRow row in table.Rows) {
                column = (string) row["ColumnName"];
                sField = classDesc.GetFieldByColumn(column);
                cType = GetColumnType(column);
                
                if ((sField != String.Empty || sField != null) && cType != null) {
                    field = type.GetField(sField,
                                          BindingFlags.NonPublic | 
                                          BindingFlags.Instance | 
                                          BindingFlags.Public);
                    try {
                        dbColumn = Provider.ConvertToType(cType, reader[column]);
                        field.SetValue(obj, dbColumn);
                    } catch (Exception) {
						Console.WriteLine("Column '" + column + "' failed to convert from '" + cType + "' to '" + field.FieldType+ "'");
                    }
                }
            }

            return obj;
        }

        /** PopulateObject2
        private object PopulateObject2(object obj, IDataReader reader)
        {
            Class classDesc = RepositoryManager.GetClass(type);
            FieldInfo[] fields = TypeFields;
            string column;
            object dbColumn;

            for (int i=0; i < fields.Length; i++) {
                column = classDesc.GetColumnByField(fields[i].Name);
                if (column == null)
                    continue;

                try {
                    dbColumn = Provider.ConvertToType(GetColumnType(column), 
                                                      reader[column]);

                    fields[i].SetValue(obj, dbColumn);
                } catch (Exception e) {
                    Console.WriteLine("Column failed to convert : \n" + e);
                }
            }
            
            return obj;
        }
        **/


        public FieldInfo[] TypeFields {
            get {
                if (fieldCache.ContainsKey(type.ToString()))
                    return (FieldInfo[]) fieldCache[type.ToString()];

                FieldInfo[] _fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                fieldCache[type.ToString()] = _fields;

                return _fields;
            }
        }

        public string Table {
            get {
                return RepositoryManager.GetClass(type).Table;
            }
        }

        public string[] Columns {
            get {
                ArrayList columns = new ArrayList();
                Class classDesc = RepositoryManager.GetClass(type);

                foreach (Field field in classDesc.Fields) {
                    if (field.PrimaryKey == false) {
                        columns.Add(field.Column);
                    }
                }
                
                return (string[])columns.ToArray(typeof(string));
            }            
        }

        public string[] PrimaryKeys {
            get {
                ArrayList keys = new ArrayList();
                Class cdesc = RepositoryManager.GetClass(type);

                foreach (Field field in cdesc.Fields) {
                    if (field.PrimaryKey == true) {
                        keys.Add(field.Column);
                    }
                }
                
                return (string[])keys.ToArray(typeof(string));
            }
        }

        public bool IsColumnDefaultIgnored(string colName)
        {
            Class cdesc = RepositoryManager.GetClass(type);

            foreach (Field field in cdesc.Fields) {
                if (field.Column.Equals(colName)) {
                    return field.IgnoreDefault;
                }
            }

            return false;
        }

        public object GetColumnValue(string colName)
        {
            Class classDesc = RepositoryManager.GetClass(type);
            string name = classDesc.GetFieldByColumn(colName);
            foreach (FieldInfo field in TypeFields) {
                if (field.Name.Equals(name))
                    return field.GetValue(obj);
            }

            return null;
        }

        public string GetColumnType(string column)
        {
            Class cdesc = RepositoryManager.GetClass(type);

            if (cdesc == null)
                return null;

            foreach (Field field in cdesc.Fields) {
                if (field.Column.Equals(column)) {
                    return field.DbType;
                }
            }

            return null;
        }

        public Field GetField(string name)
        {
            return RepositoryManager.GetField(type, name);
        }

        public string GetSqlValue(string column)
        {
            string dbType = GetColumnType(column);
            object val = GetColumnValue(column);

            return Provider.PrepareSqlValue(dbType, val);
        }
        
        public bool IsFieldSet(string column)
        {
            return IsFieldSet(column, this.GetColumnValue(column));
        }

        public bool IsFieldSet(string column, object value)
        {
            //Console.WriteLine("   Column: " + column);
            //Console.WriteLine("    Value: " + value);

            if (value != null) {
                string ctype = GetColumnType(column);
                object init = Provider.GetTypeInit(ctype);

                //Console.WriteLine("   DbType: " + ctype);
                //Console.WriteLine("  Default: " + init);
                //Console.WriteLine("   Ignore: " + IsColumnDefaultIgnored(column));
                //Console.WriteLine("IsIgnored: " + value.Equals(init));
                //Console.WriteLine();

                if (IsColumnDefaultIgnored(column) && value.Equals(init)) {
                    return false;
                }
                
                return true;
            }

            //Console.WriteLine();

            return false;
        }
    }
}
