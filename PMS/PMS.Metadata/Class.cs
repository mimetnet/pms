using System;
using System.Collections;
using System.Reflection;
using System.Xml.Serialization;

using PMS.Metadata;

namespace PMS.Metadata
{  
    public class Class : IXmlSerializable
    {
        public string Name;
        public string Table;

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ArrayList fields = new ArrayList();
        private ArrayList references = new ArrayList();
        private ArrayList collections = new ArrayList();
        private Type listType = null;

        public Class()
        {
        }

        public Class(Type type, string table, ArrayList fields)
        {
            this.Name = type.ToString();
            this.Table = table;
            this.fields = fields;
        }

        public Class(Type type, string table, Field[] fieldsArray)
        {
            this.Name = type.ToString();
            this.Table = table;

            for (int x = 0; x < fieldsArray.Length; x++) {
                fields.Add(fieldsArray[x]);
            }
        }

        public Class(Type type, string table, ArrayList fields, Type listType)
        {
            this.Name = type.ToString();
            this.Table = table;
            this.fields = fields;
            this.ListType = listType;
        }

        public Class(Type type, string table, Field[] fieldsArray, Type listType)
        {
            this.Name = type.ToString();
            this.Table = table;
            this.ListType = listType;

            for (int x = 0; x < fieldsArray.Length; x++) {
                fields.Add(fieldsArray[x]);
            }
        }

        public Field[] PrimaryKeys {
            get {
                ArrayList list = new ArrayList();
                foreach (Field field in Fields)
                    if (field.PrimaryKey)
                        list.Add(field);

                return (Field[])fields.ToArray(typeof(Field));
            }
        }

        public Field[] Fields {
            get { return (Field[])fields.ToArray(typeof(Field)); }
            set {
                for (int x=0; x < value.Length; x++) {
                    fields.Add(value[x]);
                }
            }
        }

        public Type ListType {
            get { return this.listType; }
            set { this.listType = value; }
        }

        public Collection[] Collections {
            get { return (Collection[])collections.ToArray(typeof(Collection));  }
        }

        public Reference[] References {
            get { return (Reference[])references.ToArray(typeof(Reference)); }
        }

        public string GetColumnByField(string fieldName)
        {
            foreach (Field field in Fields)
                if (field.Name.Equals(fieldName))
                    return field.Column;
            
            return null;
        }

        public string GetFieldByColumn(string colName)
        {
            foreach (Field field in Fields)
                if (field.Column.Equals(colName))
                    return field.Name;
            
            return null;
        }

        public Field GetField(string name)
        {
            foreach (Field field in Fields)
                if (field.Name.Equals(name))
                    return field;
            
            return null;
        }

        public Field GetField(FieldInfo fieldInfo)
        {
            foreach (Field field in Fields)
                if (field.Name.Equals(fieldInfo.Name))
                    return field;
            
            return null;
        }

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.Name != "class") {
                log.Error("Class:ReadXml did not find <class> tag, but <" + reader.Name + "> instead");
                return;
            }

            this.Name = reader.GetAttribute("name");
            this.Table = reader.GetAttribute("table");

            //if (log.IsDebugEnabled) {
            //    log.Debug("<class name=" + this.Name + " table=" + this.Table + ">");
            //}

            while (reader.Read()) {
                reader.MoveToElement();

                if (reader.LocalName == "list") {
                    if (reader.HasAttributes) {
                        this.listType = LoadType(reader["type"], reader["assembly"]);
                    }
                } else if (reader.LocalName == "field") {
                    Field f = new Field(reader["name"], reader["column"], reader["db_type"]);

                    string pk = reader["primarykey"];
                    string ig = reader["ignore_default"];

                    if (pk != null && (pk == "true" || pk == "True" || pk == "TRUE" || pk == "1")) {
                        f.PrimaryKey = true;
                    }

                    if (ig != null && (ig == "true" || ig == "True" || ig == "TRUE" || ig == "1")) {
                        f.IgnoreDefault = true;
                    }
                    
                    fields.Add(f);

                    //if (log.IsDebugEnabled) {
                    //    log.Debug("\t<field name=" + f.Name + " column=" + f.Column + " db_type=" + f.DbType + " primarykey=" + f.PrimaryKey + " ignore_default=" + f.IgnoreDefault + " >");
                    //}

                    f = null;
                } else {
                    // WEIRDNESS
                    // break out on new <class> element so not instance can handle it
                    if (reader.LocalName == "class" && reader.IsStartElement("class"))
                        return;

                }
            }
        }

        private Type LoadType(string stype, string sassembly)
        {
            if (sassembly == null || sassembly == String.Empty) {
                if (log.IsInfoEnabled)
                    log.Info("Class:LoadType @assembly is empty or missing");
                return null;
            }

            if (stype == null || stype == String.Empty) {
                if (log.IsInfoEnabled)
                    log.Info("Class:LoadType @type is empty or missing");
                return null;
            }

            try {
                return Assembly.Load(sassembly).GetType(stype, false);
            } catch (Exception e) {
                if (log.IsInfoEnabled)
                    log.Info("Class:LoadType", e);
                return null;
            }
        }

        /// <summary>
        /// Write XML to stream
        /// </summary>
        /// <param name="writer">XmlWriter</param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("name", this.Name);
            writer.WriteAttributeString("table", this.Table);

            if (listType != null) {
                writer.WriteStartElement("list");
                writer.WriteAttributeString("type", listType.FullName);
                writer.WriteAttributeString("assembly", listType.Assembly.FullName);
                writer.WriteEndElement();
            }

            writer.WriteStartElement("fields");

            foreach (Field f in this.Fields) {
                writer.WriteStartElement("field");
                writer.WriteAttributeString("name", f.Name);
                writer.WriteAttributeString("column", f.Column);
                writer.WriteAttributeString("db_type", f.DbType);
                writer.WriteAttributeString("ignore_default", f.IgnoreDefault.ToString().ToLower());
                writer.WriteAttributeString("primarykey", f.PrimaryKey.ToString().ToLower());
                writer.WriteEndElement();
            }

            writer.WriteEndElement(); // end fields
        }

        #endregion

        #region ObjectOverloads

        ///<summary>
        ///OverLoading == operator
        ///</summary> 
        public static bool operator ==(Class obj1, Class obj2)
        {
            if (Object.ReferenceEquals(obj1, obj2)) return true;
            if (Object.ReferenceEquals(obj1, null)) return false;
            if (Object.ReferenceEquals(obj2, null)) return false;

            if (obj1.Name != obj2.Name) return false;
            if (obj1.Table != obj2.Table) return false;
            if (obj1.ListType != obj2.ListType) return false;
            if (obj1.PrimaryKeys.Length != obj2.PrimaryKeys.Length) return false;
            if (obj1.Fields.Length != obj2.Fields.Length) return false;

            return true;
        }

        ///<summary>
        ///OverLoading != operator
        ///</summary> 
        public static bool operator !=(Class obj1, Class obj2)
        {
            return !(obj1 == obj2);
        }

        ///<summary>
        ///Overriden Equals
        ///</summary> 
        public override bool Equals(object obj)
        {
            if (!(obj is Class)) return false;

            return this == (Class)obj;
        }

        ///<summary>
        ///ToString()
        ///</summary> 
        public override string ToString()
        {
            return String.Format("[Class (Name={0}) (Table={1}) (Type={2})]", Name, Table, ListType);
        }

        ///<summary>
        ///Overriden GetHashCode()
        ///</summary> 
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        } 

        #endregion
    }
}
