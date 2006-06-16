namespace PMS.Query
{
    public class EqualToClause : ValueClause
    {
        public EqualToClause(string field, object value) : 
            base(field, value, "=")
        {
        }
    }
}
