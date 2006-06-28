using System;
using System.Collections;
using System.Reflection;
using System.Text;

using PMS.Data;
using PMS.Metadata;

namespace PMS.Query
{
    /// <summary>
    /// Query by values set by Criteria
    /// </summary>
    public class QueryByCriteria : AbstractQuery
    {
        /// <summary>
        /// Construct with criteria
        /// </summary>
        /// <param name="criteria">Criteria to use for SQL</param>
        public QueryByCriteria(Criteria criteria)
        {
            this.criteria = criteria;
            this.metaObject = criteria.metaObject;
        }

        /// <summary>
        /// Return WhereClauses from criteria
        /// </summary>
        public override string Condition {
            get {
                return (criteria.ClauseCount > 0) ? 
                    criteria.GetWhereClause() : String.Empty;
            }
        }

        /// <summary>
        /// Return OrderByClauses from criteria
        /// </summary>
        public override string OrderBy {
            get {
                return (criteria.OrderCount > 0) ?
                    criteria.GetOrderByClause() : String.Empty;
            }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public override string InsertClause {
            get {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public override string UpdateClause {
            get {
                throw new NotImplementedException();
            }
        }
    }
}
