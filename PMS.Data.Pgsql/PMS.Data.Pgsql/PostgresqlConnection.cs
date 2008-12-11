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
			
			try {
				Type exType = ex.GetType();
				Type iexType = (ex.InnerException != null)? ex.InnerException.GetType() : null;

				//log.Warn("exType: " + exType);
				//log.Warn("iexType: " + iexType);

				// older Npgsql (8.1) threw this on database restart
				if ((exType == typeof(NotSupportedException) || exType == typeof(IOException)) || 
						(iexType != null && (iexType == typeof(NotSupportedException) || iexType == typeof(IOException))))
					return this.Reopen();

				NpgsqlException nex = ex as NpgsqlException;

				if (nex == null)
					return false;

				// 8.3
				if (nex.Code == "57P01")
					return this.Reopen();

				//Console.WriteLine("ShouldReopen: " + nex.ToString());
				//Console.WriteLine("ShouldReopen: " + nex.InnerException);
				//Console.WriteLine("ShouldReopen: " + (nex.Code == "57P01").ToString());
			} catch (Exception e) {
				log.Error("CanReopen(Pgsql): ", e);
			}

			return false;
		}
	}
}
