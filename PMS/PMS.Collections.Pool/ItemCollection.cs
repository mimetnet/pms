using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace PMS.Collections.Pool
{
    /// <summary>
    /// ItemCollection Class
    /// </summary>
    public class ItemCollection : List<Item>
    {
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
		public Item(Object obj) : this(obj, System.Threading.Thread.CurrentPrincipal)
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
