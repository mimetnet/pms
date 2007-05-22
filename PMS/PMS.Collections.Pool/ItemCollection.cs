using System;
using System.Collections;
using System.Security.Principal;
using System.Threading;

namespace PMS.Collections.Pool
{
    /// <summary>
    /// ItemCollection Class
    /// </summary>
    public class ItemCollection : CollectionBase
    {
        ///<summary>
        /// Default constructor.
        ///</summary>
        public ItemCollection()
        {
        }
        
        ///<summary>
        /// The zero-based index of the element to get or set.
        ///</summary>
        public Item this[int index]
        {
            get
            {
                return (Item)this.List[index];
            }
            set
            {
                this.List[index] = value;
            }
        }

        ///<summary>
        /// Gets the number of elements contained in the ItemCollection.
        ///</summary>
        public new int Count
        {
            get
            {
                return this.List.Count;
            }
        }

        ///<summary>
        /// Removes all items from the ItemCollection.
        ///</summary>
        public new void Clear()
        {
            this.List.Clear();
        }

        ///<summary>
        /// Adds an item to the ItemCollection.
        ///</summary>
        public void Add(Item value)
        {
            this.List.Add(value);
        }

        ///<summary>
        /// Removes the first occurrence of a specific object from the ItemCollection.
        ///</summary>
        public void Remove(Item value)
        {
            this.List.Remove(value);
        }

        ///<summary>
        /// Removes the ItemCollection item at the specified index.
        ///</summary>
        public new void RemoveAt(int index)
        {
            this.List.RemoveAt(index);
        }

        ///<summary>
        /// Inserts an item to the ItemCollection at the specified index.
        ///</summary>
        public void Insert(int index, Item value)
        {
            this.List.Insert(index, value);
        }

        ///<summary>
        /// Determines the index of a specific item in the ItemCollection.
        ///</summary>
        public int IndexOf(Item value)
        {
            return this.List.IndexOf(value);
        }

        ///<summary>
        /// Determines whether the ItemCollection contains a specific value.
        ///</summary>
        public bool Contains(Item value)
        {
            return this.List.Contains(value);
        }

        ///<summary>
        /// Copies elements of the ItemCollection to a Syste.Array, starting at a particular System.Array index.
        ///</summary>
        public void CopyTo(Array array, int index)
        {
            this.List.CopyTo(array, index);
        }

    }

	/// <summary>
	/// Wraps an object in the Pool
	/// </summary>
	public class Item
	{
		/// <summary>
		/// Contains the element in the pool
		/// </summary>
		public object Object = null;

		/// <summary>
		/// Identity of the wrapped object
		/// </summary>
		public IPrincipal Principal = null;

		/// <summary>
		/// When was this last used ?
		/// </summary>
		public DateTime LastUsed = DateTime.Now;

		public bool Available = true;

		/// <summary>
		/// Create Item to wrap obj (Available by default)
		/// </summary>
		/// <param name="obj">Element to wrap</param>
		public Item(Object obj) : this(obj, Thread.CurrentPrincipal)
		{
		}

		/// <summary>
		/// Create Item to wrap with availability settings
		/// </summary>
		/// <param name="obj">Element to Wrap</param>
		/// <param name="principal">Principal owner of object</param>
		public Item(Object obj, IPrincipal principal)
		{
			Object = obj;
			Principal = principal;
		}

		public object Checkout()
		{
			LastUsed = DateTime.Now;
			Available = false;
			return this.Object;
		}

		public void Checkin()
		{
			Available = true;
		}
	} 


}
