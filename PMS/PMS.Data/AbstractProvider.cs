using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Reflection;

using PMS.Metadata;
using PMS.Query;

namespace PMS.Data
{
    /// <summary>
    /// Used to Compare/Convert .NET types to database types
    /// Should be changed to work with events
    /// </summary>
    [Serializable]
    public abstract class AbstractProvider : IProvider
    {
        protected static Binder binder = new PMS.Util.TypeBinder();
        protected string name = null;

        public string Name {
            get { return this.name; }
            set { this.name = value; }
        }

        //public virtual string PrepareSqlTimestamp(object value, bool tz)
        //{
        //    DateTime t = Convert.ToDateTime(value);

        //    if (t.Kind != DateTimeKind.Unspecified)
        //        t = t.ToLocalTime();

        //    return "'" + t.ToString("yyyy-MM-dd HH:mm:sszz") + "'";
        //}

        /*
        public virtual object ConvertToType(string dbType, String obj) 
        {
            if (dbType == "bit" || dbType == "varbit") {
                String strObj = obj as String;
                BitArray bit = new BitArray(strObj.Length);
                for (int i = 0; i < strObj.Length; i++) {
                    bit.Set(i, ((strObj[i] == '0') ? false : true));
                }

                return bit;
            } else if (dbType == "char") {
                return Convert.ToChar(obj);
            }

            return obj; 
        }

        public virtual object ConvertToType(string dbType, DateTime obj) 
        {
            if (dbType.EndsWith("z")) { 
                // timez, timestampz
                //Console.WriteLine("ConvertToType: B4: " + obj);
                obj = new DateTime(obj.ToUniversalTime().Ticks, DateTimeKind.Utc);
                //Console.WriteLine("ConvertToType: AF: " + obj);
            } else {
                // time, timestamp, date
                obj = new DateTime(obj.Ticks, DateTimeKind.Local);
            }

            return obj;
        }

        case "timestampz":
            if (dbType.EndsWith("z")) { 
                obj = new DateTime(Convert.ToDateTime(obj).ToUniversalTime().Ticks, DateTimeKind.Utc);
            } else {
                obj = new DateTime(Convert.ToDateTime(obj).Ticks, DateTimeKind.Local);
            }
        break;
        */

        public abstract Type Type { get; }

		public abstract IDbConnection GetConnection();
		public abstract IDbConnection GetConnection(string connString);
        
        public abstract IDataParameter CreateParameter(string name, object value, PMS.DbType dbType);

		public abstract IDbInspector GetInspector();
		public abstract IDbInspector GetInspector(IDbConnection conn);

        public virtual Query<T> CreateQuery<T>(Class cdesc, IDbConnection connection) where T : new()
        {
            if (cdesc == null)
                throw new ArgumentNullException("Class");

            if (connection == null)
                throw new ArgumentNullException("IDbConnection");

            return new Query<T>(cdesc, this, connection);
        }

        public virtual PMS.DbType GetDbType(string dbTypeName)
        {
			switch (dbTypeName) {
				case "int":
				case "int4":
				case "integer":
				case "serial":
				case "serial4":
                    return new PMS.DbType(System.Data.DbType.Int32);

				case "int2":
				case "smallint":
                    return new PMS.DbType(System.Data.DbType.Int16);

				case "int8":
				case "serial8":
				case "bigint":
                    return new PMS.DbType(System.Data.DbType.Int64);

				case "char":
                    return new PMS.DbType(System.Data.DbType.String);

				case "varchar":
				case "text":
                    return new PMS.DbType(System.Data.DbType.String);

				case "bool":
				case "boolean":
                    return new PMS.DbType(System.Data.DbType.Boolean);

				case "float":
				case "float4":
                    return new PMS.DbType(System.Data.DbType.Single);

				case "float8":
                    return new PMS.DbType(System.Data.DbType.Double);

				case "numeric":
				case "money":
                    return new PMS.DbType(System.Data.DbType.Decimal);

				case "time":
				case "timez":
                    return new PMS.DbType(System.Data.DbType.Time);

				case "date":
                    return new PMS.DbType(System.Data.DbType.Date);

				case "timestamp":
				case "timestampz":
                    return new PMS.DbType(System.Data.DbType.DateTime);

                case "bin":
                case "binary":
                    return new PMS.DbType(System.Data.DbType.Binary);
            }

            return null;
        }

        public virtual Binder GetBinder()
        {
            return binder;
        }

        public virtual Binder GetBinder(Type type)
        {
            if (null == type)
                throw new ArgumentNullException("type");

            return binder;
        }
    }
}
