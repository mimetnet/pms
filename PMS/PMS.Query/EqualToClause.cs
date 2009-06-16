namespace PMS.Query
{
    public class EqualToClause : ValueClause
    {
        public EqualToClause(string field, object value) : 
            base(field, value, "=")
        {
        }

        public EqualToClause(string field, object value, PMS.DbType dbType) : 
            base(field, value, dbType, "=")
        {
        }
    }
}
