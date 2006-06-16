namespace PMS.Query
{
    public class NotEqualToClause : ValueClause
    {
        public NotEqualToClause(string field, object value) :
            base(field, value, "!=")
        {
        }
    }
}
