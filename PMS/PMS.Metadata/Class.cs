using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

using PMS.Metadata;

namespace PMS.Metadata
{
    [XmlRoot("class")]
    [Serializable]
    public sealed class Class : IXmlSerializable
    {
        public Type Type;
        public FieldCollection Fields = new FieldCollection();
        public string Table;

        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger("PMS.Metadata.Class");

        #region Constructors
        public Class()
        {
        }

        public Class(System.Xml.XmlReader reader)
        {
            this.ReadXml(reader);
        }

        public Class(Type type, string table, FieldCollection fields)
        {
            this.Type = type;
            this.Table = table;
            this.Fields = fields;
            this.LoadCTypes();
        }

        public Class(Type type)
        {
            this.Type = type;
            this.Table = Generator.CamelToCString(type.Name);
            this.Fields = Generator.GenerateFields(type);
            this.LoadCTypes();
        }
        #endregion

        public bool HasReferences {
            get {
                foreach (Field field in Fields)
                    if (field.HasReference)
                        return true;

                return false;
            }
        }

        #region Methods

        public Field GetFieldByColumn(string colName)
        {
            if (!String.IsNullOrEmpty(colName))
                foreach (Field field in Fields)
                    if (field.Column.Equals(colName))
                        return field;

            return null;
        }

        public Field this[int index] {
            get { return Fields[index]; }
            set { Fields[index] = value; }
        }

        public Field this[string fieldName] {
            get { return Fields[fieldName]; }
        }

        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (!(reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "class"))
                throw new InvalidOperationException("ReadXml expected <class/>, but found <" + reader.LocalName + "/>");

            this.Type = PMS.Util.TypeLoader.Load(reader.GetAttribute("type"));
            this.Table = reader.GetAttribute("table");

            //Console.WriteLine("Class.Enter: {0} {2} {1}", reader.LocalName, reader.NodeType, this.Table);

            if (this.Table == "order")
                this.Table = "\"" + this.Table + "\"";

            if (reader.IsEmptyElement)
                return;

            XmlSerializer xml = new XmlSerializer(typeof(FieldCollection));
            if (reader.ReadToDescendant("fields"))
                this.Fields = (FieldCollection) xml.Deserialize(reader);

#if NET_2_0
            reader.Read();
#endif
            this.LoadCTypes();

            //Console.WriteLine("Class.Exit: {0} {1}", reader.LocalName, reader.NodeType);
        }

        public void WriteXml(XmlWriter writer)
        {
			if (this.Type != null) {
				writer.WriteAttributeString("type", this.Type.FullName + ", " + this.Type.Assembly.GetName().ToString());
			}

            writer.WriteAttributeString("table", this.Table);

            XmlSerializer xml = new XmlSerializer(typeof(Field));

            writer.WriteStartElement("fields");
            foreach (Field f in this.Fields) {
                xml.Serialize(writer, f);
            }
            writer.WriteEndElement();
        }
        #endregion

        private void LoadCTypes()
        {
            if (null == this.Type)
                return;

            List<Field> erase = new List<Field>();

            foreach (Field f in this.Fields) {
                try {
                    if (!f.Load(this.Type))
                        erase.Add(f);
                } catch (Exception e) {
                    log.Error("LoadCTypes(" + Type + "): ", e);
                }
            }

            foreach (Field f in erase) {
                log.WarnFormat("Removing \"{0}:{1}\" - member not found", Type.ToString(), f.Name);
                this.Fields.Remove(f);
            }
        }

        #region Object Overloads
        ///<summary>
        ///OverLoading == operator
        ///</summary> 
        public static bool operator ==(Class obj1, Class obj2)
        {
            if (Object.ReferenceEquals(obj1, obj2)) return true;
            if (Object.ReferenceEquals(obj1, null)) return false;
            if (Object.ReferenceEquals(obj2, null)) return false;

            if (obj1.Type != obj2.Type) {
                return false;
            }

            if (obj1.Table != obj2.Table) {
                return false;
            }

            if (obj1.Fields.Count != obj2.Fields.Count) {
                return false;
            }

            for (int y = 0; y < obj2.Fields.Count; y++) {
                if (obj1.Fields[y] != obj2.Fields[y]) {
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
            return String.Format("[ Class (Name={0}) (Table={1}) (Fields={2}) ]",
                                 Type, Table, Fields.Count);
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
