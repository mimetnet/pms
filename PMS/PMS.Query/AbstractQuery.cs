using System;
using System.Text;

using PMS.Data;
using PMS.Metadata;

namespace PMS.Query
{
    public abstract class AbstractQuery : IQuery
    {
		protected static readonly log4net.ILog log = log4net.LogManager.GetLogger("PMS.Query.AQuery");
		protected static readonly bool verbose = (Environment.GetEnvironmentVariable("PMS_VERBOSE") != null)? true : false;

        protected SqlCommand command;
        protected Criteria criteria = null;
		protected FieldCollection columns;
        protected FieldCollection keys;
        protected String selection = "*";
		protected Object obj = null;
		protected Class cdesc = null;
		protected IProvider provider = null;

		protected AbstractQuery()
		{
			this.keys = new FieldCollection();
			this.columns = new FieldCollection();

			if (this.provider == null) {
				//this.provider = RepositoryManager.CurrentConnection.Provider;
			}
		}

		protected AbstractQuery(Class classDescription)
		{
			this.cdesc = classDescription;
		}

		protected AbstractQuery(object obj)
			: this(obj, null)
        {
        }

		protected AbstractQuery(object obj, Criteria crit) : this()
        {
			if (obj == null) throw new ArgumentNullException("Object cannot be null");

			this.obj = obj;
			//this.cdesc = RepositoryManager.GetClass(obj.GetType());

			if (this.cdesc == null)
				throw new ClassNotFoundException(obj.GetType());

			foreach (Field field in this.cdesc.Fields) {
				if (!field.PrimaryKey)
					this.columns.Add(field);
				else
					this.keys.Add(field);
			}

			this.criteria = (crit == null) ? new Criteria(obj.GetType()) : crit;
        }

		protected string GetSqlValue(Field field)
		{
			Object o = cdesc.GetValue(field, this.obj);

			if ((o == null && field.Default == null) || (o != null && field.Default != null && field.Default.ToString() == o.ToString())) {
				return field.DefaultDb;
			}

			return provider.PrepareSqlValue(field.DbType, o);
		}

		protected bool IsFieldSet(Field field)
		{
			return IsFieldSet(field, cdesc.GetValue(field, this.obj));
		}

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
				init = (field.Default == null)? 
					provider.GetTypeInit(field.DbType) : provider.ConvertToType(field.DbType, field.Default);

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

        public SqlCommand Command {
            get { return command; }
            set { command = value; }
        }

        public virtual string Selection {
            get { return selection; }
            set { selection = value; }
        }

        public virtual string Table {
            get { return cdesc.Table; }
        }

        public virtual Type Type {
            get { return cdesc.Type; }
        }

        public virtual string Limit {
            get {
                if (criteria.Limit > -1) {
                    return String.Format(" LIMIT {0} OFFSET {1} ",
                                         criteria.Limit,
                                         criteria.Offset);
                }

                return String.Empty;
            }
        }

        public Criteria Criteria {
            get { return criteria; }
            set { criteria = value; }
        }

        public virtual string Condition {
            get { throw new NotImplementedException(); }
        }

        public virtual string OrderBy {
            get { throw new NotImplementedException(); }
        }

        public virtual string InsertClause {
            get { throw new NotImplementedException(); }
        }

        public virtual string UpdateClause {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Loop through criteria object elements and build sql from values
        /// </summary>
        public virtual string Delete()
        {
            StringBuilder sql = new StringBuilder("DELETE FROM ");
            sql.Append(GetTable());
            sql.Append(this.Condition);

            return sql.ToString();
        }

        /// INSERT INTO _table_
        /// (column1, column2, column3) VALUES (value1, value2, value3);
        public virtual string Insert()
        {
            StringBuilder sql = new StringBuilder("INSERT INTO ");
            sql.Append(GetTable());
            sql.Append(this.InsertClause);
            
            return sql.ToString();
        }
        
        /// UPDATE _table_
        /// SET column=value, column=value
        /// WHERE column=value AND column=value;
        public virtual string Update()
        {
            StringBuilder sql = new StringBuilder("UPDATE ");
            sql.Append(GetTable());
            sql.Append(this.UpdateClause);
            sql.Append(this.Condition);

            return sql.ToString();
        }

        /// SELECT _table_.* FROM _table_
        /// WHERE column=value AND column=value;
        public virtual string Select()
        {
            StringBuilder sql = new StringBuilder("SELECT ");
            sql.Append(this.Selection);
            sql.Append(" FROM ");
            sql.Append(GetTable());
            sql.Append(this.Condition);
            sql.Append(this.OrderBy);
            sql.Append(this.Limit);

            return sql.ToString();
        }

        /// SELECT COUNT(*) FROM _table_
        /// WHERE column=value AND column=value;
        public virtual string Count()
        {
            StringBuilder sql = new StringBuilder("SELECT COUNT(*) FROM ");
            sql.Append(GetTable());
            sql.Append(this.Condition);

            return sql.ToString();
        }

		private string GetTable()
		{
			return (cdesc.Table != "order")? cdesc.Table : ("\"" + cdesc.Table + "\"");
		}

        public override string ToString()
        {
			switch (Command) {
				case SqlCommand.Select:
					return Select();

				case SqlCommand.Insert:
					return Insert();

				case SqlCommand.Update:
					return Update();

				case SqlCommand.Delete:
					return Delete();
			}

			return Select();
        }
    }
}
