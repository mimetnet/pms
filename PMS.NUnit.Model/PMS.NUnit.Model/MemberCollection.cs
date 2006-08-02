using System;
using System.Collections;
using System.Data;
using System.Xml;
using System.Xml.Serialization;

namespace PMS.NUnit.Model
{
    /// <summary>
    /// MemberCollection Class
    /// </summary>
    [XmlRoot("members")]
    public class MemberCollection : CollectionBase
    {
        ///<summary>
        /// Default constructor.
        ///</summary>
        public MemberCollection()
        {
        }
        
        ///<summary>
        /// The zero-based index of the element to get or set.
        ///</summary>
        public Member this[int index]
        {
            get
            {
                return (Member)this.List[index];
            }
            set
            {
                this.List[index] = value;
            }
        }

        ///<summary>
        /// Gets the number of elements contained in the MemberCollection.
        ///</summary>
        public new int Count
        {
            get
            {
                return this.List.Count;
            }
        }

        ///<summary>
        /// Removes all items from the MemberCollection.
        ///</summary>
        public new void Clear()
        {
            this.List.Clear();
        }

        ///<summary>
        /// Adds an item to the MemberCollection.
        ///</summary>
        public void Add(Member value)
        {
            this.List.Add(value);
        }

        ///<summary>
        /// Removes the first occurrence of a specific object from the MemberCollection.
        ///</summary>
        public void Remove(Member value)
        {
            this.List.Remove(value);
        }

        ///<summary>
        /// Removes the MemberCollection item at the specified index.
        ///</summary>
        public new void RemoveAt(int index)
        {
            this.List.RemoveAt(index);
        }

        ///<summary>
        /// Inserts an item to the MemberCollection at the specified index.
        ///</summary>
        public void Insert(int index, Member value)
        {
            this.List.Insert(index, value);
        }

        ///<summary>
        /// Determines the index of a specific item in the MemberCollection.
        ///</summary>
        public int IndexOf(Member value)
        {
            return this.List.IndexOf(value);
        }

        ///<summary>
        /// Determines whether the MemberCollection contains a specific value.
        ///</summary>
        public bool Contains(Member value)
        {
            return this.List.Contains(value);
        }

        ///<summary>
        /// Copies elements of the MemberCollection to a Syste.Array, starting at a particular System.Array index.
        ///</summary>
        public void CopyTo(Array array, int index)
        {
            this.List.CopyTo(array, index);
        }

    }
 
}
