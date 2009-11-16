namespace PMS.Query
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;

    public abstract class ValueClause : IClause
    {
        protected string field;
        protected string sqlFieldFunc;
        protected string sqlValueFunc;
        protected object value;
        protected PMS.DbType dbType;
        protected string oper;

        internal ValueClause(string field, object value, string op) :
            this(field, value, null, op)
        {
        }

        internal ValueClause(string sqlFieldFunc, string field, object value, string op) :
            this(sqlFieldFunc, field, null, value, null, op)
        {
        }

        internal ValueClause(string sqlFieldFunc, string field, string sqlValueFunc, object value, string op) :
            this(sqlFieldFunc, field, sqlValueFunc, value, null, op)
        {
        }

        internal ValueClause(string field, object value, PMS.DbType dbType, string op) :
            this(null, field, null, value, dbType, op)
        {
        }

        internal ValueClause(string sqlFieldFunc, string field, object value, PMS.DbType dbType, string op) :
            this(sqlFieldFunc, field, null, value, dbType, op)
        {
        }

        internal ValueClause(string sqlFieldFunc, string field, string sqlValueFunc, object value, PMS.DbType dbType, string op)
        {
            this.field = field;
            this.oper = op;
            this.dbType = dbType;
            this.value = value;
            
            this.sqlFieldFunc = sqlFieldFunc;
            this.sqlValueFunc = sqlValueFunc;
        }

        public string Name {
            get { return this.field; }
        }

        public bool IsCondition { 
            get { return true; } 
        }

        public override string ToString()
        {
            StringBuilder ret = new StringBuilder(30);

            if (null == this.sqlFieldFunc) {
                ret.Append(this.field);
            } else {
                ret.Append(this.sqlFieldFunc);
                ret.Append('(');
                ret.Append(this.field);
                ret.Append(')');
            }

            ret.Append(' ');
            ret.Append(this.oper);
            ret.Append(' ');

            if (null == this.sqlValueFunc) {
                ret.Append('@');
                ret.Append(this.field);
            } else {
                ret.Append(this.sqlValueFunc);
                ret.Append("(@");
                ret.Append(this.field);
                ret.Append(')');
            }

            return ret.ToString();
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
