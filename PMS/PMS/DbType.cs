using System;

namespace PMS
{
    public class DbType
    {
        protected System.Data.DbType type;

        public System.Data.DbType SystemDbType {
            get { return type; }
        }

        public DbType(System.Data.DbType dbType)
        {
            this.type = dbType;
        }
    }
}
