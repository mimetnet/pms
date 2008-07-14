using System;
using System.Data;

namespace PMS.Data.Postgresql
{
    internal sealed class PostgresqlProvider : PMS.Data.AbstractProvider
    {
		public PostgresqlProvider()
		{
			//Npgsql.NpgsqlEventLog.Level = Npgsql.LogLevel.Debug;
			//Npgsql.NpgsqlEventLog.LogName = "NpgsqlTests.log";
			//Npgsql.NpgsqlEventLog.EchoMessages = true;
		}

		public override Type Type {
			get { return typeof(PostgresqlConnection); }
		}

        public override IDbConnection GetConnection()
        {
            return new PostgresqlConnection();
        }

        public override IDbConnection GetConnection(string properties)
        {
            return new PostgresqlConnection(properties);
        }

        public override IDbInspector GetInspector()
        {
            return new PostgresqlInspector();
        }

        public override IDbInspector GetInspector(IDbConnection conn)
        {
            return new PostgresqlInspector(conn);
        }
    }
}
