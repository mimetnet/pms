using System;
using System.Data.SqlClient;

using PMS.Data;

namespace PMS.Data.MSSQL
{
    internal sealed class MSSQLConnection : DbConnectionProxy
    {
		public MSSQLConnection() : 
			base(typeof(SqlConnection))
		{
		}

		public MSSQLConnection(string connectionString) : 
			base(typeof(SqlConnection))
		{
			this.ConnectionString = connectionString;
		}
    }
}
