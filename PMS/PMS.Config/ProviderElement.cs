using System;
using System.Xml.Serialization;

namespace PMS.Config
{
    [Serializable]
    [XmlRoot("add")]
    public class ProviderElement : IXmlSerializable
    {
        public string Name;
        public string Type;

        public ProviderElement()
        {
        }

        public ProviderElement(string name, string type)
        {
            this.Name = name;
            this.Type = type;
        }

        #region IXmlSerializable Members
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            this.Name = reader.GetAttribute("name");
            this.Type = reader.GetAttribute("type");
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("name", this.Name);
            writer.WriteAttributeString("type", this.Type);
        }
        #endregion
    }
}
