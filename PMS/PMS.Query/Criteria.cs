using System;
using System.Collections.Generic;
using System.Text;

using PMS.Data;
using PMS.Metadata;

namespace PMS.Query
{
    [Serializable]
    public partial class Criteria
    {
        //private static readonly log4net.ILog log = 
		//	log4net.LogManager.GetLogger("PMS.Query.Criteria");
        protected List<IClause> clause = new List<IClause>();
        protected List<String> order = new List<String>();
		private int limit = -1;
        private uint offset = 0;
        private string columns = "*";

		protected IProvider provider = null;
		protected Class klass = null;

        public Criteria(Class klass, IProvider provider)
        {
            this.klass = klass;
            this.provider = provider;
        }

        public Criteria(Type type)
        {
			//cdesc = RepositoryManager.GetClass(type);
			//provider = RepositoryManager.CurrentConnection.Provider;
        }

        public string Columns {
            get { return this.columns; }
        }

        public Class Description {
            get { return this.klass; }
        }

        private string PrepareValue(string field, object value)
        {
            return (value is String)? (String)value : value.ToString();
            //if (cdesc != null) {
            //    foreach (Field fs in cdesc.Fields) {
            //        if (field == fs.Name || field == fs.Column) {
            //            if (fs.DbType.StartsWith("serial") == false)
            //                return provider.PrepareSqlValue(fs.DbType, value);
            //            else
            //                return provider.PrepareSqlValue("int", value);
            //        }
            //    }
            //}

            //if (log.IsDebugEnabled)
            //    log.Debug("PrepareValue " + field + " not found, defaulting to varchar dbType");

            //return provider.PrepareSqlValue("varchar", value);
        }

        #region Filter
        public Criteria Filter(string sClause)
        {
            return AndFilter(sClause);
        }

        public Criteria AndFilter(string sClause)
        {
            return And().Add(new RawClause(sClause));
        }

        public Criteria OrFilter(string sClause)
        {
            return Or().Add(new RawClause(sClause));
        }

        public Criteria NotFilter(string sClause)
        {
            return Not().AndFilter(sClause);
        }

        public Criteria AndNotFilter(string sClause)
        {
            return And().AndFilter(sClause);
        }

        public Criteria OrNotFilter(string sClause)
        {
            return Or().OrFilter(sClause);
        }
        #endregion
        
        #region GreaterOrEqual
        public Criteria GreaterOrEqual(string field, object value)
        {
            return AndGreaterOrEqual(field, value);
        }

        public Criteria AndGreaterOrEqual(string field, object value)
        {
            return And().Add(new GreaterOrEqualToClause(field, value));
        }

        public Criteria OrGreaterOrEqual(string field, object value)
        {
            return Or().Add(new GreaterOrEqualToClause(field, value));
        }
        #endregion

        #region LessOrEqual
        public Criteria LessOrEqual(string field, object value)
        {
            return AndLessOrEqual(field, value);
        }
        
        public Criteria AndLessOrEqual(string field, object value)
        {
            return And().Add(new LessOrEqualToClause(field, PrepareValue(field, value)));
        }

        public Criteria OrLessOrEqual(string field, object value)
        {
            return Or().Add(new LessOrEqualToClause(field, PrepareValue(field, value)));
        }
        #endregion

        #region GreaterThan
        public Criteria GreaterThan(string field, object value)
        {
            return AndGreaterThan(field, value);
        }
        
        public Criteria AndGreaterThan(string field, object value)
        {
            return And().Add(new GreaterThanClause(field, PrepareValue(field, value)));
        }

        public Criteria OrGreaterThan(string field, object value)
        {
            return Or().Add(new GreaterThanClause(field, PrepareValue(field, value)));
        }
        #endregion

        #region LessThan
        public Criteria LessThan(string field, object value)
        {
            return AndLessThan(field, value);
        } 
        
        public Criteria AndLessThan(string field, object value)
        {
            return And().Add(new LessThanClause(field, PrepareValue(field, value)));
        }

        public Criteria OrLessThan(string field, object value)
        {
            return Or().Add(new LessThanClause(field, PrepareValue(field, value)));
        }
        #endregion

        #region EqualTo
        public Criteria EqualTo(string field, object value)
        {
            return AndEqualTo(field, value);
        }
        
        public Criteria AndEqualTo(string field, object value)
        {
            return And().Add(new EqualToClause(field, PrepareValue(field, value)));
        }

        public Criteria OrEqualTo(string field, object value)
        {
            return Or().Add(new EqualToClause(field, PrepareValue(field, value)));
        }
        #endregion

        #region NotEqualTo
        public Criteria NotEqualTo(string field, object value)
        {
            return AndNotEqualTo(field, value);
        }
        
        public Criteria AndNotEqualTo(string field, object value)
        {
            return And().Add(new NotEqualToClause(field, value));
        }

        public Criteria OrNotEqualTo(string field, object value)
        {
            return Or().Add(new NotEqualToClause(field, value));
        }
        #endregion

        #region NotNull
        public Criteria IsNotNull(string field)
        {
            return AndIsNotNull(field);
        }
        
        public Criteria AndIsNotNull(string field)
        {
            return And().Add(new IsNotNullClause(field));
        }

        public Criteria OrIsNotNull(string field)
        {
            return Or().Add(new IsNotNullClause(field));
        }
        #endregion

        #region IsNull
        public Criteria IsNull(string field)
        {
            return AndIsNull(field);
        }

        public Criteria AndIsNull(string field)
        {
            return And().Add(new IsNullClause(field));
        }

        public Criteria OrIsNull(string field)
        {
            return Or().Add(new IsNullClause(field));
        }
        #endregion

        #region Like
        public Criteria Like(string field, object value)
        {
            return AndLike(field, value);
        }

        public Criteria AndLike(string field, object value)
        {
            return And().Add(new LikeClause(field, PrepareValue(field, value)));
        }

        public Criteria AndLike(string field, object value, string fieldFunction)
        {
            return And().Add(new LikeClause(fieldFunction + "(" + field + ")", PrepareValue(field, value)));
        }

        public Criteria OrLike(string field, object value)
        {
            return Or().Add(new LikeClause(field, PrepareValue(field, value)));
        }

        public Criteria OrLike(string field, object value, string fieldFunction)
        {
            return Or().Add(new LikeClause(fieldFunction + "(" + field + ")", PrepareValue(field, value)));
        } 
        #endregion

        #region NotLike
        public Criteria NotLike(string field, object value)
        {
            return AndNotLike(field, value);
        }

        public Criteria AndNotLike(string f, object v)
        {
            return And().Add(new NotLikeClause(f, PrepareValue(f, v)));
        }

        public Criteria OrNotLike(string f, object v)
        {
            return Or().Add(new NotLikeClause(f, PrepareValue(f, v)));
        } 
        #endregion

        #region Between
        public Criteria Between(string field, object val1, object val2)
        {
            return AndBetween(field, val1, val2);
        }

        public Criteria AndBetween(string field, object val1, object val2)
        {
            return And().Add(new BetweenClause(field,
                                         PrepareValue(field, val1),
                                         PrepareValue(field, val2)));
        }

        public Criteria OrBetween(string field, object val1, object val2)
        {
            return Or().Add(new BetweenClause(field,
                                         PrepareValue(field, val1),
                                         PrepareValue(field, val2)));
        } 
        #endregion

        #region NotBetween
        public Criteria NotBetween(string field, object val1, object val2)
        {
            return AndNotBetween(field, val1, val2);
        }

        public Criteria AndNotBetween(string field, object val1, object val2)
        {
            return And().Add(new NotBetweenClause(field,
                                            PrepareValue(field, val1),
                                            PrepareValue(field, val2)));
        }

        public Criteria OrNotBetween(string field, object val1, object val2)
        {
            return Or().Add(new NotBetweenClause(field,
                                            PrepareValue(field, val1),
                                            PrepareValue(field, val2)));
        } 
        #endregion

        #region Internal
        public Criteria And()
        {
            if (clause.Count != 0)
                clause.Add(new AndClause());
            return this;
        }

        public Criteria Or()
        {
            if (clause.Count != 0)
                clause.Add(new OrClause());
            return this;
        }

        public Criteria Not()
        {
            clause.Add(new RawClause(" NOT "));
            return this;
        }

        public Criteria Add(IClause item)
        {
            this.clause.Add(item);
            return this;
        }
        #endregion

        #region Order
        public Criteria OrderBy(string field)
        {
            if (order.Count > 0)
                order.Add(", ");
            order.Add(field);
            return this;
        }

        public Criteria OrderByAsc(string field)
        {
            if (order.Count > 0)
                order.Add(", ");
            order.Add(field + " ASC");
            return this;
        }

        public Criteria OrderByDesc(string field)
        {
            if (order.Count > 0)
                order.Add(", ");
            order.Add(field + " DESC");
            return this;
        } 
        #endregion

        #region Grouping
        public Criteria StartGroup()
        {
            this.clause.Add(new RawClause("("));
            return this;
        }

        public Criteria StopGroup()
        {
            this.clause.Add(new RawClause(")"));
            return this;
        }
        #endregion

        public Criteria SetColumns(string columns) 
        {
            this.columns = String.IsNullOrEmpty(columns)?
                "*" : columns;
            return this;
        }

        #region Append
        public void AppendOrderBy(StringBuilder sql)
        {
            if (this.order.Count == 0)
                return;

            sql.Append(" WHERE ");
            foreach (String str in this.order)
                sql.Append(str.ToString());
        }

        public void AppendCondition(StringBuilder sql)
        {
            if (this.clause.Count == 0)
                return;

            sql.Append(" WHERE ");
            foreach (IClause c in this.clause)
                sql.Append(c.ToString());
        }
        #endregion

        #region Properties
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
        #endregion
    }
}
