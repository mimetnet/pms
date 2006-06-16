using System;

namespace PMS.Data
{
    public class DbResult
    {
        private int records;
        private string sql;
        private Exception ex;

        public DbResult(int records, string sql, Exception ex)
        {
            this.records = records;
            this.sql = sql;
            this.ex = ex;
        }

        public DbResult(int records, string sql) : this(records, sql, null)
        {
        }

        public DbResult(string sql) : this(0, sql, null)
        {
        }

        public DbResult(string sql, Exception ex): this(0, sql, ex)
        {
        }

        public DbResult(int records) : this(records, String.Empty, null)
        {
        }

        public DbResult(int records, Exception ex)
            : this(records, String.Empty, ex)
        {
        }

        public DbResult() : this(0, String.Empty, null)
        {
        }
        
        public int Count {
            get { return records; }
        }

        public string SQL {
            get { return sql; }
        }

        public Exception Exception {
            get { return this.ex; }
        }

        public override string ToString()
        {
            string stype = ((ex != null)? ex.GetType().ToString() : String.Empty);
            return String.Format("SQL = {0} | Records = {1} | Ex = {2}", records, sql, stype);
        }
        
        public static DbResult operator +(DbResult a, DbResult b)
        {
            return new DbResult(a.Count + b.Count, a.SQL + ";\n" + b.SQL);
        }
    }
}
