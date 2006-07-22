using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace PMS.Metadata
{
    public class RepositoryManager : MarshalByRefObject
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static XmlSerializer serializer = new XmlSerializer(typeof(Repository));
        private static Repository repository = new Repository();
        private static Connection cConn = null;
        private static bool isLoaded = false;
        
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

                return (Connection) Repository.Connections[0];
            }
            set {
                value.IsDefault = true;
                foreach (Connection conn in Repository.Connections) {
                    conn.IsDefault = false;
                }

                Repository.Connections.Add(value);
            }
        }

        public static Connection CurrentConnection {
            get { 
                if (cConn == null)
                    cConn = DefaultConnection;
                return cConn; 
            }
            set { cConn = value; }
        }

        public static bool IsFieldInClass(Type type, string field)
        {
            if (GetField(type, field) != null)
                return true;
            else
                return false;
        }

        public static string GetColumn(Type type, string fieldName)
        {
            Field field = GetField(type, fieldName);

            if (field != null)
                return field.Column;
            
            return null;
        }

        public static Field GetField(Type type, string Sfield)
        {
            Field field = GetClass(type).GetField(Sfield);
            if (field != null)
                return field;

            return null;

            // if strict throw Exception();
        }

        public static Field GetField(Type type, FieldInfo fieldInfo)
        {
            return GetClass(type).GetField(fieldInfo);
            // if NULL && strict throw Exception();
        }

        public static Class GetClass(Type type)
        {
            string stype = type.ToString();

            foreach (Class cdesc in Repository.Classes) {
                if (cdesc.Type.Equals(stype)) {
                    return cdesc;
                }
            }

            if (log.IsErrorEnabled)
                log.Error("Type '" + type.FullName + ", " + type.Assembly + "' Not Found in Repository");

            return null;
        }

        public static Type GetClassListType(Type type)
        {
            Class cdesc = GetClass(type);

            if (cdesc != null)
                return cdesc.ListType;

            if (log.IsErrorEnabled)
                log.Error("ListType '" + type + "' Not Found in Repository");

            return null;
        }

        public static bool Exists(Type type)
        {
            return (GetClass(type) != null) ? true : false;
        }
        
        public static bool SaveAs(string fileName)
        {
            return SaveAs(repository, fileName);
        }

        public static bool SaveAs(Repository rep, string fileName)
        {
            using (TextWriter writer = new StreamWriter(fileName)) {
                serializer.Serialize(writer, rep);
                writer.Close();
            }

            return true;
        }

        public static bool Load(string file)
        {
            return Load(new FileInfo(Path.GetFullPath(file)));
        }

        public static bool Load(FileInfo file)
        {
            try {
                using (FileStream fs = file.OpenRead()) {
                    repository += (Repository) serializer.Deserialize(fs);

                    fs.Close();
                    isLoaded = true;    
                }
            } catch (FileNotFoundException) {
                if (log.IsErrorEnabled)
                    log.Error("Load(" + file + ") failed");
            } catch (Exception e) {
                if (log.IsErrorEnabled)
                    log.Error("Unkown failure", e);
            }

            return IsLoaded;
        }

        public static void Close()
        {
            if (isLoaded) {
                repository = null;
            }
        }

        public static bool IsLoaded {
            get { return isLoaded; }
        }
    }
}
