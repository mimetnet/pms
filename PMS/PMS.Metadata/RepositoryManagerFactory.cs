using System;
using System.Collections.Generic;

using PMS.Metadata;

namespace PMS.Metadata
{
    internal static class RepositoryManagerFactory
    {
        private static SortedList<string, RepositoryManager> managers = new SortedList<string,RepositoryManager>(StringComparer.Ordinal);

        public static RepositoryManager Factory(string package)
        {
            RepositoryManager m = null;

            if (RepositoryManagerFactory.managers.TryGetValue(package, out m))
                return m;

            m = new RepositoryManager(package);

            managers.Add(package, m);

            return m;
        }
    }
}
