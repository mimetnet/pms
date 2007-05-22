using System;
using System.Collections;
using System.Reflection;
using System.Xml.Schema;
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
        public Type ListType = null;
        public string Table;

        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger("PMS.Metadata.Class");

        #region Constructors
        public Class()
        {
        }

        public Class(Type type, string table, FieldCollection fields)
        {
            this.Type = type;
            this.Table = table;
            this.Fields = fields;
        }

        public Class(Type type, string table, FieldCollection fields, Type listType)
        {
            this.Type = type;
            this.Table = table;
            this.Fields = fields;
            this.ListType = listType;
        }
        #endregion

        public bool HasReferences {
            get {
                for (int x = 0; x < this.Fields.Count; x++)
                    if (this.Fields[x].HasReference)
                        return true;

                return false;
            }
        }

        #region Methods

        public Field GetFieldByColumn(string colName)
        {
            foreach (Field field in Fields)
                if (field.Column.Equals(colName))
                    return field;

            return null;
        }

		public object GetValue(Field field, Object obj)
		{
			FieldInfo finfo = Type.GetField(field.Name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

			if (finfo != null) {
				return finfo.GetValue(obj);
			}

			return null;
		}

        public Field this[int index] 
		{
            get { return Fields[index]; }
            set { Fields[index] = value; }
        }

        public Field this[string fieldName] 
		{
            get { return Fields[fieldName]; }
        }

        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
			return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.Name != "class") {
                log.Error("ReadXml did not find <class> tag, but <" + reader.Name + "> instead");
                return;
            }

            this.Type = MetaObject.LoadType(reader.GetAttribute("type"));
            this.Table = reader.GetAttribute("table");

            string ltype = reader.GetAttribute("list-type");
			if (!String.IsNullOrEmpty(ltype)) {
				try {
					this.ListType = MetaObject.LoadType(ltype);
				} catch (Exception e) {
					log.Error("ReadXml: " + e.Message);
				}
			}

            XmlSerializer xml = new XmlSerializer(typeof(FieldCollection));

            while (reader.Read()) {
                reader.MoveToElement();

                switch (reader.LocalName) {
                    case "fields":
                        this.Fields = (FieldCollection)xml.Deserialize(reader);
                        break;

                    case "class":
                        return;
                }
            }
        }

        /// <summary>
        /// Write XML to stream
        /// </summary>
        /// <param name="writer">XmlWriter</param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            string stype;

            stype = this.Type.FullName + ", ";
            stype += this.Type.Assembly.GetName().Name;

            writer.WriteAttributeString("type", stype);
            writer.WriteAttributeString("table", this.Table);

            if (this.ListType != null) {
                stype = this.ListType.FullName + ", ";
                stype += this.ListType.Assembly.GetName().Name;
                writer.WriteAttributeString("list-type", stype);
            }

            XmlSerializer xml = new XmlSerializer(typeof(Field));

            writer.WriteStartElement("fields");
            foreach (Field f in this.Fields) {
                xml.Serialize(writer, f);
            }

            writer.WriteEndElement(); // end fields
        }

        #endregion

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
                Console.WriteLine("1");
                return false;
            }

            if (obj1.Table != obj2.Table) {
                Console.WriteLine("2");
                return false;
            }

            if (obj1.ListType != obj2.ListType) {
                Console.WriteLine("3");
                return false;
            }

            if (obj1.Fields.Count != obj2.Fields.Count) {
                Console.WriteLine("f1.Count {0} f2.Count {1}", obj1.Fields.Count, obj2.Fields.Count);
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
