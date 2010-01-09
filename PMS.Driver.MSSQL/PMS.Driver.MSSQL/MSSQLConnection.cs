using System;
using System.Data.SqlClient;

namespace PMS.Driver.MSSQL
{
    internal sealed class MSSQLConnection : PMS.Data.DbConnectionProxy
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
