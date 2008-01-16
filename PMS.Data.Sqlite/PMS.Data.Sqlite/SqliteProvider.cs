using System;
using System.Data;

#if NET_2_0
using System.Data.SQLite;
#else
using Mono.Data.Sqlite;
#endif

namespace PMS.Data.Sqlite
{
    [Serializable]
    internal sealed class SqliteProvider : PMS.Data.AbstractProvider
    {
		public override Type Type {
#if NET_2_0
			get { return typeof(SQLiteConnection); }
#else
			get { return typeof(SqliteConnection); }
#endif
		}

        public override IDbConnection GetConnection()
        {
#if NET_2_0
			return new SQLiteConnection();
#else
			return new SqliteConnection();
#endif
        }

        public override IDbConnection GetConnection(string properties)
        {
#if NET_2_0
			return new SQLiteConnection(properties);
#else
			return new SqliteConnection(properties);
#endif
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
