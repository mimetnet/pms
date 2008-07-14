using System;
using System.Data;

namespace PMS.Data
{
    public interface IProvider
    {
		String Name { get; set; }
		Type Type { get; }

        string PrepareSqlValue(string dbType, object value);
        string PrepareSqlString(object value);
        string PrepareSqlBoolean(object value);
        string PrepareSqlAutoIncrement(object value);
        string PrepareSqlTimestamp(object value);
        string PrepareSqlTimestamp(object value, bool tz);
        string PrepareSqlDate(object value);
        string PrepareSqlBit(object value);
        
        // ---------------------------------------------------------------

        object GetTypeInit(string dbType);

        // ---------------------------------------------------------------
		
        object ConvertToType(string dbType, Object obj);
		object ConvertToType(string dbType, String obj);
		object ConvertToType(string dbType, Boolean obj);
		object ConvertToType(string dbType, Int16 obj);
		object ConvertToType(string dbType, Int32 obj);
		object ConvertToType(string dbType, Int64 obj);
		object ConvertToType(string dbType, Double obj);
		object ConvertToType(string dbType, Single obj);
		object ConvertToType(string dbType, DateTime obj);

        // ---------------------------------------------------------------
		
		Type GetType(string type);

        // ---------------------------------------------------------------
		
		IDbConnection GetConnection();
		IDbConnection GetConnection(string connString);

        // ---------------------------------------------------------------
		
		IDbInspector GetInspector();
		IDbInspector GetInspector(IDbConnection conn);
	}
}
