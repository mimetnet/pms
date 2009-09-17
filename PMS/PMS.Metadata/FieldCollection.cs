using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace PMS.Metadata
{
	[Serializable]
    [XmlRoot("fields")]
    public class FieldCollection : List<Field>, IXmlSerializable
    {
        public FieldCollection()
        {
        }

        public Field this[string name] {
            get { 
                if (!String.IsNullOrEmpty(name))
                    foreach (Field f in this)
                        if (0 == StringComparer.InvariantCulture.Compare(f.Name, name))
                            return f;

                return null;
            }
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
			return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.LocalName != "fields")
                throw new InvalidOperationException("ReadXml expected <fields/>, but found <" + reader.LocalName + "/> " + reader.LocalName);

            if (reader.IsEmptyElement)
                return;

            //Console.WriteLine("Fields.Enter: {0} {1}", reader.LocalName, reader.NodeType);

            XmlSerializer xml = new XmlSerializer(typeof(Field));
            if (reader.ReadToDescendant("field")) {
                do {
                    //Console.WriteLine("Fields.Middle(1): {0} {1}", reader.LocalName, reader.NodeType);
                    this.Add((Field) xml.Deserialize(reader));
                    //Console.WriteLine("Fields.Middle(2): {0} {1}\n", reader.LocalName, reader.NodeType);
                } while (reader.ReadToNextSibling("field"));
            }

            //Console.WriteLine("Fields.Exit: {0} {1}\n", reader.LocalName, reader.NodeType);
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer xml = new XmlSerializer(typeof(Field));

            foreach (Field f in this)
                xml.Serialize(writer, f);
        }
    }
}
