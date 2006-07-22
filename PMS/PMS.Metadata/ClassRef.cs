using System;
using System.Collections;
using System.Reflection;
using System.Xml.Serialization;

using PMS.Metadata;

namespace PMS.Metadata
{
    [XmlRoot("class-ref")]
    public class ClassRef : IXmlSerializable
    {
        public string Field;
        public string RefClassType;
        public string Sql;
        public Auto Auto = Auto.Create | Auto.Retrieve | Auto.Delete;

        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger("PMS.Metadata.ClassRef");

        #region Constructors
        public ClassRef()
        {
        }

        public ClassRef(string field, Type refClassType, string sql)
        {
            this.Field = field;
            this.RefClassType = refClassType.ToString();
            this.Sql = sql;
        }
        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.Name != "class-ref") {
                log.Error("ReadXml did not find <class-ref>, but <" + reader.Name + ">");
                return;
            }

            this.Field = reader.GetAttribute("field");
            this.RefClassType = reader.GetAttribute("ref-class-name");

            string auto = reader.GetAttribute("auto");
            if (auto != null && auto != String.Empty) {
                if (auto.Contains("c")) this.Auto |= Auto.Create;
                if (auto.Contains("r")) this.Auto |= Auto.Retrieve;
                if (auto.Contains("u")) this.Auto |= Auto.Update;
                if (auto.Contains("d")) this.Auto |= Auto.Delete;
            }

            this.Sql = reader.ReadString();
        }

        /// <summary>
        /// Write XML to stream
        /// </summary>
        /// <param name="writer">XmlWriter</param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("field", this.Field);
            writer.WriteAttributeString("ref-class-name", this.RefClassType);

            if (this.Auto != 0) {
                writer.WriteAttributeString("auto", this.AutoCrud);
            }

            if (this.Sql != null && this.Sql != String.Empty) {
                writer.WriteValue(this.Sql);
            }
        }

        #endregion

        #region ObjectOverloads

        ///<summary>
        ///OverLoading == operator
        ///</summary> 
        public static bool operator ==(ClassRef obj1, ClassRef obj2)
        {
            if (Object.ReferenceEquals(obj1, obj2)) return true;
            if (Object.ReferenceEquals(obj1, null)) return false;
            if (Object.ReferenceEquals(obj2, null)) return false;

            if (obj1.Field != obj2.Field) return false;
            if (obj1.RefClassType != obj2.RefClassType) return false; 
            if (obj1.Sql != obj2.Sql) return false;
            if (obj1.Auto != obj2.Auto) return false;

            return true;
        }

        ///<summary>
        ///OverLoading != operator
        ///</summary> 
        public static bool operator !=(ClassRef obj1, ClassRef obj2)
        {
            return !(obj1 == obj2);
        }

        ///<summary>
        ///Overriden Equals
        ///</summary> 
        public override bool Equals(object obj)
        {
            if (!(obj is ClassRef)) return false;

            return this == (ClassRef)obj;
        }

        ///<summary>
        ///ToString()
        ///</summary> 
        public override string ToString()
        {
            return String.Format("[ ClassRef (Field={0}) (RefClass={1}) (Sql={2}) (Auto={3}) ]",
                                 Field, RefClassType, Sql, AutoCrud);
        }

        ///<summary>
        ///Overriden GetHashCode()
        ///</summary> 
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        } 

        #endregion

        public string AutoCrud {
            get {
                if (Auto != 0) {
                    string crud = String.Empty;
                    if ((Auto & Auto.Create) != 0) crud += "c";
                    if ((Auto & Auto.Retrieve) != 0) crud += "r";
                    if ((Auto & Auto.Update) != 0) crud += "u";
                    if ((Auto & Auto.Delete) != 0) crud += "d";
                    return crud;
                }

                return null;
            }
        }
    }
}