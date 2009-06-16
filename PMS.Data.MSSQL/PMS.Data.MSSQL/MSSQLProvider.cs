using System;
using System.Data;

using PMS.Metadata;
using PMS.Query;

namespace PMS.Data.MSSQL
{
    [Serializable]
    internal sealed class MSSQLProvider : PMS.Data.AbstractProvider
    {
        public override Type Type {
            get { return typeof(MSSQLConnection); }
        }

        public override IDbConnection GetConnection()
        {
            return new MSSQLConnection();
        }

        public override IDbConnection GetConnection(string properties)
        {
            return new MSSQLConnection(properties);
        }

        public override IDataParameter CreateParameter(string name, object value, PMS.DbType dbType)
        {
            IDataParameter p = new System.Data.SqlClient.SqlParameter();
            p.ParamterName = name;
            if (dbType != null)
                p.DbType = dbType.SystemDbType;
            p.Value = value;
            return p;
        }

        public override IDbInspector GetInspector()
        {
            return new MSSQLInspector();
        }

        public override IDbInspector GetInspector(IDbConnection conn)
        {
            return new MSSQLInspector(conn);
        }

        //public override Query<T> CreateQuery<T>(Class cdesc, IDbConnection connection)
        //{
        //    throw new Exception();
        //}
    }
}
