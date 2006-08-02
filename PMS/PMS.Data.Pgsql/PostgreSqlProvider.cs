using System;
using System.Data;

using Npgsql;

using PMS.Data;

namespace PMS.Data.PostgreSql
{
    [Serializable]
    internal sealed class PostgreSqlProvider : AbstractProvider
    {
        private static IProvider _instance;

        public static IProvider Instance {
            get {
                if (_instance == null)
                    _instance = new PostgreSqlProvider();
 
                return _instance;
            }
        }

        public override IDbConnection GetConnection()
        {
            return new NpgsqlConnection();
        }

        public override IDbConnection GetConnection(string connString)
        {
            return new NpgsqlConnection(connString);
        }

        public override IDbInspector GetInspector()
        {
            return new PostgreSqlInspector();
        }

        public override IDbInspector GetInspector(IDbConnection conn)
        {
            return new PostgreSqlInspector(conn);
        }
    }
}
