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

			return String.CompareOrdinal(x.ToString(), y.ToString());
		}
	}
}
