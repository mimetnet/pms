namespace PMS.Query
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;

    public class RangeClause : IClause
    {
        protected PMS.DbType dbType;
        protected bool parenthesis;
        protected string field;
        protected string op;
        protected string separator;
        protected object[] values;

        internal RangeClause(string field, string oper, string separator, params object[] list)
            : this(field, oper, separator, true, list)
        {
        }

        internal RangeClause(string field, string oper, string separator, bool parenthesis, params object[] list)
            : this(field, null, oper, separator, parenthesis, list)
        {
        }

        internal RangeClause(string field, PMS.DbType dbType, string oper, string separator, bool parenthesis, params object[] list)
        {
            this.field = field;
            this.separator = separator;
            this.op = oper;
            this.values = list;
            this.parenthesis = parenthesis;
        }

        public string Name {
            get { return field; }
        }

        public bool IsCondition { 
            get { return true; } 
        }

        // id NOT BETWEEN 3 AND 5
        // id IN (1,2,3)
        public override string ToString()
        {
            StringBuilder str = new StringBuilder(this.field);
            str.Append(' ');
            str.Append(this.op);
            str.Append(' ');

            if (this.parenthesis)
                str.Append('(');
            
            for (int i=0; i<this.values.Length; i++) {
                if (i > 0) {
                    str.Append(this.separator);
                }
                str.Append("@");
                str.Append(this.field);
                str.Append(i+1);
            }

            if (this.parenthesis)
                str.Append(')');
            
            return str.ToString();
            
            //("(" + field + " " + op + " @" + field + "1 " + comparison + " @" + field + "2)");
        }

        public IList<IDataParameter> CreateParameters(CreateParameterDelegate callback)
        {
            if (callback == null)
                throw new ArgumentNullException("CreateParameterDelegate");

            List<IDataParameter> list = new List<IDataParameter>();
            
            for (int i=0; i<this.values.Length; i++)
                list.Add(callback("@" + this.field + (i+1), this.values[i], dbType));

            return list;
        }
    }
}
