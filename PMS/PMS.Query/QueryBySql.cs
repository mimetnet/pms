using System;
using System.Text;

using PMS.Metadata;

namespace PMS.Query
{
    /// <summary>
    /// Perform Query based on raw SQL and Type
    /// </summary>
    public class QueryBySql : AbstractQuery
    {
        private Type mtype;
        private string sql;

        /// <summary>
        /// Construct with raw SQL and Type
        /// </summary>
        /// <param name="type">Type of class</param>
        /// <param name="sql">SQL</param>
        public QueryBySql(Type type, string sql)
        {
            this.criteria = new Criteria();
            this.sql = sql;
            this.mtype = type;
        }

        public override Type Type {
            get { return mtype; }
        }

        /// <summary>
        /// SQL for Insert
        /// </summary>
        /// <returns>SQL for Insert</returns>
        public override string Insert()
        {
            return sql;
        }

        /// <summary>
        /// SQL for Update
        /// </summary>
        /// <returns>SQL for Update</returns>
        public override string Update()
        {
            return sql;
        }

        /// <summary>
        /// SQL for Delete
        /// </summary>
        /// <returns>SQL for Delete</returns>
        public override string Delete()
        {
            return sql;
        }

        /// <summary>
        /// SQL for Select
        /// </summary>
        /// <returns>SQL for Select</returns>
        public override string Select()
        {
            return sql;
        }

        /// <summary>
        /// SQL for Count
        /// </summary>
        /// <returns>SQL for Count</returns>
        public override string Count()
        {
            return sql;
        }
    }
}
