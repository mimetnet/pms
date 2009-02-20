using System;
using System.Collections.Generic;

namespace PMS.Util
{
	public class TypeComparer : IComparer<Type>
	{
		public int Compare(Type x, Type y)
		{
			if (x == null)
				throw new ArgumentNullException("x");

			if (y == null)
				throw new ArgumentNullException("y");

            //Console.WriteLine("\n\nTypeComparer:\n{0}\n{1}\n", x.GUID, y.GUID);

            return x.GUID.CompareTo(y.GUID);
			//return String.CompareOrdinal(x.ToString(), y.ToString());
		}
	}
}
