using System;
using System.Data;

namespace PMS.Data
{
    public interface IProvider
    {
        object PrepareSqlValue(string dbType, object value);
        object PrepareSqlString(object value);
        object PrepareSqlBoolean(object value);
        object PrepareSqlAutoIncrement(object value);
        object PrepareSqlTimestamp(object value);
        object PrepareSqlDate(object value);
        object PrepareSqlBit(object value);
        
        // ---------------------------------------------------------------

        object GetTypeInit(string dbType);
        object GetTypeInit(Type type);

        object ConvertToType(string dbType, object obj);
        object ConvertToType(Type type, object obj);

        Type GetType(string type);

        IDbConnection GetConnection();
        IDbConnection GetConnection(string connString);

        IDbInspector GetInspector();
        IDbInspector GetInspector(IDbConnection conn);
    }
}
