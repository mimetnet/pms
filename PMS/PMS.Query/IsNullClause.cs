using System.Collections.Generic;
using System.Data;
namespace PMS.Query
{
    public class IsNullClause : IClause
    {
        protected string field;
        protected string oper;

        public IsNullClause(string field) : this(field, "is")
        {
        }

        internal IsNullClause(string field, string op)
        {
            this.field = field;
            this.oper = op;
        }

        public string Name {
            get { return this.field; }
        }

        public bool IsCondition { 
            get { return true; } 
        }

        public override string ToString()
        {
            return (this.field + " " + this.oper + " null");
        }

        public virtual IList<IDataParameter> CreateParameters(CreateParameterDelegate callback)
        {
            return new List<IDataParameter>();
        }
    }
}
