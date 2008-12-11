using System;

namespace PMS.Data
{
    [Serializable]
    public sealed class DbResult
    {
		private object obj;
        private Int64 records;
        private string sql;
        private Exception ex;

        public DbResult(Int64 records, Object obj, string sql, Exception ex)
        {
            this.records = records;
			this.obj = obj;
            this.sql = sql;
            this.ex = ex;
        }

        public DbResult(Object obj, string sql)
        {
			Int64 x=0;

			if (obj != null) {
				this.records = Int64.TryParse(obj.ToString(), out x)? x : 1;
			} else {
				this.records = -1;
			}

			this.obj = obj;
			this.sql = sql;
        }

        public DbResult(Int64 records, string sql, Exception ex) : this(records, null, sql, ex)
        {
        }

        public DbResult(Int64 records, string sql) : this(records, null, sql, null)
        {
        }

        public DbResult(string sql) : this(0, null, sql, null)
        {
        }

        public DbResult(string sql, Exception ex): this(0, null, sql, ex)
        {
        }

        public DbResult(Int64 records) : this(records, null, String.Empty, null)
        {
        }

        /// <summary>
        /// Construct with records and exception
        /// </summary>
        /// <param name="records">Records Affected/Returned</param>
        /// <param name="ex">Exception fromd database</param>
        public DbResult(Int64 records, Exception ex) : this(records, String.Empty, ex)
        {
        }

        public DbResult(Exception exception) : this(0, String.Empty, exception)
        {
        }

        /// <summary>
        /// Simple construct used to help combine results (PersistenceBroker.Delete(object[] list)
        /// </summary>
        /// <see cref="PMS.Broker.PersistenceBroker"/>
        public DbResult() : this(0, String.Empty, null)
        {
        }
        
        /// <summary>
        /// Records Retrieved/Affected
        /// </summary>
        public object Object {
            get { return obj; }
        }
		
		/// <summary>
        /// Records Retrieved/Affected
        /// </summary>
        public Int64 Count {
            get { return records; }
        }

        /// <summary>
        /// SQL Executed
        /// </summary>
        public string SQL {
            get { return sql; }
        }

        /// <summary>
        /// Exception thrown when executing SQL
        /// </summary>
        public Exception Exception {
            get { return this.ex; }
        }

        /// <summary>
        /// PrInt64er friendly description of object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
			return this.ToString(true);
		}

		public string ToString(bool exception)
		{
            if (exception && ex != null) {
                return String.Format("SQL = {0} | Records = {1} | Ex = {2} - {3}", 
                    sql, records, ex.GetType(), ex.Message);
            }

            return String.Format("SQL = {0} | Records = {1}", sql, records);
        }
        
        /// <summary>
        /// Combine DbResults
        /// </summary>
        /// <param name="a">Result to append</param>
        /// <param name="b">Result to be appended</param>
        /// <returns></returns>
        public static DbResult operator +(DbResult a, DbResult b)
        {
            return new DbResult((a.Count + b.Count), 
                                (a.SQL + ";\n" + b.SQL));
        }
    }
}
