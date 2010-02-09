using System;
using System.Data;
using System.Reflection;

using PMS.Metadata;
using PMS.Query;

namespace PMS.Data
{
    public interface IProvider
    {
        String Name { get; set; }
        Type Type { get; }

        IDbConnection GetConnection();
        IDbConnection GetConnection(string connString);

        IDataParameter CreateParameter(string name, object value, PMS.DbType dbType);

        PMS.DbType GetDbType(string dbTypeName);

        // ---------------------------------------------------------------

        IDbInspector GetInspector();
        IDbInspector GetInspector(IDbConnection conn);

        // ---------------------------------------------------------------

        Query<T> CreateQuery<T>(Class cdesc, IDbConnection connection) where T : new();

        Binder GetBinder();
        Binder GetBinder(Type type);
    }
}
