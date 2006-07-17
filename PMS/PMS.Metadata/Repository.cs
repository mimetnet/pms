using System;
using System.Xml.Serialization;
using System.Collections;

namespace PMS.Metadata
{
    public enum DbManagerMode : short 
    { 
        [XmlEnum("Single")] Single = 0,
        [XmlEnum("Replication")] Replication = 1
    };

    [XmlRoot("repository")]
    public sealed class Repository //: IXmlSerializable
    {
        [XmlElementAttribute("dbmanager-mode", typeof(DbManagerMode))]
        public DbManagerMode DbManagerMode;

        [XmlArray("connections")]
        [XmlArrayItem("connection", typeof(Connection))]
        public ArrayList Connections = new ArrayList();

        [XmlArray("classes")]
        [XmlArrayItem("class", typeof(Class))]
        public ArrayList Classes = new ArrayList();

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

        /**
        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            
        }

        #endregion
        **/
    }
}
