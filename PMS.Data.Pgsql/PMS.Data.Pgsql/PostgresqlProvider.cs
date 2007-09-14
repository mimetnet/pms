using System;
using System.Data;

namespace PMS.Data.Postgresql
{
    [Serializable]
    internal sealed class PostgresqlProvider : PMS.Data.AbstractProvider
    {
		public override Type Type {
			get { return typeof(Npgsql.NpgsqlConnection); }
		}

        public override IDbConnection GetConnection()
        {
            return new Npgsql.NpgsqlConnection();
        }

        public override IDbConnection GetConnection(string properties)
        {
            return new Npgsql.NpgsqlConnection(properties);
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
