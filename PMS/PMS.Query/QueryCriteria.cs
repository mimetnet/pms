using System;
using System.Collections.Generic;
using System.Text;

using PMS.Data;
using PMS.Metadata;

namespace PMS.Query
{
    public partial class Query <Table>
    {
        protected List<IClause> clause = new List<IClause>();
        protected List<String> order = new List<String>();
		protected uint limit = 0;
        protected uint offset = 0;
        protected string columns = "*";

        //public string Columns {
        //    get { return this.columns; }
        //}

        //public Class Description {
        //    get { return this.klass; }
        //}

        #region Filter
        public Query<Table> Filter(string sClause)
        {
            return AndFilter(sClause);
        }

        public Query<Table> AndFilter(string sClause)
        {
            return And().Add(new RawClause(sClause));
        }

        public Query<Table> OrFilter(string sClause)
        {
            return Or().Add(new RawClause(sClause));
        }

        public Query<Table> NotFilter(string sClause)
        {
            return Not().AndFilter(sClause);
        }

        public Query<Table> AndNotFilter(string sClause)
        {
            return And().AndFilter(sClause);
        }

        public Query<Table> OrNotFilter(string sClause)
        {
            return Or().OrFilter(sClause);
        }
        #endregion
        
        #region GreaterOrEqual
        public Query<Table> GreaterOrEqual(string field, object value)
        {
            return AndGreaterOrEqual(field, value);
        }

        public Query<Table> AndGreaterOrEqual(string field, object value)
        {
            return And().Add(new GreaterOrEqualToClause(field, value));
        }

        public Query<Table> OrGreaterOrEqual(string field, object value)
        {
            return Or().Add(new GreaterOrEqualToClause(field, value));
        }
        #endregion

        #region LessOrEqual
        public Query<Table> LessOrEqual(string field, object value)
        {
            return AndLessOrEqual(field, value);
        }
        
        public Query<Table> AndLessOrEqual(string field, object value)
        {
            return And().Add(new LessOrEqualToClause(field, value));
        }

        public Query<Table> OrLessOrEqual(string field, object value)
        {
            return Or().Add(new LessOrEqualToClause(field, value));
        }
        #endregion

        #region GreaterThan
        public Query<Table> GreaterThan(string field, object value)
        {
            return AndGreaterThan(field, value);
        }
        
        public Query<Table> AndGreaterThan(string field, object value)
        {
            return And().Add(new GreaterThanClause(field, value));
        }

        public Query<Table> OrGreaterThan(string field, object value)
        {
            return Or().Add(new GreaterThanClause(field, value));
        }
        #endregion

        #region LessThan
        public Query<Table> LessThan(string field, object value)
        {
            return AndLessThan(field, value);
        } 
        
        public Query<Table> AndLessThan(string field, object value)
        {
            return And().Add(new LessThanClause(field, value));
        }

        public Query<Table> OrLessThan(string field, object value)
        {
            return Or().Add(new LessThanClause(field, value));
        }
        #endregion

        #region EqualTo
        public Query<Table> EqualTo(string field, object value)
        {
            return AndEqualTo(field, value);
        }
        
        public Query<Table> AndEqualTo(string field, object value)
        {
            return And().Add(new EqualToClause(field, value));
        }

        public Query<Table> OrEqualTo(string field, object value)
        {
            return Or().Add(new EqualToClause(field, value));
        }
        #endregion

        #region NotEqualTo
        public Query<Table> NotEqualTo(string field, object value)
        {
            return AndNotEqualTo(field, value);
        }
        
        public Query<Table> AndNotEqualTo(string field, object value)
        {
            return And().Add(new NotEqualToClause(field, value));
        }

        public Query<Table> OrNotEqualTo(string field, object value)
        {
            return Or().Add(new NotEqualToClause(field, value));
        }
        #endregion

        #region NotNull
        public Query<Table> IsNotNull(string field)
        {
            return AndIsNotNull(field);
        }
        
        public Query<Table> AndIsNotNull(string field)
        {
            return And().Add(new IsNotNullClause(field));
        }

        public Query<Table> OrIsNotNull(string field)
        {
            return Or().Add(new IsNotNullClause(field));
        }
        #endregion

        #region IsNull
        public Query<Table> IsNull(string field)
        {
            return AndIsNull(field);
        }

        public Query<Table> AndIsNull(string field)
        {
            return And().Add(new IsNullClause(field));
        }

        public Query<Table> OrIsNull(string field)
        {
            return Or().Add(new IsNullClause(field));
        }
        #endregion

        #region Like
        public Query<Table> Like(string field, object value)
        {
            return AndLike(field, value);
        }

        public Query<Table> AndLike(string field, object value)
        {
            return And().Add(new LikeClause(field, value));
        }

        public Query<Table> AndLike(string field, object value, string fieldFunction)
        {
            return And().Add(new LikeClause(fieldFunction + "(" + field + ")", value));
        }

        public Query<Table> OrLike(string field, object value)
        {
            return Or().Add(new LikeClause(field, value));
        }

        public Query<Table> OrLike(string field, object value, string fieldFunction)
        {
            return Or().Add(new LikeClause(fieldFunction + "(" + field + ")", value));
        } 
        #endregion

        #region NotLike
        public Query<Table> NotLike(string field, object value)
        {
            return AndNotLike(field, value);
        }

        public Query<Table> AndNotLike(string f, object v)
        {
            return And().Add(new NotLikeClause(f, v));
        }

        public Query<Table> OrNotLike(string f, object v)
        {
            return Or().Add(new NotLikeClause(f, v));
        } 
        #endregion

        #region Between
        public Query<Table> Between(string field, object val1, object val2)
        {
            return AndBetween(field, val1, val2);
        }

        public Query<Table> AndBetween(string field, object val1, object val2)
        {
            return And().Add(new BetweenClause(field, val1, val2));
        }

        public Query<Table> OrBetween(string field, object val1, object val2)
        {
            return Or().Add(new BetweenClause(field, val1, val2));
        } 
        #endregion

        #region NotBetween
        public Query<Table> NotBetween(string field, object val1, object val2)
        {
            return AndNotBetween(field, val1, val2);
        }

        public Query<Table> AndNotBetween(string field, object val1, object val2)
        {
            return And().Add(new NotBetweenClause(field, val1, val2));
        }

        public Query<Table> OrNotBetween(string field, object val1, object val2)
        {
            return Or().Add(new NotBetweenClause(field, val1, val2));
        } 
        #endregion

        #region Internal
        public Query<Table> And()
        {
            if (clause.Count != 0)
                clause.Add(new AndClause());
            return this;
        }

        public Query<Table> Or()
        {
            if (clause.Count != 0)
                clause.Add(new OrClause());
            return this;
        }

        public Query<Table> Not()
        {
            clause.Add(new RawClause(" NOT "));
            return this;
        }

        public Query<Table> Add(IClause item)
        {
            this.clause.Add(item);
            return this;
        }
        #endregion

        #region Order
        public Query<Table> OrderBy(string field)
        {
            if (order.Count > 0)
                order.Add(", ");
            order.Add(field);
            return this;
        }

        public Query<Table> OrderByAsc(string field)
        {
            if (order.Count > 0)
                order.Add(", ");
            order.Add(field + " ASC");
            return this;
        }

        public Query<Table> OrderByDesc(string field)
        {
            if (order.Count > 0)
                order.Add(", ");
            order.Add(field + " DESC");
            return this;
        } 
        #endregion

        #region Grouping
        public Query<Table> StartGroup()
        {
            this.clause.Add(new RawClause("("));
            return this;
        }

        public Query<Table> StopGroup()
        {
            this.clause.Add(new RawClause(")"));
            return this;
        }
        #endregion

        public Query<Table> SetColumns(string columns) 
        {
            this.columns = String.IsNullOrEmpty(columns)?
                "*" : columns;
            return this;
        }

        public Query<Table> SetLimit(uint limit)
        {
            this.limit = limit;
            return this;
        }
        
        public Query<Table> SetOffset(uint offset)
        {
            this.offset = offset;
            return this;
        }
    }
}
