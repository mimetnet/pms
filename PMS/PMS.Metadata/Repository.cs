using System;
using System.Xml.Serialization;
using System.Collections;

namespace PMS.Metadata
{
    public enum DbManagerMode : short
    { 
        [XmlEnum("single")] Single = 0,
        [XmlEnum("replication")] Replication = 1
    };

    [XmlRoot("repository")]
    public sealed class Repository : IXmlSerializable
    {
        public DbManagerMode DbManagerMode = DbManagerMode.Single;
        public AssemblyCollection Assemblies = new AssemblyCollection();
        public ConnectionCollection Connections = new ConnectionCollection();
        public ClassCollection Classes = new ClassCollection();

        #region ObjectOverloads

        /// <summary>
        /// Overload addition of repositories
        /// </summary>
        /// <param name="a">Repository A</param>
        /// <param name="b">Repository B</param>
        /// <returns>new Repository containing A and B</returns>
        public static Repository operator+(Repository a, Repository b)
        {
            Repository c = new Repository();
            
            if (a.Classes != null) {
                foreach (Class classDesc in a.Classes) {
                    c.Classes.Add(classDesc);
                }
            }

            if (b.Classes != null) {
                foreach (Class classDesc in b.Classes) {
                    c.Classes.Add(classDesc);
                }
            }

            if (a.Connections != null) {
                foreach (Connection connDesc in a.Connections) {
                    c.Connections.Add(connDesc);
                }
            }

            if (b.Connections != null) {
                foreach (Connection connDesc in b.Connections) {
                    c.Connections.Add(connDesc);
                }
            }

            return c;
        }

        ///<summary>
        ///OverLoading == operator
        ///</summary> 
        public static bool operator ==(Repository obj1, Repository obj2)
        {
            if (Object.ReferenceEquals(obj1, obj2)) return true;
            if (Object.ReferenceEquals(obj1, null)) return false;
            if (Object.ReferenceEquals(obj2, null)) return false;

            if (obj1.Connections.Count != obj2.Connections.Count) {
                return false;    
            }

            Connection cn1, cn2;

            for (int x = 0; x < obj1.Connections.Count; x++) {
                cn1 = obj1.Connections[x];
                cn2 = obj2.Connections[x];
                if (cn1 != cn2) {
                    return false;
                }
            }

            if (obj1.Classes.Count != obj2.Classes.Count) {
                return false;
            }

            Class c1, c2;

            for (int x = 0; x < obj1.Classes.Count; x++) {
                c1 = obj1.Classes[x];
                c2 = obj2.Classes[x];
                if (c1 != c2) {
                    return false;
                }
            }

            return true;
        }

        ///<summary>
        ///OverLoading != operator
        ///</summary> 
        public static bool operator !=(Repository obj1, Repository obj2)
        {
            return !(obj1 == obj2);
        }

        ///<summary>
        ///Overriden Equals
        ///</summary> 
        public override bool Equals(object obj)
        {
            if (!(obj is Repository)) return false;

            return this == (Repository)obj;
        }

        ///<summary>
        ///ToString()
        ///</summary> 
        public override string ToString()
        {
            return String.Format("[ Repository (DbManager={0}) (Connections={1}) (Classes={2}) ]",
                                 DbManagerMode, Connections.Count, Classes.Count);
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
            XmlSerializer xml = null;

            while (reader.Read()) {
                reader.MoveToElement();

                switch (reader.LocalName) {
                    case "dbmanager-mode":
                        if (reader.ReadString().ToLower() == "single")
                            this.DbManagerMode = DbManagerMode.Single;
                        break;

                    case "assemblies":
                        xml = new XmlSerializer(typeof(AssemblyCollection));
                        this.Assemblies = (AssemblyCollection)xml.Deserialize(reader);
                        break;

                    case "connections":
                        xml = new XmlSerializer(typeof(ConnectionCollection));
                        this.Connections = (ConnectionCollection)xml.Deserialize(reader);
                        break;

                    case "classes":
                        xml = new XmlSerializer(typeof(ClassCollection));
                        this.Classes = (ClassCollection)xml.Deserialize(reader);
                        break;
                }

                //if (reader.LocalName == "dbmanager-mode") {
                //    string mode = reader.ReadString();
                //    if (mode.ToLower() == "single") {
                //        this.DbManagerMode = DbManagerMode.Single;
                //    }
                //}

                //if (reader.LocalName == "connections") {
                //    xml = new XmlSerializer(typeof(ConnectionCollection));
                //    this.Connections = (ConnectionCollection)xml.Deserialize(reader);
                //}

                //if (reader.LocalName == "classes") {
                //    xml = new XmlSerializer(typeof(ClassCollection));
                //    this.Classes = (ClassCollection)xml.Deserialize(reader);
                //}
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("dbmanager-mode");
            writer.WriteValue(this.DbManagerMode.ToString());
            writer.WriteEndElement();

            XmlSerializer xml = null;

            if (Assemblies.Count > 0) {
                xml = new XmlSerializer(typeof(AssemblyCollection));
                xml.Serialize(writer, this.Assemblies);
            }

            if (Connections.Count > 0) {
                xml = new XmlSerializer(typeof(ConnectionCollection));
                xml.Serialize(writer, this.Connections);
            }

            if (Classes.Count > 0) {
                xml = new XmlSerializer(typeof(ClassCollection));
                xml.Serialize(writer, this.Classes);
            }
        }

        #endregion
    }
}
