using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Security.Cryptography;

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

			lockFile = new FileInfo(Path.Combine(Path.GetTempPath(), this.GetLock(filePath)));

			try {
				// try for 5 seconds to obtain lock
				while (File.Exists(lockFile.FullName)) {
					if (++steps == 50) {
						throw new ApplicationException(String.Format("File {0} is locked by the writing process for more than {1} seconds: ", lockFile, ((steps * 300) / 1000)));
					}

					try {
						Thread.Sleep(100);
					} catch (Exception e) {
						log.Error("AcquireLock (A): ", e);
					}
				}
			} catch (Exception e) {
				log.Error("AcquireLock (B): ", e);
			}

			using (FileStream fs = lockFile.Create()) {
				fs.Close();
			}
		}

		public override String ToString()
		{
			return "Flock://" + lockFile.FullName;
		}

		private string GetLock(string file)
		{
			return Convert.ToBase64String(new MD5CryptoServiceProvider().ComputeHash(System.Text.UTF8Encoding.Default.GetBytes(file))).Replace("/", "");
		}

		void IDisposable.Dispose()
		{
			try {
				if (File.Exists(lockFile.FullName))
					File.Delete(lockFile.FullName);
			} catch {
			}

			lockFile = null;
		}
	}
}
