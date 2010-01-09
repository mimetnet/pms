using System;
using System.Data;

namespace PMS.Driver.Postgresql
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

        public override IDataParameter CreateParameter(string name, object value, PMS.DbType dbType)
        {
            IDataParameter p = new Npgsql.NpgsqlParameter();
            p.ParameterName = name;
            if (dbType != null)
                p.DbType = dbType.SystemDbType;
            p.Value = (value != null)? value : DBNull.Value;
            return p;

        }

        public override IDbConnection GetConnection()
        {
            return new PostgresqlConnection();
        }

        public override IDbConnection GetConnection(string properties)
        {
            return new PostgresqlConnection(properties);
        }

        public override PMS.Data.IDbInspector GetInspector()
        {
            return new PostgresqlInspector();
        }

        public override PMS.Data.IDbInspector GetInspector(IDbConnection conn)
        {
            return new PostgresqlInspector(conn);
        }
    }
}
