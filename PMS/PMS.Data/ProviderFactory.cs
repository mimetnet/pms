using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

using PMS;
using PMS.IO;
using PMS.Metadata;

namespace PMS.Data
{
    public static class ProviderFactory
    {
		private static readonly log4net.ILog log =
			log4net.LogManager.GetLogger("PMS.Data.ProviderFactory");
		private static SortedList<String, IProvider> list = new SortedList<String, IProvider>(StringComparer.Ordinal);

		public const string FILE_NAME = "providers";

		static ProviderFactory()
		{
			Load(new FileInfo(Path.Combine(Config.SystemPath, FILE_NAME)));
			Load(new FileInfo(Path.Combine(Config.UserPath, FILE_NAME)));
		}

		public static SortedList<String, IProvider> Providers {
			get { return list; }
		}

		public static void Load(FileInfo file)
		{
			if (!file.Exists)
				return;

			log.Debug("Load << " + file);

			Int32 div;
			Type type;
			String line, key;

			using (TextReader reader = file.OpenText()) {
				while ((line = reader.ReadLine()) != null) {
					try {
						if (line.Length == 0 || line[0] == '#')
							continue;

						if ((div = line.IndexOf('=')) < 1)
							continue;

						if (String.IsNullOrEmpty(key = line.Substring(0, div)))
							continue;

						key = key.Trim();

						if (list.ContainsKey(key)) {
							log.WarnFormat("Duplicate IProvider key found << '{0}'", key);
							continue;
						}

						type = PMS.Util.TypeLoader.Load(line.Substring(div+1).Trim());

						if (Array.IndexOf(type.GetInterfaces(), typeof(IProvider)) == -1)
							continue;

						list[key] = (IProvider)Activator.CreateInstance(type);

					} catch (Exception e) {
						log.Error("Load << " + e.Message.Trim());
					}
				}
			}
		}

		public static void Add(string name, IProvider provider, bool local)
		{
			list[name] = provider;

			Type type = provider.GetType();
			FileInfo file = new FileInfo(Path.Combine((local? Config.UserPath : Config.SystemPath), FILE_NAME));

			using (FileLock fl = new FileLock(file)) {
				using (TextWriter w = new StreamWriter(new FileStream(file.FullName, (FileMode.OpenOrCreate | FileMode.Append), FileAccess.Write))) {
					if (file.Length == 0) {
						w.WriteLine();
					}
					w.WriteLine("{0} = {1}, {2}", name, type.FullName, type.Assembly);
				}
			}
		}

		public static bool Remove(string name, bool local)
		{
			String line = null;
			Boolean status = true;
			FileInfo file = new FileInfo(Path.Combine((local? Config.UserPath : Config.SystemPath), FILE_NAME));
			FileInfo copy = new FileInfo(Path.GetTempFileName());

			if (file.Exists == false)
				return false;

			using (FileLock fl = new FileLock(file)) {
				using (TextReader reader = file.OpenText()) {
					using (TextWriter writer = copy.CreateText()) {
						while ((line = reader.ReadLine()) != null) {
							if (line.StartsWith(name) == false) {
								writer.WriteLine(line);
							}
						}
					}
				}

				try {
					File.Copy(copy.FullName, file.FullName, true);
					File.Delete(copy.FullName);
				} catch (Exception e) {
					log.Error("Failed to move TMP providers over " + file.FullName + "\n : " + e);
					status = false;
				}
			}

			return status;
		}

        public static IProvider Create(string name)
        {
			if (String.IsNullOrEmpty(name)) {
				throw new ArgumentNullException("name");
			}

			IProvider p = null;

			if (list.TryGetValue(name, out p))
				return p;

            throw new ProviderNotFoundException("'" + name + "' cannot be found");
        }
    }
}
