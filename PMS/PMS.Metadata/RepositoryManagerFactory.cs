using System;
using System.Collections.Generic;
using System.Threading;

using PMS.Metadata;

namespace PMS.Metadata
{
    internal static class RepositoryManagerFactory
    {
        private static SortedList<string, RepositoryManager> managers = new SortedList<string,RepositoryManager>(StringComparer.Ordinal);
		private static ReaderWriterLock mLock = new ReaderWriterLock();


        public static RepositoryManager Factory(string package)
        {
            RepositoryManager m = null;

			mLock.AcquireReaderLock(2000);

			if (RepositoryManagerFactory.managers.TryGetValue(package, out m)) {
				mLock.ReleaseReaderLock();
				return m;
			}

			try {
				mLock.UpgradeToWriterLock(5000);
			} catch (Exception e) {
				mLock.ReleaseReaderLock();
				throw e;
			}

			try {
				m = new RepositoryManager(package);
				managers[package] = m;
			} finally {
				mLock.ReleaseWriterLock();
			}

			return m;
		}
	}
}
