using System;
using System.Data;

namespace PMS.Driver.Sqlite
{
    internal sealed class SqliteConnection : PMS.Data.DbConnectionProxy
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

        public override bool CanReopen(Exception ex)
        {
            log.Info("CanReopen<?>: " + ex.GetType());

            try {
                if (ex == null)
                    return false;

#if NET_2_0
                System.Data.SQLite.SQLiteException err = ex as System.Data.SQLite.SQLiteException;

                if (null == err)
                    return false;

                if (err.ErrorCode == System.Data.SQLite.SQLiteErrorCode.IOErr)
                    return Reopen();
#endif

                return false;
            } catch (Exception e) {
                log.Error("CanReopen: ", e);
            }

            return false;
        }
    }
}
