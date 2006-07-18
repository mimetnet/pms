using System;
using System.Data;

namespace PMS.Data
{
    internal interface IProvider
    {
        string PrepareSqlValue(string dbType, object value);
        string PrepareSqlString(object value);
        string PrepareSqlBoolean(object value);
        string PrepareSqlAutoIncrement(object value);
        string PrepareSqlTimestamp(object value);
        string PrepareSqlDate(object value);
        string PrepareSqlBit(object value);
        
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
