using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace PMS.Metadata
{
    /// <summary>
    /// FieldCollection Class
    /// </summary>
	[Serializable]
    [XmlRoot("fields")]
    public class FieldCollection : List<Field>, IXmlSerializable
    {
        ///<summary>
        /// Default constructor.
        ///</summary>
        public FieldCollection()
        {
        }

        ///<summary>
        /// The zero-based index of the element to get or set.
        ///</summary>
        public Field this[string name] {
            get { 
                foreach (Field f in this)
                    if (f.Name == name)
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
            while (reader.Read()) {
                reader.MoveToElement();

				switch (reader.LocalName) {
					case "field":
						this.Add(new Field(reader));
						break;
					case "fields":
						return;
				}
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer xml = new XmlSerializer(typeof(Field));

            foreach (Field f in this)
                xml.Serialize(writer, f);
        }
    }
}
