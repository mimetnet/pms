using System;
using System.Data;

using Mono.Data.Sqlite;

namespace PMS.Data.Sqlite
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

        public override IDbInspector GetInspector()
        {
            return new SqliteInspector();
        }

        public override IDbInspector GetInspector(IDbConnection conn)
        {
            return new SqliteInspector(conn);
        }
    }
}
