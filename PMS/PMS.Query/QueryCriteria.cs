﻿using System;
using System.Collections.Generic;
using System.Text;

using PMS.Data;
using PMS.Metadata;

namespace PMS.Query
{
    public partial class Query <Table>
    {
        protected List<IClause> criteria = new List<IClause>();
        protected List<IClause> values = new List<IClause>();
        protected List<IClause> unique = new List<IClause>();
        protected List<IClause> pkey = new List<IClause>();
        protected List<String> order = new List<String>();
        protected uint limit = 0;
        protected uint offset = 0;
        protected string columns = "*";
        protected string procedure = null;
        protected System.Data.CommandType commandType = System.Data.CommandType.Text;

        #region Filter
        public Query<Table> Filter(string sClause)
        {
            return AndFilter(sClause);
        }

        public Query<Table> AndFilter(string sClause)
        {
            return And().Add(new RawClause(sClause, true));
        }

        public Query<Table> OrFilter(string sClause)
        {
            return Or().Add(new RawClause(sClause, true));
        }

        public Query<Table> NotFilter(string sClause)
        {
            return AndNotFilter(sClause);
        }

        public Query<Table> AndNotFilter(string sClause)
        {
            return And().Not().Add(new RawClause(sClause, true));
        }

        public Query<Table> OrNotFilter(string sClause)
        {
            return Or().Not().Add(new RawClause(sClause, true));
        }
        #endregion

        #region GreaterOrEqual
        public Query<Table> GreaterOrEqual(string field, object value)
        {
            return AndGreaterOrEqual(field, value);
        }

        public Query<Table> AndGreaterOrEqual(string field, object value)
        {
            return And().Add(new GreaterOrEqualToClause(GetColumn(field), value));
        }

        public Query<Table> OrGreaterOrEqual(string field, object value)
        {
            return Or().Add(new GreaterOrEqualToClause(GetColumn(field), value));
        }
        #endregion

        #region LessOrEqual
        public Query<Table> LessOrEqual(string field, object value)
        {
            return AndLessOrEqual(field, value);
        }

        public Query<Table> AndLessOrEqual(string field, object value)
        {
            return And().Add(new LessOrEqualToClause(GetColumn(field), value));
        }

        public Query<Table> OrLessOrEqual(string field, object value)
        {
            return Or().Add(new LessOrEqualToClause(GetColumn(field), value));
        }
        #endregion

        #region GreaterThan
        public Query<Table> GreaterThan(string field, object value)
        {
            return AndGreaterThan(field, value);
        }

        public Query<Table> AndGreaterThan(string field, object value)
        {
            return And().Add(new GreaterThanClause(GetColumn(field), value));
        }

        public Query<Table> OrGreaterThan(string field, object value)
        {
            return Or().Add(new GreaterThanClause(GetColumn(field), value));
        }
        #endregion

        #region LessThan
        public Query<Table> LessThan(string field, object value)
        {
            return AndLessThan(field, value);
        }

        public Query<Table> AndLessThan(string field, object value)
        {
            return And().Add(new LessThanClause(GetColumn(field), value));
        }

        public Query<Table> OrLessThan(string field, object value)
        {
            return Or().Add(new LessThanClause(GetColumn(field), value));
        }
        #endregion

        #region EqualTo
        public Query<Table> EqualTo(string field, object value)
        {
            return AndEqualTo(field, value);
        }

        public Query<Table> AndEqualTo(string field, object value)
        {
            return And().Add(new EqualToClause(GetColumn(field), value));
        }

        public Query<Table> OrEqualTo(string field, object value)
        {
            return Or().Add(new EqualToClause(GetColumn(field), value));
        }
        #endregion

        #region NotEqualTo
        public Query<Table> NotEqualTo(string field, object value)
        {
            return AndNotEqualTo(field, value);
        }

        public Query<Table> AndNotEqualTo(string field, object value)
        {
            return And().Add(new NotEqualToClause(GetColumn(field), value));
        }

        public Query<Table> OrNotEqualTo(string field, object value)
        {
            return Or().Add(new NotEqualToClause(GetColumn(field), value));
        }
        #endregion

        #region NotNull
        public Query<Table> IsNotNull(string field)
        {
            return AndIsNotNull(field);
        }

        public Query<Table> AndIsNotNull(string field)
        {
            return And().Add(new IsNotNullClause(GetColumn(field)));
        }

        public Query<Table> OrIsNotNull(string field)
        {
            return Or().Add(new IsNotNullClause(GetColumn(field)));
        }
        #endregion

        #region IsNull
        public Query<Table> IsNull(string field)
        {
            return AndIsNull(field);
        }

        public Query<Table> AndIsNull(string field)
        {
            return And().Add(new IsNullClause(GetColumn(field)));
        }

        public Query<Table> OrIsNull(string field)
        {
            return Or().Add(new IsNullClause(GetColumn(field)));
        }
        #endregion

        #region Like
        public Query<Table> Like(string field, object value)
        {
            return AndLike(field, value);
        }

        public Query<Table> Like(string field, object value, string sqlFieldFunction)
        {
            return AndLike(field, value, sqlFieldFunction);
        }

        public Query<Table> AndLike(string field, object value)
        {
            return And().Add(new LikeClause(GetColumn(field), value));
        }

        public Query<Table> AndLike(string field, object value, string sqlFieldFunction)
        {
            return And().Add(new LikeClause(sqlFieldFunction, GetColumn(field) , value));
        }

        public Query<Table> OrLike(string field, object value)
        {
            return Or().Add(new LikeClause(GetColumn(field), value));
        }

        public Query<Table> OrLike(string field, object value, string sqlFieldFunction)
        {
            return Or().Add(new LikeClause(sqlFieldFunction, GetColumn(field), value));
        }
        #endregion

        #region ILike
        public Query<Table> ILike(string field, object value)
        {
            return AndILike(field, value);
        }

        public Query<Table> AndILike(string field, object value)
        {
            return And().Add(new LikeClause("LOWER", GetColumn(field), "LOWER", value));
        }

        public Query<Table> OrILike(string field, object value)
        {
            return Or().Add(new LikeClause("LOWER", GetColumn(field), "LOWER", value));
        }
        #endregion

        #region NotLike
        public Query<Table> NotLike(string field, object value)
        {
            return AndNotLike(field, value);
        }

        public Query<Table> AndNotLike(string field, object v)
        {
            return And().Add(new NotLikeClause(GetColumn(field), v));
        }

        public Query<Table> OrNotLike(string field, object v)
        {
            return Or().Add(new NotLikeClause(GetColumn(field), v));
        }
        #endregion

        #region In (...)
        public Query<Table> In(string field, params object[] list)
        {
            return this.AndIn(field, list);
        }

        public Query<Table> AndIn(string field, params object[] list)
        {
            return And().Add(new InClause(GetColumn(field), list));
        }

        public Query<Table> OrIn(string field, params object[] list)
        {
            return Or().Add(new InClause(GetColumn(field), list));
        }
        #endregion

        #region NotIn (...)
        public Query<Table> NotIn(string field, params object[] list)
        {
            return this.AndNotIn(field, list);
        }

        public Query<Table> AndNotIn(string field, params object[] list)
        {
            return And().Add(new NotInClause(GetColumn(field), list));
        }

        public Query<Table> OrNotIn(string field, params object[] list)
        {
            return Or().Add(new NotInClause(GetColumn(field), list));
        }
        #endregion

        #region Between
        public Query<Table> Between(string field, object val1, object val2)
        {
            return AndBetween(field, val1, val2);
        }

        public Query<Table> AndBetween(string field, object val1, object val2)
        {
            return And().Add(new BetweenClause(GetColumn(field), val1, val2));
        }

        public Query<Table> OrBetween(string field, object val1, object val2)
        {
            return Or().Add(new BetweenClause(GetColumn(field), val1, val2));
        }
        #endregion

        #region NotBetween
        public Query<Table> NotBetween(string field, object val1, object val2)
        {
            return AndNotBetween(field, val1, val2);
        }

        public Query<Table> AndNotBetween(string field, object val1, object val2)
        {
            return And().Add(new NotBetweenClause(GetColumn(field), val1, val2));
        }

        public Query<Table> OrNotBetween(string field, object val1, object val2)
        {
            return Or().Add(new NotBetweenClause(GetColumn(field), val1, val2));
        }
        #endregion

        #region Internal
        public Query<Table> And()
        {
            if (criteria.Count != 0 && criteria[criteria.Count - 1].IsCondition)
                criteria.Add(new AndClause());
            return this;
        }

        public Query<Table> Or()
        {
            if (criteria.Count != 0 && criteria[criteria.Count - 1].IsCondition)
                criteria.Add(new OrClause());
            return this;
        }

        public Query<Table> Not()
        {
            criteria.Add(new RawClause(" NOT "));
            return this;
        }

        public Query<Table> Add(IClause item)
        {
            this.criteria.Add(item);
            return this;
        }

        protected string GetColumn(string value)
        {
            Field f = this.cdesc.Fields[value];

            return (null != f)? f.Column : value;
        }
        #endregion

        #region Order
        public Query<Table> OrderBy(string field)
        {
            if (order.Count > 0)
                order.Add(", ");
            order.Add(GetColumn(field));
            return this;
        }

        public Query<Table> OrderByAsc(string field)
        {
            if (order.Count > 0)
                order.Add(", ");
            order.Add(GetColumn(field) + " ASC");
            return this;
        }

        public Query<Table> OrderByDesc(string field)
        {
            if (order.Count > 0)
                order.Add(", ");
            order.Add(GetColumn(field) + " DESC");
            return this;
        }
        #endregion

        #region Grouping
        public Query<Table> StartGroup()
        {
            this.criteria.Add(new RawClause("("));
            return this;
        }

        public Query<Table> StopGroup()
        {
            this.criteria.Add(new RawClause(")"));
            return this;
        }

        public delegate void QueryGroupDelegate(Query<Table> item);

        public Query<Table> Group(QueryGroupDelegate callback)
        {
            this.StartGroup();
            callback(this);
            this.StopGroup();

            return this;
        }
        #endregion

        #region UPDATE foo SET
        public Query<Table> Set(string field, object value)
        {
            if (String.IsNullOrEmpty(field))
                throw new ArgumentNullException("field");

            IClause c = null;
            Field f = this.cdesc.Fields[field];

            if (null != f) {
                c = new EqualToClause(f.Column, value, this.provider.GetDbType(f.DbType));
            } else {
                c = new EqualToClause(field, value);
            }

            if (null != f && f.PrimaryKey) {
                this.pkey.Add(c);
            } else if (null != f && f.Unique) {
                this.unique.Add(c);
            } else {
                this.values.Add(c);
            }

            return this;
        }

        public Query<Table> Set(Table record)
        {
			if (null == record) return this;

            foreach (Field field in this.cdesc.Fields) {
                Object fvalue = field.GetValue(record);

			    //if ((fvalue == null && field.Default == null) || (fvalue != null && field.Default != null && field.Default.ToString() == fvalue.ToString()))
				//    fvalue = field.DefaultDb;

                if (!IsFieldSet(field, fvalue))
					continue;

				IClause c = new EqualToClause(field.Column, fvalue, this.provider.GetDbType(field.DbType));

				if (field.PrimaryKey) {
					this.pkey.Add(c);
				} else if (field.Unique) {
					this.unique.Add(c);
				} else {
					this.values.Add(c);
                }
            }

            return this;
        }
        #endregion

        protected bool IsFieldSet(Field field, object value)
		{
			object init = null;

			if (verbose) {
				log.InfoFormat("   Column: '{0}'", field.Column);
				if (value != null)
					log.InfoFormat("    Value: '{0}'", value);
				else
					log.Info("    Value: NULL");

				log.Info("   DbType: " + field.DbType);
				log.InfoFormat("  Default: '{0}' | {1}", field.Default, ((field.Default != null)? field.Default.GetType().ToString() : ""));
				log.Info("   Ignore: " + field.IgnoreDefault);
			}

			if (value != null) {
				//init = (field.Default == null)?
				//	provider.GetTypeInit(field.DbType, value.GetType()) : provider.ConvertToType(field.DbType, field.Default);
                init = field.Default;

				//init = (field.Default == null)?
				//	Activator.CreateInstance(value.GetType()) : provider.ConvertToType(field.DbType, field.Default);

				if (verbose) {
					log.InfoFormat("     Init: '{0}'", init);
					log.InfoFormat("      << : " + !(init != null && field.IgnoreDefault && init.Equals(value)));
					log.InfoFormat("--");
				}

				return !(init != null && field.IgnoreDefault && init.Equals(value));
			}
			
			if (verbose) {
				log.InfoFormat("      << : " + (field.IgnoreDefault && field.Default == null));
				log.InfoFormat("--");
			}

			// VALUE IS NULL
			return !(field.IgnoreDefault && field.Default == null);
		}

        public Query<Table> Columns(string columns)
        {
            this.columns = String.IsNullOrEmpty(columns)?
                "*" : columns;
            return this;
        }

        public Query<Table> Limit(uint limit)
        {
            this.limit = limit;
            return this;
        }

        public Query<Table> Offset(uint offset)
        {
            this.offset = offset;
            return this;
        }

        public Query<Table> Procedure(string name)
        {
            if (String.Empty.Equals(name))
                throw new ArgumentException("Procedure cannot be set to an empty string");

            this.procedure = name;
            this.commandType = System.Data.CommandType.StoredProcedure;
            return this;
        }

        public Query<Table> AddParameters(params object[] args)
        {
            if (null == args || 0 == args.Length)
                return this;

            if (0 != (args.Length % 2))
                throw new ArgumentException("args must be even pairs of name=value");

            String name = null;

            for (int i=0; i<args.Length; i++) {
                if (1 == (i % 2)) {
                    name = (String) args[i-1];
                    if (!String.IsNullOrEmpty(name)) {
                        this.Set(name, args[i]);
                    }
                }
            }

            return this;
        }

        public Query<Table> Clear()
        {
            this.parameters.Clear();
            this.criteria.Clear();
            this.values.Clear();
            this.unique.Clear();
            this.pkey.Clear();
            this.order.Clear();
            return this;
        }
    }
}
