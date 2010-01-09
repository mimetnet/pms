using System;
using System.Data;

namespace PMS.Driver.MSSQL
{
    public sealed class MSSQLInspector : PMS.Data.IDbInspector
    {
        private DataSet database = new DataSet();
        private IDbConnection connection;

        private const string sqlTables = "SELECT oid as id, relname as name, relnatts as column_count FROM pg_class WHERE relkind = 'r' AND relname NOT LIKE 'pg_%'";

        // replace {0} with table name
        private const string sqlTableColumns = "select a.attname as field, t.typname as type, a.attlen as type_size, a.atttypmod as field_size, a.attnotnull as not_null, a.atthasdef as has_default, d.adsrc as default from pg_attribute a inner join pg_type t on a.atttypid = t.oid inner join pg_class c on a.attrelid = c.oid left join pg_attrdef d on c.oid = d.oid and a.attnum = d.adnum where a.attnum > 0 and a.attisdropped = 'f' and c.relname = '{0}'";

        private const string selectConstraints = "select c.relname, c.relkind from pg_class c inner join pg_constraint x on c.oid = x.conrelid where c.relkind in ('r','i','S');";

        public MSSQLInspector()
        {

        }

        public MSSQLInspector(IDbConnection conn)
        {
            connection = conn;
        }

        public IDbConnection Connection {
            get { return connection; }
            set { connection = value; }
        }

        public void Examine()
        {
            try {
                if (connection.State.Equals(ConnectionState.Open) == false)
                    connection.Open();

                DataTable table;
                DataColumn column;

                IDataReader colReader;

                IDbCommand cmd = connection.CreateCommand();
                cmd.CommandText = sqlTables;

                IDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    table = new DataTable();
                    table.TableName = (string) reader["name"];

                    cmd.CommandText = String.Format(sqlTableColumns,
                                                    table.TableName);
                    colReader = cmd.ExecuteReader();
                    while (colReader.Read()) {
                        column = new DataColumn();
                        column.ColumnName = (string) colReader["field"];
                        //column.DataType = 
                        //    provider.GetType((string)colReader["type"]);
                        table.Columns.Add(column);
                    }

                    database.Tables.Add(table);
                }
            } catch (Exception e) {
                throw e;
            } finally {
                if (connection != null)
                    connection.Close();
            }
        }

        public DataSet Database {
            get { return database; }
        }
    }
}
