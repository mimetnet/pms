using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace PMS.Collections.Pool
{
    public class ItemCollection : List<Item>
    {
    }

	public class Item
	{
		public object Object = null;

		public IPrincipal Principal = null;

		public DateTime LastUsed = DateTime.Now;

		public bool Available = true;

		public Item(Object obj) : this(obj, System.Threading.Thread.CurrentPrincipal)
		{
		}

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
