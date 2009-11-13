using System;
using System.Data;

using PMS.Metadata;
using PMS.Query;

namespace PMS.Data
{
    public interface IProvider
    {
		String Name { get; set; }
		Type Type { get; }

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

        IDataParameter CreateParameter(string name, object value, PMS.DbType dbType);

        PMS.DbType GetDbType(string dbTypeName);
        
        // ---------------------------------------------------------------
		
		IDbInspector GetInspector();
		IDbInspector GetInspector(IDbConnection conn);

        // ---------------------------------------------------------------

        Query<T> CreateQuery<T>(Class cdesc, IDbConnection connection) where T : new();
	}
}
