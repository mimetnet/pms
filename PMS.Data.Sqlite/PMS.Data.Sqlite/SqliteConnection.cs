using System;
using System.Data;
using System.IO;

using PMS.Data;

namespace PMS.Data.Sqlite
{
    internal sealed class SqliteConnection : DbConnectionProxy
    {
		public SqliteConnection() : 
#if NET_2_0
			base(typeof(System.Data.SQLite.SQLiteConnection))
#else
			base(typeof(Mono.Data.Sqlite.SqliteConnection))
#endif
		{
		}

		public SqliteConnection(string connectionString) : 
#if NET_2_0
			base(typeof(System.Data.SQLite.SQLiteConnection))
#else
			base(typeof(Mono.Data.Sqlite.SqliteConnection))
#endif
		{
			this.ConnectionString = connectionString;
		}
    }
}
