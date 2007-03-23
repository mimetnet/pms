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
            if (dbType == "varchar" || dbType == "text") {
                return PrepareSqlString(value);
            } else if (dbType.StartsWith("bool")) {
                return PrepareSqlBoolean(value);
            } else if (dbType.StartsWith("serial")) {
                return PrepareSqlAutoIncrement(value);
            } else if (dbType == "timestamp") {
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
            return "'" + Convert.ToDateTime(value) + "'";
        }

        public virtual string PrepareSqlDate(object value)
        {
            return "'" + Convert.ToDateTime(value).ToString("yyyyMMdd") + "'";
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

            if ((type == "varchar") || (type == "char")) {
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
            } else if ((type == "smallint") || (type == "int2")) {
                return new Int16();
            } else if ((type == "date") || (type == "timestamp")) {
                return new DateTime();
            } else if (type == "bit") {
                return new BitArray(0);
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
            } else {
                return System.Type.GetType("System.String");
            }
        }

        public virtual object ConvertToType(string dbType, object obj)
        {
            if (obj is System.DBNull)
                return GetTypeInit(dbType);
            
            if (dbType == "varchar" || dbType == "text") {
                return Convert.ToString(obj);
            } else if ((dbType == "boolean") || (dbType == "bool")) {
                return Convert.ToBoolean(obj);
            } else if ((dbType == "int") || (dbType == "int4") || 
                       (dbType == "serial") || (dbType == "serial4") ||
                       (dbType == "integer")) {
                return Convert.ToInt32(obj);
            } else if ((dbType == "smallint") || (dbType == "int2")) {
                return Convert.ToInt16(obj);
            } else if ((dbType == "timestamp") || (dbType == "date")) {
                return Convert.ToDateTime(obj);
            } else if (dbType == "bit" || dbType == "varbit") {
                string strObj = obj as string;
                BitArray bit = new BitArray(strObj.Length);
                for (int i = 0; i < strObj.Length; i++) {
                    bit.Set(i, ((strObj[i] == '0') ? false : true));
                }
            
                return bit;
            }
            
            return obj;
        }
        
        public object ConvertToType(Type type, object obj)
        {
            if (type == typeof(string))
                return Convert.ToString(obj);
            else if (type == typeof(Int32))
                return Convert.ToInt32(obj);
            else if (type == typeof(Int64))
                return Convert.ToInt64(obj);
            else if (type == typeof(Int16))
                return Convert.ToInt16(obj);
            else if (type == typeof(bool))
                return Convert.ToBoolean(obj);
            else if (type.BaseType == typeof(Enum)) {
                return Enum.Parse(type, obj.ToString());
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
