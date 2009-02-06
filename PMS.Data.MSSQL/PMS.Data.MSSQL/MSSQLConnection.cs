using System;
using System.Data;
using System.IO;

using PMS.Data;

namespace PMS.Data.MSSQL
{
    internal sealed class MSSQLConnection : DbConnectionProxy
    {
		public MSSQLConnection() : 
			base(typeof(System.Data.SQLiteConnection))
		{
		}

		public MSSQLConnection(string connectionString) : 
			base(typeof(System.Data.SQLiteConnection))
		{
			this.ConnectionString = connectionString;
		}
    }
}
