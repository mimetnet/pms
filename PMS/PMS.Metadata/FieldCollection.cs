using System;
using System.Collections;
using System.Data;
using System.Xml;
using System.Xml.Serialization;

namespace PMS.Metadata
{
    /// <summary>
    /// FieldCollection Class
    /// </summary>
    [XmlRoot("fields")]
    public class FieldCollection : CollectionBase, IXmlSerializable
    {
        #region CollectionBase Members
        ///<summary>
        /// Default constructor.
        ///</summary>
        public FieldCollection()
        {
        }

        ///<summary>
        /// The zero-based index of the element to get or set.
        ///</summary>
        public Field this[int index]
        {
            get { return (Field)this.List[index]; }
            set { this.List[index] = value; }
        }

        ///<summary>
        /// Gets the number of elements contained in the FieldCollection.
        ///</summary>
        public new int Count
        {
            get { return this.List.Count; }
        }

        ///<summary>
        /// Removes all items from the FieldCollection.
        ///</summary>
        public new void Clear()
        {
            this.List.Clear();
        }

        ///<summary>
        /// Adds an item to the FieldCollection.
        ///</summary>
        public void Add(Field value)
        {
            this.List.Add(value);
        }

        ///<summary>
        /// Removes the first occurrence of a specific object from the FieldCollection.
        ///</summary>
        public void Remove(Field value)
        {
            this.List.Remove(value);
        }

        ///<summary>
        /// Removes the Field item at the specified index.
        ///</summary>
        public new void RemoveAt(int index)
        {
            this.List.RemoveAt(index);
        }

        ///<summary>
        /// Inserts an item to the FieldCollection at the specified index.
        ///</summary>
        public void Insert(int index, Field value)
        {
            this.List.Insert(index, value);
        }

        ///<summary>
        /// Determines the index of a specific item in the FieldCollection.
        ///</summary>
        public int IndexOf(Field value)
        {
            return this.List.IndexOf(value);
        }

        ///<summary>
        /// Determines whether the FieldCollection contains a specific value.
        ///</summary>
        public bool Contains(Field value)
        {
            return this.List.Contains(value);
        }

        ///<summary>
        /// Copies elements of the FieldCollection to a System.Array, 
        /// starting at a particular System.Array index.
        ///</summary>
        public void CopyTo(Array array, int index)
        {
            this.List.CopyTo(array, index);
        } 
        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            Field field = null;
            XmlSerializer xml = new XmlSerializer(typeof(Field));

            while (reader.Read()) {
                reader.MoveToElement();

                if (reader.LocalName == "field")
                    if ((field = (Field)xml.Deserialize(reader)) != null)
                        this.List.Add(field);

                if (reader.LocalName == "fields")
                    break;
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer xml = new XmlSerializer(typeof(Field));

            foreach (Field f in this.List)
                xml.Serialize(writer, f);
        }

        #endregion
    }
}
