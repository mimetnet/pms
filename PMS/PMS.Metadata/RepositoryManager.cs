using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace PMS.Metadata
{
    public class RepositoryManager
    {
        private static XmlSerializer serializer = new XmlSerializer(typeof(Repository));
        private static Repository repository = new Repository();
        private static Connection _currentConnection = null;
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
                if (_currentConnection == null)
                    _currentConnection = DefaultConnection;
                return _currentConnection; 
            }
            set { _currentConnection = value; }
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
            Field field = GetClass(type).GetField(fieldInfo);
            if (field != null)
                return field;

            return null;

            // if strict throw Exception();
        }

        public static Class GetClass(Type type)
        {
            foreach (Class cdesc in Repository.Classes)
                if (cdesc.Name.Equals(type.ToString()))
                    return cdesc;

            throw new ApplicationException(type + " Not Found in Repository");
        }

        public static Type GetClassListType(Type type)
        {
            string sType = type.ToString();

            foreach (Class cdesc in Repository.Classes)
                if (cdesc.Name.Equals(sType))
                    return cdesc.ListType;

            throw new ApplicationException(type + " Not Found in Repository");
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
            file = Path.GetFullPath(file);

            if (File.Exists(file) == false) {
                throw new FileNotFoundException(file);
            }

            using (FileStream fs = new FileStream(file, 
                                                  FileMode.Open, 
                                                  FileAccess.Read, 
                                                  FileShare.Read)) {
                repository += (Repository) serializer.Deserialize(fs);

                fs.Close();
                isLoaded = true;
            }

            return IsLoaded;
        }

        public static bool IsLoaded {
            get {
                return isLoaded;
            }
        }

    }
}
