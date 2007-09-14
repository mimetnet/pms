using System;
using System.Collections;
using System.Text;

using PMS.Data;
using PMS.Metadata;

namespace PMS.Query
{
    [Serializable]
    public sealed class Criteria
    {
        private static readonly log4net.ILog log = 
			log4net.LogManager.GetLogger("PMS.Query.Criteria");
        private ArrayList clause = null;
        private ArrayList order = null;
		private int limit = -1;
        private uint offset = 0;

		internal IProvider provider = null;
		internal Class cdesc = null;
		
        public Criteria(Type type)
        {
            clause = new ArrayList();
            order = new ArrayList();
			cdesc = RepositoryManager.GetClass(type);
			provider = RepositoryManager.CurrentConnection.Provider;
        }

        private string PrepareValue(string field, object value)
        {
			if (cdesc != null) {
				foreach (Field fs in cdesc.Fields) {
					if (field == fs.Name || field == fs.Column) {
						if (fs.DbType.StartsWith("serial") == false)
							return provider.PrepareSqlValue(fs.DbType, value);
						else
							return provider.PrepareSqlValue("int", value);
					}
				}
			}

            if (log.IsDebugEnabled)
                log.Debug("PrepareValue " + field + " not found, defaulting to varchar dbType");

            return provider.PrepareSqlValue("varchar", value);
        }

        #region GreaterOrEqual
        /// <summary>
        /// AND + GreaterOrEqualToClause()
        /// </summary>
        /// <param name="field">database field</param>
        /// <param name="value">value of field</param>
        public void GreaterOrEqual(string field, object value)
        {
            AndGreaterOrEqual(field, value);
        }

        /// <summary>
        /// AND + GreaterOrEqualToClause()
        /// </summary>
        /// <param name="field">database field</param>
        /// <param name="value">value of field</param>
        public void AndGreaterOrEqual(string field, object value)
        {
            AndClause();
            clause.Add(new GreaterOrEqualToClause(field, PrepareValue(field, value)));
        }

        /// <summary>
        /// Or + GreaterOrEqualToClause()
        /// </summary>
        /// <param name="field">database field</param>
        /// <param name="value">value of field</param>
        public void OrGreaterOrEqual(string field, object value)
        {
            OrClause();
            clause.Add(new GreaterOrEqualToClause(field, PrepareValue(field, value)));
        }
        #endregion

        #region LessOrEqual
        /// <summary>
        /// AND + LessOrEqualToClause()
        /// </summary>
        /// <param name="field">database field</param>
        /// <param name="value">value of field</param>
        public void LessOrEqual(string field, object value)
        {
            AndLessOrEqual(field, value);
        }

        /// <summary>
        /// AND + LessOrEqualToClause()
        /// </summary>
        /// <param name="field">database field</param>
        /// <param name="value">value of field</param>
        public void AndLessOrEqual(string field, object value)
        {
            AndClause();
            clause.Add(new LessOrEqualToClause(field, PrepareValue(field, value)));
        }

        /// <summary>
        /// Or + LessOrEqualToClause()
        /// </summary>
        /// <param name="field">database field</param>
        /// <param name="value">value of field</param>
        public void OrLessOrEqual(string field, object value)
        {
            OrClause();
            clause.Add(new LessOrEqualToClause(field, PrepareValue(field, value)));
        }
        #endregion

        #region GreaterThan
        public void GreaterThan(string field, object value)
        {
            AndGreaterThan(field, value);
        }

        public void AndGreaterThan(string field, object value)
        {
            AndClause();
            clause.Add(new GreaterThanClause(field, PrepareValue(field, value)));
        }

        public void OrGreaterThan(string field, object value)
        {
            OrClause();
            GreaterThan(field, value);
        }
        #endregion

        #region LessThan
        public void AndLessThan(string field, object value)
        {
            AndClause();
            LessThan(field, value);
        }

        public void OrLessThan(string field, object value)
        {
            OrClause();
            LessThan(field, value);
        }

        public void LessThan(string field, object value)
        {
            AndClause();
            clause.Add(new LessThanClause(field, PrepareValue(field, value)));
        } 
        #endregion

        #region EqualTo
        public void EqualTo(string field, object value)
        {
            AndEqualTo(field, value);
        }

        public void AndEqualTo(string field, object value)
        {
            AndClause();
            clause.Add(new EqualToClause(field, PrepareValue(field, value)));
        }

        public void OrEqualTo(string field, object value)
        {
            OrClause();
            clause.Add(new EqualToClause(field, PrepareValue(field, value)));
        }
        #endregion

        #region NotEqualTo
        public void NotEqualTo(string field, object value)
        {
            AndNotEqualTo(field, value);
        }

        public void AndNotEqualTo(string field, object value)
        {
            AndClause();
            this.NotEqualTo(field, value);
        }

        public void OrNotEqualTo(string field, object value)
        {
            OrClause();
            this.NotEqualTo(field, value);
        }
        #endregion

        #region NotNull
        public void IsNotNull(string field)
        {
            AndIsNotNull(field);
        }

        public void AndIsNotNull(string field)
        {
            AndClause();
            clause.Add(new IsNotNullClause(field));
        }

        public void OrIsNotNull(string field)
        {
            OrClause();
            clause.Add(new IsNotNullClause(field));
        } 
        #endregion

        #region IsNull
        public void IsNull(string field)
        {
            AndIsNull(field);
        }

        public void OrIsNull(string field)
        {
            OrClause();
            clause.Add(new IsNullClause(field));
        }

        public void AndIsNull(string field)
        {
            AndClause();
            clause.Add(new IsNullClause(field));
        } 
        #endregion

        #region Like
        public void Like(string field, object value)
        {
            AndLike(field, value);
        }

        public void AndLike(string field, object value)
        {
            AndClause();
            clause.Add(new LikeClause(field, PrepareValue(field, value)));
        }

        public void AndLike(string field, object value, string fieldFunction)
        {
            AndClause();
            clause.Add(new LikeClause(fieldFunction + "(" + field + ")",
                                      PrepareValue(field, value)));
        }

        public void OrLike(string field, object value)
        {
            OrClause();
            clause.Add(new LikeClause(field, PrepareValue(field, value)));
        }

        public void OrLike(string field, object value, string fieldFunction)
        {
            OrClause();
            clause.Add(new LikeClause(fieldFunction + "(" + field + ")",
                                      PrepareValue(field, value)));
        } 
        #endregion

        #region NotLike
        public void NotLike(string field, object value)
        {
            AndNotLike(field, value);
        }

        public void AndNotLike(string f, object v)
        {
            AndClause();
            clause.Add(new NotLikeClause(f, PrepareValue(f, v)));
        }

        public void OrNotLike(string f, object v)
        {
            OrClause();
            clause.Add(new NotLikeClause(f, PrepareValue(f, v)));
        } 
        #endregion

        #region Between
        public void Between(string field, object val1, object val2)
        {
            AndBetween(field, val1, val2);
        }

        public void AndBetween(string field, object val1, object val2)
        {
            AndClause();
            clause.Add(new BetweenClause(field,
                                         PrepareValue(field, val1),
                                         PrepareValue(field, val2)));
        }

        public void OrBetween(string field, object val1, object val2)
        {
            OrClause();
            clause.Add(new BetweenClause(field,
                                         PrepareValue(field, val1),
                                         PrepareValue(field, val2)));
        } 
        #endregion

        #region NotBetween
        public void NotBetween(string field, object val1, object val2)
        {
            AndNotBetween(field, val1, val2);
        }

        public void AndNotBetween(string field, object val1, object val2)
        {
            AndClause();
            clause.Add(new NotBetweenClause(field,
                                            PrepareValue(field, val1),
                                            PrepareValue(field, val2)));
        }

        public void OrNotBetween(string field, object val1, object val2)
        {
            OrClause();
            clause.Add(new NotBetweenClause(field,
                                            PrepareValue(field, val1),
                                            PrepareValue(field, val2)));
        } 
        #endregion

        #region Internal
        internal void AndClause()
        {
            if (clause.Count != 0)
                clause.Add(new AndClause());
        }

        internal void OrClause()
        {
            if (clause.Count != 0)
                clause.Add(new OrClause());
        } 
        #endregion

        #region Order
        public void OrderBy(string field)
        {
            OrderByAsc(field);
        }

        public void OrderByAsc(string field)
        {
            if (order.Count > 0)
                order.Add(", ");
            order.Add(field + " ASC");
        }

        public void OrderByDesc(string field)
        {
            if (order.Count > 0)
                order.Add(", ");
            order.Add(field + " DESC");
        } 
        #endregion

        #region GetClause
        public string GetWhereClause()
        {
            StringBuilder buildClause = new StringBuilder(" WHERE ");
            for (int i = 0; i < clause.Count; i++)
                buildClause.Append(clause[i].ToString());

            return buildClause.ToString();
        }

        public string GetOrderByClause()
        {
            StringBuilder buildClause = new StringBuilder(" ORDER BY ");
            for (int i = 0; i < order.Count; i++)
                buildClause.Append(order[i].ToString());

            return buildClause.ToString();
        } 
        #endregion

        #region Properties
        public int ClauseCount
        {
            get { return clause.Count; }
        }

        public int OrderCount
        {
            get { return order.Count; }
        }

        public int Limit
        {
            get { return limit; }
            set { limit = value; }
        }

        public uint Offset
        {
            get { return offset; }
            set { offset = value; }
        } 
        #endregion
    }
}
