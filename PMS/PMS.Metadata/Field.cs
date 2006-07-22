using System;
using System.Collections;
using System.Xml.Serialization;

namespace PMS.Metadata
{
    [XmlRoot("field")]
    public class Field : IXmlSerializable
    {
        public string Name;
        public string Column;
        public string DbType;
        public bool PrimaryKey = false;
        public bool IgnoreDefault = false;
        public ClassRef ClassReference = null;

        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger("PMS.Metadata.Class");

        #region Constructors
        public Field()
        {
        }

        public Field(string name, string column, string dbType)
            : this(name, column, dbType, false, false, null)
        {
        }

        public Field(string name, string column, string dbType, ClassRef classReference)
            : this(name, column, dbType, false, false, classReference)
        {
        }

        public Field(string name, string column, string dbType, bool ignoreDefault)
            : this(name, column, dbType, ignoreDefault, false, null)
        {
        }

        public Field(string name, string column, string dbType, bool ignoreDefault, ClassRef classReference)
            : this(name, column, dbType, ignoreDefault, false, classReference)
        {
        }

        public Field(string name, string column, string dbType, bool ignoreDefault, bool iskey)
            : this(name, column, dbType, ignoreDefault, iskey, null)
        {
        }

        public Field(string name, string column, string dbType, bool ignoreDefault, bool iskey, ClassRef classReference)
        {
            this.Name = name;
            this.Column = column;
            this.DbType = dbType;
            this.PrimaryKey = iskey;
            this.IgnoreDefault = ignoreDefault;
            this.ClassReference = classReference;
        }
        #endregion

        #region ObjectOverloads

        ///<summary>
        ///OverLoading == operator
        ///</summary> 
        public static bool operator ==(Field obj1, Field obj2)
        {
            if (Object.ReferenceEquals(obj1, obj2)) return true;
            if (Object.ReferenceEquals(obj1, null)) return false;
            if (Object.ReferenceEquals(obj2, null)) return false;

            if (obj1.Name != obj2.Name) {
                return false;
            }

            if (obj1.Column != obj2.Column) {
                return false;
            }

            if (obj1.DbType != obj2.DbType) {
                return false;
            }
            
            if (obj1.PrimaryKey != obj2.PrimaryKey) {
                return false;
            }
            
            if (obj1.IgnoreDefault != obj2.IgnoreDefault) {
                return false;
            }
            
            if (obj1.ClassReference != obj2.ClassReference) {
                return false;
            }

            return true;
        }

        ///<summary>
        ///OverLoading != operator
        ///</summary> 
        public static bool operator !=(Field obj1, Field obj2)
        {
            return !(obj1 == obj2);
        }

        ///<summary>
        ///Overriden Equals
        ///</summary> 
        public override bool Equals(object obj)
        {
            if (!(obj is Field)) return false;

            return this == (Field)obj;
        }

        ///<summary>
        ///ToString()
        ///</summary> 
        public override string ToString()
        {
            return String.Format("[ Field (Name={0}) (Column={1}) (DbType={2}) (PrimaryKey={3}) (IgnoreDefault={4}) ]", Name, Column, DbType, PrimaryKey, IgnoreDefault);
        }

        ///<summary>
        ///Overriden GetHashCode()
        ///</summary> 
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.Name != "field") {
                log.Error("ReadXml did not find <field> tag, but <" + reader.Name + "> instead");
                return;
            }

            this.Name = reader.GetAttribute("name");
            this.Column = reader.GetAttribute("column");
            this.DbType = reader.GetAttribute("db_type");
            this.PrimaryKey = Convert.ToBoolean(reader.GetAttribute("primarykey"));
            this.IgnoreDefault = Convert.ToBoolean(reader.GetAttribute("ignore_default"));

            if (reader.IsEmptyElement == false) {
                if (reader.Read()) {
                    reader.MoveToElement();
                    XmlSerializer xml = new XmlSerializer(typeof(ClassRef));
                    this.ClassReference = (ClassRef)xml.Deserialize(reader);
                }
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("name", this.Name);
            writer.WriteAttributeString("column", this.Column);
            writer.WriteAttributeString("db_type", this.DbType);

            if (this.IgnoreDefault)
                writer.WriteAttributeString("ignore_default", 
                                            this.IgnoreDefault.ToString().ToLower());
        
            if (this.PrimaryKey)
                writer.WriteAttributeString("primarykey", 
                                            this.PrimaryKey.ToString().ToLower());

            if (this.ClassReference != null) {
                XmlSerializer xml = new XmlSerializer(typeof(ClassRef));
                xml.Serialize(writer, this.ClassReference);
            }
        }

        #endregion
    }
}
