using System;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;

namespace PMS.Metadata
{
    [XmlRoot("field")]
    [Serializable]
    public sealed class Field : IXmlSerializable
    {
        //private static readonly log4net.ILog log =
        //    log4net.LogManager.GetLogger("PMS.Metadata.Field");

        public string Name;
        public string Column;
        public string DbType;
        public object Default;
        public string DefaultDb;
        public bool Unique = false;
        public bool PrimaryKey = false;
        public bool IgnoreDefault = true;
        public Reference Reference = null;
        public Type CType = null;

        public bool HasReference {
            get { return (Reference != null); }
        }

        #region Constructors
        public Field()
        {
        }

        public Field(System.Xml.XmlReader reader)
        {
            ReadXml(reader);
        }

        public Field(string name, string column, string dbType)
            : this(name, column, dbType, false, false, null)
        {
        }

        public Field(string name, string column, string dbType, Reference classReference)
            : this(name, column, dbType, false, false, classReference)
        {
        }

        public Field(string name, string column, string dbType, bool ignoreDefault)
            : this(name, column, dbType, ignoreDefault, false, null)
        {
        }

        public Field(string name, string column, string dbType, bool ignoreDefault, Reference classReference)
            : this(name, column, dbType, ignoreDefault, false, classReference)
        {
        }

        public Field(string name, string column, string dbType, bool ignoreDefault, bool iskey)
            : this(name, column, dbType, ignoreDefault, iskey, null)
        {
        }

        public Field(string name, string column, string dbType, bool ignoreDefault, bool iskey, Reference classReference)
        {
            this.Name = name;
            this.Column = column;
            this.DbType = dbType;
            this.PrimaryKey = iskey;
            this.IgnoreDefault = ignoreDefault;
            this.Reference = classReference;
        }
        #endregion

        #region ObjectOverloads

        public static bool operator ==(Field obj1, Field obj2)
        {
            if (Object.ReferenceEquals(obj1, obj2)) return true;
            if (Object.ReferenceEquals(obj1, null)) return false;
            if (Object.ReferenceEquals(obj2, null)) return false;

            if (obj1.Name != obj2.Name)
                return false;

            if (obj1.Column != obj2.Column)
                return false;

            if (obj1.DbType != obj2.DbType)
                return false;

            if (obj1.PrimaryKey != obj2.PrimaryKey)
                return false;

            if (obj1.IgnoreDefault != obj2.IgnoreDefault)
                return false;

            if (obj1.Reference != obj2.Reference)
                return false;

            return true;
        }

        public static bool operator !=(Field obj1, Field obj2)
        {
            return !(obj1 == obj2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Field)) return false;

            return this == (Field)obj;
        }

        public override string ToString()
        {
            //return String.Format("[ Field (Name={0}) (Column={1}) (DbType={2}) (PrimaryKey={3}) (IgnoreDefault={4}) ]", Name, Column, DbType, PrimaryKey, IgnoreDefault);

            return String.Format("{0} {1} {2} {3}", //{4} == NOT NULL
                this.Column,
                this.DbType,
                (this.PrimaryKey) ? "PRIMARY KEY" : String.Empty,
                (this.Unique && !this.PrimaryKey) ? "UNIQUE" : String.Empty);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        #endregion

        internal void LoadType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (Default == null) {
                if (type.IsEnum) {
                    Default = Enum.ToObject(type, Enum.GetValues(type).GetValue(0));
                } else if (!type.IsAbstract && !type.IsInterface && !type.IsClass) {
                    Default = Activator.CreateInstance(type);
                }
            } else {
                if ((CType = type).IsPrimitive) {
                    if (type == typeof(Boolean))
                        Default = Convert.ToBoolean(Default);
                    else if (type == typeof(Byte))
                        Default = Convert.ToByte(Default);
                    else if (type == typeof(SByte))
                        Default = Convert.ToSByte(Default);
                    else if (type == typeof(Int16))
                        Default = Convert.ToInt16(Default);
                    else if (type == typeof(UInt16))
                        Default = Convert.ToUInt16(Default);
                    else if (type == typeof(Int32))
                        Default = Convert.ToInt32(Default);
                    else if (type == typeof(UInt32))
                        Default = Convert.ToUInt32(Default);
                    else if (type == typeof(Int64))
                        Default = Convert.ToInt64(Default);
                    else if (type == typeof(UInt64))
                        Default = Convert.ToUInt64(Default);
                    else if (type == typeof(Char))
                        Default = Convert.ToChar(Default);
                    else if (type == typeof(Double))
                        Default = Convert.ToDouble(Default);
                    else if (type == typeof(Single))
                        Default = Convert.ToSingle(Default);
                } else if (type.IsEnum) {
                    Default = Enum.Parse(type, Default.ToString(), true);
                } else if (!type.IsAbstract && !type.IsInterface && !type.IsClass) {
                    Default = Activator.CreateInstance(type);
                }
            }
        }

#region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            if (!(reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "field"))
                throw new InvalidOperationException("ReadXml expected <field/>, but found <" + reader.LocalName + "/>");

            Name = reader.GetAttribute("name");
            Column = reader.GetAttribute("column");
            DbType = reader.GetAttribute("db_type");
            Default = reader.GetAttribute("default");

            //Console.WriteLine("Field.Enter: {0} ({2}) {1}", reader.LocalName, reader.NodeType, Name);

            string tmp = null;

            if (!String.IsNullOrEmpty(tmp = reader.GetAttribute("primarykey"))) {
                if (Compare(tmp, "true")) PrimaryKey = true;
            } else if (!String.IsNullOrEmpty(tmp = reader.GetAttribute("primary"))) {
                if (Compare(tmp, "true")) PrimaryKey = true;
            }

            if (!String.IsNullOrEmpty(tmp = reader.GetAttribute("unique")))
                if (Compare(tmp, "true")) Unique = true;

            if (!String.IsNullOrEmpty(tmp = reader.GetAttribute("ignore_default"))) {
                if (Compare(tmp, "false")) IgnoreDefault = false;
            } else if (!String.IsNullOrEmpty(tmp = reader.GetAttribute("not-null"))) {
                if (Compare(tmp, "false")) IgnoreDefault = false;
            } else if (!String.IsNullOrEmpty(tmp = reader.GetAttribute("nullable"))) {
                if (Compare(tmp, "false")) IgnoreDefault = false;
            }

            if ((DefaultDb = reader.GetAttribute("default_db")) == null)
                DefaultDb = "null";

            if (reader.IsEmptyElement == false)
                reader.Read();

            //Console.WriteLine("Field.Exit: {0} {1}", reader.LocalName, reader.NodeType);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("name", this.Name);
            writer.WriteAttributeString("column", this.Column);
            writer.WriteAttributeString("db_type", this.DbType);

            if (!IgnoreDefault)
                writer.WriteAttributeString("nullable", "false");

            if (Default != null)
                writer.WriteAttributeString("default", Default.ToString());

            if (PrimaryKey)
                writer.WriteAttributeString("primary", "true");

            if (Unique)
                writer.WriteAttributeString("unique", "true");

            if (this.Reference != null) {
                XmlSerializer xml = new XmlSerializer(typeof(Reference));
                xml.Serialize(writer, this.Reference);
            }
        }

#endregion

        private bool Compare(string a, string b)
        {
            return 0 == StringComparer.InvariantCultureIgnoreCase.Compare(a, b);
        }
    }
}
