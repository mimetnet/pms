using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using PMS.Data;
using PMS.Metadata;

namespace PMS.Query
{
    public partial class Query <Table> where Table : new()
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger("Query");
        protected static bool verbose = false;
        protected IProvider provider = null;
		protected Class cdesc = null;
        protected IDbConnection connection = null;
        protected List<IDataParameter> parameters = new List<IDataParameter>();

        public Query(Class klass, IProvider provider, IDbConnection connection)
        {
            this.cdesc = klass;
            this.provider = provider;
            this.connection = connection;
        }

        public Class Class {
            get { return this.cdesc; }
        }

        public IProvider Provider {
            get { return this.provider; }
        }

        public List<IDataParameter> Parameters {
            get { return this.parameters; }
        }

        internal IDbConnection Connection {
            get { return this.connection; }
        }

        #region SQL Generators
        protected virtual string DeleteSql()
        {
            StringBuilder sql = new StringBuilder("DELETE FROM ");
            sql.Append(this.cdesc.Table);
            AppendWhere(sql);
            AppendCondition(sql);

            if (this.values.Count > 0 && this.values[0].IsCondition == false)
                this.values.RemoveAt(0);
            
            this.values.ForEach(delegate(IClause c) {
                sql.Append(c.ToString());
            
                if (c.IsCondition)
                    this.parameters.AddRange(c.CreateParameters(this.provider.CreateParameter));
            });

            AppendLimit(sql);

            return sql.ToString();
        }

        protected virtual string InsertSql()
        {
            if (this.criteria.Count == 0 && this.values.Count == 0)
                throw new QueryException("No criteria found to perform INSERT with");

            StringBuilder sql = new StringBuilder("INSERT INTO ");
            sql.Append(this.cdesc.Table);
            sql.Append(' ');
            AppendInsert(sql);
            return sql.ToString();
        }

        protected virtual string UpdateSql()
        {
            StringBuilder sql = new StringBuilder("UPDATE ");
            sql.Append(this.cdesc.Table);
            
            if (this.criteria.Count > 0 || this.values.Count > 0) {
                sql.Append(" SET ");

                if (this.values[0].IsCondition == false)
                    this.values.RemoveAt(0);

                this.values.ForEach(delegate(IClause c) {
                    sql.Append(c.ToString());
                    
                    if (c.IsCondition)
                        this.parameters.AddRange(c.CreateParameters(this.provider.CreateParameter));
                });
                AppendWhere(sql);
                AppendCondition(sql);
            } else {
                throw new QueryException("No criteria found to perform UPDATE with");
            }

            return sql.ToString();
        }

        protected virtual string SelectSql()
        {
            StringBuilder sql = new StringBuilder("SELECT ");
            sql.Append(this.columns);
            sql.Append(" FROM ");
            sql.Append(this.cdesc.Table);
            AppendWhere(sql);
            AppendCondition(sql);
            AppendOrderBy(sql);
            AppendLimit(sql);
            return sql.ToString();
        }

        protected virtual string CountSql()
        {
            StringBuilder sql = new StringBuilder("SELECT COUNT(*) FROM ");
            sql.Append(this.cdesc.Table);
            AppendWhere(sql);
            AppendCondition(sql);
            AppendOrderBy(sql);
            AppendLimit(sql);
            return sql.ToString();
        }

        protected virtual string CreateSql()
        {
            StringBuilder sql = new StringBuilder("CREATE TABLE ");
            sql.Append(this.cdesc.Table);
            sql.AppendLine(" (");
            AppendColumns(sql);
            sql.Append("\n)");
            return sql.ToString();
        }

        protected virtual string DropSql()
        {
            StringBuilder sql = new StringBuilder("DROP TABLE ");
            sql.Append(this.cdesc.Table);
            return sql.ToString();
        }
        #endregion

        #region StringBuilder Appenders
        public void AppendOrderBy(StringBuilder sql)
        {
            if (this.order.Count > 0) {
                sql.Append(" ORDER BY ");
                this.order.ForEach(delegate(String str) {
                    sql.Append(str);
                });
            }
        }

        public void AppendWhere(StringBuilder sql)
        {
            if (this.criteria.Count > 0)
                sql.Append(" WHERE ");
        }

        public void AppendCondition(StringBuilder sql)
        {
            if (this.criteria.Count > 0) {
                if (this.criteria[0].IsCondition == false)
                    this.criteria.RemoveAt(0);

                this.criteria.ForEach(delegate(IClause c) {
                    sql.Append(c.ToString());
                    
                    if (c.IsCondition)
                        this.parameters.AddRange(c.CreateParameters(this.provider.CreateParameter));
                });
            }
        }
        
        public void AppendInsert(StringBuilder sql)
        {
            sql.Append('(');

            if (this.criteria.Count > 0 && this.criteria[0].IsCondition == false)
                this.criteria.RemoveAt(0);
            
            if (0 == AppendInsert(sql, this.criteria, String.Empty))
                if (this.values.Count > 0 && this.values[0].IsCondition == false)
                    this.values.RemoveAt(0);

            AppendInsert(sql, this.values, String.Empty);

            sql.Append(") VALUES (");

            AppendInsert(sql, this.criteria, "@");
            AppendInsert(sql, this.values, "@");

            sql.Append(')');
        }

        public int AppendInsert(StringBuilder sql, List<IClause> list, string prepend)
        {
            int x=0;

            for (int i=0; i<list.Count; i++) {
                if (list[i].IsCondition) {
                    if (i > 0)
                        sql.Append(',');
                    sql.Append(prepend);
                    sql.Append(list[i].Name);

                    if (!String.IsNullOrEmpty(prepend))
                        this.parameters.AddRange(list[i].CreateParameters(this.provider.CreateParameter));
                    
                    x++;
                }
            }
            return x;
        }

        public void AppendColumns(StringBuilder sql)
        {
            for (int i = 0; i < this.cdesc.Fields.Count; i++) {
                if (i > 0)
                    sql.AppendLine(",");

                this.AppendColumns(sql, this.cdesc.Fields[i]);
            }
        }

        public virtual void AppendColumns(StringBuilder sql, Field field)
        {
            sql.Append(field.ToString());
        }

        public void AppendLimit(StringBuilder sql)
        {
            if (this.limit == 0)
                return;

            sql.Append(" LIMIT ");
            sql.Append(this.limit);
            sql.Append(" OFFSET ");
            sql.Append(this.offset);
        }
        #endregion

        public DbExecutor<Table> Exec()
        {
            return new DbExecutor<Table>(this);
        }

        #region ToString()
        public override string ToString()
        {
            return this.ToString(SqlCommand.Select);
        }

        public string ToString(SqlCommand cmd)
        {
			switch (cmd) {
				case SqlCommand.Select:
					return SelectSql();

				case SqlCommand.Insert:
					return InsertSql();

				case SqlCommand.Update:
					return UpdateSql();

				case SqlCommand.Delete:
					return DeleteSql();

                case SqlCommand.Count:
                    return CountSql();

                case SqlCommand.Create:
                    return CreateSql();

                case SqlCommand.Drop:
                    return DropSql();
			}

			return SelectSql();
        }
        #endregion
    }
}
