using System;
using System.Xml.Serialization;
using System.Collections;

namespace PMS.Metadata
{
    [XmlRoot("repository")]
    public sealed class Repository : IXmlSerializable
    {
        public bool GenerateTypes = false;
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

            c.GenerateTypes = (a.GenerateTypes || b.GenerateTypes);

            if (a.Classes != null)
                foreach (Class classDesc in a.Classes)
                    c.Classes.Add(classDesc);

            if (b.Classes != null)
                foreach (Class classDesc in b.Classes)
                    c.Classes.Add(classDesc);

            if (a.Connections != null)
                foreach (Connection connDesc in a.Connections)
                    c.Connections.Add(connDesc);

            if (b.Connections != null)
                foreach (Connection connDesc in b.Connections)
                    c.Connections.Add(connDesc);

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

            if (obj1.GenerateTypes != obj2.GenerateTypes)
                return false;

            if (obj1.Connections.Count != obj2.Connections.Count) {
                return false;
            }

            for (int x = 0; x < obj1.Connections.Count; x++) {
                if (obj1.Connections[x] != obj2.Connections[x]) {
                    return false;
                }
            }

            if (obj1.Classes.Count != obj2.Classes.Count) {
                return false;
            }

            for (int x = 0; x < obj1.Classes.Count; x++) {
                if (obj1.Classes[x] != obj2.Classes[x]) {
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
            return String.Format("[ Repository Connections=({1}) Classes=({2}) ]",
                                 Connections.Count, Classes.Count);
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
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            string gt = null;

            while (reader.Read()) {
                reader.MoveToElement();

                switch (reader.LocalName) {
                    case "generate-types":
                        if ((gt = reader.ReadString()) != null && gt.Length > 0)
                            Boolean.TryParse(gt, out this.GenerateTypes);
                    break;

                    case "assemblies":
                        this.Assemblies.ReadXml(reader);
                        break;

                    case "connections":
                        this.Connections.ReadXml(reader);
                        break;

                    case "classes":
                        this.Classes.ReadXml(reader);
                        break;
                }
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            if (this.GenerateTypes)
                writer.WriteElementString("generate-types", "true");

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
