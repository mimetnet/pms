using System;
using System.Reflection;

namespace PMS.Util
{
	public sealed class TypeLoader
	{
		//private static readonly log4net.ILog log = 
		//	log4net.LogManager.GetLogger("PMS.Util.TypeLoader");

		public static Type Load(string typeName)
		{
			if (String.IsNullOrEmpty(typeName))
				throw new ArgumentNullException("typeName");

			Int32 div;
			Type type;
			Assembly assembly;
			String sType, sAssembly;

			if ((div = typeName.IndexOf(',')) < 1) {
				throw new ArgumentException("type is badly formated: " + typeName);
			}

			if (String.IsNullOrEmpty((sType = typeName.Substring(0, div).Trim())))
				throw new ArgumentException(String.Format("Failed to substring '{0}' to get type", typeName));

			sAssembly = typeName.Substring(div+1).Trim();

			if (!System.IO.File.Exists(sAssembly)) {
				assembly = Assembly.Load(sAssembly);
			} else {
				assembly = Assembly.LoadFile(sAssembly);
			}

			if ((type = assembly.GetType(sType)) == null)
				throw new TypeLoadException(sType + " not found" + Environment.NewLine);

			return type;
		}
	}
}
