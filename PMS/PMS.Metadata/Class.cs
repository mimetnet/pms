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
                Console.WriteLine("ReadXml did not find <class> tag, but <{0}> instead", reader.Name);
                return;
            }

            this.Name = reader.GetAttribute("name");
            this.Table = reader.GetAttribute("table");

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
            if (sassembly == null || sassembly == String.Empty)
                return null;

            if (stype == null || stype == String.Empty)
                return null;

            try {
                return Assembly.Load(sassembly).GetType(stype, false);
            } catch (Exception) {
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
    }
}
