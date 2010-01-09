using System;
using System.Data;
#if NET_2_0
using System.Data.SQLite;
#else
using Mono.Data.Sqlite;
#endif

namespace PMS.Driver.Sqlite
{
    [Serializable]
    internal sealed class SqliteProvider : PMS.Data.AbstractProvider
    {
        public override Type Type {
            get { return typeof(SqliteConnection); }
        }

        public override IDbConnection GetConnection()
        {
            return new SqliteConnection();
        }

        public override IDbConnection GetConnection(string properties)
        {
            return new SqliteConnection(properties);
        }

        public override PMS.Data.IDbInspector GetInspector()
        {
            return new SqliteInspector();
        }

        public override PMS.Data.IDbInspector GetInspector(IDbConnection conn)
        {
            return new SqliteInspector(conn);
        }

        public override IDataParameter CreateParameter(string name, object value, PMS.DbType dbType)
        {
#if NET_2_0
            IDataParameter p = new SQLiteParameter();
#else
            IDataParameter p = new SqliteParameter();
#endif
            p.ParameterName = name;
            if (dbType != null)
                p.DbType = dbType.SystemDbType;
            p.Value = (value != null)? value : DBNull.Value;
            return p;

        }
    }
}
