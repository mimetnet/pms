using System;
using System.Collections;
using System.Reflection;
using System.Text;

using PMS.Data;
using PMS.Metadata;

namespace PMS.Query
{
    /// <summary>
    /// Query based on Object.FieldInfo[]
    /// </summary>
    public class QueryByObject : AbstractQuery
    {
        /// <summary>
        /// Construct with object
        /// </summary>
        /// <param name="obj">Object used to create SQL from</param>
        public QueryByObject(Object obj) : base (obj, null)
        {
        }

        /// <summary>
        /// Construct with object and detailed criteria
        /// </summary>
        /// <param name="obj">Object used to create SQL from</param>
        /// <param name="crit">Criteria to append object properties with</param>
        public QueryByObject(Object obj, Criteria crit) : base(obj, crit)
        {
        }

        #region Overrides

        /// <summary>
        /// Return the conditional statement.  If the primaryKey is set,
        /// just return key=value.  Else loop through the objects Fields and
        /// setup key=value pairs.
        /// </summary>
        public override string Condition
        {
            get
            {
                bool withClause = false;
                StringBuilder sql = new StringBuilder();

                sql.Append(" WHERE ");

                if (BuildList(this.keys, " AND ", ref sql) == true)
                    return sql.ToString();

                withClause = BuildList(this.columns, " AND ", ref sql);

                if (criteria.ClauseCount > 0) {
                    sql.Append(criteria.GetWhereClause());
                    withClause = true;
                }

                if (withClause == true)
                    return sql.ToString();

                return String.Empty;
            }
        }

        /// <summary>
        /// (column, column, column) VALUES (value, value, value)
        /// </summary>
        public override string InsertClause
        {
            get {
                bool keyClause = false;
                bool clause = false;
                StringBuilder sql1 = new StringBuilder();
                StringBuilder sql2 = new StringBuilder();

                for (int i = 0; i < this.keys.Count; i++) {
                    if (IsFieldSet(this.keys[i])) {
                        if (i > 0 && keyClause) {
                            sql1.Append(", ");
                            sql2.Append(", ");
                        }
                        sql1.Append(this.keys[i].Column);
                        sql2.Append(GetSqlValue(this.keys[i]));
                        keyClause = true;
                    }
                }

                for (int i = 0; i < this.columns.Count; i++) {
                    if (IsFieldSet(columns[i])) {
                        if (keyClause == true || i > 0 && clause) {
                            sql1.Append(", ");
                            sql2.Append(", ");
                        }
                        sql1.Append(columns[i].Column);
                        sql2.Append(GetSqlValue(columns[i]));
                        clause = true;
                    }
                }

                return "(" + sql1.ToString() +
                    ") VALUES (" + sql2.ToString() + ")";
            }
        }

        /// <summary>
        /// "SET key=value, column=value"
        /// </summary>
        public override string UpdateClause {
            get {
                StringBuilder sql = new StringBuilder(" SET ");

                BuildList(this.columns, ", ", ref sql);

                return sql.ToString();
            }
        }

        /// <summary>
        /// "ORDER BY x ASC, y DESC, z"
        /// </summary>
        public override string OrderBy
        {
            get {
                if (criteria.OrderCount > 0)
                    return criteria.GetOrderByClause();

                return String.Empty;
            }
        } 
        #endregion

        private bool BuildList(FieldCollection fields, string seperator, ref StringBuilder sql)
        {
            bool result = false;

            for (int i = 0; i < fields.Count; i++) {
                if (IsFieldSet(fields[i])) {
                    if (i > 0 && result) {
                        sql.Append(seperator);
                    }

                    sql.Append(fields[i].Column);
                    sql.Append("=");
                    sql.Append(GetSqlValue(fields[i]));
                    result = true;
                }
            }

            return result;
        }
    }
}
