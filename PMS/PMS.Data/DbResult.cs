using System;

namespace PMS.Data
{
    public class DbResult
    {
        private int records;
        private string sql;

        public DbResult()
        {
            this.records = -1;
            this.sql = String.Empty;
        }

        public DbResult(int records, string sql)
        {
            this.records = records;
            this.sql = sql;
        }

        public DbResult(string sql)
        {
            this.records = -1;
            this.sql = sql;
        }

        public DbResult(int records)
        {
            this.records = records;
            this.sql = "Unknown";
        }
        
        public int Count {
            get {
                return records;
            }
        }

        public string SQL {
            get {
                return sql;
            }
        }

        public override string ToString()
        {
            return "SQL = " + this.sql + " | Records : " + this.records;
        }
        
        public static DbResult operator +(DbResult a, DbResult b)
        {
            return new DbResult(a.Count + b.Count, a.SQL + ";\n" + b.SQL);
        }
    }
}
