using System;
using System.Collections;
using System.Reflection;
using System.Xml.Serialization;

using PMS.Metadata;

namespace PMS.Metadata
{
    [XmlRoot("class")]
    public class Class : IXmlSerializable
    {
        public string Type;
        public string Table;
        private FieldCollection fields = new FieldCollection();
        private Type listType = null;

        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger("PMS.Metadata.Class");

        #region Constructors
        public Class()
        {
        }

        public Class(Type type, string table, FieldCollection fields)
        {
            this.Type = type.ToString();
            this.Table = table;
            this.fields = fields;
        }

        public Class(Type type, string table, FieldCollection fields, Type listType)
        {
            this.Type = type.ToString();
            this.Table = table;
            this.fields = fields;
            this.ListType = listType;
        }
        #endregion

        #region Properties

        public FieldCollection Fields {
            get { return fields; }
            set { fields = value; }
        }

        public Type ListType {
            get { return this.listType; }
            set { listType = value; }
        }
        #endregion

        #region Methods
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
        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.Name != "class") {
                log.Error("ReadXml did not find <class> tag, but <" + reader.Name + "> instead");
                return;
            }

            this.Type = reader.GetAttribute("type");
            this.Table = reader.GetAttribute("table");

            XmlSerializer xml = new XmlSerializer(typeof(FieldCollection));

            while (reader.Read()) {
                reader.MoveToElement();

                if (reader.LocalName == "list") {
                    string stype = reader.GetAttribute("type");
                    string sassembly = reader.GetAttribute("assembly");
                    this.ListType = this.LoadType(stype, sassembly);
                }

                if (reader.LocalName == "fields")
                    this.Fields = (FieldCollection)xml.Deserialize(reader);

                if (reader.LocalName == "class")
                    break;
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
            writer.WriteAttributeString("type", this.Type);
            writer.WriteAttributeString("table", this.Table);

            if (listType != null) {
                writer.WriteStartElement("list");
                writer.WriteAttributeString("type", listType.FullName);
                writer.WriteAttributeString("assembly", listType.Assembly.GetName().Name);
                writer.WriteEndElement();
            }

            XmlSerializer xml = new XmlSerializer(typeof(Field));

            writer.WriteStartElement("fields");
            foreach (Field f in this.Fields) {
                xml.Serialize(writer, f);
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

            if (obj1.Type != obj2.Type) return false;
            if (obj1.Table != obj2.Table) return false;
            if (obj1.ListType != obj2.ListType) return false;
            if (obj1.Fields.Count != obj2.Fields.Count) return false;

            FieldCollection fc1 = obj1.Fields;
            FieldCollection fc2 = obj2.Fields;

            if (fc1.Count != fc2.Count) {
                if (log.IsDebugEnabled)
                    log.DebugFormat("fc1.Count({0}) != fc2.Count({1})", fc1.Count, fc2.Count);
                return false;
            }

            for (int y = 0; y < fc1.Count; y++) {
                if (fc1[y] != fc2[y]) {
                    if (log.IsDebugEnabled)
                        log.Debug("fc1 != fc2");
                    return false;
                }
            }

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
            return String.Format("[ Class (Name={0}) (Table={1}) (Type={2}) (Fields={3}) ]",
                                 Type, Table, ListType, Fields.Count);
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
