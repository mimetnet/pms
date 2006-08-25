using System;
using System.Collections;
using System.Data;
using System.Xml;
using System.Xml.Serialization;

namespace PMS.Metadata
{
    /// <summary>
    /// ConnectionCollection Connection
    /// </summary>
    [XmlRoot("connections")]
    public class ConnectionCollection : CollectionBase, IXmlSerializable
    {
        ///<summary>
        /// Default constructor.
        ///</summary>
        public ConnectionCollection()
        {
        }
        
        #region CollectionBase Members
		///<summary>
        /// The zero-based index of the element to get or set.
        ///</summary>
        public Connection this[int index] {
            get { return (Connection)this.List[index]; }
            set { this.List[index] = value; }
        }

        ///<summary>
        /// Gets the number of elements contained in the ConnectionCollection.
        ///</summary>
        public new int Count {
            get { return this.List.Count; }
        }

        ///<summary>
        /// Removes all items from the ConnectionCollection.
        ///</summary>
        public new void Clear()
        {
            this.List.Clear();
        }

        ///<summary>
        /// Adds an item to the ConnectionCollection.
        ///</summary>
        public void Add(Connection value)
        {
            this.List.Add(value);
        }

        ///<summary>
        /// Removes the first occurrence of a specific object from the ConnectionCollection.
        ///</summary>
        public void Remove(Connection value)
        {
            this.List.Remove(value);
        }

        ///<summary>
        /// Removes the Connection item at the specified index.
        ///</summary>
        public new void RemoveAt(int index)
        {
            this.List.RemoveAt(index);
        }

        ///<summary>
        /// Inserts an item to the ConnectionCollection at the specified index.
        ///</summary>
        public void Insert(int index, Connection value)
        {
            this.List.Insert(index, value);
        }

        ///<summary>
        /// Determines the index of a specific item in the ConnectionCollection.
        ///</summary>
        public int IndexOf(Connection value)
        {
            return this.List.IndexOf(value);
        }

        ///<summary>
        /// Determines whether the ConnectionCollection contains a specific value.
        ///</summary>
        public bool Contains(Connection value)
        {
            return this.List.Contains(value);
        }

        ///<summary>
        /// Copies elements of the ConnectionCollection to a System.Array, 
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
			return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Connection conn = null;
            XmlSerializer xml = new XmlSerializer(typeof(Connection));

            while (reader.Read()) {
                reader.MoveToElement();

                if (reader.LocalName == "connection")
                    if ((conn = (Connection)xml.Deserialize(reader)) != null)
                        this.List.Add(conn);

                if (reader.LocalName == "connections")
                    return;
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer xml = new XmlSerializer(typeof(Connection));

            foreach (Connection c in this.List)
                xml.Serialize(writer, c);
        }

        #endregion
    }
 
}
