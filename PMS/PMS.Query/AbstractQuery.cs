using System;
using System.Text;

using PMS.Metadata;

namespace PMS.Query
{
    public abstract class AbstractQuery : MarshalByRefObject, IQuery
    {
        internal MetaObject metaObject = null;
        protected SqlCommand command;
        protected Criteria criteria = null;
        protected FieldCollection columns;
        protected FieldCollection keys;
        protected string _selection = "*";
        protected Exception vExe = null;

        public void LoadMetaObject(object obj)
        {
            this.LoadMetaObject(obj, null);
        }

        public void LoadMetaObject(object obj, Criteria crit)
        {
            

            this.metaObject = new MetaObject(obj);
            if (this.metaObject.Exists) {
                this.criteria = (crit == null) ? new Criteria(obj.GetType()) : crit;
                this.columns = metaObject.Columns;
                this.keys = metaObject.PrimaryKeys;
            }
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
            get { return metaObject.Table; }
        }

        public virtual Type Type {
            get { return metaObject.Type; }
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
            sql.Append(this.Table);
            sql.Append(this.Condition);

            return sql.ToString();
        }

        /// INSERT INTO _table_
        /// (column1, column2, column3) VALUES (value1, value2, value3);
        public virtual string Insert()
        {
            StringBuilder sql = new StringBuilder("INSERT INTO ");
            sql.Append(this.Table);
            sql.Append(this.InsertClause);
            
            return sql.ToString();
        }
        
        /// UPDATE _table_
        /// SET column=value, column=value
        /// WHERE column=value AND column=value;
        public virtual string Update()
        {
            StringBuilder sql = new StringBuilder("UPDATE ");
            sql.Append(this.Table);
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
            sql.Append(this.Table);
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
            sql.Append(this.Table);
            sql.Append(this.Condition);

            return sql.ToString();
        }

        public virtual bool IsValid {
            get { 
                bool exists = metaObject.Exists;

                if (!exists) {
                    this.vExe = new ClassNotFoundException(this.Type);
                }

                return exists;
            }
        }

        public virtual Exception ValidationException {
            get { return vExe; }
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
