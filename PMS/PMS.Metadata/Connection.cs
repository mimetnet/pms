using System;
using System.Reflection;
using System.Xml.Serialization;

namespace PMS.Metadata
{
    [XmlRoot("connection")]
    public sealed class Connection
    {
        #region Public Properties
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

        [XmlIgnore]
        public Type Type
        {
            get
            {
                if (type == null) {

                    try {
                        type = Assembly.Load(this.sAssembly).GetType(this.sType, true);
                    } catch (System.TypeLoadException) {
                        Console.WriteLine("Connection.Type attribute from repository.xml file cannot be loaded.  Please check that '" + sType + "' is REAL and lies within .NET\'s reaches.");
                        System.Environment.Exit(1);
                    }
                }

                return type;
            }
            set
            {
                type = value;
                sType = type.ToString();
            }
        }
        #endregion

        #region Constructors
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
        #endregion

        #region ObjectOverloads

        ///<summary>
        ///OverLoading == operator
        ///</summary> 
        public static bool operator ==(Connection obj1, Connection obj2)
        {
            if (Object.ReferenceEquals(obj1, obj2)) return true;
            if (Object.ReferenceEquals(obj1, null)) return false;
            if (Object.ReferenceEquals(obj2, null)) return false;

            if (obj1.Id != obj2.Id) return false;
            if (obj1.sType != obj2.sType) return false;
            if (obj1.sAssembly != obj2.sAssembly) return false;
            if (obj1.Value != obj2.Value) return false;
            if (obj1.IsDefault != obj2.IsDefault) return false;
            if (obj1.PoolSize != obj2.PoolSize) return false;

            return true;
        }

        ///<summary>
        ///OverLoading != operator
        ///</summary> 
        public static bool operator !=(Connection obj1, Connection obj2)
        {
            return !(obj1 == obj2);
        }

        ///<summary>
        ///Overriden Equals
        ///</summary> 
        public override bool Equals(object obj)
        {
            if (!(obj is Connection)) return false;

            return this == (Connection)obj;
        }

        ///<summary>
        ///ToString()
        ///</summary> 
        public override string ToString()
        {
            return String.Format("[ Connection (Id={0}) (sType={1}) (sAssembly={2}) (Value={3}) (IsDefault={4}) (PoolSize={5} ]", Id, sType, sAssembly, Value, IsDefault, PoolSize);
        }

        ///<summary>
        ///Overriden GetHashCode()
        ///</summary> 
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        #endregion
    }
}
