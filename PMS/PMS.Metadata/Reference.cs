using System;
using System.Collections;
using System.Reflection;
using System.Xml.Serialization;

using PMS.Metadata;

namespace PMS.Metadata
{
    [XmlRoot("reference")]
    public sealed class Reference : IXmlSerializable
    {
        public string Field;
        public Type Type;
        public Auto Auto;
        public string Create;
        public string Retrieve;
        public string Update;
        public string Delete;

        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger("PMS.Metadata.ClassRef");

        #region Constructors
        public Reference()
        {
        }

        public Reference(string field, Type type) :
            this(field, type, null, null, null, null)
        {
        }

        public Reference(string field, Type type, string createSql)
            : this(field, type, createSql, null, null, null)
        {
        }

        public Reference(string field, Type type, string createSql, string retrieveSql) 
            : this (field, type, createSql, retrieveSql, null, null)
        {
        }

        public Reference(string field, Type type, 
                         string createSql, 
                         string retrieveSql, 
                         string updateSql)
            : this (field, type, createSql, retrieveSql, updateSql, null)
        {
        }

        public Reference(string field, 
                         Type type, 
                         string createSql, 
                         string retrieveSql, 
                         string updateSql, 
                         string deleteSql)
        {
            this.Field = field;
            this.Type = type;
            this.Create = createSql;
            this.Retrieve = retrieveSql;
            this.Update = updateSql;
            this.Delete = deleteSql;
        }
        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.Name != "reference") {
                log.Error("ReadXml did not find <reference>, but <" + reader.Name + ">");
                return;
            }

            this.Field = reader.GetAttribute("field");
            this.Type = MetaObject.LoadType(reader.GetAttribute("type"));

            string auto = reader.GetAttribute("auto");
            if (auto != null && auto != String.Empty) {
                if (auto.Contains("c")) this.Auto |= Auto.Create;
                if (auto.Contains("r")) this.Auto |= Auto.Retrieve;
                if (auto.Contains("u")) this.Auto |= Auto.Update;
                if (auto.Contains("d")) this.Auto |= Auto.Delete;
            }

            // has children
            if (reader.IsEmptyElement == false) {
                while (reader.Read()) {
                    reader.MoveToElement();

                    if (reader.LocalName == "create")
                        this.Create = reader.ReadElementString();

                    if (reader.LocalName == "retrieve")
                        this.Retrieve = reader.ReadElementString();

                    if (reader.LocalName == "update")
                        this.Update = reader.ReadElementString();

                    if (reader.LocalName == "delete")
                        this.Delete = reader.ReadElementString();

                    if (reader.LocalName == "reference")
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
            string stype = this.Type.FullName + ", ";
            stype += this.Type.Assembly.GetName().Name;

            writer.WriteAttributeString("field", this.Field);
            writer.WriteAttributeString("type", stype);

            if (this.Auto != 0) {
                writer.WriteAttributeString("auto", this.AutoCrud);
            }

            if (this.Create != null && this.Create != String.Empty) {
                writer.WriteElementString("create", this.Create);
            }

            if (this.Retrieve != null && this.Retrieve != String.Empty) {
                writer.WriteElementString("retrieve", this.Retrieve);
            }

            if (this.Update != null && this.Update != String.Empty) {
                writer.WriteElementString("update", this.Update);
            }

            if (this.Delete != null && this.Delete != String.Empty) {
                writer.WriteElementString("delete", this.Delete);
            }
        }

        #endregion

        #region Object Overloads

        ///<summary>
        ///OverLoading == operator
        ///</summary> 
        public static bool operator ==(Reference obj1, Reference obj2)
        {
            if (Object.ReferenceEquals(obj1, obj2)) return true;
            if (Object.ReferenceEquals(obj1, null)) return false;
            if (Object.ReferenceEquals(obj2, null)) return false;

            if (obj1.Field != obj2.Field) {
                return false;
            }

            if (obj1.Type != obj2.Type) {
                return false;
            }

            if (obj1.Create != obj2.Create) {
                return false;
            }

            if (obj1.Retrieve != obj2.Retrieve) {
                return false;
            }

            if (obj1.Update != obj2.Update) {
                return false;
            }

            if (obj1.Delete != obj2.Delete) {
                return false;
            }
            
            if (obj1.Auto != obj2.Auto) {
                return false;
            }

            return true;
        }

        ///<summary>
        ///OverLoading != operator
        ///</summary> 
        public static bool operator !=(Reference obj1, Reference obj2)
        {
            return !(obj1 == obj2);
        }

        ///<summary>
        ///Overriden Equals
        ///</summary> 
        public override bool Equals(object obj)
        {
            if (!(obj is Reference)) return false;

            return this == (Reference)obj;
        }

        ///<summary>
        ///ToString()
        ///</summary> 
        public override string ToString()
        {
            return String.Format("[ Reference (Field={0}) (Type={1}) (Auto={3}) ]",
                                 Field, Type, AutoCrud);
        }

        ///<summary>
        ///Overriden GetHashCode()
        ///</summary> 
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        } 

        #endregion

        private string AutoCrud {
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