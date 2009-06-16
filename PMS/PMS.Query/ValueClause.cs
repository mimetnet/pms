namespace PMS.Query
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;

    public abstract class ValueClause : IClause
    {
        protected string field;
        protected object value;
        protected PMS.DbType dbType;
        protected string oper;

        internal ValueClause(string field, object value, string op)
        {
            this.field = field;
            this.oper = op;
            this.value = value;
        }

        internal ValueClause(string field, object value, PMS.DbType dbType, string op)
        {
            this.field = field;
            this.oper = op;
            this.dbType = dbType;
            this.value = value;
        }

        public string Name {
            get { return this.field; }
        }

        public bool IsCondition { 
            get { return true; } 
        }

        public override string ToString()
        {
            return (this.field + " " + this.oper + " @" + this.field);
        }

        public virtual IList<IDataParameter> CreateParameters(CreateParameterDelegate callback)
        {
            if (callback == null)
                throw new ArgumentNullException("CreateParameterDelegate");

            List<IDataParameter> list = new List<IDataParameter>();
            list.Add(callback("@" + field, value, dbType));

            return list;
        }
    }
}
