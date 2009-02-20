using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;

using PMS.IO;
using PMS.Metadata;

namespace PMS.Metadata
{
    public sealed class RepositoryManager
    {
        #region Private Fields
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("PMS.Metadata.SessionManager");
        private XmlSerializer serializer = new XmlSerializer(typeof(Repository));
        private Repository repository = new Repository();
        private string package = null;
        #endregion

        public RepositoryManager(string package)
        {
            if (null == (this.package = this.Load(package)))
                throw new RepositoryNotFoundException("Failed to load " + package);
        }

        #region Properties
        public string Package {
            get { return this.package; }
        }

        public Repository Repository {
            get { return repository; }
        }
        #endregion

        #region Methods
        public Class GetClass(Type type)
        {
			Class klass = Repository.Classes[type];

            if (klass != null)
                return klass;
		
			if (Load(type)) {
				return GetClass(type);
			} else if (Repository.GenerateTypes && Build(type)) {
				return GetClass(type);
			}

            throw new ClassNotFoundException(type);
        }

		private bool Build(Type type)
		{
			try {
				Repository.Classes.Add(new Class(type));
			} catch (Exception e) {
				log.Error("Build: ", e);
				return false;
			}

			log.Info("Type(" + type.FullName + ") was autogenerated (it may not be to your liking)");

			return true;
		}

        public Type GetClassListType(Type type)
        {
            Class cdesc = GetClass(type);

            if (cdesc != null)
                return cdesc.ListType;

            return null;
        }

        public bool Exists(Type type)
        {
            return GetClass(type) != null;
        } 
        #endregion

        #region File IO
        private string Load(string file)
        {
			FileInfo f = new FileInfo(Path.GetFullPath(file));

			// repository.xml style
			if (f.Exists)
				return Load(f);

			// /etc/libpms/[package] style
			DirectoryInfo dir = new DirectoryInfo(GetPath(file));
			
            if (!dir.Exists)
				return null;

			XmlSerializer xml = new XmlSerializer(typeof(Connection));

			foreach (FileInfo pmsFile in dir.GetFiles("*.pmx")) {
				using (FileLock flock = new FileLock(f.FullName)) {
					Connection conn = (Connection) xml.Deserialize(new FileStream(pmsFile.FullName, FileMode.Open, FileAccess.Read, FileShare.Read));
					if (conn != null && conn.Provider != null) {
						repository.Connections.Add(conn);
					}
				}
			}

			return dir.FullName;
        }

        private string Load(FileInfo file)
        {
			using (FileStream fs = file.OpenRead()) {
				try {
					repository += (Repository)serializer.Deserialize(fs);
					return file.FullName;
				} catch (FileNotFoundException) {
					if (log.IsErrorEnabled)
						log.Error("Load(" + file + ") failed");
				} catch (Exception e) {
					if (log.IsErrorEnabled)
						log.Error("Unkown failure", e);
				}
			}

            return null;
        }

		private bool Load(Type type)
		{
			XmlSerializer xml = new XmlSerializer(typeof(Class));
			FileInfo f = new FileInfo(Path.Combine(GetPath(Package), type.FullName + ".pmc"));

			if (f.Exists == false) {
				log.Error("Load("+type+") File does not exist: " + f);
				return false;
			}

			try {
				using (FileLock flock = new FileLock(f.FullName)) {
					Class klass = (Class) xml.Deserialize(new FileStream(f.FullName, FileMode.Open, FileAccess.Read, FileShare.Read));
					if (klass != null && klass.Type != null) {
						repository.Classes.Add(klass);
						return true;
					}
				}
			} catch (Exception e) {
				log.Error("Load(" + type.Name + "): ", e);
			}

			return false;
		}

        public void Close()
        {
			repository = new Repository();
        }

		private string GetPath(string package)
		{
			return Path.Combine(PMS.Config.Section.SystemPath, package);
		}
        #endregion

        public Connection GetDescriptor(string connectionID)
        {
            foreach (Connection c in this.repository.Connections)
                if (0 == StringComparer.InvariantCulture.Compare(c.ID, connectionID))
                    return c;

            throw new RepositoryException(String.Format("Package({0}) doesn't have <connection id='{1}' />", this.package, connectionID));
        }

        public Connection GetDescriptor()
        {
            foreach (Connection c in this.repository.Connections)
                if (c.IsDefault)
                    return c;

            log.DebugFormat("Package({0}) doesn't have default <connection /> - using first one", this.package);

            if (this.repository.Connections.Count > 0)
                return this.repository.Connections[0];

            throw new RepositoryException(String.Format("Package({0}) contains no <connection/> tags", this.package));
        }
    }
}
