using System;
using System.Collections;
using System.Text;

using PMS.Data;
using PMS.Metadata;

namespace PMS.Query
{
    public class Criteria
    {
        private ArrayList clause = null;
        private ArrayList order = null;
        private string[] columns = null;
        private string[] keys = null;
        private int limit = -1;
        private uint offset = 0;

        internal MetaObject metaObject = null;

        public Criteria()
        {
            clause     = new ArrayList();
            order      = new ArrayList();
            metaObject = new MetaObject();
        }

        public Criteria(Type type)
        {
            clause     = new ArrayList();
            order      = new ArrayList();
            metaObject = new MetaObject(type);
            columns    = metaObject.Columns;
            keys       = metaObject.PrimaryKeys;
        }

        private object PrepareValue(string field, object value)
        {
            for (int i = 0; i < columns.Length; i++) {
                if (field == columns[i]) {
                    return metaObject.Provider.PrepareSqlValue(metaObject.GetColumnType(columns[i]), value);
                }
            }

            for (int i = 0; i < keys.Length; i++) {
                if (field == keys[i]) {
                    return metaObject.Provider.PrepareSqlValue(metaObject.GetColumnType(keys[i]), value);
                }
            }

            return metaObject.Provider.PrepareSqlValue("varchar", value);
        }

        public void AndGreaterOrEqual(string field, object value)
        {
            if (clause.Count != 0)
                this.AndClause();
            clause.Add(new GreaterOrEqualToClause(field, 
                                                   PrepareValue(field,value)));
        }

        public void OrGreaterOrEqual(string field, object value)
        {
            if (clause.Count != 0)
                this.OrClause();
            clause.Add(new GreaterOrEqualToClause(field, 
                                                   PrepareValue(field,value)));
        }

        public void GreaterOrEqual(string field, object value)
        {
            if (clause.Count != 0)
                this.AndClause();
            clause.Add(new GreaterOrEqualToClause(field, 
                                                  PrepareValue(field,value)));
        }

        public void AndLessOrEqual(string field, object value)
        {
            if (clause.Count != 0)
                this.AndClause();
            this.LessOrEqual(field, value);
        }

        public void OrLessOrEqual(string field, object value)
        {
            if (clause.Count != 0)
                this.OrClause();
            this.LessOrEqual(field, value);
        }

        public void LessOrEqual(string field, object value)
        {
            clause.Add(new LessOrEqualToClause(field, PrepareValue(field, value)));
        }

        public void AndGreaterThan(string field, object value)
        {
            if (clause.Count != 0)
                this.AndClause();
            this.GreaterThan(field, value);                
        }

        public void OrGreaterThan(string field, object value)
        {
            if (clause.Count != 0)
                this.OrClause();
            this.GreaterThan(field, value);                
        }

        public void GreaterThan(string field, object value)
        {
            clause.Add(new GreaterThanClause(field, PrepareValue(field, value)));
        }

        public void AndLessThan(string field, object value)
        {
            if (clause.Count != 0)
                this.AndClause();
            this.LessThan(field, value);
        }

        public void OrLessThan(string field, object value)
        {
            if (clause.Count != 0)
                this.OrClause();
            this.LessThan(field, value);
        }

        public void LessThan(string field, object value)
        {
            clause.Add(new LessThanClause(field, PrepareValue(field, value)));
        }

        public void AndEqualTo(string field, object value)
        {
            EqualTo(field, value);
        }

        public void OrEqualTo(string field, object value)
        {
            if(clause.Count != 0)
                this.OrClause();
            clause.Add(new EqualToClause(field, PrepareValue(field, value)));
        }

        public void EqualTo(string field, object value)
        {
            if (clause.Count != 0)
                this.AndClause();
            clause.Add(new EqualToClause(field, PrepareValue(field, value)));
        }

        public void AndNotEqualTo(string field, object value)
        {
            if (clause.Count != 0)
                this.AndClause();
            this.NotEqualTo(field, value);
        }

        public void OrNotEqualTo(string field, object value)
        {
            if (clause.Count != 0)
                this.OrClause();
            this.NotEqualTo(field, value);
        }

        public void NotEqualTo(string field, object value)
        {
            if (clause.Count != 0)
                this.AndClause();
            clause.Add(new NotEqualToClause(field, PrepareValue(field,value)));
        }

        public void OrIsNotNull(string field)
        {
            if(clause.Count != 0)
                this.OrClause();
            this.IsNotNull(field);
        }

        public void AndIsNotNull(string field)
        {
            if(clause.Count != 0)
                this.AndClause();
            this.IsNotNull(field);
        }

        public void IsNotNull(string field)
        {
            clause.Add(new IsNotNullClause(field));
        }

        public void OrIsNull(string field)
        {
            if(clause.Count != 0)
                this.OrClause();
            this.IsNull(field);
        }

        public void AndIsNull(string field)
        {
            IsNull(field);
        }

        public void IsNull(string field)
        {
            if (clause.Count != 0)
                this.AndClause();
            clause.Add(new IsNullClause(field));
        }

        public void Like(string f, object v)
        {
            if (clause.Count != 0)
                this.AndClause();
            clause.Add(new LikeClause(f, PrepareValue(f, v)));
        }

        public void Like(string f, object v, string method)
        {
            if (clause.Count != 0)
                this.AndClause();
            clause.Add(new LikeClause(method + "(" + f + ")",
                                      PrepareValue(f, v)));
        }

        public void AndLike(string f, object v)
        {
            if (clause.Count != 0)
                this.AndClause();
            clause.Add(new LikeClause(f, PrepareValue(f, v)));
        }

        public void OrLike(string f, object v)
        {
            if (clause.Count != 0)
                this.OrClause();
            clause.Add(new LikeClause(f, PrepareValue(f, v)));
        }

        public void OrLike(string f, object v, string method)
        {
            if (clause.Count != 0)
                this.OrClause();

            clause.Add(new LikeClause(method + "(" + f + ")",
                                      PrepareValue(f, v)));
        }

        public void NotLike(string f, object v)
        {
            if (clause.Count != 0)
                this.AndClause();
            clause.Add(new NotLikeClause(f, PrepareValue(f, v)));
        }

        public void AndNotLike(string f, object v)
        {
            if (clause.Count != 0)
                this.AndClause();
            clause.Add(new NotLikeClause(f, PrepareValue(f, v)));
        }

        public void OrNotLike(string f, object v)
        {
            if (clause.Count != 0)
                this.OrClause();
            clause.Add(new NotLikeClause(f, PrepareValue(f, v)));
        }

        internal void AndClause()
        {
            clause.Add(new AndClause());
        }

        internal void OrClause()
        {
            clause.Add(new OrClause());
        }

        public void OrderBy(string field)
        {
            OrderByAsc(field);
        }

        public void OrderByAsc(string field)
        {
            if (order.Count > 0)
                order.Add(", ");
            order.Add(field + " asc");
        }

        public void OrderByDesc(string field)
        {
            if (order.Count > 0)
                order.Add(", ");
            order.Add(field + " desc");
        }

        public string GetWhereClause()
        {
            StringBuilder buildClause = new StringBuilder(" WHERE ");
            for(int i = 0; i < clause.Count; i++)
                buildClause.Append(clause[i].ToString());

            return buildClause.ToString();
        }

        public string GetOrderByClause()
        {
            StringBuilder buildClause = new StringBuilder(" ORDER BY ");
            for(int i = 0; i < order.Count; i++)
                buildClause.Append(order[i].ToString());

            return buildClause.ToString();
        }

        public int ClauseCount {
            get { return clause.Count; }
        }

        public int OrderCount {
            get { return order.Count; }
        }

        public int Limit {
            get { return limit; }
            set { limit = value; }
        }

        public uint Offset {
            get { return offset; }
            set { offset = value; }
        }
    }
}
