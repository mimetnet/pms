using System;
using System.IO;

namespace PMS
{
	public static class Config
	{
		//private static readonly log4net.ILog log = log4net.LogManager.GetLogger("PMS.Config");

		public static string SystemPath {
			get {
				string foo = (Environment.OSVersion.Platform == PlatformID.Unix) ?
					("/etc/libpms") : ("c:\\Program Files\\Common Files\\PMS\\etc");

				string env = Environment.GetEnvironmentVariable("PMS_CONFIG_PATH");
				if (env != null) {
					foo = env;
				}

				return foo;
			}
		}

		public static string UserPath {
			get {
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "libpms");
			}
		}
	}
}
