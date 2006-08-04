using System;
using System.Collections;
using System.Reflection;
using System.Text;

using PMS.Data;
using PMS.Metadata;

namespace PMS.Query
{
    /// <summary>
    /// Query by comparing two objects properties
    /// </summary>
    [Serializable]
    public class QueryByObjectDiff : AbstractQuery
    {
        /// <summary>
        /// Construct with old and new objects
        /// </summary>
        /// <param name="oldObj">Old Object</param>
        /// <param name="newObj">Updated Object</param>
        public QueryByObjectDiff(object oldObj, object newObj)
        {
            throw new NotImplementedException();
            /*
            MetaObject m1 = new MetaObject(oldObj);
            MetaObject m2 = new MetaObject(newObj);

            // loop through keys (not columns)
            foreach (string k in m1.PrimaryKeys) {
                if (m1.IsFieldSet(k)) {

                }
            }


            this.metaObject = m2;
            this.criteria = new Criteria(newObj.GetType());
            this.columns = m2.Columns;
            this.keys = m2.PrimaryKeys;
            */
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
                for (int i=0; i < keys.Count; i++) {
                    if (metaObject.IsFieldSet(keys[i])) {
                        if (i > 0 && keyClause) 
                            sql.Append(" AND ");

                        sql.Append(keys[i].Column);
                        sql.Append("=");
                        sql.Append(metaObject.GetSqlValue(keys[i]));
                        keyClause = true;
                    }
                }
                
                if (keyClause == true)
                    return sql.ToString();
                
                // loop through columns (not keys)
                for (int i = 0; i < columns.Count; i++) {
                    if (metaObject.IsFieldSet(columns[i])) {
                        if (keyClause == true || (i > 0 && withClause)) 
                            sql.Append(" AND ");

                        sql.Append(columns[i].Column);
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

                for (int i = 0; i < keys.Count; i++) {
                    if (metaObject.IsFieldSet(keys[i])) {
                        if (i > 0 && keyClause) {
                            sql1.Append(", ");
                            sql2.Append(", ");
                        }
                        sql1.Append(keys[i]);
                        sql2.Append(metaObject.GetSqlValue(keys[i]));
                        keyClause = true;
                    }
                }

                for (int i = 0; i < columns.Count; i++) {
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

                for (int i = 0; i < columns.Count; i++) {
                    if (metaObject.IsFieldSet(columns[i])) {
                        if (i > 0 && clause) 
                            sql.Append(", ");
                        
                        sql.Append(columns[i].Column);
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
