namespace PMS.Query
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;

    public abstract class ValueClause : IClause
    {
        private string field;
        private object value;
        private string op;

        internal ValueClause(string field, object value, string op)
        {
            this.field = field;
            this.op = op;
            this.value = value;
        }

        public bool IsCondition { 
            get { return true; } 
        }

        public override string ToString()
        {
            return (this.field + " " + this.op + " @" + this.field);
        }

        public IList<IDataParameter> CreateParameters(CreateParameterDelegate callback)
        {
            if (callback == null)
                throw new ArgumentNullException("CreateParameterDelegate");

            List<IDataParameter> list = new List<IDataParameter>();
            list.Add(callback("@" + field, value));

            return list;
        }
    }
}
