using System;
using System.IO;
using System.Text;
using System.Threading;

namespace PMS.IO
{
	public sealed class FileLock : IDisposable
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger("PMS.IO");
		private FileInfo lockFile;

		public FileLock(FileInfo file) : this(file.FullName)
		{
		}

		public FileLock(string filePath)
		{
			int steps = 0;

			lockFile = new FileInfo(Path.Combine(Path.GetTempPath(), (filePath.Replace(Path.DirectorySeparatorChar, '.') + ".lock")));

			try {
				// try for 5 seconds to obtain lock
				while (File.Exists(lockFile.FullName)) {
					if (++steps == 50) {
						throw new ApplicationException(String.Format("File {0} is locked by the writing process for more than {1} seconds: ", lockFile, ((steps * 300) / 1000)));
					}

					try {
						Thread.Sleep(100);
					} catch (Exception e) {
						Console.WriteLine("AcquireLock (A)");
						Console.WriteLine(e);
						log.Error("AcquireLock (A): ", e);
					}
				}
			} catch (Exception e) {
				Console.WriteLine("AcquireLock (B)");
				Console.WriteLine(e);
				log.Error("AcquireLock (B): ", e);
			}

			lockFile.Create().Close();
		}

		public override String ToString()
		{
			return "FLock@" + lockFile.FullName;
		}

		void IDisposable.Dispose()
		{
			try {
				if (File.Exists(lockFile.FullName))
					File.Delete(lockFile.FullName);
			} catch (Exception) {
			}

			lockFile = null;
		}
	}
}
