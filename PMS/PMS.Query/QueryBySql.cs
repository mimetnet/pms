using System;
using System.Reflection;

using PMS.Metadata;

namespace PMS.Query
{
    /// <summary>
    /// Perform Query based on pure SQL
    /// </summary>
    [Serializable]
    public sealed class QueryBySql : AbstractQuery
    {
        private Type mtype;
        private String sql;

        /// <summary>
        /// Construct with raw SQL and Type
        /// </summary>
        /// <param name="type">Type of class</param>
        /// <param name="sql">SQL</param>
        public QueryBySql(Type type, String sql) : this(type, sql, null)
        {
            this.criteria = new Criteria(type);
        }

        /// <summary>
        /// Construct with raw SQL, Type and Object to replace SQL variables
        /// </summary>
        /// <param name="type">Type of class</param>
        /// <param name="sql">SQL</param>
        /// <param name="obj">Object's Fields are used to substitute SQL variables</param>
        public QueryBySql(Type type, String sql, Object obj)
        {
			if (type == null) throw new ArgumentNullException("type");
			if (String.IsNullOrEmpty(sql)) throw new ArgumentNullException("sql");

            this.obj = obj;
            this.mtype = type;
            this.sql = (this.obj != null) ? PrepareSql(sql) : sql;
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

        private string PrepareSql(string s)
        {
            Object val;

            FieldInfo[] finfos = 
                this.obj.GetType().GetFields(BindingFlags.NonPublic |
                                             BindingFlags.Instance |
                                             BindingFlags.Public);

            foreach (FieldInfo info in finfos) {
                if ((val = info.GetValue(obj)) != null)
                    s = s.Replace((':' + info.Name), val.ToString());
            }

            return s;
        }
    }
}
