using System;
using System.Text;

using PMS.Data;
using PMS.Metadata;

namespace PMS.Query
{
    public abstract class AbstractQuery : IQuery
    {
		protected static readonly log4net.ILog log = log4net.LogManager.GetLogger("PMS.Query.AQuery");

        protected SqlCommand command;
        protected Criteria criteria = null;
		protected FieldCollection columns;
        protected FieldCollection keys;
        protected string _selection = "*";
		protected Object obj = null;
		protected Class cdesc = null;
		protected IProvider provider = null;

		protected AbstractQuery()
		{
			this.keys = new FieldCollection();
			this.columns = new FieldCollection();

			if (this.provider == null)
				this.provider = ProviderFactory.Factory(RepositoryManager.CurrentConnection.Type);
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
			this.obj = obj;
			this.cdesc = RepositoryManager.GetClass(obj.GetType());

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
			return provider.PrepareSqlValue(field.DbType, cdesc.GetValue(field, this.obj));
		}

		protected bool IsFieldSet(Field field)
		{
			return IsFieldSet(field, cdesc.GetValue(field, this.obj));
		}

		protected bool IsFieldSet(Field field, object value)
		{
			object init = null;
			//Console.WriteLine("   Column: '{0}'", field.Column);
			//Console.WriteLine("    Value: '{0}'", value);

			if (value != null) {
				init = provider.GetTypeInit(field.DbType);
				//Console.WriteLine("   DbType: " + field.DbType);
				//Console.WriteLine("  Default: '{0}'", init);
				//Console.WriteLine("   Ignore: " + field.IgnoreDefault);

				if (init == null) {
					//Console.WriteLine("IsIgnored: " + init.Equals(value));
					log.Error("IsFieldSet: Type(" + field.DbType + ") return null");
				} 
				//Console.WriteLine();

				return !(init != null && field.IgnoreDefault && init.Equals(value));
			}

			return false;
		}

        public SqlCommand Command {
            get { return command; }
            set { command = value; }
        }

        public virtual string Selection {
            get { return _selection; }
            set { _selection = value; }
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
			return (cdesc.Table[0] != 'o' && cdesc.Table != "order")? cdesc.Table : ("\"" + cdesc.Table + "\"");
		}

        public override string ToString()
        {
            if (Command == SqlCommand.Select)
                return Select();
            else if (Command == SqlCommand.Update)
                return Update();
            else if (Command == SqlCommand.Insert)
                return Insert();
            else if (Command == SqlCommand.Delete)
                return Delete();

            return Select();
        }
    }
}
