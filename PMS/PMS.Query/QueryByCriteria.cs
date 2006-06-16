using System;
using System.Collections;
using System.Reflection;
using System.Text;

using PMS.Data;
using PMS.Metadata;

namespace PMS.Query
{
    public class QueryByCriteria : AbstractQuery
    {
        public QueryByCriteria(Criteria c)
        {
            criteria = c;
            metaObject = c.metaObject;
        }

        public override string Condition {
            get {
                return (criteria.ClauseCount > 0) ? 
                    criteria.GetWhereClause() : String.Empty;
            }
        }

        public override string OrderBy {
            get {
                return (criteria.OrderCount > 0) ?
                    criteria.GetOrderByClause() : String.Empty;
            }
        }

        public override string InsertClause {
            get {
                throw new NotImplementedException();
            }
        }

        public override string UpdateClause {
            get {
                throw new NotImplementedException();
            }
        }
    }
}
