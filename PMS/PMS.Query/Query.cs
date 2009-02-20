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
            AppendCondition(sql);

            return sql.ToString();
        }

        protected virtual string InsertSql()
        {
            StringBuilder sql = new StringBuilder("INSERT INTO ");
            sql.Append(this.cdesc.Table);
            //sql.Append(this.InsertClause);
            
            return sql.ToString();
        }
        
        protected virtual string UpdateSql()
        {
            StringBuilder sql = new StringBuilder("UPDATE ");
            sql.Append(this.cdesc.Table);
            //sql.Append(this.UpdateClause);
            AppendCondition(sql);

            return sql.ToString();
        }

        protected virtual string SelectSql()
        {
            StringBuilder sql = new StringBuilder("SELECT ");
            sql.Append(this.cdesc.Table);
            sql.Append('.');
            sql.Append(this.columns);
            sql.Append(" FROM ");
            sql.Append(this.cdesc.Table);
            AppendCondition(sql);
            AppendOrderBy(sql);
            AppendLimit(sql);

            return sql.ToString();
        }

        public virtual string CountSql()
        {
            StringBuilder sql = new StringBuilder("SELECT COUNT(*) FROM ");
            sql.Append(this.cdesc.Table);
            AppendCondition(sql);
            AppendOrderBy(sql);
            AppendLimit(sql);

            return sql.ToString();
        }
        #endregion

        #region StringBuilder Appenders
        public void AppendOrderBy(StringBuilder sql)
        {
            if (this.order.Count == 0)
                return;

            sql.Append(" ORDER BY ");
            foreach (String str in this.order)
                sql.Append(str);
        }

        public void AppendCondition(StringBuilder sql)
        {
            if (this.clause.Count == 0)
                return;

            sql.Append(" WHERE ");
            foreach (IClause c in this.clause) {
                sql.Append(c.ToString());
                
                if (c.IsCondition) {
                    this.parameters.AddRange(c.CreateParameters(this.provider.CreateParameter));
                }
            }
        }

        public void AppendLimit(StringBuilder sql)
        {
            if (this.limit == 0)
                return;

            sql.Append(" LIMIT ");
            sql.Append(this.limit.ToString());
            sql.Append(" OFFSET ");
            sql.Append(this.offset.ToString());
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
			}

			return SelectSql();
        }
        #endregion
    }
}
