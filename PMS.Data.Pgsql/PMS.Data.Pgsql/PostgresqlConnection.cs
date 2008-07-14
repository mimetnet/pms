using System;
using System.Data;
using System.IO;

using PMS.Data;

using Npgsql;

namespace PMS.Data.Postgresql
{
    internal sealed class PostgresqlConnection : DbConnectionProxy
    {
		public PostgresqlConnection() : base(typeof(NpgsqlConnection))
		{
		}

		public PostgresqlConnection(string connectionString) : base(typeof(NpgsqlConnection))
		{
			this.ConnectionString = connectionString;
		}

		public override bool CanReopen(Exception ex)
		{
			if (ex == null)
				return false;
			
			// older Npgsql threw this on database restart
			if (ex.GetType() == typeof(NotSupportedException)) 
				return true;
			else if (ex.InnerException != null && ex.InnerException.GetType() == typeof(IOException))
				return this.Reopen();

			NpgsqlException nex = ex as NpgsqlException;

			if (nex == null)
				return false;

			Console.WriteLine("ShouldReopen: " + nex.ToString());
			Console.WriteLine("ShouldReopen: " + nex.InnerException);

			return false;
		}
    }
}
