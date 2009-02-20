using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Serialization;

namespace PMS.Metadata
{
    [XmlRoot("connections")]
    public class ConnectionCollection : List<Connection>, IXmlSerializable
    {
        public ConnectionCollection()
        {
        }
        
        #region IXmlSerializable Members
        public System.Xml.Schema.XmlSchema GetSchema()
        {
			return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Connection conn = null;
            XmlSerializer xml = new XmlSerializer(typeof(Connection));

            reader.Read();

            while (reader.NodeType != XmlNodeType.EndElement) {
                if ((conn = (Connection) xml.Deserialize(reader)) != null && conn.IsValid)
                    this.Add(conn);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer xml = new XmlSerializer(typeof(Connection));

            foreach (Connection c in this)
                xml.Serialize(writer, c);
        }
        #endregion
    }
 
}
