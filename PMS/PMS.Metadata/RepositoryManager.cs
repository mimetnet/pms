using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;

using PMS.IO;

namespace PMS.Metadata
{
    public sealed class RepositoryManager : MarshalByRefObject
    {
        #region Private Fields
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("PMS.Metadata.RepositoryManager");
        private static XmlSerializer serializer = new XmlSerializer(typeof(Repository));
        private static Repository repository = new Repository();
        private static Connection cConn = null;
        private static bool isLoaded = false;
        #endregion

		public static string Package = String.Empty;

        #region Properties
        public static Repository Repository {
            get { return repository; }
            set { repository = value; }
        }

        public static Connection DefaultConnection {
            get {
                foreach (Connection conn in Repository.Connections) {
                    if (conn.IsDefault == true)
                        return conn;
                }

				if (Repository.Connections.Count > 0)
					return Repository.Connections[0];

				return null;
            }
            set {
                value.IsDefault = true;
                foreach (Connection conn in Repository.Connections) {
                    conn.IsDefault = false;
                }

                Repository.Connections.Add(value);
            }
        }

        public static Connection CurrentConnection
        {
            get {
                if (cConn == null)
                    cConn = DefaultConnection;

                return cConn;
            }
            set { cConn = value; }
        } 
        #endregion

        #region Methods
        public static Class GetClass(Type type)
        {
			Class klass = Repository.Classes[type];

			if (klass == null) {
				if (Load(type)) {
					return GetClass(type);
				} else {
					log.Error("Type '" + type.FullName + ", " + type.Assembly + "' Not Found in Repository");

					foreach (Class c in Repository.Classes) {
						log.Info(c);
					}
				}
			}

			//Console.WriteLine(new System.Diagnostics.StackTrace());

			/**
			log.Info("Type asked: " + type);
			log.Info("Class gotten: " + klass.Table);
			log.Info("Class gotten: " + klass.Type);
			log.Info("-");
			**/

			return klass;
        }

        public static Type GetClassListType(Type type)
        {
            Class cdesc = GetClass(type);

            if (cdesc != null)
                return cdesc.ListType;

            return null;
        }

        public static bool Exists(Type type)
        {
            return (GetClass(type) != null) ? true : false;
        } 
        #endregion

        #region File IO
        public static bool SaveAs(string fileName)
        {
            return SaveAs(repository, fileName);
        }

        public static bool SaveAs(Repository rep, string fileName)
        {
            using (TextWriter writer = new StreamWriter(fileName)) {
                try {
                    serializer.Serialize(writer, rep);
                } catch (Exception e) {
                    log.Error("SaveAs", e);
                }
                writer.Close();
            }

            return true;
        }

		public static bool Save(string package)
		{
			return Save(repository, package);
		}

		public static bool Save(Repository repo, string package)
		{
			DirectoryInfo dir = new DirectoryInfo(GetPath(package));

			if (dir.Exists == false) {
				dir.Create();
			}

			String path = null;
			FileStream fs = null;
			XmlSerializer xml = null;

			xml = new XmlSerializer(typeof(Class));
			foreach (Class klass in repo.Classes) {
				path = Path.Combine(dir.FullName, (klass.Type.FullName + ".pmc"));
				fs = new FileStream(path, FileMode.Create);
				
				try {
					xml.Serialize(fs, klass);
				} catch (Exception se) {
					log.Error("Save ", se);
				} finally {
					fs.Close();
				}
			}

			xml = new XmlSerializer(typeof(Connection));
			foreach (Connection conn in repo.Connections) {
				path = Path.Combine(dir.FullName, (conn.Id + ".pmx"));
				fs = new FileStream(path, FileMode.Create);

				try {
					xml.Serialize(fs, conn);
				} catch (Exception se2) {
					log.Error("Save ", se2);
				} finally {
					fs.Close();
				}
			}

			Package = package;

			return true;
		}

        public static bool Load(string file)
        {
			FileInfo f = new FileInfo(Path.GetFullPath(file));

			// repository.xml style
			if (f.Exists) {
				return Load(f);
			}

			// /etc/libpms/[package] style
			DirectoryInfo dir = new DirectoryInfo(GetPath(file));
			if (!dir.Exists) {
				return false;
			}

			XmlSerializer xml = new XmlSerializer(typeof(Connection));

			foreach (FileInfo pmsFile in dir.GetFiles("*.pmx")) {
				using (FileLock flock = new FileLock(f.FullName)) {
					Connection conn = (Connection) xml.Deserialize(new FileStream(pmsFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read));
					if (conn != null && conn.Provider != null) {
						repository.Connections.Add(conn);
					}
				}
			}

			if (repository.Connections.Count == 0) {
				Console.WriteLine("no connections loaded");
				return false;
			}

			Package = file;

			return (isLoaded = true);
        }

        public static bool Load(FileInfo file)
        {
			bool res = false;

			using (FileStream fs = file.OpenRead()) {
				try {
					repository += (Repository)serializer.Deserialize(fs);
					res = (repository.Connections.Count > 0);
				} catch (FileNotFoundException) {
					if (log.IsErrorEnabled)
						log.Error("Load(" + file + ") failed");
				} catch (Exception e) {
					if (log.IsErrorEnabled)
						log.Error("Unkown failure", e);
				}
			}

            return (isLoaded = res);
        }

		public static bool Load(Type type)
		{
			XmlSerializer xml = new XmlSerializer(typeof(Class));
			FileInfo f = new FileInfo(Path.Combine(GetPath(Package), type.FullName + ".pmc"));
			bool status = false;

			if (f.Exists == false) {
				log.Error("Load("+type+") File does not exist: " + f);
				return false;
			}

			try {
				using (FileLock flock = new FileLock(f.FullName)) {
					Class klass = (Class) xml.Deserialize(new FileStream(f.FullName, FileMode.Open, FileAccess.Read, FileShare.Read));
					if (klass != null && klass.Type != null) {
						repository.Classes.Add(klass);
						status = true;
					}
				}
			} catch (Exception e) {
				log.Error("Load(" + type.Name + "): ", e);
			}

			return status;
		}

        public static void Close()
        {
			isLoaded = false;
			repository = new Repository();
        }

        public static bool IsLoaded {
            get { return isLoaded; }
        }

		private static string GetPath(string package)
		{
			return Path.Combine(PMS.Config.SystemPath, package);
		}
        #endregion
    }
}
