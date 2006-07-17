using System;
using System.Collections;
using System.Reflection;
using System.Text;

using PMS.Data;
using PMS.Metadata;

namespace PMS.Query
{
    /// <summary>
    /// Query based on object properties
    /// </summary>
    public class QueryByObject : AbstractQuery
    {
        /// <summary>
        /// Construct with object
        /// </summary>
        /// <param name="obj">Object used to create SQL from</param>
        public QueryByObject(Object obj) : this (obj, null)
        {
        }

        /// <summary>
        /// Construct with object and detailed criteria
        /// </summary>
        /// <param name="obj">Object used to create SQL from</param>
        /// <param name="crit">Criteria to append object properties with</param>
        public QueryByObject(Object obj, Criteria crit)
        {
            this.metaObject = new MetaObject(obj);
            this.criteria = (crit == null) ? new Criteria(obj.GetType()) : crit;
            this.columns = metaObject.Columns;
            this.keys = metaObject.PrimaryKeys;
        }

        /// <summary>
        /// Return the conditional statement.  If the primaryKey is set,
        /// just return key=value.  Else loop through the objects Fields and
        /// setup key=value pairs.
        /// </summary>
        public override string Condition {
            get {
                StringBuilder sql = new StringBuilder();
                sql.Append(" WHERE ");

                bool keyClause  = false;
                bool withClause = false;

                // loop through keys (not columns)
                for (int i=0; i < keys.Length; i++) {
                    if (metaObject.IsFieldSet(keys[i])) {
                        if (i > 0 && keyClause) 
                            sql.Append(" AND ");
                        
                        sql.Append(keys[i]);
                        sql.Append("=");
                        sql.Append(metaObject.GetSqlValue(keys[i]));
                        keyClause = true;
                    }
                }
                
                if (keyClause == true)
                    return sql.ToString();
                
                // loop through columns (not keys)
                for (int i=0; i < columns.Length; i++) {
                    if (metaObject.IsFieldSet(columns[i])) {
                        if (keyClause == true || (i > 0 && withClause)) 
                            sql.Append(" AND ");
                        
                        sql.Append(columns[i]);
                        sql.Append("=");
                        sql.Append(metaObject.GetSqlValue(columns[i]));
                        
                        withClause = true;
                    }
                }

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
        public override string InsertClause {
            get {
                bool keyClause = false;
                bool clause = false;
                StringBuilder sql1 = new StringBuilder();
                StringBuilder sql2 = new StringBuilder();

                for (int i=0; i < this.keys.Length; i++) {
                    if (metaObject.IsFieldSet(this.keys[i])) {
                        if (i > 0 && keyClause) {
                            sql1.Append(", ");
                            sql2.Append(", ");
                        }
                        sql1.Append(this.keys[i]);
                        sql2.Append(metaObject.GetSqlValue(this.keys[i]));
                        keyClause = true;
                    }
                }

                for (int i=0; i < columns.Length; i++) {
                    if (metaObject.IsFieldSet(columns[i])) {
                        if (keyClause == true || i > 0 && clause) {
                            sql1.Append(", ");
                            sql2.Append(", ");
                        }
                        sql1.Append(columns[i]);
                        sql2.Append(metaObject.GetSqlValue(columns[i]));
                        clause = true;
                    }
                }
                
                return "(" + sql1.ToString() + 
                    ") VALUES (" + sql2.ToString() + ")";
            }
        }

        /// <summary>
        /// Create SQL like "SET key=value, column=value"
        /// </summary>
        public override string UpdateClause {
            get {
                bool clause    = false;
                StringBuilder sql = new StringBuilder(" SET ");

                for (int i=0; i < columns.Length; i++) {
                    if (metaObject.IsFieldSet(columns[i])) {
                        if (i > 0 && clause) 
                            sql.Append(", ");
                        
                        sql.Append(columns[i]);
                        sql.Append("=");
                        sql.Append(metaObject.GetSqlValue(columns[i]));
                        clause = true;
                    }
                }
                
                return sql.ToString();
            }
        }

        /// <summary>
        /// Create SQL like "ORDER BY x ASC, y DESC, z"
        /// </summary>
        public override string OrderBy {
            get {
                if (criteria.OrderCount > 0)
                    return criteria.GetOrderByClause();

                return String.Empty;
            }
        }
    }
}
