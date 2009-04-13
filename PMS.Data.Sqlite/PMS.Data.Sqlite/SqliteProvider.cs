using System;
using System.Data;
using System.Data.SQLite;

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

        public override IDataParameter CreateParameter(string name, object value)
        {
            Type t = value.GetType();

            if (typeof(DateTime) == t) {
                value = ((DateTime)value).ToString("yyyy-MM-dd HH:mm:sszz");
            }

            return new SQLiteParameter(name, value);
        }
    }
}
