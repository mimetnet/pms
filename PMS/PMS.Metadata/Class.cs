using System;
using System.Collections;
using System.Reflection;
using System.Xml.Serialization;

using PMS.Metadata;

namespace PMS.Metadata
{  
    public class Class
    {
        [XmlAttributeAttribute("name")]
        public string Name;

        [XmlAttributeAttribute("table")]
        public string Table;

        [XmlArrayItem("field", typeof(Field))]
        [XmlArray("fields")]
        public ArrayList fields = new ArrayList();

        [XmlArrayItem("reference", typeof(Reference))]
        [XmlArray("references")]
        public ArrayList references = new ArrayList();

        [XmlArrayItem("collection", typeof(Collection))]
        [XmlArray("collections")]
        public ArrayList collections = new ArrayList();

        public Class()
        {
        }

        public Class(string name, string table, ArrayList fields)
        {
            this.Name = name;
            this.Table = table;
            this.fields = fields;
        }

        public Class(string name, string table, Field[] fieldsArray)
        {
            this.Name = name;
            this.Table = table;

            for (int x = 0; x < fieldsArray.Length; x++) {
                fields.Add(fieldsArray[x]);
            }
        }

        [XmlIgnore]
        public Field[] PrimaryKeys {
            get {
                ArrayList list = new ArrayList();
                foreach (Field field in Fields)
                    if (field.PrimaryKey)
                        list.Add(field);

                return (Field[])fields.ToArray(typeof(Field));
            }
        }

        [XmlIgnore]
        public Field[] Fields {
            get { return (Field[])fields.ToArray(typeof(Field)); }
            set {
                for (int x=0; x < value.Length; x++) {
                    fields.Add(value[x]);
                }
            }
        }

        [XmlIgnore]
        public Collection[] Collections {
            get { return (Collection[])collections.ToArray(typeof(Collection));  }
        }

        [XmlIgnore]
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
    }
}
