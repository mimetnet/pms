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

    [XmlRootAttribute("repository")]
    public class Repository
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
    }
}
