using System;
using System.Collections.Generic;
using System.Data;

using PMS.Metadata;

namespace PMS.Data
{
    [Serializable]
    internal sealed class ProviderFactory
    {
		private static readonly log4net.ILog log =
			log4net.LogManager.GetLogger("PMS.Data.ProviderFactory");

        public static IProvider Create(string name)
        {
			if (String.IsNullOrEmpty(name)) {
				throw new ArgumentNullException("name");
			}

			IProvider p = null;

			if (PMS.Config.Instance.Providers.TryGetValue(name, out p))
				return p;

            throw new ProviderNotFoundException("'" + name + "' does not exist within known providers");
        }
    }
}
