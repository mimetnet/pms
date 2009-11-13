using System;
using System.Collections;
using System.Data;
using System.Globalization;

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
		protected string name;

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

        public virtual Type GetType(string type)
        {
            if ((type == "int") || (type == "integer") || (type == "int4") || 
                (type == "serial") || (type == "serial4")) {
                return System.Type.GetType("System.Int32");
            } else if ((type == "bigint") || (type == "int8") || 
                       (type == "serial8") || (type == "bigserial")) {
                return System.Type.GetType("System.Int64");
            } else if ((type == "smallint") || (type == "int2")) {
                return System.Type.GetType("System.Int16");
            } else if ((type == "bool") || (type == "boolean")) {
                return System.Type.GetType("System.Boolean");
            } else if ((type == "date") || (type == "timestamp")) {
                return System.Type.GetType("System.DateTime");
            } else if (type == "numeric" || type == "money") {
                return System.Type.GetType("System.Decimal");
            } else {
                return System.Type.GetType("System.String");
            }
        }

        public virtual object ConvertToType(string dbType, object obj)
        {
			if (obj is String) {
				if (dbType == "varchar" || dbType == "text" || dbType == "nvarchar") {
					return obj;
				} else {
					return ConvertToType(dbType, (String)obj);
				}
			} else if (obj is Boolean) {
				if (dbType.StartsWith("bool")) {
					return obj;
				} else {
					return ConvertToType(dbType, (Boolean)obj);
				}
			} else if (obj is Int16) {
				if (dbType == "int2" || dbType == "smallint" || dbType == "short") {
					return obj;
				} else {
					return ConvertToType(dbType, (Int16)obj);
				}
			} else if (obj is Int32) {
				if (dbType == "int" || dbType == "int4" || dbType == "serial" || dbType == "serial4" || dbType == "integer") {
					return obj;
				} else {
					return ConvertToType(dbType, (Int32)obj);
				}
			} else if (obj is Int64) {
				if (dbType == "int8" || dbType == "serial8" || dbType == "bigint" || dbType == "bigserial") {
					return obj;
				} else {
					return ConvertToType(dbType, (Int64)obj);
				}
			} else if (obj is Double) {
				if (dbType == "float8") {
					return obj;
				} else {
					return ConvertToType(dbType, (Double)obj);
				}
			} else if (obj is Single) {
				if (dbType.StartsWith("float")) {
					return obj;
				} else {
					return ConvertToType(dbType, (Single)obj);
				}
			} else if (obj is DateTime) {
				return ConvertToType(dbType, (DateTime)obj);
			} else if (obj is Decimal) {
				if (dbType == "numeric" || dbType == "money") {
					return obj;
				} else {
					return ConvertToType(dbType, (Decimal)obj);
				}
			} 

            return obj;
        }

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

        public virtual object ConvertToType(string dbType, Boolean obj)
		{
			return ConvertToTypeXXX(dbType, obj);
		}

        public virtual object ConvertToType(string dbType, Decimal obj)
		{
			return ConvertToTypeXXX(dbType, obj);
		}

        public virtual object ConvertToType(string dbType, Int16 obj)
		{
			return ConvertToTypeXXX(dbType, obj);
		}

        public virtual object ConvertToType(string dbType, Int32 obj)
		{
			return ConvertToTypeXXX(dbType, obj);
		}

        public virtual object ConvertToType(string dbType, Int64 obj)
		{
			return ConvertToTypeXXX(dbType, obj);
		}

        public virtual object ConvertToType(string dbType, Double obj)
		{
			return ConvertToTypeXXX(dbType, obj);
		}

        public virtual object ConvertToType(string dbType, Single obj)
		{
			return ConvertToTypeXXX(dbType, obj);
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

		private object ConvertToTypeXXX(string dbType, object obj)
		{
			switch (dbType) {
				case "int":
				case "int4":
				case "integer":
				case "serial":
				case "serial4":
					obj = Convert.ToInt32(obj);
					break;

				case "int2":
				case "smallint":
					obj = Convert.ToInt16(obj);
					break;

				case "int8":
				case "serial8":
				case "bigint":
					obj = Convert.ToInt64(obj);
					break;

				case "char":
					obj = Convert.ToChar(obj);
					break;

				case "varchar":
				case "text":
					obj = Convert.ToString(obj);
					break;

				case "bool":
				case "boolean":
					obj = Convert.ToBoolean(obj);
					break;

				case "float":
				case "float4":
					obj = Convert.ToSingle(obj);
					break;

				case "float8":
					obj = Convert.ToDouble(obj);
					break;

				case "numeric":
				case "money":
					obj = Convert.ToDecimal(obj);
					break;

				case "date":
				case "time":
				case "timez":
				case "timestamp":
				case "timestampz":
					if (dbType.EndsWith("z")) { 
						obj = new DateTime(Convert.ToDateTime(obj).ToUniversalTime().Ticks, DateTimeKind.Utc);
					} else {
						obj = new DateTime(Convert.ToDateTime(obj).Ticks, DateTimeKind.Local);
					}
				break;
			}

			return obj;
		}

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
	}
}
