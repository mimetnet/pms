using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

using PMS;
using PMS.Config;
using PMS.IO;
using PMS.Metadata;

using System.Configuration;
using System.Xml;

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
			Load(new FileInfo(Path.Combine(PMS.Config.Section.SystemPath, FILE_NAME)));
			Load(new FileInfo(Path.Combine(PMS.Config.Section.UserPath, FILE_NAME)));

            try  {
                foreach (ProviderElement e in Section.Providers) {
                    Add(e.Name, e.Type);
                }
            } catch (Exception e) {
                Console.WriteLine("Problem reading configuration section : {0}", e.Message);
                log.Warn("Problem reading config section: ", e);
                return;
            }
		}

		public static SortedList<String, IProvider> Providers {
			get { return list; }
		}

        public static IProvider Create(string name)
        {
			if (String.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");

			IProvider p = null;

			if (list.TryGetValue(name, out p))
				return p;

            throw new ProviderNotFoundException("'" + name + "' not loaded");
        }

		public static void Load(FileInfo file)
		{
			if (file == null)
				throw new ArgumentNullException("file");

			if (!file.Exists) {
				if (file.Directory.Exists)
					log.Debug("Not Found << " + file);
				return;
			}

			log.Debug("Load << " + file);

			Int32 div;
			String line, key;

			try {
				using (TextReader reader = file.OpenText()) {
					while ((line = reader.ReadLine()) != null) {
						try {
							if (line.Length == 0 || line[0] == '#')
								continue;

							if ((div = line.IndexOf('=')) < 1)
								continue;

							if (String.IsNullOrEmpty(key = line.Substring(0, div)))
								continue;

							Add(key, line.Substring(div+1).Trim());

						} catch (Exception e2) {
							log.Error("Load.Line << " + e2.Message);
						}
					}
				}
			} catch (UnauthorizedAccessException e) {
				log.Error("Load << " + e.Message);
			} catch (Exception e) {
				log.Error("Load << ", e);
			}
		}

        private static void Add(string key, string sType)
        {
            if (key == null || sType == null)
                return;

            key = key.Trim();
            sType = sType.Trim();

			if (String.IsNullOrEmpty(key) || String.IsNullOrEmpty(sType))
                return;

			if (list.ContainsKey(key)) {
				log.WarnFormat("Duplicate IProvider key found: '{0}' << SKIP", key);
				log.WarnFormat("Duplicate> old({0}) new({0})", list[key], sType);
				return;
			}

			Type type = null;
            
            try {
                type = PMS.Util.TypeLoader.Load(sType);
            } catch (Exception) {
                log.WarnFormat("Failed to load type: '{0}'", sType);
                //log.Warn("Failed to load type: ", e);
                return;
            }

			if (Array.IndexOf(type.GetInterfaces(), typeof(IProvider)) == -1) {
                log.WarnFormat("Specified type({0}) doesn't implement IProvider", type.FullName);
				return;
            }

			list[key] = (IProvider)Activator.CreateInstance(type);
        }
    }
}
