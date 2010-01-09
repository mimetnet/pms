using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace PMS.Config
{
    internal class ProviderElementList : List<ProviderElement>, IXmlSerializable
    {
        #region IXmlSerializable Members
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            ProviderElement e = null;
            XmlSerializer xml = new XmlSerializer(typeof(ProviderElement));

            reader.Read();

            while (reader.NodeType != XmlNodeType.EndElement) {
                if ((e = (ProviderElement) xml.Deserialize(reader)) != null)
                    this.Add(e);
            }

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer xml = new XmlSerializer(typeof(ProviderElement));

            foreach (ProviderElement e in this)
                xml.Serialize(writer, e);
        }
        #endregion
    }
}
