namespace PMS.Query
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public class RangeClause : IClause
    {
        private string field;
        private string op;
        private string comparison;
        private object[] values = new Object[2];

        internal RangeClause(string field, string oper, object val1, string comparison, object val2)
        {
            this.field = field;
            this.comparison = comparison;
            this.op = oper;
            this.values[0] = val1;
            this.values[1] = val2;
        }

        public bool IsCondition { 
            get { return true; } 
        }

        // (id NOT BETWEEN 3 AND 5)
        public override string ToString()
        {
            return ("(" + field + " " + op + " @" + field + "1 " + comparison + " @" + field + "2)");
        }

        public IList<IDataParameter> CreateParameters(CreateParameterDelegate callback)
        {
            if (callback == null)
                throw new ArgumentNullException("CreateParameterDelegate");

            List<IDataParameter> list = new List<IDataParameter>();
            list.Add(callback("@" + field + "1", values[0]));
            list.Add(callback("@" + field + "2", values[1]));

            return list;
        }
    }
}
