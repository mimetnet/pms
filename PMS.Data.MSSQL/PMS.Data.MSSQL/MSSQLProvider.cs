using System;
using System.Data;
using System.Data.SqlClient;

namespace PMS.Data.MSSQL
{
    [Serializable]
    internal sealed class MSSQLProvider : PMS.Data.AbstractProvider
    {
		public override Type Type {
			get { return typeof(SqlConnection); }
		}

        public override IDbConnection GetConnection()
        {
			return new SqlConnection();
        }

        public override IDbConnection GetConnection(string properties)
        {
			return new SqlConnection(properties);
		}

        public override IDbInspector GetInspector()
        {
            return new MSSQLInspector();
        }

        public override IDbInspector GetInspector(IDbConnection conn)
        {
            return new MSSQLInspector(conn);
        }
    }
}
