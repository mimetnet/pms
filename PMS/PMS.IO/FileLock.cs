using System;
using System.IO;
using System.Text;
using System.Threading;

namespace PMS.IO
{
	internal sealed class FileLock : IDisposable
	{
		#region Private Fields

		private static readonly log4net.ILog log = log4net.LogManager.GetLogger("PMS.IO");

		public static long LOCK_POLL_INTERVAL = 1000;	// 1000 milli seconds = 1 sec
		public static int DEFAULT_TIMEOUT = 10;			// 10 seconds
		private FileInfo lockFile;
		private bool isWriteLockCalled;
		private static System.Security.Cryptography.MD5 MD5_DIGESTER;
		private static readonly char[] HEX_DIGITS = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
		
		#endregion

		#region Paremeterized Constructor

		public FileLock(string filePath) {
			this.MakeLock(filePath);
		}

		#endregion


		#region Private Members
		
		private void MakeLock(string filePath)
		{
			this.lockFile = new FileInfo(filePath + "." + GetUniqueLock(filePath));
			this.isWriteLockCalled = false;

			log.Info("File: " + filePath);
			log.Info("Lock: " + lockFile);
			log.Info("--");
		}

		private string GetUniqueLock(String dirName)
		{
			int b = 0;
			byte[] digest;
			
			if (MD5_DIGESTER == null) {
				MD5_DIGESTER = System.Security.Cryptography.MD5.Create();
			}

			lock (MD5_DIGESTER) {
				digest = MD5_DIGESTER.ComputeHash(System.Text.Encoding.UTF8.GetBytes(dirName));
			}

			StringBuilder buf = new StringBuilder();

			for (int i = 0; i < digest.Length; i++)
			{
				b = digest[i];
				buf.Append(HEX_DIGITS[(b >> 4) & 0xf]);
				buf.Append(HEX_DIGITS[b & 0xf]);
			}

			return buf.ToString();
		}

		private bool Obtain()
		{
			if (this.lockFile == null)
				return false;

			if (File.Exists(this.lockFile.FullName))
				return true;

			try
			{
				// create a lock file
				FileStream fs = this.lockFile.Create();
				fs.Close();
				this.isWriteLockCalled = true;

				//log.InfoFormat("File Locked ..... {0}", this.lockFile.FullName);

				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		#endregion


		#region Public Members

		public bool IsLocked()
		{
			return ((lockFile != null && File.Exists(lockFile.FullName)) ? true : false);
		}

		public bool AcquireWriteLock()
		{
			return AcquireWriteLock(DEFAULT_TIMEOUT);
		}

		public bool AcquireWriteLock(int lockWaitTimeout)
		{
			long timeout = lockWaitTimeout * 1000;
			bool locked = Obtain();
			int maxSleepCount = (int)(timeout / LOCK_POLL_INTERVAL);
			int sleepCount = 0;

			while (locked == false) {
				if (sleepCount++ == maxSleepCount) {
					log.InfoFormat("File {0} is locked by the writing process for more than {1} seconds: ", this.lockFile, lockWaitTimeout);
					return false;
				}

				try {
					Thread.Sleep(new System.TimeSpan((System.Int64)10000 * LOCK_POLL_INTERVAL));
				} catch (ThreadInterruptedException e) {
					log.ErrorFormat("ThreadInterruptedException : {0}", e);
					return false;
				}

				locked = Obtain();
			}

			return locked;
		}

		public bool AcquireReadLock()
		{
			return AcquireReadLock(DEFAULT_TIMEOUT);
		}

		public bool AcquireReadLock(int lockWaitTimeout)
		{
			long timeout = lockWaitTimeout * 1000;
			bool locked = this.IsLocked();
			int maxSleepCount = (int)(timeout / LOCK_POLL_INTERVAL);
			int sleepCount = 0;
			while (locked)
			{
				if (sleepCount++ == maxSleepCount)
				{
					log.InfoFormat("File {0} is locked by the writing process for more than {1} seconds: ", this.lockFile.DirectoryName, lockWaitTimeout);
					return false;
				}
				try
				{
					Thread.Sleep(new System.TimeSpan((System.Int64)10000 * LOCK_POLL_INTERVAL));
				}
				catch (ThreadInterruptedException e)
				{
					log.ErrorFormat("ThreadInterruptedException : {0}", e);
					return false;
				}
				locked = this.IsLocked();
			}
			return ( (locked) ? false : true);
		}

		public void Release()
		{
			if (this.lockFile == null)
				return;

			if (this.isWriteLockCalled && File.Exists(this.lockFile.FullName))
			{
				//log.InfoFormat("File Lock Realesed ..... {0}", this.lockFile.FullName);

				File.Delete(this.lockFile.FullName);
			}
		}

		public override System.String ToString()
		{
			if (this.lockFile != null)
				return "Lock@" + this.lockFile;
			else
				return "Lock@NotFound";
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			Release();
			this.lockFile = null;
		}

		#endregion
	}
}
