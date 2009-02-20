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
        public QueryByCriteria(Criteria criteria) : base(criteria.Description)
        {
            this.criteria = criteria;
        }

        /// <summary>
        /// Return WhereClauses from criteria
        /// </summary>
        public override string Condition {
            get {
                StringBuilder str = new StringBuilder();
                this.criteria.AppendCondition(str);
                return str.ToString();
            }
        }

        /// <summary>
        /// Return OrderByClauses from criteria
        /// </summary>
        public override string OrderBy {
            get {
                StringBuilder str = new StringBuilder();
                this.criteria.AppendOrderBy(str);
                return str.ToString();
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
