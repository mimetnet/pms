using System;
using System.Reflection;
using System.Xml.Serialization;

namespace PMS.Metadata
{
    public sealed class Connection
    {
        [XmlIgnore]
        private Type type;

        [XmlElementAttribute("string")]
        public string String;

        [XmlAttributeAttribute("type")]
        public string sType;

        [XmlAttributeAttribute("assembly")]
        public string sAssembly;

        [XmlAttributeAttribute("id")]
        public string Id;

        [XmlAttributeAttribute("default")]
        public bool IsDefault;

        [XmlAttributeAttribute("pool-size")]
        public int PoolSize = 5;

        public Connection()
        {
        }

        public Connection(string id, string conn, string stype, bool isDefault, int pool)
        {
            Id = id;
            sType = stype;
            String = conn;
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
                "\n  String = " + String +
                "\n  IsDefault = " + IsDefault +
                "\n  PoolSize = " + PoolSize;
        }
    }
}
