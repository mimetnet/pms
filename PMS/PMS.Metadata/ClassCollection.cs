using System;
using System.Collections;
using System.Data;
using System.Xml;
using System.Xml.Serialization;

namespace PMS.Metadata
{
    /// <summary>
    /// ClassCollection Class
    /// </summary>
    [XmlRoot("classes")]
    public class ClassCollection : CollectionBase, IXmlSerializable
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger("PMS.Metadata.ClassCollection");

        ///<summary>
        /// Default constructor.
        ///</summary>
        public ClassCollection()
        {
        }

        #region CollectionBase Members
        ///<summary>
        /// The zero-based index of the element to get or set.
        ///</summary>
        public Class this[int index]
        {
            get { return (Class)this.List[index]; }
            set { this.List[index] = value; }
        }

        ///<summary>
        /// Gets the number of elements contained in the ClassCollection.
        ///</summary>
        public new int Count
        {
            get { return this.List.Count; }
        }

        ///<summary>
        /// Removes all items from the ClassCollection.
        ///</summary>
        public new void Clear()
        {
            this.List.Clear();
        }

        ///<summary>
        /// Adds an item to the ClassCollection.
        ///</summary>
        public void Add(Class value)
        {
            this.List.Add(value);
        }

        ///<summary>
        /// Removes the first occurrence of a specific object from the ClassCollection.
        ///</summary>
        public void Remove(Class value)
        {
            this.List.Remove(value);
        }

        ///<summary>
        /// Removes the Class item at the specified index.
        ///</summary>
        public new void RemoveAt(int index)
        {
            this.List.RemoveAt(index);
        }

        ///<summary>
        /// Inserts an item to the ClassCollection at the specified index.
        ///</summary>
        public void Insert(int index, Class value)
        {
            this.List.Insert(index, value);
        }

        ///<summary>
        /// Determines the index of a specific item in the ClassCollection.
        ///</summary>
        public int IndexOf(Class value)
        {
            return this.List.IndexOf(value);
        }

        ///<summary>
        /// Determines whether the ClassCollection contains a specific value.
        ///</summary>
        public bool Contains(Class value)
        {
            return this.List.Contains(value);
        }

        ///<summary>
        /// Copies elements of the ClassCollection to a System.Array, 
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
            Class klass = null;
            XmlSerializer xml = new XmlSerializer(typeof(Class));

            while (reader.Read()) {
                reader.MoveToElement();

                if (reader.LocalName == "class") {
                    try {
                        if ((klass = (Class)xml.Deserialize(reader)) != null)
                            this.List.Add(klass);
                    } catch (Exception) {}
                }

                if (reader.LocalName == "classes")
                    return;
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer xml = new XmlSerializer(typeof(Class));

            foreach (Class c in this.List)
                xml.Serialize(writer, c);
        }

        #endregion
    }
 
}
