using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PMS.Util
{
    public sealed class TypeLoader
    {
        private static readonly log4net.ILog log = 
            log4net.LogManager.GetLogger("PMS.Util.TypeLoader");

        public static Type Load(string fullTypeName)
        {
			if (String.IsNullOrEmpty(fullTypeName))
				throw new ArgumentNullException("fullTypeName");

			Type type = null;
            Assembly ass = null;
			String[] pieces = fullTypeName.Split(',');
			String sType = pieces[0].Trim();
			String sAssembly = null;
			
			if (pieces.Length > 1) {
				sAssembly = String.Join(",", pieces, 1, pieces.Length - 1).Trim();
			}

			try {
				if (!String.IsNullOrEmpty(sAssembly)) {
					if ((ass = Assembly.Load(sAssembly)) != null) {
						if ((type = ass.GetType(sType, false)) != null) {
							//Console.WriteLine("loaded: '" + type + ", " + type.Assembly + "'");
							return type;
						}
					} else {
						log.Warn("failed to load: " + sAssembly);
					}
				} else {
					log.Error("no assembly for : " + fullTypeName);
				}
			} catch (Exception e) {
				log.Warn(e.Message + "\n");
			}

            log.Error(sType + " not found" + Environment.NewLine);

            throw new TypeLoadException(sType + " not found" + Environment.NewLine);
        }
    }
}
