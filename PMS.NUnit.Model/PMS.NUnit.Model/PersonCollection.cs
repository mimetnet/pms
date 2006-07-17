using System;
using System.Collections;
using System.Data;
using System.Xml;
using System.Xml.Serialization;

namespace PMS.NUnit.Model
{
    /// <summary>
    /// PersonCollection Class
    /// </summary>
    [XmlRoot("persons")]
    public class PersonCollection : CollectionBase
    {
        ///<summary>
        /// Default constructor.
        ///</summary>
        public PersonCollection()
        {
        }
        
        ///<summary>
        /// The zero-based index of the element to get or set.
        ///</summary>
        public Person this[int index]
        {
            get
            {
                return (Person)this.List[index];
            }
            set
            {
                this.List[index] = value;
            }
        }

        ///<summary>
        /// Gets the number of elements contained in the PersonCollection.
        ///</summary>
        public new int Count
        {
            get
            {
                return this.List.Count;
            }
        }

        ///<summary>
        /// Removes all items from the PersonCollection.
        ///</summary>
        public new void Clear()
        {
            this.List.Clear();
        }

        ///<summary>
        /// Adds an item to the PersonCollection.
        ///</summary>
        public void Add(Person value)
        {
            this.List.Add(value);
        }

        ///<summary>
        /// Removes the first occurrence of a specific object from the PersonCollection.
        ///</summary>
        public void Remove(Person value)
        {
            this.List.Remove(value);
        }

        ///<summary>
        /// Removes the PersonCollection item at the specified index.
        ///</summary>
        public new void RemoveAt(int index)
        {
            this.List.RemoveAt(index);
        }

        ///<summary>
        /// Inserts an item to the PersonCollection at the specified index.
        ///</summary>
        public void Insert(int index, Person value)
        {
            this.List.Insert(index, value);
        }

        ///<summary>
        /// Determines the index of a specific item in the PersonCollection.
        ///</summary>
        public int IndexOf(Person value)
        {
            return this.List.IndexOf(value);
        }

        ///<summary>
        /// Determines whether the PersonCollection contains a specific value.
        ///</summary>
        public bool Contains(Person value)
        {
            return this.List.Contains(value);
        }

        ///<summary>
        /// Copies elements of the PersonCollection to a Syste.Array, starting at a particular System.Array index.
        ///</summary>
        public void CopyTo(Array array, int index)
        {
            this.List.CopyTo(array, index);
        }

    }
 
}
