using System;
using System.Collections;
using System.Data;

namespace PMS.Data
{
    /// <summary>
    /// Used to Compare/Convert .NET types to database types
    /// Should be changed to work with events
    /// </summary>
    [Serializable]
    internal abstract class AbstractProvider : IProvider
    {
        public virtual string PrepareSqlValue(string dbType, object value)
        {
            if (dbType == "varchar" || dbType == "text" || dbType == "char") {
                return PrepareSqlString(value);
            } else if (dbType.StartsWith("bool")) {
                return PrepareSqlBoolean(value);
            } else if (dbType.StartsWith("serial")) {
                return PrepareSqlAutoIncrement(value);
            } else if (dbType == "timestamp" || dbType == "timestampz") {
                return PrepareSqlTimestamp(value);
            } else if (dbType == "date") {
                return PrepareSqlDate(value);
            } else if (dbType == "bit" || dbType == "varbit") {
                return PrepareSqlBit(value);
            } else if (dbType == "inet") {
                return PrepareSqlInetAddr(value);
            }

            return value.ToString();
        }

        public virtual string PrepareSqlString(object value)
        {
            return "'" + value.ToString().Replace("\\", "\\\\").Replace("'", "''") + "'";
        }

        public virtual string PrepareSqlBoolean(object value)
        {
            return (Convert.ToBoolean(value) == true)? "'t'" : "'f'";
        }

        public virtual string PrepareSqlAutoIncrement(object value)
        {
            return (Convert.ToInt32(value) == 0)? "DEFAULT" : value.ToString();
        }

        public virtual string PrepareSqlTimestamp(object value)
		{
			return PrepareSqlTimestamp(value, false);
		}

        public virtual string PrepareSqlTimestamp(object value, bool tz)
        {
			DateTime t = Convert.ToDateTime(value);

			if (t.Kind != DateTimeKind.Unspecified)
				t = t.ToLocalTime();

			return "'" + t.ToString("yyyy-MM-dd HH:mm:sszz") + "'";
        }

        public virtual string PrepareSqlDate(object value)
        {
            return "'" + Convert.ToDateTime(value).ToString("yyyy-MM-dd") + "'";
        }

        public virtual string PrepareSqlBit(object value)
        {
            string sBit = String.Empty;
            if (value is BitArray) { 
                BitArray bitArr = new BitArray((value as BitArray));
                foreach (bool bit in bitArr) {
                    sBit += (bit == true)? '1' : '0';
                }
            }
            
            return "B'" + sBit + "'";
        }

        public virtual string PrepareSqlInetAddr(object value)
        {
            return "inet'" + value + "'";
        }

        public virtual object GetTypeInit(string type)
        {
            type = type.ToLower();

            if (type == "varchar" || type == "text") {
                return String.Empty;
            } else if ((type == "int") || (type == "integer") || 
                       (type == "int4") || (type == "serial") || 
                       (type == "serial4")) {
                return new Int32();
            } else if ((type == "bigint") || (type == "int8") || 
                       (type == "serial8") || (type == "bigserial")) {
                return new Int64();
            } else if ((type == "bool") || (type == "boolean")) {
                return false;
            } else if ((type == "int2") || (type == "smallint")) {
                return new Int16();
            } else if ((type == "date") || (type == "timestamp") || (type == "timestampz")) {
                return new DateTime();
            } else if (type == "char") {
                return new Char();
            } else if (type == "bit") {
                return new BitArray(0);
            } else if (type == "numeric" || type == "money") {
                return new Decimal();
            } else {
                return null;
            }
        }

        public virtual object GetTypeInit(Type type)
        {
            if (type == typeof(string))
                return String.Empty;

            return Activator.CreateInstance(type);
        }

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
				if (dbType == "varchar" || dbType == "text") {
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
				if (dbType == "int2" || dbType == "smallint") {
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
				obj = new DateTime(obj.ToUniversalTime().Ticks, DateTimeKind.Utc);
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

		public virtual IDbConnection GetConnection()
		{
			throw new NotImplementedException("AbstractProvider");
		}

		public virtual IDbConnection GetConnection(string connString)
		{
			throw new NotImplementedException("AbstractProvider");
		}

		public virtual IDbInspector GetInspector()
		{
			throw new NotImplementedException("AbstractProvider");
		}

		public virtual IDbInspector GetInspector(IDbConnection conn)
		{
			throw new NotImplementedException("AbstractProvider");
		}
	}
}
