using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

using PMS.Util;
using PMS.Data;

namespace PMS
{
	public sealed class Config
	{
		private static Config config = new Config();

		public static Config Instance {
			get { return config; }
		}

		public static string SystemPath {
			get {
				string foo = (Environment.OSVersion.Platform == PlatformID.Unix) ?
					("/etc/libpms") : ("c:\\Program Files\\Common Files\\PMS");

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

		public SortedList<string, IProvider> Providers = new SortedList<string, IProvider>(StringComparer.Ordinal);

		private Config()
		{
			FileInfo f = new FileInfo(Path.Combine(Config.SystemPath, "pms.conf"));

			if (f.Exists)
				Load(f);

			Console.WriteLine("Loading: " + f);
			f = new FileInfo(Path.Combine(Config.UserPath, "pms.conf"));
			Console.WriteLine("Loading: " + f);

			if (f.Exists)
				Load(f);
		}

		private void Load(FileInfo file)
		{
			using (new PMS.IO.FileLock(file)) {
				XmlReader reader = new XmlTextReader(file.OpenRead());
				while (reader.Read()) {
					reader.MoveToElement();

					switch (reader.LocalName) {
						case "providers":
							LoadProviders(reader);
							break;
					}
				}
			}
		}

		private void LoadProviders(XmlReader reader)
		{
			while (reader.Read()) {
				reader.MoveToElement();

				if (reader.LocalName == "add") {
					string name = reader.GetAttribute("name");
					IProvider p = null;

					if (!reader.IsEmptyElement) {
						try {
							p = (IProvider) Activator.CreateInstance(TypeLoader.Load(reader.ReadElementContentAsString()));
						} catch (Exception e) {
							Console.WriteLine("LoadProviders: " + e);
						}

						if (!String.IsNullOrEmpty(name) && p != null) {
							Providers[name] = p;
						}
					} else {
						reader.Read();
					}
				} else if (reader.LocalName == "remove") {
					string name = reader.GetAttribute("name");
					if (Providers.ContainsKey(name)) {
						Providers.Remove(name);
					}
				} else if (reader.LocalName == "clear") {
					Providers.Clear();
				} else if (reader.LocalName == "providers" && reader.NodeType == XmlNodeType.EndElement) {
					break;
				}
			}

		}
	}
}
