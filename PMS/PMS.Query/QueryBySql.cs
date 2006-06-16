using System;
using System.Text;

using PMS.Metadata;

namespace PMS.Query
{
    public class QueryBySql : AbstractQuery
    {
        private Type mtype;
        private string sql;

        public QueryBySql(Type type, string sQuery)
        {
            criteria = new Criteria();
            sql = sQuery;
            mtype = type;
        }

        public override Type Type {
            get { return mtype; }
        }

        public override string Insert()
        {
            return sql;
        }

        public override string Update()
        {
            return sql;
        }

        public override string Delete()
        {
            return sql;
        }

        public override string Select()
        {
            return sql;
        }

        public override string Count()
        {
            return sql;
        }
    }
}
