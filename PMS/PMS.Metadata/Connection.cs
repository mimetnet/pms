using System;
using System.Reflection;
using System.Xml.Serialization;

namespace PMS.Metadata
{
    public sealed class Connection
    {
        [XmlIgnore]
        public const int DEFAULT_POOL_SIZE = 5;

        [XmlIgnore]
        private Type type;

        [XmlText]
        public string Value;

        [XmlAttribute("type")]
        public string sType;

        [XmlAttribute("assembly")]
        public string sAssembly;

        [XmlAttribute("id")]
        public string Id;

        [XmlAttribute("default")]
        public bool IsDefault;

        [XmlAttribute("pool-size")]
        public int PoolSize = DEFAULT_POOL_SIZE;

        public Connection()
        {
        }

        public Connection(string id, string conn, Type type)
            : this(id, conn, type, true, DEFAULT_POOL_SIZE)
        {
        }

        public Connection(string id, string conn, Type type, bool isDefault)
            : this(id, conn, type, isDefault, DEFAULT_POOL_SIZE)
        {
        }

        public Connection(string id, string conn, Type type, bool isDefault, int pool)
        {
            Id = id;
            sType = type.FullName;
            sAssembly = type.Assembly.FullName;
            Value = conn;
            IsDefault = isDefault;
            PoolSize = pool;
        }

        [XmlIgnore]
        public Type Type {
            get {
                if (type == null) {

                    try {
                        type = LoadType().GetType(this.sType, true);
                    } catch (System.TypeLoadException) {
                        Console.WriteLine("Connection.Type attribute from repository.xml file cannot be loaded.  Please check that '" + sType + "' is REAL and lies within .NET\'s reaches.");
                        System.Environment.Exit(1);
                    }
                }

                return type;
            }
            set {
                type = value;
                sType = type.ToString();
            }
        }

		private Assembly LoadType()
		{
            return Assembly.Load(this.sAssembly);
		}

        public override string ToString()
        {
            return "\nConnection:" +
                "\n  Id = " + Id +
                "\n  sType = " + sType +
                "\n  sAssembly = " + sAssembly +
                "\n  String = " + Value +
                "\n  IsDefault = " + IsDefault +
                "\n  PoolSize = " + PoolSize;
        }
    }
}
