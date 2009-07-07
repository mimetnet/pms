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

        public override IDataParameter CreateParameter(string name, object value, PMS.DbType dbType)
        {
            //Type t = value.GetType();
            //if (typeof(DateTime) == t) {
            //    value = ((DateTime)value).ToString("yyyy-MM-dd HH:mm:sszz");
            //}
            //return new SqliteParameter(name, value);

            IDataParameter p = new SqliteParameter();
            p.ParameterName = name;
            if (dbType != null)
                p.DbType = dbType.SystemDbType;
            p.Value = value;
            return p;

        }
    }
}
