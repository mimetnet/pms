using System;
using System.Configuration;
using System.Xml;
using System.IO;

namespace PMS.Config
{
    public class Section
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("KMBS.SEC.Web.Config.Manager");	
        private static XmlNode section = null;
        
        public static ProviderElementList Providers = new ProviderElementList();

        static Section()
        {
            Load();
        }

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

        private static void Load()
        {
            try {
#if MONO_1_1
                section = (XmlNode) ConfigurationSettings.GetConfig("pms"); // 1.1.13
#else
                section = (XmlNode) ConfigurationManager.GetSection("pms"); // 1.2.x
#endif
                if (section == null) {
                    log.Debug("Could not find <section name=\"pms\" type=\"PMS.Config.Handler, PMS\" /> for configuration");
                    return;
                }

                foreach (XmlNode child in section.ChildNodes) {
                    if (child.LocalName == "providers") {
                        foreach (XmlNode pchild in child.ChildNodes) {
                            switch (pchild.LocalName) {
                                case "add":
                                    AddProvider(pchild);
                                    break;
                                case "clear":
                                    Providers.Clear();
                                    break;
                            }
                        }
                    }
                }
            } catch (ConfigurationException confEx) {
                if (confEx.BareMessage.IndexOf("Unrecognized element") >= 0) {
                    log.Info("Failed to parse config file. Check that it is well formed XML.", confEx);
                } else {
                    log.Info("Failed to parse config file. Is the <configSections> specified as: <section name=\"pms\" type=\"PMS.Config.Handler, PMS\" />", confEx);
                }
            }
        }

        private static void AddProvider(XmlNode node)
        {
            ProviderElement p = new ProviderElement();

            foreach (XmlAttribute a in node.Attributes) {
                switch (a.LocalName) {
                    case "name":
                        p.Name = a.Value;
                        break;

                    case "type":
                        p.Type = a.Value;
                        break;
                }
            }

            if (!String.IsNullOrEmpty(p.Name) && !String.IsNullOrEmpty(p.Type))
                Providers.Add(p);
        }
    }
}
